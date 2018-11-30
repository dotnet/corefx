﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace System.Runtime.InteropServices.CustomMarshalers
{
    internal static class ComDataHelpers
    {
        public static TView GetOrCreateManagedViewFromComData<T, TView>(object comObject, Func<T, TView> createCallback)
        {
            object key = typeof(TView);

            if (Marshal.GetComObjectData(comObject, key) is TView managedView)
            {
                return managedView;
            }
            
            managedView = createCallback((T)comObject);
            if (!Marshal.SetComObjectData(comObject, key, managedView))
            {
                managedView = (TView)Marshal.GetComObjectData(comObject, key);
            }
            return managedView;
        }
    }
}
