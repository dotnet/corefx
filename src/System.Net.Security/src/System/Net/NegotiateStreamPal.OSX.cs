// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
    internal static partial class NegotiateStreamPal
    {
        private static bool EstablishNtlmSecurityContext(
            SafeFreeNegoCredentials credential,
            ref SafeDeleteContext context,
            string targetName,
            ContextFlagsPal inFlags,
            SecurityBuffer inputBuffer,
            SecurityBuffer outputBuffer,
            ref ContextFlagsPal outFlags)
        {
            if (context == null)
            {
                context = new SafeDeleteNegoContext(credential);
            }

            SafeDeleteNegoContext negoContext = (SafeDeleteNegoContext) context;
            SafeGssContextHandle contextHandle = negoContext.GssContext;

            uint outputFlags;
            Interop.NetSecurityNative.GssFlags inputFlags = ContextFlagsAdapterPal.GetInteropFromContextFlagsPal(inFlags);
            bool done = Interop.GssApi.EstablishSecurityContext(
                ref contextHandle,
                credential.GssCredential,
                true,
                negoContext.TargetName,
                inputFlags,
                ((inputBuffer != null) ? inputBuffer.token : null),
                out outputBuffer.token,
                out outputFlags);

            outFlags = ContextFlagsAdapterPal.GetContextFlagsPalFromInterop((Interop.NetSecurityNative.GssFlags) outputFlags);

            // Save the inner context handle for further calls to NetSecurityNative
            if (null == negoContext.GssContext)
            {
                negoContext.SetGssContext(contextHandle, true);
            }

            return done;
        }
    }
}
