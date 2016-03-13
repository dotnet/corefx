// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Reflection.Metadata.Ecma335.Tests
{
    public class MetadataSerializerTests
    {
        [Fact]
        public void TypeSystemSerializer_EmptyRowCounts()
        {
            var serializer = CreateTSSerializer();
            var externalRowCounts = serializer.MetadataSizes.ExternalRowCounts;
            Assert.Equal(MetadataTokens.TableCount, externalRowCounts.Length);
            Assert.DoesNotContain(externalRowCounts, count => count != 0); // array should be zeroed out
        }

        private TypeSystemMetadataSerializer CreateTSSerializer() =>
            new TypeSystemMetadataSerializer(new MetadataBuilder(), "PDB v1.0", false);
    }
}
