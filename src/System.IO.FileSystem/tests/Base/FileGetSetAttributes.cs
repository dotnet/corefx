﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    // Tests that are valid for File and FileInfo
    public abstract class FileGetSetAttributes : BaseGetSetAttributes
    {
        [Theory]
        [InlineData(FileAttributes.ReadOnly)]
        [InlineData(FileAttributes.Normal)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void SettingAttributes_Unix(FileAttributes attributes)
        {
            string path = CreateItem();
            SetAttributes(path, attributes);
            Assert.Equal(attributes, GetAttributes(path));
            SetAttributes(path, 0);
        }

        [Theory]
        [InlineData(FileAttributes.ReadOnly)]
        [InlineData(FileAttributes.Hidden)]
        [InlineData(FileAttributes.System)]
        [InlineData(FileAttributes.Archive)]
        [InlineData(FileAttributes.Normal)]
        [InlineData(FileAttributes.Temporary)]
        [InlineData(FileAttributes.ReadOnly | FileAttributes.Hidden)]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void SettingAttributes_Windows(FileAttributes attributes)
        {
            string path = CreateItem();
            SetAttributes(path, attributes);
            Assert.Equal(attributes, GetAttributes(path));
            SetAttributes(path, 0);
        }

        [Theory]
        [InlineData(FileAttributes.Temporary)]
        [InlineData(FileAttributes.Encrypted)]
        [InlineData(FileAttributes.SparseFile)]
        [InlineData(FileAttributes.ReparsePoint)]
        [InlineData(FileAttributes.Compressed)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void SettingInvalidAttributes_Unix(FileAttributes attributes)
        {
            string path = CreateItem();
            SetAttributes(path, attributes);
            Assert.Equal(FileAttributes.Normal, GetAttributes(path));
        }

        [Theory]
        [InlineData(FileAttributes.Normal)]
        [InlineData(FileAttributes.Encrypted)]
        [InlineData(FileAttributes.SparseFile)]
        [InlineData(FileAttributes.ReparsePoint)]
        [InlineData(FileAttributes.Compressed)]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void SettingInvalidAttributes_Windows(FileAttributes attributes)
        {
            string path = CreateItem();
            SetAttributes(path, attributes);
            Assert.Equal(FileAttributes.Normal, GetAttributes(path));
        }

        [Theory,
            InlineData(":bar"),
            InlineData(":bar:$DATA")]
        [PlatformSpecific(TestPlatforms.Windows)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public void GettingAndSettingAttributes_AlternateDataStream_Windows(string streamName)
        {
            string path = CreateItem();
            streamName = path + streamName;
            File.Create(streamName);

            FileAttributes attributes = GetAttributes(streamName);
            Assert.NotEqual((FileAttributes)0, attributes);
            Assert.NotEqual((FileAttributes)(-1), attributes);

            // Attributes are shared for the file and all streams
            SetAttributes(streamName, FileAttributes.Hidden);
            Assert.Equal(FileAttributes.Hidden, GetAttributes(streamName));
            Assert.Equal(FileAttributes.Hidden, GetAttributes(path));

            SetAttributes(path, FileAttributes.Normal);
            Assert.Equal(FileAttributes.Normal, GetAttributes(streamName));
            Assert.Equal(FileAttributes.Normal, GetAttributes(path));
        }
    }
}
