// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using System.Collections.Generic;
using Xunit;

namespace System.Runtime.Serialization.Xml.Tests.Performance
{
    public class DcsDeserializationTests
    {
        public static IEnumerable<object[]> SerializeMemberData()
        {
            return PerformanceTestCommon.PerformanceMemberData(SerializerType.DataContractSerializer);
        }

        [Benchmark]
        [MemberData(nameof(SerializeMemberData))]
        public void DcsDeSerializationTest(int iterations, TestType testType, int testSize, SerializerType serializerType)
        {
            PerformanceTestCommon.RunDeSerializationPerformanceTest(iterations, serializerType, testType, testSize, DcsSerializerFactory.GetInstance());
        }
    }
}
