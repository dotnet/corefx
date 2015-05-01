// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Xunit;

namespace System.IO.FileSystem.Tests
{
    public class Directory_SetCreationTime_str_dt : SetTime_str_dt
    {
        protected override void m_Set(string path, DateTime creationTime)
        {
            Directory.SetCreationTime(path, creationTime);
        }

        protected override DateTime m_Get(string path)
        {
            return Directory.GetCreationTime(path);
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows | PlatformID.OSX)] // birth time only available on some Unix systems
        public override void PositiveTests()
        {
            base.PositiveTests();
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows | PlatformID.OSX)] // birth time only available on some Unix systems
        public override void RelativeTimePositiveTests()
        {
            base.RelativeTimePositiveTests();
        }
    }
}