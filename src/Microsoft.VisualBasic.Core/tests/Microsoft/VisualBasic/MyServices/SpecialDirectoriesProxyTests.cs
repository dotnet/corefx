// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.VisualBasic.Devices;
using Xunit;

namespace Microsoft.VisualBasic.MyServices.Tests
{
    public class SpecialDirectoriesProxyTests
    {
        [Fact]
        public void Properties()
        {
            SpecialDirectoriesProxy specialDirectories = new ServerComputer().FileSystem.SpecialDirectories;
            Assert.Throws<PlatformNotSupportedException>(() => specialDirectories.AllUsersApplicationData);
            Assert.Throws<PlatformNotSupportedException>(() => specialDirectories.CurrentUserApplicationData);
            Assert.Equal(Microsoft.VisualBasic.FileIO.SpecialDirectories.Desktop, specialDirectories.Desktop);
            Assert.Equal(Microsoft.VisualBasic.FileIO.SpecialDirectories.MyDocuments, specialDirectories.MyDocuments);
            Assert.Equal(Microsoft.VisualBasic.FileIO.SpecialDirectories.MyMusic, specialDirectories.MyMusic);
            Assert.Equal(Microsoft.VisualBasic.FileIO.SpecialDirectories.MyPictures, specialDirectories.MyPictures);
            Assert.Equal(Microsoft.VisualBasic.FileIO.SpecialDirectories.Programs, specialDirectories.Programs);
            Assert.Equal(Microsoft.VisualBasic.FileIO.SpecialDirectories.ProgramFiles, specialDirectories.ProgramFiles);
            Assert.Equal(Microsoft.VisualBasic.FileIO.SpecialDirectories.Temp, specialDirectories.Temp);
        }
    }
}
