// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public static class ValueTypeTests
    {
        [Fact]
        public static void ToStringTest()
        {
            object obj = new S();
            Assert.Equal(obj.ToString(), obj.GetType().ToString());
            Assert.Equal("System.Tests.ValueTypeTests+S", obj.ToString());
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The fix was made in coreclr that is not in netfx. See https://github.com/dotnet/coreclr/issues/11452")]
        public static void StructWithDoubleFieldTightlyPackedZeroCompareTest()
        {
            StructWithDoubleFieldTightlyPacked obj1 = new StructWithDoubleFieldTightlyPacked();
            obj1.value1 = 1;
            obj1.value2 = 0.0;

            StructWithDoubleFieldTightlyPacked obj2 = new StructWithDoubleFieldTightlyPacked();
            obj2.value1 = 1;
            obj2.value2 = -0.0;

            Assert.Equal(true, obj1.Equals(obj2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The fix was made in coreclr that is not in netfx. See https://github.com/dotnet/coreclr/issues/11452")]
        public static void StructWithDoubleFieldTightlyPackedNaNCompareTest()
        {
            StructWithDoubleFieldTightlyPacked obj1 = new StructWithDoubleFieldTightlyPacked();
            obj1.value1 = 1;
            obj1.value2 = double.NaN;

            StructWithDoubleFieldTightlyPacked obj2 = new StructWithDoubleFieldTightlyPacked();
            obj2.value1 = 1;
            obj2.value2 = -double.NaN;

            Assert.Equal(true, obj1.Equals(obj2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The fix was made in coreclr that is not in netfx. See https://github.com/dotnet/coreclr/issues/11452")]
        public static void StructWithNestedDoubleFieldZeroCompareTest()
        {
            StructWithDoubleFieldNested obj1 = new StructWithDoubleFieldNested();
            obj1.value1.value1 = 1;
            obj1.value2.value2 = 0.0;

            StructWithDoubleFieldNested obj2 = new StructWithDoubleFieldNested();
            obj2.value1.value1 = 1;
            obj2.value2.value2 = -0.0;

            Assert.Equal(true, obj1.Equals(obj2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The fix was made in coreclr that is not in netfx. See https://github.com/dotnet/coreclr/issues/11452")]
        public static void StructWithNestedDoubleFieldNaNCompareTest()
        {
            StructWithDoubleFieldNested obj1 = new StructWithDoubleFieldNested();
            obj1.value1.value1 = 1;
            obj1.value2.value2 = double.NaN;

            StructWithDoubleFieldNested obj2 = new StructWithDoubleFieldNested();
            obj2.value1.value1 = 1;
            obj2.value2.value2 = -double.NaN;

            Assert.Equal(true, obj1.Equals(obj2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The fix was made in coreclr that is not in netfx. See https://github.com/dotnet/coreclr/issues/11452")]
        public static void StructWithFloatFieldTightlyPackedZeroCompareTest()
        {
            StructWithFloatFieldTightlyPacked obj1 = new StructWithFloatFieldTightlyPacked();
            obj1.value1 = 1;
            obj1.value2 = 0.0f;

            StructWithFloatFieldTightlyPacked obj2 = new StructWithFloatFieldTightlyPacked();
            obj2.value1 = 1;
            obj2.value2 = -0.0f;

            Assert.Equal(true, obj1.Equals(obj2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The fix was made in coreclr that is not in netfx. See https://github.com/dotnet/coreclr/issues/11452")]
        public static void StructWithFloatFieldTightlyPackedNaNCompareTest()
        {
            StructWithFloatFieldTightlyPacked obj1 = new StructWithFloatFieldTightlyPacked();
            obj1.value1 = 1;
            obj1.value2 = float.NaN;

            StructWithFloatFieldTightlyPacked obj2 = new StructWithFloatFieldTightlyPacked();
            obj2.value1 = 1;
            obj2.value2 = -float.NaN;

            Assert.Equal(true, obj1.Equals(obj2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The fix was made in coreclr that is not in netfx. See https://github.com/dotnet/coreclr/issues/11452")]
        public static void StructWithNestedFloatFieldZeroCompareTest()
        {
            StructWithFloatFieldNested obj1 = new StructWithFloatFieldNested();
            obj1.value1.value1 = 1;
            obj1.value2.value2 = 0.0f;

            StructWithFloatFieldNested obj2 = new StructWithFloatFieldNested();
            obj2.value1.value1 = 1;
            obj2.value2.value2 = -0.0f;

            Assert.Equal(true, obj1.Equals(obj2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The fix was made in coreclr that is not in netfx. See https://github.com/dotnet/coreclr/issues/11452")]
        public static void StructWithNestedFloatFieldNaNCompareTest()
        {
            StructWithFloatFieldNested obj1 = new StructWithFloatFieldNested();
            obj1.value1.value1 = 1;
            obj1.value2.value2 = float.NaN;

            StructWithFloatFieldNested obj2 = new StructWithFloatFieldNested();
            obj2.value1.value1 = 1;
            obj2.value2.value2 = -float.NaN;

            Assert.Equal(true, obj1.Equals(obj2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "The fix was made in coreclr that is not in netfx. See https://github.com/dotnet/coreclr/issues/11452")]
        public static void StructWithNestedOverriddenEqualsCompareTest()
        {
            StructWithNestedOverriddenEqualsAndGetHashCode obj1 = new StructWithNestedOverriddenEqualsAndGetHashCode();
            obj1.value1.value = 1;
            obj1.value2.value = 2;

            StructWithNestedOverriddenEqualsAndGetHashCode obj2 = new StructWithNestedOverriddenEqualsAndGetHashCode();
            obj2.value1.value = 1;
            obj2.value2.value = 2;

            Assert.Equal(false, obj1.Equals(obj2));
        }

        public struct S
        {
            public int x;
            public int y;
        }

        public struct StructWithDoubleFieldTightlyPacked
        {
            public double value1;
            public double value2;
        }

        public struct StructWithDoubleFieldNested
        {
            public StructWithDoubleFieldTightlyPacked value1;
            public StructWithDoubleFieldTightlyPacked value2;
        }

        public struct StructWithFloatFieldTightlyPacked
        {
            public float value1;
            public float value2;
        }

        public struct StructWithFloatFieldNested
        {
            public StructWithFloatFieldTightlyPacked value1;
            public StructWithFloatFieldTightlyPacked value2;
        }

        public struct StructNeverEquals
        {
            public byte value;

            public override bool Equals(object obj)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public struct StructWithNestedOverriddenEqualsAndGetHashCode
        {
            public StructNeverEquals value1;
            public StructNeverEquals value2;
        }
    }
}
