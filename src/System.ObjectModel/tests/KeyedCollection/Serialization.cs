// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Tests;
using Xunit;

namespace System.Collections.ObjectModel.Tests
{
    public partial class KeyedCollection_Serialization
    {
        public static IEnumerable<object[]> SerializeDeserialize_Roundtrips_MemberData()
        {
            yield return new object[] { new TestCollection() };
            yield return new object[] { new TestCollection() { "hello" } };
            yield return new object[] { new TestCollection() { "hello", "world" } };
        }

        [Theory]
        [MemberData(nameof(SerializeDeserialize_Roundtrips_MemberData))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapAot, "Serialization still WIP in UapAot: https://github.com/dotnet/corefx/issues/18942")]
        public void SerializeDeserialize_Roundtrips(TestCollection c)
        {
            TestCollection clone = BinaryFormatterHelpers.Clone(c);
            Assert.NotSame(c, clone);
            Assert.Equal(c, clone);
        }

        [Serializable]
        public sealed class TestCollection : KeyedCollection<string, string>
        {
            protected override string GetKeyForItem(string item) => item;
        }
    }
}
