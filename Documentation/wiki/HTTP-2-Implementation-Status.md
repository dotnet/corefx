This page tracks our progress on implementing HTTP/2 support in `HttpClient`.

## RFC support

| Status | Issue | Area | Feature | Effort |
| ------ | ----- | ---- | ------- | -----: |
| planned | [#41208](../issues/41208) | Headers | Dynamic receiving | low |
| planned | [#41209](../issues/41209) | Headers | Dynamic sending | medium/high |
| planned | [#31308](../issues/31308) | Headers | Huffman coding | medium |
| planned | [#31299](../issues/31299) | Streams | Flow control | medium |
| planned | [#38559](../issues/38559) | Streams | General-purpose bidirectional content | medium |
| unplanned | [#41212](../issues/41212) | Streams | Prioritization and dependencies | high |
| unplanned | [#34192](../issues/34192) | Streams | Server push | high |
| unplanned | [#41213](../issues/41213) | Proxy | CONNECT over HTTP2 | medium |
| unplanned | [#41215](../issues/41215) | Security | Frame padding | medium |

For unplanned features, discussion has not yet indicated enough interest. We would encourage giving clear usage scenarios on the issues if these are important for you.

## General architecture

| Issue | Feature | Effort |
| ----- | ------- | -----: |
| [#40514](../issues/40514) | MaxConnectionsPerServer | medium |
| [#39049](../issues/39049) | Duplex cancellation behavior | medium/high |
| [#38759](../issues/38759) | Post-header exception surface location | medium |

## Test coverage

| Issue | Feature |
| ----- | ------- |
| [#39167](../issues/39167) | Connection timeouts |
| [#39166](../issues/39166) | Connection-level WINDOW_UPDATE |
| [#39165](../issues/39165) | Header CONTINUATION frames |
| [#39163](../issues/39163) | Receipt of HEADERS frame with with padding and/or priority |
| [#39162](../issues/39162) | Receipt of DATA frame with padding |
| [#39161](../issues/39161) | Buffer flush in StartWriteAsync |

## Performance

| Issue | Feature |
| ----- | ------- |
| [#31751](../issues/31751) | Huffman decoding |
| [#31309](../issues/31309) | HPACK static table decoding |
| [#41221](../issues/41221) | General HPACK decoder optimizations |
| [#35387](../issues/35387) | Smarter usage of WINDOW_UPDATE frames |
| n/a | Create benchmarks and profile |