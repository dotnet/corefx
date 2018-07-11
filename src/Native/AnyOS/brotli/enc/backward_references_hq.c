/* Copyright 2013 Google Inc. All Rights Reserved.

   Distributed under MIT license.
   See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/

/* Function to find backward reference copies. */

#include "./backward_references_hq.h"

#include <string.h>  /* memcpy, memset */

#include "../common/constants.h"
#include <brotli/types.h>
#include "./command.h"
#include "./fast_log.h"
#include "./find_match_length.h"
#include "./literal_cost.h"
#include "./memory.h"
#include "./port.h"
#include "./prefix.h"
#include "./quality.h"

#if defined(__cplusplus) || defined(c_plusplus)
extern "C" {
#endif

static const float kInfinity = 1.7e38f;  /* ~= 2 ^ 127 */

static const uint32_t kDistanceCacheIndex[] = {
  0, 1, 2, 3, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1,
};
static const int kDistanceCacheOffset[] = {
  0, 0, 0, 0, -1, 1, -2, 2, -3, 3, -1, 1, -2, 2, -3, 3
};

void BrotliInitZopfliNodes(ZopfliNode* array, size_t length) {
  ZopfliNode stub;
  size_t i;
  stub.length = 1;
  stub.distance = 0;
  stub.insert_length = 0;
  stub.u.cost = kInfinity;
  for (i = 0; i < length; ++i) array[i] = stub;
}

static BROTLI_INLINE uint32_t ZopfliNodeCopyLength(const ZopfliNode* self) {
  return self->length & 0xffffff;
}

static BROTLI_INLINE uint32_t ZopfliNodeLengthCode(const ZopfliNode* self) {
  const uint32_t modifier = self->length >> 24;
  return ZopfliNodeCopyLength(self) + 9u - modifier;
}

static BROTLI_INLINE uint32_t ZopfliNodeCopyDistance(const ZopfliNode* self) {
  return self->distance & 0x7ffffff;
}

static BROTLI_INLINE uint32_t ZopfliNodeDistanceCode(const ZopfliNode* self) {
  const uint32_t short_code = self->distance >> 27;
  return short_code == 0 ?
      ZopfliNodeCopyDistance(self) + BROTLI_NUM_DISTANCE_SHORT_CODES - 1 :
      short_code - 1;
}

static BROTLI_INLINE uint32_t ZopfliNodeCommandLength(const ZopfliNode* self) {
  return ZopfliNodeCopyLength(self) + self->insert_length;
}

/* Histogram based cost model for zopflification. */
typedef struct ZopfliCostModel {
  /* The insert and copy length symbols. */
  float cost_cmd_[BROTLI_NUM_COMMAND_SYMBOLS];
  float cost_dist_[BROTLI_NUM_DISTANCE_SYMBOLS];
  /* Cumulative costs of literals per position in the stream. */
  float* literal_costs_;
  float min_cost_cmd_;
  size_t num_bytes_;
} ZopfliCostModel;

static void InitZopfliCostModel(
    MemoryManager* m, ZopfliCostModel* self, size_t num_bytes) {
  self->num_bytes_ = num_bytes;
  self->literal_costs_ = BROTLI_ALLOC(m, float, num_bytes + 2);
  if (BROTLI_IS_OOM(m)) return;
}

static void CleanupZopfliCostModel(MemoryManager* m, ZopfliCostModel* self) {
  BROTLI_FREE(m, self->literal_costs_);
}

static void SetCost(const uint32_t* histogram, size_t histogram_size,
                    float* cost) {
  size_t sum = 0;
  float log2sum;
  size_t i;
  for (i = 0; i < histogram_size; i++) {
    sum += histogram[i];
  }
  log2sum = (float)FastLog2(sum);
  for (i = 0; i < histogram_size; i++) {
    if (histogram[i] == 0) {
      cost[i] = log2sum + 2;
      continue;
    }

    /* Shannon bits for this symbol. */
    cost[i] = log2sum - (float)FastLog2(histogram[i]);

    /* Cannot be coded with less than 1 bit */
    if (cost[i] < 1) cost[i] = 1;
  }
}

static void ZopfliCostModelSetFromCommands(ZopfliCostModel* self,
                                           size_t position,
                                           const uint8_t* ringbuffer,
                                           size_t ringbuffer_mask,
                                           const Command* commands,
                                           size_t num_commands,
                                           size_t last_insert_len) {
  uint32_t histogram_literal[BROTLI_NUM_LITERAL_SYMBOLS];
  uint32_t histogram_cmd[BROTLI_NUM_COMMAND_SYMBOLS];
  uint32_t histogram_dist[BROTLI_NUM_DISTANCE_SYMBOLS];
  float cost_literal[BROTLI_NUM_LITERAL_SYMBOLS];
  size_t pos = position - last_insert_len;
  float min_cost_cmd = kInfinity;
  size_t i;
  float* cost_cmd = self->cost_cmd_;

  memset(histogram_literal, 0, sizeof(histogram_literal));
  memset(histogram_cmd, 0, sizeof(histogram_cmd));
  memset(histogram_dist, 0, sizeof(histogram_dist));

  for (i = 0; i < num_commands; i++) {
    size_t inslength = commands[i].insert_len_;
    size_t copylength = CommandCopyLen(&commands[i]);
    size_t distcode = commands[i].dist_prefix_;
    size_t cmdcode = commands[i].cmd_prefix_;
    size_t j;

    histogram_cmd[cmdcode]++;
    if (cmdcode >= 128) histogram_dist[distcode]++;

    for (j = 0; j < inslength; j++) {
      histogram_literal[ringbuffer[(pos + j) & ringbuffer_mask]]++;
    }

    pos += inslength + copylength;
  }

  SetCost(histogram_literal, BROTLI_NUM_LITERAL_SYMBOLS, cost_literal);
  SetCost(histogram_cmd, BROTLI_NUM_COMMAND_SYMBOLS, cost_cmd);
  SetCost(histogram_dist, BROTLI_NUM_DISTANCE_SYMBOLS, self->cost_dist_);

  for (i = 0; i < BROTLI_NUM_COMMAND_SYMBOLS; ++i) {
    min_cost_cmd = BROTLI_MIN(float, min_cost_cmd, cost_cmd[i]);
  }
  self->min_cost_cmd_ = min_cost_cmd;

  {
    float* literal_costs = self->literal_costs_;
    size_t num_bytes = self->num_bytes_;
    literal_costs[0] = 0.0;
    for (i = 0; i < num_bytes; ++i) {
      literal_costs[i + 1] = literal_costs[i] +
          cost_literal[ringbuffer[(position + i) & ringbuffer_mask]];
    }
  }
}

static void ZopfliCostModelSetFromLiteralCosts(ZopfliCostModel* self,
                                               size_t position,
                                               const uint8_t* ringbuffer,
                                               size_t ringbuffer_mask) {
  float* literal_costs = self->literal_costs_;
  float* cost_dist = self->cost_dist_;
  float* cost_cmd = self->cost_cmd_;
  size_t num_bytes = self->num_bytes_;
  size_t i;
  BrotliEstimateBitCostsForLiterals(position, num_bytes, ringbuffer_mask,
                                    ringbuffer, &literal_costs[1]);
  literal_costs[0] = 0.0;
  for (i = 0; i < num_bytes; ++i) {
    literal_costs[i + 1] += literal_costs[i];
  }
  for (i = 0; i < BROTLI_NUM_COMMAND_SYMBOLS; ++i) {
    cost_cmd[i] = (float)FastLog2(11 + (uint32_t)i);
  }
  for (i = 0; i < BROTLI_NUM_DISTANCE_SYMBOLS; ++i) {
    cost_dist[i] = (float)FastLog2(20 + (uint32_t)i);
  }
  self->min_cost_cmd_ = (float)FastLog2(11);
}

static BROTLI_INLINE float ZopfliCostModelGetCommandCost(
    const ZopfliCostModel* self, uint16_t cmdcode) {
  return self->cost_cmd_[cmdcode];
}

static BROTLI_INLINE float ZopfliCostModelGetDistanceCost(
    const ZopfliCostModel* self, size_t distcode) {
  return self->cost_dist_[distcode];
}

static BROTLI_INLINE float ZopfliCostModelGetLiteralCosts(
    const ZopfliCostModel* self, size_t from, size_t to) {
  return self->literal_costs_[to] - self->literal_costs_[from];
}

static BROTLI_INLINE float ZopfliCostModelGetMinCostCmd(
    const ZopfliCostModel* self) {
  return self->min_cost_cmd_;
}

/* REQUIRES: len >= 2, start_pos <= pos */
/* REQUIRES: cost < kInfinity, nodes[start_pos].cost < kInfinity */
/* Maintains the "ZopfliNode array invariant". */
static BROTLI_INLINE void UpdateZopfliNode(ZopfliNode* nodes, size_t pos,
    size_t start_pos, size_t len, size_t len_code, size_t dist,
    size_t short_code, float cost) {
  ZopfliNode* next = &nodes[pos + len];
  next->length = (uint32_t)(len | ((len + 9u - len_code) << 24));
  next->distance = (uint32_t)(dist | (short_code << 27));
  next->insert_length = (uint32_t)(pos - start_pos);
  next->u.cost = cost;
}

typedef struct PosData {
  size_t pos;
  int distance_cache[4];
  float costdiff;
  float cost;
} PosData;

/* Maintains the smallest 8 cost difference together with their positions */
typedef struct StartPosQueue {
  PosData q_[8];
  size_t idx_;
} StartPosQueue;

static BROTLI_INLINE void InitStartPosQueue(StartPosQueue* self) {
  self->idx_ = 0;
}

static size_t StartPosQueueSize(const StartPosQueue* self) {
  return BROTLI_MIN(size_t, self->idx_, 8);
}

static void StartPosQueuePush(StartPosQueue* self, const PosData* posdata) {
  size_t offset = ~(self->idx_++) & 7;
  size_t len = StartPosQueueSize(self);
  size_t i;
  PosData* q = self->q_;
  q[offset] = *posdata;
  /* Restore the sorted order. In the list of |len| items at most |len - 1|
     adjacent element comparisons / swaps are required. */
  for (i = 1; i < len; ++i) {
    if (q[offset & 7].costdiff > q[(offset + 1) & 7].costdiff) {
      BROTLI_SWAP(PosData, q, offset & 7, (offset + 1) & 7);
    }
    ++offset;
  }
}

static const PosData* StartPosQueueAt(const StartPosQueue* self, size_t k) {
  return &self->q_[(k - self->idx_) & 7];
}

/* Returns the minimum possible copy length that can improve the cost of any */
/* future position. */
static size_t ComputeMinimumCopyLength(const float start_cost,
                                       const ZopfliNode* nodes,
                                       const size_t num_bytes,
                                       const size_t pos) {
  /* Compute the minimum possible cost of reaching any future position. */
  float min_cost = start_cost;
  size_t len = 2;
  size_t next_len_bucket = 4;
  size_t next_len_offset = 10;
  while (pos + len <= num_bytes && nodes[pos + len].u.cost <= min_cost) {
    /* We already reached (pos + len) with no more cost than the minimum
       possible cost of reaching anything from this pos, so there is no point in
       looking for lengths <= len. */
    ++len;
    if (len == next_len_offset) {
      /* We reached the next copy length code bucket, so we add one more
         extra bit to the minimum cost. */
      min_cost += 1.0f;
      next_len_offset += next_len_bucket;
      next_len_bucket *= 2;
    }
  }
  return len;
}

/* REQUIRES: nodes[pos].cost < kInfinity
   REQUIRES: nodes[0..pos] satisfies that "ZopfliNode array invariant". */
static uint32_t ComputeDistanceShortcut(const size_t block_start,
                                        const size_t pos,
                                        const size_t max_backward,
                                        const size_t gap,
                                        const ZopfliNode* nodes) {
  const size_t clen = ZopfliNodeCopyLength(&nodes[pos]);
  const size_t ilen = nodes[pos].insert_length;
  const size_t dist = ZopfliNodeCopyDistance(&nodes[pos]);
  /* Since |block_start + pos| is the end position of the command, the copy part
     starts from |block_start + pos - clen|. Distances that are greater than
     this or greater than |max_backward| are static dictionary references, and
     do not update the last distances. Also distance code 0 (last distance)
     does not update the last distances. */
  if (pos == 0) {
    return 0;
  } else if (dist + clen <= block_start + pos + gap &&
             dist <= max_backward + gap &&
             ZopfliNodeDistanceCode(&nodes[pos]) > 0) {
    return (uint32_t)pos;
  } else {
    return nodes[pos - clen - ilen].u.shortcut;
  }
}

/* Fills in dist_cache[0..3] with the last four distances (as defined by
   Section 4. of the Spec) that would be used at (block_start + pos) if we
   used the shortest path of commands from block_start, computed from
   nodes[0..pos]. The last four distances at block_start are in
   starting_dist_cache[0..3].
   REQUIRES: nodes[pos].cost < kInfinity
   REQUIRES: nodes[0..pos] satisfies that "ZopfliNode array invariant". */
static void ComputeDistanceCache(const size_t pos,
                                 const int* starting_dist_cache,
                                 const ZopfliNode* nodes,
                                 int* dist_cache) {
  int idx = 0;
  size_t p = nodes[pos].u.shortcut;
  while (idx < 4 && p > 0) {
    const size_t ilen = nodes[p].insert_length;
    const size_t clen = ZopfliNodeCopyLength(&nodes[p]);
    const size_t dist = ZopfliNodeCopyDistance(&nodes[p]);
    dist_cache[idx++] = (int)dist;
    /* Because of prerequisite, p >= clen + ilen >= 2. */
    p = nodes[p - clen - ilen].u.shortcut;
  }
  for (; idx < 4; ++idx) {
    dist_cache[idx] = *starting_dist_cache++;
  }
}

/* Maintains "ZopfliNode array invariant" and pushes node to the queue, if it
   is eligible. */
static void EvaluateNode(
    const size_t block_start, const size_t pos, const size_t max_backward_limit,
    const size_t gap, const int* starting_dist_cache,
    const ZopfliCostModel* model, StartPosQueue* queue, ZopfliNode* nodes) {
  /* Save cost, because ComputeDistanceCache invalidates it. */
  float node_cost = nodes[pos].u.cost;
  nodes[pos].u.shortcut = ComputeDistanceShortcut(
      block_start, pos, max_backward_limit, gap, nodes);
  if (node_cost <= ZopfliCostModelGetLiteralCosts(model, 0, pos)) {
    PosData posdata;
    posdata.pos = pos;
    posdata.cost = node_cost;
    posdata.costdiff = node_cost -
        ZopfliCostModelGetLiteralCosts(model, 0, pos);
    ComputeDistanceCache(
        pos, starting_dist_cache, nodes, posdata.distance_cache);
    StartPosQueuePush(queue, &posdata);
  }
}

/* Returns longest copy length. */
static size_t UpdateNodes(
    const size_t num_bytes, const size_t block_start, const size_t pos,
    const uint8_t* ringbuffer, const size_t ringbuffer_mask,
    const BrotliEncoderParams* params, const size_t max_backward_limit,
    const int* starting_dist_cache, const size_t num_matches,
    const BackwardMatch* matches, const ZopfliCostModel* model,
    StartPosQueue* queue, ZopfliNode* nodes) {
  const size_t cur_ix = block_start + pos;
  const size_t cur_ix_masked = cur_ix & ringbuffer_mask;
  const size_t max_distance = BROTLI_MIN(size_t, cur_ix, max_backward_limit);
  const size_t max_len = num_bytes - pos;
  const size_t max_zopfli_len = MaxZopfliLen(params);
  const size_t max_iters = MaxZopfliCandidates(params);
  size_t min_len;
  size_t result = 0;
  size_t k;
  size_t gap = 0;

  EvaluateNode(block_start, pos, max_backward_limit, gap, starting_dist_cache,
      model, queue, nodes);

  {
    const PosData* posdata = StartPosQueueAt(queue, 0);
    float min_cost = (posdata->cost + ZopfliCostModelGetMinCostCmd(model) +
        ZopfliCostModelGetLiteralCosts(model, posdata->pos, pos));
    min_len = ComputeMinimumCopyLength(min_cost, nodes, num_bytes, pos);
  }

  /* Go over the command starting positions in order of increasing cost
     difference. */
  for (k = 0; k < max_iters && k < StartPosQueueSize(queue); ++k) {
    const PosData* posdata = StartPosQueueAt(queue, k);
    const size_t start = posdata->pos;
    const uint16_t inscode = GetInsertLengthCode(pos - start);
    const float start_costdiff = posdata->costdiff;
    const float base_cost = start_costdiff + (float)GetInsertExtra(inscode) +
        ZopfliCostModelGetLiteralCosts(model, 0, pos);

    /* Look for last distance matches using the distance cache from this
       starting position. */
    size_t best_len = min_len - 1;
    size_t j = 0;
    for (; j < BROTLI_NUM_DISTANCE_SHORT_CODES && best_len < max_len; ++j) {
      const size_t idx = kDistanceCacheIndex[j];
      const size_t backward =
          (size_t)(posdata->distance_cache[idx] + kDistanceCacheOffset[j]);
      size_t prev_ix = cur_ix - backward;
      size_t len = 0;
      uint8_t continuation = ringbuffer[cur_ix_masked + best_len];
      if (cur_ix_masked + best_len > ringbuffer_mask) {
        break;
      }
      if (BROTLI_PREDICT_FALSE(backward > max_distance + gap)) {
        continue;
      }
      if (backward <= max_distance) {
        if (prev_ix >= cur_ix) {
          continue;
        }

        prev_ix &= ringbuffer_mask;
        if (prev_ix + best_len > ringbuffer_mask ||
            continuation != ringbuffer[prev_ix + best_len]) {
          continue;
        }
        len = FindMatchLengthWithLimit(&ringbuffer[prev_ix],
                                       &ringbuffer[cur_ix_masked],
                                       max_len);
      } else {
        continue;
      }
      {
        const float dist_cost = base_cost +
            ZopfliCostModelGetDistanceCost(model, j);
        size_t l;
        for (l = best_len + 1; l <= len; ++l) {
          const uint16_t copycode = GetCopyLengthCode(l);
          const uint16_t cmdcode =
              CombineLengthCodes(inscode, copycode, j == 0);
          const float cost = (cmdcode < 128 ? base_cost : dist_cost) +
              (float)GetCopyExtra(copycode) +
              ZopfliCostModelGetCommandCost(model, cmdcode);
          if (cost < nodes[pos + l].u.cost) {
            UpdateZopfliNode(nodes, pos, start, l, l, backward, j + 1, cost);
            result = BROTLI_MAX(size_t, result, l);
          }
          best_len = l;
        }
      }
    }

    /* At higher iterations look only for new last distance matches, since
       looking only for new command start positions with the same distances
       does not help much. */
    if (k >= 2) continue;

    {
      /* Loop through all possible copy lengths at this position. */
      size_t len = min_len;
      for (j = 0; j < num_matches; ++j) {
        BackwardMatch match = matches[j];
        size_t dist = match.distance;
        BROTLI_BOOL is_dictionary_match =
            TO_BROTLI_BOOL(dist > max_distance + gap);
        /* We already tried all possible last distance matches, so we can use
           normal distance code here. */
        size_t dist_code = dist + BROTLI_NUM_DISTANCE_SHORT_CODES - 1;
        uint16_t dist_symbol;
        uint32_t distextra;
        uint32_t distnumextra;
        float dist_cost;
        size_t max_match_len;
        PrefixEncodeCopyDistance(dist_code, 0, 0, &dist_symbol, &distextra);
        distnumextra = distextra >> 24;
        dist_cost = base_cost + (float)distnumextra +
            ZopfliCostModelGetDistanceCost(model, dist_symbol);

        /* Try all copy lengths up until the maximum copy length corresponding
           to this distance. If the distance refers to the static dictionary, or
           the maximum length is long enough, try only one maximum length. */
        max_match_len = BackwardMatchLength(&match);
        if (len < max_match_len &&
            (is_dictionary_match || max_match_len > max_zopfli_len)) {
          len = max_match_len;
        }
        for (; len <= max_match_len; ++len) {
          const size_t len_code =
              is_dictionary_match ? BackwardMatchLengthCode(&match) : len;
          const uint16_t copycode = GetCopyLengthCode(len_code);
          const uint16_t cmdcode = CombineLengthCodes(inscode, copycode, 0);
          const float cost = dist_cost + (float)GetCopyExtra(copycode) +
              ZopfliCostModelGetCommandCost(model, cmdcode);
          if (cost < nodes[pos + len].u.cost) {
            UpdateZopfliNode(nodes, pos, start, len, len_code, dist, 0, cost);
            result = BROTLI_MAX(size_t, result, len);
          }
        }
      }
    }
  }
  return result;
}

static size_t ComputeShortestPathFromNodes(size_t num_bytes,
    ZopfliNode* nodes) {
  size_t index = num_bytes;
  size_t num_commands = 0;
  while (nodes[index].insert_length == 0 && nodes[index].length == 1) --index;
  nodes[index].u.next = BROTLI_UINT32_MAX;
  while (index != 0) {
    size_t len = ZopfliNodeCommandLength(&nodes[index]);
    index -= len;
    nodes[index].u.next = (uint32_t)len;
    num_commands++;
  }
  return num_commands;
}

/* REQUIRES: nodes != NULL and len(nodes) >= num_bytes + 1 */
void BrotliZopfliCreateCommands(const size_t num_bytes,
                                const size_t block_start,
                                const size_t max_backward_limit,
                                const ZopfliNode* nodes,
                                int* dist_cache,
                                size_t* last_insert_len,
                                const BrotliEncoderParams* params,
                                Command* commands,
                                size_t* num_literals) {
  size_t pos = 0;
  uint32_t offset = nodes[0].u.next;
  size_t i;
  size_t gap = 0;
  BROTLI_UNUSED(params);
  for (i = 0; offset != BROTLI_UINT32_MAX; i++) {
    const ZopfliNode* next = &nodes[pos + offset];
    size_t copy_length = ZopfliNodeCopyLength(next);
    size_t insert_length = next->insert_length;
    pos += insert_length;
    offset = next->u.next;
    if (i == 0) {
      insert_length += *last_insert_len;
      *last_insert_len = 0;
    }
    {
      size_t distance = ZopfliNodeCopyDistance(next);
      size_t len_code = ZopfliNodeLengthCode(next);
      size_t max_distance =
          BROTLI_MIN(size_t, block_start + pos, max_backward_limit);
      BROTLI_BOOL is_dictionary = TO_BROTLI_BOOL(distance > max_distance + gap);
      size_t dist_code = ZopfliNodeDistanceCode(next);
      InitCommand(&commands[i], insert_length,
          copy_length, (int)len_code - (int)copy_length, dist_code);

      if (!is_dictionary && dist_code > 0) {
        dist_cache[3] = dist_cache[2];
        dist_cache[2] = dist_cache[1];
        dist_cache[1] = dist_cache[0];
        dist_cache[0] = (int)distance;
      }
    }

    *num_literals += insert_length;
    pos += copy_length;
  }
  *last_insert_len += num_bytes - pos;
}

static size_t ZopfliIterate(size_t num_bytes,
                            size_t position,
                            const uint8_t* ringbuffer,
                            size_t ringbuffer_mask,
                            const BrotliEncoderParams* params,
                            const size_t max_backward_limit,
                            const size_t gap,
                            const int* dist_cache,
                            const ZopfliCostModel* model,
                            const uint32_t* num_matches,
                            const BackwardMatch* matches,
                            ZopfliNode* nodes) {
  const size_t max_zopfli_len = MaxZopfliLen(params);
  StartPosQueue queue;
  size_t cur_match_pos = 0;
  size_t i;
  nodes[0].length = 0;
  nodes[0].u.cost = 0;
  InitStartPosQueue(&queue);
  for (i = 0; i + 3 < num_bytes; i++) {
    size_t skip = UpdateNodes(num_bytes, position, i, ringbuffer,
        ringbuffer_mask, params, max_backward_limit, dist_cache,
        num_matches[i], &matches[cur_match_pos], model, &queue, nodes);
    if (skip < BROTLI_LONG_COPY_QUICK_STEP) skip = 0;
    cur_match_pos += num_matches[i];
    if (num_matches[i] == 1 &&
        BackwardMatchLength(&matches[cur_match_pos - 1]) > max_zopfli_len) {
      skip = BROTLI_MAX(size_t,
          BackwardMatchLength(&matches[cur_match_pos - 1]), skip);
    }
    if (skip > 1) {
      skip--;
      while (skip) {
        i++;
        if (i + 3 >= num_bytes) break;
        EvaluateNode(position, i, max_backward_limit, gap, dist_cache, model,
            &queue, nodes);
        cur_match_pos += num_matches[i];
        skip--;
      }
    }
  }
  return ComputeShortestPathFromNodes(num_bytes, nodes);
}

/* REQUIRES: nodes != NULL and len(nodes) >= num_bytes + 1 */
size_t BrotliZopfliComputeShortestPath(MemoryManager* m,
                                       const BrotliDictionary* dictionary,
                                       size_t num_bytes,
                                       size_t position,
                                       const uint8_t* ringbuffer,
                                       size_t ringbuffer_mask,
                                       const BrotliEncoderParams* params,
                                       const size_t max_backward_limit,
                                       const int* dist_cache,
                                       HasherHandle hasher,
                                       ZopfliNode* nodes) {
  const size_t max_zopfli_len = MaxZopfliLen(params);
  ZopfliCostModel model;
  StartPosQueue queue;
  BackwardMatch matches[2 * (MAX_NUM_MATCHES_H10 + 64)];
  const size_t store_end = num_bytes >= StoreLookaheadH10() ?
      position + num_bytes - StoreLookaheadH10() + 1 : position;
  size_t i;
  size_t gap = 0;
  size_t lz_matches_offset = 0;
  nodes[0].length = 0;
  nodes[0].u.cost = 0;
  InitZopfliCostModel(m, &model, num_bytes);
  if (BROTLI_IS_OOM(m)) return 0;
  ZopfliCostModelSetFromLiteralCosts(
      &model, position, ringbuffer, ringbuffer_mask);
  InitStartPosQueue(&queue);
  for (i = 0; i + HashTypeLengthH10() - 1 < num_bytes; i++) {
    const size_t pos = position + i;
    const size_t max_distance = BROTLI_MIN(size_t, pos, max_backward_limit);
    size_t num_matches = FindAllMatchesH10(hasher, dictionary, ringbuffer,
        ringbuffer_mask, pos, num_bytes - i, max_distance, gap, params,
        &matches[lz_matches_offset]);
    size_t skip;
    if (num_matches > 0 &&
        BackwardMatchLength(&matches[num_matches - 1]) > max_zopfli_len) {
      matches[0] = matches[num_matches - 1];
      num_matches = 1;
    }
    skip = UpdateNodes(num_bytes, position, i, ringbuffer, ringbuffer_mask,
        params, max_backward_limit, dist_cache, num_matches, matches, &model,
        &queue, nodes);
    if (skip < BROTLI_LONG_COPY_QUICK_STEP) skip = 0;
    if (num_matches == 1 && BackwardMatchLength(&matches[0]) > max_zopfli_len) {
      skip = BROTLI_MAX(size_t, BackwardMatchLength(&matches[0]), skip);
    }
    if (skip > 1) {
      /* Add the tail of the copy to the hasher. */
      StoreRangeH10(hasher, ringbuffer, ringbuffer_mask, pos + 1, BROTLI_MIN(
          size_t, pos + skip, store_end));
      skip--;
      while (skip) {
        i++;
        if (i + HashTypeLengthH10() - 1 >= num_bytes) break;
        EvaluateNode(position, i, max_backward_limit, gap, dist_cache, &model,
            &queue, nodes);
        skip--;
      }
    }
  }
  CleanupZopfliCostModel(m, &model);
  return ComputeShortestPathFromNodes(num_bytes, nodes);
}

void BrotliCreateZopfliBackwardReferences(
    MemoryManager* m, const BrotliDictionary* dictionary, size_t num_bytes,
    size_t position, const uint8_t* ringbuffer, size_t ringbuffer_mask,
    const BrotliEncoderParams* params, HasherHandle hasher, int* dist_cache,
    size_t* last_insert_len, Command* commands, size_t* num_commands,
    size_t* num_literals) {
  const size_t max_backward_limit = BROTLI_MAX_BACKWARD_LIMIT(params->lgwin);
  ZopfliNode* nodes;
  nodes = BROTLI_ALLOC(m, ZopfliNode, num_bytes + 1);
  if (BROTLI_IS_OOM(m)) return;
  BrotliInitZopfliNodes(nodes, num_bytes + 1);
  *num_commands += BrotliZopfliComputeShortestPath(m, dictionary, num_bytes,
      position, ringbuffer, ringbuffer_mask, params, max_backward_limit,
      dist_cache, hasher, nodes);
  if (BROTLI_IS_OOM(m)) return;
  BrotliZopfliCreateCommands(num_bytes, position, max_backward_limit, nodes,
      dist_cache, last_insert_len, params, commands, num_literals);
  BROTLI_FREE(m, nodes);
}

void BrotliCreateHqZopfliBackwardReferences(
    MemoryManager* m, const BrotliDictionary* dictionary, size_t num_bytes,
    size_t position, const uint8_t* ringbuffer, size_t ringbuffer_mask,
    const BrotliEncoderParams* params, HasherHandle hasher, int* dist_cache,
    size_t* last_insert_len, Command* commands, size_t* num_commands,
    size_t* num_literals) {
  const size_t max_backward_limit = BROTLI_MAX_BACKWARD_LIMIT(params->lgwin);
  uint32_t* num_matches = BROTLI_ALLOC(m, uint32_t, num_bytes);
  size_t matches_size = 4 * num_bytes;
  const size_t store_end = num_bytes >= StoreLookaheadH10() ?
      position + num_bytes - StoreLookaheadH10() + 1 : position;
  size_t cur_match_pos = 0;
  size_t i;
  size_t orig_num_literals;
  size_t orig_last_insert_len;
  int orig_dist_cache[4];
  size_t orig_num_commands;
  ZopfliCostModel model;
  ZopfliNode* nodes;
  BackwardMatch* matches = BROTLI_ALLOC(m, BackwardMatch, matches_size);
  size_t gap = 0;
  size_t shadow_matches = 0;
  if (BROTLI_IS_OOM(m)) return;
  for (i = 0; i + HashTypeLengthH10() - 1 < num_bytes; ++i) {
    const size_t pos = position + i;
    size_t max_distance = BROTLI_MIN(size_t, pos, max_backward_limit);
    size_t max_length = num_bytes - i;
    size_t num_found_matches;
    size_t cur_match_end;
    size_t j;
    /* Ensure that we have enough free slots. */
    BROTLI_ENSURE_CAPACITY(m, BackwardMatch, matches, matches_size,
        cur_match_pos + MAX_NUM_MATCHES_H10 + shadow_matches);
    if (BROTLI_IS_OOM(m)) return;
    num_found_matches = FindAllMatchesH10(hasher, dictionary, ringbuffer,
        ringbuffer_mask, pos, max_length, max_distance, gap, params,
        &matches[cur_match_pos + shadow_matches]);
    cur_match_end = cur_match_pos + num_found_matches;
    for (j = cur_match_pos; j + 1 < cur_match_end; ++j) {
      assert(BackwardMatchLength(&matches[j]) <=
          BackwardMatchLength(&matches[j + 1]));
    }
    num_matches[i] = (uint32_t)num_found_matches;
    if (num_found_matches > 0) {
      const size_t match_len = BackwardMatchLength(&matches[cur_match_end - 1]);
      if (match_len > MAX_ZOPFLI_LEN_QUALITY_11) {
        const size_t skip = match_len - 1;
        matches[cur_match_pos++] = matches[cur_match_end - 1];
        num_matches[i] = 1;
        /* Add the tail of the copy to the hasher. */
        StoreRangeH10(hasher, ringbuffer, ringbuffer_mask, pos + 1,
                      BROTLI_MIN(size_t, pos + match_len, store_end));
        memset(&num_matches[i + 1], 0, skip * sizeof(num_matches[0]));
        i += skip;
      } else {
        cur_match_pos = cur_match_end;
      }
    }
  }
  orig_num_literals = *num_literals;
  orig_last_insert_len = *last_insert_len;
  memcpy(orig_dist_cache, dist_cache, 4 * sizeof(dist_cache[0]));
  orig_num_commands = *num_commands;
  nodes = BROTLI_ALLOC(m, ZopfliNode, num_bytes + 1);
  if (BROTLI_IS_OOM(m)) return;
  InitZopfliCostModel(m, &model, num_bytes);
  if (BROTLI_IS_OOM(m)) return;
  for (i = 0; i < 2; i++) {
    BrotliInitZopfliNodes(nodes, num_bytes + 1);
    if (i == 0) {
      ZopfliCostModelSetFromLiteralCosts(
          &model, position, ringbuffer, ringbuffer_mask);
    } else {
      ZopfliCostModelSetFromCommands(&model, position, ringbuffer,
          ringbuffer_mask, commands, *num_commands - orig_num_commands,
          orig_last_insert_len);
    }
    *num_commands = orig_num_commands;
    *num_literals = orig_num_literals;
    *last_insert_len = orig_last_insert_len;
    memcpy(dist_cache, orig_dist_cache, 4 * sizeof(dist_cache[0]));
    *num_commands += ZopfliIterate(num_bytes, position, ringbuffer,
        ringbuffer_mask, params, max_backward_limit, gap, dist_cache,
        &model, num_matches, matches, nodes);
    BrotliZopfliCreateCommands(num_bytes, position, max_backward_limit,
        nodes, dist_cache, last_insert_len, params, commands, num_literals);
  }
  CleanupZopfliCostModel(m, &model);
  BROTLI_FREE(m, nodes);
  BROTLI_FREE(m, matches);
  BROTLI_FREE(m, num_matches);
}

#if defined(__cplusplus) || defined(c_plusplus)
}  /* extern "C" */
#endif
