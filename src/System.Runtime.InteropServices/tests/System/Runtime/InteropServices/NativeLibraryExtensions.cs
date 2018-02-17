// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.InteropServices
{
    public static class NativeLibraryExtensions
    {
        public static TDelegate GetDelegate<TDelegate>(this NativeLibrary nativeLibrary, string name, bool exactSpelling = false) where TDelegate : class
        {
            nativeLibrary.TryGetDelegate<TDelegate>(name, exactSpelling, out var retVal);
            return retVal;
        }

        // Converts any delegate of signature () -> TReturn to Func<TReturn>
        public static Func<TReturn> GetDelegateReturning<TReturn>(this NativeLibrary nativeLibrary, Type delegateType, string name, bool exactSpelling = false)
        {
            // Use reflection to bind 'delegateType' to TDelegate, then invoke.
            var invocation = (Func<NativeLibrary, string, bool, Delegate>)typeof(NativeLibraryExtensions).GetMethod("GetDelegate").MakeGenericMethod(delegateType).CreateDelegate(typeof(Func<NativeLibrary, string, bool, Delegate>));
            Delegate innerDelegate = invocation(nativeLibrary, name, exactSpelling);

            return (innerDelegate != null)
                ? (Func<TReturn>)innerDelegate.GetType().GetMethod("Invoke").CreateDelegate(typeof(Func<TReturn>), innerDelegate)
                : null;
        }
    }
}
