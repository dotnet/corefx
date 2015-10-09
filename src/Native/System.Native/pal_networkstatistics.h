// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include "pal_types.h"

// Exchange types used to normalize Network protocol statistics information
// from the OS, for use in the NetworkInformation library.

struct TcpGlobalStatistics
{
    uint64_t ConnectionsAccepted;
    uint64_t ConnectionsInitiated;
    uint64_t CumulativeConnections;
    uint64_t CurrentConnections;
    uint64_t ErrorsReceived;
    uint64_t FailedConnectionAttempts;
    uint64_t MaximumConnections;
    uint64_t MaximumTransmissionTimeout;
    uint64_t MinimumTransmissionTimeout;
    uint64_t ResetConnections;
    uint64_t SegmentsReceived;
    uint64_t SegmentsResent;
    uint64_t SegmentsSent;
    uint64_t ResetsSent;
};
