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
    internal class EnumeratorViewOfEnumVariant : ICustomAdapter, IEnumerator
    {
        private readonly IEnumVARIANT m_enumVariantObject;
        private bool m_fetchedLastObject;

        public EnumeratorViewOfEnumVariant(IEnumVARIANT enumVariantObject)
        {
            m_enumVariantObject = enumVariantObject;
            m_fetchedLastObject = false;
            Current = null;

        }

        public object Current { get; private set; }

        public unsafe bool MoveNext()
        {
            if (m_fetchedLastObject)
            {
                Current = null;
                return false;
            }

            object[] next = new object[1];
            int numFetched = 0;

            if (m_enumVariantObject.Next(1, next, (IntPtr)(&numFetched)) == HResults.S_FALSE)
            {
                m_fetchedLastObject = true;

                if (numFetched == 0)
                {
                    Current = null;
                    return false;
                }
            }

            Current = next[0];

            return true;
        }

        public void Reset()
        {
            int hr = m_enumVariantObject.Reset();
            if (hr < 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        public object GetUnderlyingObject()
        {
            return m_enumVariantObject;
        }

    }
}
