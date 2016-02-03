// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Linq;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class EnumBuilderSetCustomAttribute1
    {
        private AssemblyBuilder _myAssemblyBuilder;
        private ModuleBuilder _myModuleBuilder;
        private EnumBuilder _myEnumBuilder;
        private ConstructorInfo _myInfo;

        private void CreateCallee()
        {
            AssemblyName myAssemblyName = new AssemblyName();
            myAssemblyName.Name = "EnumAssembly";
            _myAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.Run);
            _myModuleBuilder = TestLibrary.Utilities.GetModuleBuilder(_myAssemblyBuilder, "EnumModule.mod");
            _myInfo = typeof(EBMyAttribute).GetConstructor(new Type[] { typeof(bool) });
        }

        [Fact]
        public void TestSetCustomAttribute()
        {
            CreateCallee();
            _myEnumBuilder = _myModuleBuilder.DefineEnum("myEnum", TypeAttributes.Public, typeof(int));
            _myEnumBuilder.CreateTypeInfo().AsType();
            _myEnumBuilder.SetCustomAttribute(_myInfo, new byte[] { 01, 00, 01 });
            object[] objVals = _myEnumBuilder.GetCustomAttributes(true).Select(a => (object)a).ToArray().Select(a => (object)a).ToArray();
            Assert.Equal(1, objVals.Length);
            Assert.True(objVals[0].Equals(new EBMyAttribute(true)));
        }

        public class EBMyAttribute : Attribute
        {
            private bool _myBoolValue;

            public EBMyAttribute(bool myBool)
            {
                _myBoolValue = myBool;
            }
        }
    }
}
