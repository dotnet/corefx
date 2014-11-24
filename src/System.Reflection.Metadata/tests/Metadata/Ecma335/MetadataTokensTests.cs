// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection.Metadata.Ecma335;
using Xunit;

namespace System.Reflection.Metadata.Tests
{
    public class MetadataTokensTests
    {
        private static readonly Handle _assemblyRefHandle = AssemblyReferenceHandle.FromRowId(1);
        private static readonly Handle _virtualAssemblyRefHandle = AssemblyReferenceHandle.FromVirtualIndex(AssemblyReferenceHandle.VirtualIndex.System_Runtime);
        private static readonly Handle _virtualBlobHandle = BlobHandle.FromVirtualIndex(BlobHandle.VirtualIndex.AttributeUsage_AllowSingle, 0);
        private static readonly Handle _userStringHandle = UserStringHandle.FromIndex(1);
        private static readonly Handle _stringHandle = StringHandle.FromIndex(1);
        private static readonly Handle _winrtPrefixedStringHandle = StringHandle.FromIndex(1).WithWinRTPrefix();
        private static readonly Handle _blobHandle = BlobHandle.FromIndex(1);
        private static readonly Handle _guidHandle = GuidHandle.FromIndex(16);

        [Fact]
        public void GetRowNumber()
        {
            Assert.Equal(1, MetadataTokens.GetRowNumber(_assemblyRefHandle));
            Assert.Equal(-1, MetadataTokens.GetRowNumber(_virtualAssemblyRefHandle));

            Assert.Throws<ArgumentException>(() => MetadataTokens.GetRowNumber(_virtualBlobHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetRowNumber(_userStringHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetRowNumber(_stringHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetRowNumber(_winrtPrefixedStringHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetRowNumber(_blobHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetRowNumber(_guidHandle));
        }

        [Fact]
        public void GetHeapOffset()
        {
            Assert.Equal(-1, MetadataTokens.GetHeapOffset(_virtualBlobHandle));
            Assert.Equal(1, MetadataTokens.GetHeapOffset(_userStringHandle));
            Assert.Equal(1, MetadataTokens.GetHeapOffset(_stringHandle));
            Assert.Equal(-1, MetadataTokens.GetHeapOffset(_winrtPrefixedStringHandle));
            Assert.Equal(1, MetadataTokens.GetHeapOffset(_blobHandle));
            Assert.Equal(16, MetadataTokens.GetHeapOffset(_guidHandle));

            Assert.Throws<ArgumentException>(() => MetadataTokens.GetHeapOffset(_assemblyRefHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetHeapOffset(_virtualAssemblyRefHandle));
        }

        [Fact]
        public void GetToken()
        {
            Assert.Equal(0x23000001, MetadataTokens.GetToken(_assemblyRefHandle));
            Assert.Equal(0, MetadataTokens.GetToken(_virtualAssemblyRefHandle));
            Assert.Equal(0x70000001, MetadataTokens.GetToken(_userStringHandle));

            Assert.Throws<ArgumentException>(() => MetadataTokens.GetToken(_virtualBlobHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetToken(_stringHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetToken(_winrtPrefixedStringHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetToken(_blobHandle));
            Assert.Throws<ArgumentException>(() => MetadataTokens.GetToken(_guidHandle));
        }

        [Fact]
        public void CreateHandle()
        {
            Assert.Equal(_assemblyRefHandle, MetadataTokens.Handle(0x23000001));
            Assert.Equal(_userStringHandle, MetadataTokens.Handle(0x70000001));

            Assert.Throws<ArgumentException>(() => MetadataTokens.Handle(-1));
            Assert.Throws<ArgumentException>(() => MetadataTokens.Handle(0x71000001));
            Assert.Throws<ArgumentException>(() => MetadataTokens.Handle(0x72000001));
            Assert.Throws<ArgumentException>(() => MetadataTokens.Handle(0x73000001));
            Assert.Throws<ArgumentException>(() => MetadataTokens.Handle(0x74000001));
            Assert.Throws<ArgumentException>(() => MetadataTokens.Handle(0x7a000001));
            Assert.Throws<ArgumentException>(() => MetadataTokens.Handle(0x7e000001));
            Assert.Throws<ArgumentException>(() => MetadataTokens.Handle(0x7fffffff));
        }
    }
}
