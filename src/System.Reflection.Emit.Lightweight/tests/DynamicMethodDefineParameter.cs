// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class DynamicMethodDefineParameter
    {
        [Fact]
        public void DefineParameter_SetsParameterName()
        {
            Type[] mathArgs = { typeof(double), typeof(double) };

            var powerOf = new DynamicMethod("PowerOf",
                typeof(double),
                mathArgs,
                typeof(double).Module);

            ILGenerator il = powerOf.GetILGenerator(256);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, typeof(Math).GetMethod("Pow"));
            il.Emit(OpCodes.Ret);

            var paramNames = new string[] { "param1", "param2" };
            for (int i = 0; i < paramNames.Length; i++)
            {
                powerOf.DefineParameter(i+1, ParameterAttributes.In, paramNames[i]);
            }
            
            object[] invokeArgs = { 2, 5 };
            object objRet = powerOf.Invoke(null, BindingFlags.ExactBinding, null, invokeArgs, new CultureInfo("en-us"));

            ParameterInfo[] parameters = powerOf.GetParameters();

            Assert.Equal(paramNames.Length, parameters.Length);
            Assert.Equal("param1", parameters[0].Name);
            Assert.Equal("param2", parameters[1].Name);
        }
    }
}
