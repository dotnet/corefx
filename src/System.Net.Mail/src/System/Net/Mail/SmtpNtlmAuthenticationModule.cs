// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net.Mail
{
    internal sealed class SmtpNtlmAuthenticationModule : ISmtpAuthenticationModule
    {
        private readonly Dictionary<object, NegotiateAuthState> _sessions = new Dictionary<object, NegotiateAuthState>();

        internal SmtpNtlmAuthenticationModule()
        {
        }

        public Authorization Authenticate(string challenge, NetworkCredential credential, object sessionCookie, string spn, ChannelBinding channelBindingToken)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this, "Authenticate");
            try
            {
                lock (_sessions)
                {
                    NegotiateAuthState clientContext;
                    if (!_sessions.TryGetValue(sessionCookie, out clientContext))
                    {
                        if (credential == null)
                        {
                            return null;
                        }

                        _sessions[sessionCookie] =
                            clientContext =
                            new NegotiateAuthState(false, NegotiationPackages.NTLM, credential, spn, NegotiateAuthFlags.Connection, channelBindingToken);

                    }

                    string resp = clientContext.GetOutgoingBlob(challenge);

                    if (!clientContext.IsCompleted)
                    {
                        return new Authorization(resp, false);
                    }
                    else
                    {
                        clientContext.Dispose();
                        _sessions.Remove(sessionCookie);
                        return new Authorization(resp, true);
                    }
                }
            }
            // From reflected type NegotiateAuthState in System.Net.Security.
            catch (NullReferenceException)
            {
                return null;
            }
            finally
            {
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this, "Authenticate");
            }
        }

        public string AuthenticationType
        {
            get
            {
                return "ntlm";
            }
        }

        public void CloseContext(object sessionCookie)
        {
            // This is a no-op since the context is not
            // kept open by this module beyond auth completion.
        }
    }
}
