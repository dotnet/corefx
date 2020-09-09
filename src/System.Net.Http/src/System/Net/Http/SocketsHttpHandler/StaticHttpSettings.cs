// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net.Http
{
    internal static class StaticHttpSettings
    {
        private const string AllowLatin1CharactersEnvironmentVariableSettingName = "DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_ALLOWLATIN1HEADERS";
        private const string AllowLatin1CharactersAppCtxSettingName = "System.Net.Http.SocketsHttpHandler.AllowLatin1Headers";

        // Disables a validation that checks whether Http headers contain a non-ASCII character.
        // This is a workaround that has been introduced as a patch specific to the 3.1 branch.
        // Unlike options in HttpConnectionSettings, this one has a global scope.
        // Lazy initialization is being used to make sure clients can also use AppContext.SetSwitch() or Environment.SetEnvironmentVariable()
        // before the first calls to HttpClient API-s.
        internal static bool AllowLatin1Headers { get; } = GetAllowLatin1HeadersSetting();

        internal static int EncodingValidationMask => AllowLatin1Headers ? 0xFF00 : 0xFF80;

        private static bool GetAllowLatin1HeadersSetting()
        {
            // First check for the AppContext switch, giving it priority over the environment variable.
            if (AppContext.TryGetSwitch(AllowLatin1CharactersAppCtxSettingName, out bool value))
            {
                return value;
            }

            // AppContext switch wasn't used. Check the environment variable.
            string envVar = Environment.GetEnvironmentVariable(AllowLatin1CharactersEnvironmentVariableSettingName);
            if (envVar != null && (envVar.Equals("true", StringComparison.OrdinalIgnoreCase) || envVar.Equals("1")))
            {
                return true;
            }

            return false;
        }
    }
}
