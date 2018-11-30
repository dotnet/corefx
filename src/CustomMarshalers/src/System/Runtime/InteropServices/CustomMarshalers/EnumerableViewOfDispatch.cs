// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace System.Runtime.InteropServices.CustomMarshalers
{
    internal class EnumerableViewOfDispatch : ICustomAdapter, IEnumerable
    {
        private const int DispId_NewEnum = -4;
        private readonly object _dispatch;

        public EnumerableViewOfDispatch(object dispatch)
        {
            _dispatch = dispatch;
        }

        private IDispatch Dispatch => (IDispatch)_dispatch;

        public IEnumerator GetEnumerator()
        {
            DISPPARAMS dispParams = new DISPPARAMS();
            Guid guid = Guid.Empty;
            Dispatch.Invoke(
                DispId_NewEnum,
                ref guid,
                1,
                InvokeFlags.Method | InvokeFlags.PropertyGet,
                ref dispParams,
                out object result,
                IntPtr.Zero,
                IntPtr.Zero);

            if (!(result is IEnumVARIANT enumVariant))
            {
                throw new InvalidOperationException(SR.InvalidOp_InvalidNewEnumVariant);
            }

            return (IEnumerator)EnumeratorToEnumVariantMarshaler.GetInstance(null).MarshalNativeToManaged(Marshal.GetIUnknownForObject(enumVariant));
        }

        public object GetUnderlyingObject()
        {
            return _dispatch;
        }
    }
}
