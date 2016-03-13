// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Reflection.Metadata.Ecma335.Tests
{
    public class MetadataBuilderTests
    {
        [Fact]
        public void GetHeapSizes()
        {
            var builder = new MetadataBuilder();
            builder.CompleteHeaps(); // do this so _stringWriter is initialized and we don't nullref
            var heapSizes = builder.GetHeapSizes();
            Assert.Equal(MetadataTokens.HeapCount, heapSizes.Length);
        }

        [Fact]
        public void GetRowCounts()
        {
            var builder = new MetadataBuilder();
            var rowCounts = builder.GetRowCounts();
            Assert.Equal(MetadataTokens.TableCount, rowCounts.Length);
        }
    }
}
