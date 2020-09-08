// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net.Http
{
    internal static class StaticHttpSettings
    {
        private const string AllowNonAsciiCharactersEnvironmentVariableSettingName = "DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_ALLOWNONASCIIHEADERS";
        private const string AllowNonAsciiCharactersAppCtxSettingName = "System.Net.Http.SocketsHttpHandler.AllowNonAsciiHeaders";

        private static readonly Lazy<bool> s_allowNonAsciiHeaders = new Lazy<bool>(GetAllowNonAsciiCharactersSetting);

        // Disables a validation that checks whether Http headers contain a non-ASCII character.
        // This is a workaround that has been introduced as a patch specific to the 3.1 branch.
        // Unlike options in HttpConnectionSettings, this one has a global scope.
        // Lazy initialization is being used to make sure clients can also use AppContext.SetSwitch() or Environment.SetEnvironmentVariable()
        // before the first calls to HttpClient API-s.
        internal static bool AllowNonAsciiHeaders => s_allowNonAsciiHeaders.Value;

        internal static int EncodingValidationMask => AllowNonAsciiHeaders ? 0xFF00 : 0xFF80;

        private static bool GetAllowNonAsciiCharactersSetting()
        {
            // First check for the AppContext switch, giving it priority over the environment variable.
            if (AppContext.TryGetSwitch(AllowNonAsciiCharactersAppCtxSettingName, out bool value))
            {
                return value;
            }

            // AppContext switch wasn't used. Check the environment variable.
            string envVar = Environment.GetEnvironmentVariable(AllowNonAsciiCharactersEnvironmentVariableSettingName);
            if (envVar != null && (envVar.Equals("true", StringComparison.OrdinalIgnoreCase) || envVar.Equals("1")))
            {
                return true;
            }

            return false;
        }
    }
}
