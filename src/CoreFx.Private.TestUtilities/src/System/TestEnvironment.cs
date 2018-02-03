// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Xunit;

namespace System
{
    public static partial class TestEnvironment
    {
        /// <summary>
        /// Check if the stress enabling mode environment variable is set.
        /// </summary>
        /// <returns> true if the environment variable set to '1' or 'true'. returns false otherwise</returns>
        public static bool IsStressModeEnabled
        {
            get
            {
                string value = Environment.GetEnvironmentVariable("COREFX_STRESS");
                return value != null && (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
