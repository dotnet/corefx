// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;

namespace System.Runtime.InteropServices.CustomMarshalers
{
    internal class EnumVariantViewOfEnumerator : IEnumVARIANT, ICustomAdapter, ICustomQueryInterface
    {
        private static readonly Guid IID_IManagedObject = new Guid("C3FCC19E-A970-11d2-8B5A-00A0C9B7C9C4");

        public EnumVariantViewOfEnumerator(IEnumerator enumerator)
        {
            if (enumerator is null)
            {
                throw new ArgumentNullException(nameof(enumerator));
            }

            Enumerator = enumerator;
        }
        internal IEnumerator Enumerator { get; }

        public IEnumVARIANT Clone()
        {
            if (Enumerator is ICloneable clonable)
            {
                return new EnumVariantViewOfEnumerator(clonable.Clone() as IEnumerator);
            }
            else
            {
                throw new COMException(SR.Arg_EnumNotCloneable, HResults.E_FAIL);
            }
        }

        public int Next(int celt, object[] rgVar, IntPtr pceltFetched)
        {
            int numElements = 0;

            try
            {
                if (celt > 0 && rgVar == null)
                {
                    return Marshal.GetHRForException(new ArgumentException());
                }

                while ((numElements < celt) && Enumerator.MoveNext())
                {
                    rgVar[numElements++] = Enumerator.Current;
                }

                if (pceltFetched != IntPtr.Zero)
                {
                    Marshal.WriteInt32(pceltFetched, numElements);
                }
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }

            return numElements == celt ? HResults.S_OK : HResults.S_FALSE;
        }

        public int Reset()
        {
            try
            {
                Enumerator.Reset();
            }
            catch (Exception e)
            {
                Marshal.GetHRForException(e);
            }

            return HResults.S_OK;
        }

        public int Skip(int celt)
        {
            int numElements = 0;
            try
            {
                for (;(numElements < celt) && Enumerator.MoveNext(); numElements++)
                {
                }
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }

            return numElements == celt ? HResults.S_OK : HResults.S_FALSE;
        }

        public CustomQueryInterfaceResult GetInterface(ref Guid iid, out IntPtr ppv)
        {
            // We fail the QI for IManagedObject if the underlying enumerator does not support it.
            // This is to give user code a chance to get COM semantics either by implementing ICustomQI
            // or aggregating with an unmanaged COM object.
            ppv = IntPtr.Zero;
            if (iid == IID_IManagedObject)
            {
                try
                {
                    IntPtr pUnk = Marshal.GetIUnknownForObject(Enumerator);
                    try
                    {
                        if (Marshal.QueryInterface(pUnk, ref iid, out IntPtr pManagedObj) == HResults.S_OK)
                        {
                            Marshal.Release(pManagedObj);
                        }
                        else
                        {
                            return CustomQueryInterfaceResult.Failed;
                        }
                    }
                    finally
                    {
                        Marshal.Release(pUnk);
                    }
                }
                catch (Exception)
                {
                // Swallow all exceptions so we don't impact cases where the the underlying
                // enumerator is not COM visible and thus cannot be QIed.
                }
            }

            return CustomQueryInterfaceResult.NotHandled;
        }

        public object GetUnderlyingObject()
        {
            return Enumerator;
        }
    }
}
