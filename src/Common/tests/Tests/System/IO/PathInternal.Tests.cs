﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Xunit;

namespace Tests.System.IO
{
    public class PathInternalTests
    {
        public void CheckInvalidPathChars_ThrowsOnNull(string path, string expected)
        {
            Assert.Throws<ArgumentNullException>(() => PathInternal.CheckInvalidPathChars(null));
        }
    }
}
