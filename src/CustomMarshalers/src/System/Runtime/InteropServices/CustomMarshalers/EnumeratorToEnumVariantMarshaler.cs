﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace System.Runtime.InteropServices.CustomMarshalers
{
    public class EnumeratorToEnumVariantMarshaler : ICustomMarshaler
    {
        private static readonly EnumeratorToEnumVariantMarshaler Instance = new EnumeratorToEnumVariantMarshaler();

        public static ICustomMarshaler GetInstance(string cookie) => Instance;

        private EnumeratorToEnumVariantMarshaler()
        {
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.Release(pNativeData);
        }

        public int GetNativeDataSize()
        {
            // Return -1 to indicate the managed type this marshaler handles is not a value type.
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj == null)
            {
                throw new ArgumentNullException(nameof(ManagedObj));
            }

            EnumVariantViewOfEnumerator nativeView = new EnumVariantViewOfEnumerator((IEnumerator)ManagedObj);

            return Marshal.GetComInterfaceForObject<EnumVariantViewOfEnumerator, IEnumVARIANT>(nativeView);
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(pNativeData));
            }

            object comObject = Marshal.GetObjectForIUnknown(pNativeData);

            if (!comObject.GetType().IsCOMObject)
            {
                if (comObject is EnumVariantViewOfEnumerator enumVariantView)
                {
                    return enumVariantView.Enumerator;
                }

                return comObject as IEnumerator;
            }

            return ComDataHelpers.GetOrCreateManagedViewFromComData<IEnumVARIANT, EnumeratorViewOfEnumVariant>(comObject, var => new EnumeratorViewOfEnumVariant(var));
        }
    }
}
