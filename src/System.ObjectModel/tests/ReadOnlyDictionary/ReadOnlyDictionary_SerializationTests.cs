// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Tests;
using Xunit;

namespace System.Collections.ObjectModel.Tests
{
    public partial class ReadOnlyDictionary_Serialization
    {
        public static IEnumerable<object[]> SerializeDeserialize_Roundtrips_MemberData()
        {
            yield return new object[] { new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()) };
            yield return new object[] { new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() { { "a", "b" } }) };
            yield return new object[] { new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() { { "a", "b" }, { "c", "d" } }) };
        }

        [Theory]
        [MemberData(nameof(SerializeDeserialize_Roundtrips_MemberData))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapAot, "Serialization still WIP in UapAot: https://github.com/dotnet/corefx/issues/18942")]
        public void SerializeDeserialize_Roundtrips(ReadOnlyDictionary<string, string> d)
        {
            ReadOnlyDictionary<string, string> clone = BinaryFormatterHelpers.Clone(d);
            Assert.NotSame(d, clone);
            Assert.Equal(d, clone);
        }
    }
}
