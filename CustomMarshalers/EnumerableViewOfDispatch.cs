using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace System.Runtime.InteropServices.CustomMarshalers
{
    [Serializable]
    internal class EnumerableViewOfDispatch : ICustomAdapter, IEnumerable
    {
        private const int DispId_NewEnum = -4;
        private readonly object dispatch;

        public EnumerableViewOfDispatch(object dispatch)
        {
            this.dispatch = dispatch;
        }

        private IDispatch Dispatch => (IDispatch)dispatch;

        public IEnumerator GetEnumerator()
        {
            DISPPARAMS dispParams = default;
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
                throw new InvalidOperationException("InvalidOp_InvalidNewEnumVariant");
            }

            return (IEnumerator)EnumeratorToEnumVariantMarshaler.GetInstance(null).MarshalNativeToManaged(Marshal.GetIUnknownForObject(enumVariant));
        }

        public object GetUnderlyingObject()
        {
            return dispatch;
        }
    }
}
