// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceProcess
{
    internal  class Helpers
    {
        public const int MaxNameLength = 80;

        internal static bool ValidServiceName(string serviceName)
        {
            if (serviceName == null)
                return false;

            // not too long and check for empty name as well.
            if (serviceName.Length > MaxNameLength || serviceName.Length == 0)
                return false;

            // no slashes or backslash allowed
            foreach (char c in serviceName)
            {
                if ((c == '\\') || (c == '/'))
                    return false;
            }

            return true;
        }
    }
}
