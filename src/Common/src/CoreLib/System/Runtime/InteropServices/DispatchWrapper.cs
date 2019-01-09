// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//

//

/*=============================================================================
**
** Class: DispatchWrapper.
**
**
** Purpose: Wrapper that is converted to a variant with VT_DISPATCH.
**
**
=============================================================================*/

namespace System.Runtime.InteropServices
{
    public sealed class DispatchWrapper
    {
        public DispatchWrapper(Object obj)
        {
            if (obj != null)
            {
#if CORERT
                throw new PlatformNotSupportedException();
#else
                // Make sure this guy has an IDispatch
                IntPtr pdisp = Marshal.GetIDispatchForObject(obj);

                // If we got here without throwing an exception, the QI for IDispatch succeeded.
                Marshal.Release(pdisp);

                WrappedObject = obj;
#endif
            }
        }

        public object WrappedObject { get; }
    }
}
