// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Net.Mail
{
    internal static class SmtpAuthenticationManager
    {
        private static List<ISmtpAuthenticationModule> s_modules = new List<ISmtpAuthenticationModule>();

        static SmtpAuthenticationManager()
        {
            Register(new SmtpNegotiateAuthenticationModule());
            Register(new SmtpNtlmAuthenticationModule());
            Register(new SmtpLoginAuthenticationModule());
        }

        internal static void Register(ISmtpAuthenticationModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            lock (s_modules)
            {
                s_modules.Add(module);
            }
        }

        internal static ISmtpAuthenticationModule[] GetModules()
        {
            lock (s_modules)
            {
                ISmtpAuthenticationModule[] copy = new ISmtpAuthenticationModule[s_modules.Count];
                s_modules.CopyTo(0, copy, 0, s_modules.Count);
                return copy;
            }
        }
    }
}
