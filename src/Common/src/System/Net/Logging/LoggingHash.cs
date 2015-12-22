// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System.Net
{
    internal static class LoggingHash
    {
        // Converts an object to a normalized string that can be printed
        // takes System.Net.ObjectNamedFoo and coverts to ObjectNamedFoo, 
        // except IPAddress, IPEndPoint, and Uri, which return ToString().
        internal static string GetObjectName(object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            if (obj is Uri || obj is System.Net.IPAddress || obj is System.Net.IPEndPoint || obj is string)
            {
                return obj.ToString();
            }
            else
            {
                return obj.GetType().ToString();
            }
        }
        internal static int HashInt(object objectValue)
        {
            if (objectValue == null)
            {
                return 0;
            }
            else
            {
                return objectValue.GetHashCode();
            }
        }
    }
}
