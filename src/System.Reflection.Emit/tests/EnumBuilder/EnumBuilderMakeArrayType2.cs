// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Globalization;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class EnumBuilderMakeArrayType2
    {
        private AssemblyBuilder _myAssemblyBuilder;
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();

        private ModuleBuilder CreateCallee()
        {
            AssemblyName myAssemblyName = new AssemblyName();
            myAssemblyName.Name = "EnumAssembly";
            _myAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.Run);
            return TestLibrary.Utilities.GetModuleBuilder(_myAssemblyBuilder, "EnumModule.mod");
        }

        [Fact]
        public void TestMakeArrayTypeWithRank1()
        {
            var myModuleBuilder = CreateCallee();
            var myEnumBuilder = myModuleBuilder.DefineEnum("myEnum", TypeAttributes.Public, typeof(int));
            int rank = 1;
            Type myType = myEnumBuilder.MakeArrayType(rank);
            Assert.True(myType.GetTypeInfo().BaseType.Equals(typeof(Array)));
            Assert.Equal(myType.Name, "myEnum[*]");
        }

        [Fact]
        public void TestMakeArrayTypeWithRank2()
        {
            string szranks = null;
            var myModuleBuilder = CreateCallee();
            var myEnumBuilder = myModuleBuilder.DefineEnum("myEnum", TypeAttributes.Public, typeof(int));
            int rank = GetInt32(2, 256);
            for (int i = 1; i < rank; i++)
                szranks += ",";
            string s = string.Format("[{0}]", szranks);
            Type myType = myEnumBuilder.MakeArrayType(rank);
            Assert.True(myType.GetTypeInfo().BaseType.Equals(typeof(Array)));
            Assert.Equal(myType.Name, "myEnum" + s);
        }

        [Fact]
        public void TestThrowsExceptionOnInvalidRank()
        {
            var myModuleBuilder = CreateCallee();
            var myEnumBuilder = myModuleBuilder.DefineEnum("myEnum", TypeAttributes.Public, typeof(int));
            int rank = GetInt32(0, int.MaxValue) * (-1);
            Assert.Throws<IndexOutOfRangeException>(() => { Type myType = myEnumBuilder.MakeArrayType(rank); });
        }

        private int GetInt32(int minValue, int maxValue)
        {
            try
            {
                if (minValue == maxValue)
                {
                    return minValue;
                }
                if (minValue < maxValue)
                {
                    return minValue + _generator.GetInt32() % (maxValue - minValue);
                }
            }
            catch
            {
                throw;
            }

            return minValue;
        }
    }
}
