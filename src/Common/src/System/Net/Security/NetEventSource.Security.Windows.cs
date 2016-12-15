// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;
using System.Net.Security;

namespace System.Net
{
    internal sealed partial class NetEventSource
    {
        [Event(AcquireDefaultCredentialId, Keywords = Keywords.Default, Level = EventLevel.Informational)]
        public void AcquireDefaultCredential(string packageName, Interop.SspiCli.CredentialUse intent)
        {
            if (IsEnabled())
            {
                WriteEvent(AcquireDefaultCredentialId, packageName, intent);
            }
        }

        [NonEvent]
        public void AcquireCredentialsHandle(string packageName, Interop.SspiCli.CredentialUse intent, object authdata)
        {
            if (IsEnabled())
            {
                AcquireCredentialsHandle(packageName, intent, IdOf(authdata));
            }
        }

        [Event(AcquireCredentialsHandleId, Keywords = Keywords.Default, Level = EventLevel.Informational)]
        public void AcquireCredentialsHandle(string packageName, Interop.SspiCli.CredentialUse intent, string authdata)
        {
            if (IsEnabled())
            {
                WriteEvent(AcquireCredentialsHandleId, packageName, (int)intent, authdata);
            }
        }

        [NonEvent]
        public void InitializeSecurityContext(SafeFreeCredentials credential, SafeDeleteContext context, string targetName, Interop.SspiCli.ContextFlags inFlags)
        {
            if (IsEnabled())
            {
                InitializeSecurityContext(IdOf(credential), IdOf(context), targetName, inFlags);
            }
        }
        [Event(InitializeSecurityContextId, Keywords = Keywords.Default, Level = EventLevel.Informational)]
        private void InitializeSecurityContext(string credential, string context, string targetName, Interop.SspiCli.ContextFlags inFlags) =>
            WriteEvent(InitializeSecurityContextId, credential, context, targetName, (int)inFlags);

        [NonEvent]
        public void AcceptSecurityContext(SafeFreeCredentials credential, SafeDeleteContext context, Interop.SspiCli.ContextFlags inFlags)
        {
            if (IsEnabled())
            {
                AcceptSecurityContext(IdOf(credential), IdOf(context), inFlags);
            }
        }
        [Event(AcceptSecuritContextId, Keywords = Keywords.Default, Level = EventLevel.Informational)]
        private void AcceptSecurityContext(string credential, string context, Interop.SspiCli.ContextFlags inFlags) =>
            WriteEvent(AcceptSecuritContextId, credential, context, (int)inFlags);

        [Event(OperationReturnedSomethingId, Keywords = Keywords.Default, Level = EventLevel.Informational)]
        public void OperationReturnedSomething(string operation, Interop.SECURITY_STATUS errorCode)
        {
            if (IsEnabled())
            {
                WriteEvent(OperationReturnedSomethingId, operation, errorCode);
            }
        }

        [Event(SecurityContextInputBufferId, Keywords = Keywords.Default, Level = EventLevel.Informational)]
        public void SecurityContextInputBuffer(string context, int inputBufferSize, int outputBufferSize, Interop.SECURITY_STATUS errorCode)
        {
            if (IsEnabled())
            {
                WriteEvent(SecurityContextInputBufferId, context, inputBufferSize, outputBufferSize, (int)errorCode);
            }
        }

        [Event(SecurityContextInputBuffersId, Keywords = Keywords.Default, Level = EventLevel.Informational)]
        public void SecurityContextInputBuffers(string context, int inputBuffersSize, int outputBufferSize, Interop.SECURITY_STATUS errorCode)
        {
            if (IsEnabled())
            {
                WriteEvent(SecurityContextInputBuffersId, context, inputBuffersSize, outputBufferSize, (int)errorCode);
            }
        }
    }
}
