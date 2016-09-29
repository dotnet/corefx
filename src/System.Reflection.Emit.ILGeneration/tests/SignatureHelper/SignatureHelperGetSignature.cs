// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class SignatureHelperGetSignature
    {
        [Fact]
        public void GetSignature_Length_ReturnsOne()
        {
            ModuleBuilder module = Helpers.DynamicModule(); 
            SignatureHelper helper = SignatureHelper.GetFieldSigHelper(module);

            Assert.Equal(1, helper.GetSignature().Length);
        }
    }
}
