﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Xunit;

namespace System.IO.Tests
{
    public class RemoteFileSystemTest : RemoteExecutorTestBase
    {
        public static TheoryData<char> TrailingCharacters = TestData.TrailingCharacters;
    }
}
