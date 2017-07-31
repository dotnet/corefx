// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net.Test.Common
{
    public sealed class DummyWebProxy : IWebProxy
    {
        public DummyWebProxy() { }

        public ICredentials Credentials
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Uri GetProxy(Uri destination)
        {
            throw new NotImplementedException();
        }

        public bool IsBypassed(Uri host)
        {
            throw new NotImplementedException();
        }
    }
}
