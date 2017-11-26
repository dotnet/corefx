// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace Microsoft.CSharp.RuntimeBinder.Tests
{
    public class DefaultParameterTests
    {
        public class MarshalAsMethods
        {
            // Try every defined type that will compile, even if it's nonsense.
            public object AnsiBStr([Optional, MarshalAs(UnmanagedType.AnsiBStr)] object val) => val;

            public object AsAny([Optional, MarshalAs(UnmanagedType.AsAny)] object val) => val;

            public object Bool([Optional, MarshalAs(UnmanagedType.Bool)] object val) => val;

            public object BStr([Optional, MarshalAs(UnmanagedType.BStr)] object val) => val;

            public object Currency([Optional, MarshalAs(UnmanagedType.Currency)] object val) => val;

            public object Error([Optional, MarshalAs(UnmanagedType.Error)] object val) => val;

            public object FunctionPtr([Optional, MarshalAs(UnmanagedType.FunctionPtr)] object val) => val;

            public object HString([Optional, MarshalAs(UnmanagedType.HString)] object val) => val;

            public object I1([Optional, MarshalAs(UnmanagedType.I1)] object val) => val;

            public object I2([Optional, MarshalAs(UnmanagedType.I2)] object val) => val;

            public object I4([Optional, MarshalAs(UnmanagedType.I4)] object val) => val;

            public object I8([Optional, MarshalAs(UnmanagedType.I8)] object val) => val;

            public object IDispatch([Optional, MarshalAs(UnmanagedType.IDispatch)] object val) => val;

            public object IInspectable([Optional, MarshalAs(UnmanagedType.IInspectable)] object val) => val;

            public object Interface([Optional, MarshalAs(UnmanagedType.Interface)] object val) => val;

            public object IUnknown([Optional, MarshalAs(UnmanagedType.IUnknown)] object val) => val;

            public object LPArray([Optional, MarshalAs(UnmanagedType.LPArray)] object val) => val;

            public object LPStr([Optional, MarshalAs(UnmanagedType.LPStr)] object val) => val;

            public object LPStruct([Optional, MarshalAs(UnmanagedType.LPStruct)] object val) => val;

            public object LPTStr([Optional, MarshalAs(UnmanagedType.LPTStr)] object val) => val;

            public object LPWStr([Optional, MarshalAs(UnmanagedType.LPWStr)] object val) => val;

            public object R4([Optional, MarshalAs(UnmanagedType.R4)] object val) => val;

            public object R8([Optional, MarshalAs(UnmanagedType.R8)] object val) => val;

            public object SafeArray([Optional, MarshalAs(UnmanagedType.SafeArray)] object val) => val;

            public object Struct([Optional, MarshalAs(UnmanagedType.Struct)] object val) => val;

            public object SysInt([Optional, MarshalAs(UnmanagedType.SysInt)] object val) => val;

            public object SysUInt([Optional, MarshalAs(UnmanagedType.SysUInt)] object val) => val;

            public object TBStr([Optional, MarshalAs(UnmanagedType.TBStr)] object val) => val;

            public object U1([Optional, MarshalAs(UnmanagedType.U1)] object val) => val;

            public object U2([Optional, MarshalAs(UnmanagedType.U2)] object val) => val;

            public object U4([Optional, MarshalAs(UnmanagedType.U4)] object val) => val;

            public object U8([Optional, MarshalAs(UnmanagedType.U8)] object val) => val;

            public object VariantBool([Optional, MarshalAs(UnmanagedType.VariantBool)] object val) => val;

            public object VBByRefStr([Optional, MarshalAs(UnmanagedType.VBByRefStr)] object val) => val;

            public object UndefinedType([Optional, MarshalAs((UnmanagedType)2000)] object val) => val;
        }

        [Fact]
        public void MarshalAsOptionalsCorrectDefault()
        {
            dynamic d = new MarshalAsMethods();
            Assert.Same(Type.Missing, d.AnsiBStr());
            Assert.Same(Type.Missing, d.AsAny());
            Assert.Same(Type.Missing, d.Bool());
            Assert.Same(Type.Missing, d.BStr());
            Assert.Same(Type.Missing, d.Currency());
            Assert.Same(Type.Missing, d.Error());
            Assert.Same(Type.Missing, d.FunctionPtr());
            Assert.Same(Type.Missing, d.HString());
            Assert.Same(Type.Missing, d.I1());
            Assert.Same(Type.Missing, d.I2());
            Assert.Same(Type.Missing, d.I4());
            Assert.Same(Type.Missing, d.I8());
            Assert.Null(d.IDispatch());
            Assert.Same(Type.Missing, d.IInspectable());
            Assert.Null(d.Interface());
            Assert.Null(d.IUnknown());
            Assert.Same(Type.Missing, d.LPArray());
            Assert.Same(Type.Missing, d.LPStr());
            Assert.Same(Type.Missing, d.LPStruct());
            Assert.Same(Type.Missing, d.LPTStr());
            Assert.Same(Type.Missing, d.LPWStr());
            Assert.Same(Type.Missing, d.R4());
            Assert.Same(Type.Missing, d.R8());
            Assert.Same(Type.Missing, d.SafeArray());
            Assert.Same(Type.Missing, d.Struct());
            Assert.Same(Type.Missing, d.SysInt());
            Assert.Same(Type.Missing, d.SysUInt());
            Assert.Same(Type.Missing, d.TBStr());
            Assert.Same(Type.Missing, d.U1());
            Assert.Same(Type.Missing, d.U2());
            Assert.Same(Type.Missing, d.U4());
            Assert.Same(Type.Missing, d.U8());
            Assert.Same(Type.Missing, d.VariantBool());
            Assert.Same(Type.Missing, d.VBByRefStr());
        }
    }
}
