// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.NetworkInformation
{
    public class PingException : InvalidOperationException
    {
        internal PingException() { }

        public PingException(string message) : base(message)
        {
        }

        public PingException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}

