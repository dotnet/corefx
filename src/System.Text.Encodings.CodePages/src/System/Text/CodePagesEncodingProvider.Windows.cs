// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text
{
    public sealed partial class CodePagesEncodingProvider : EncodingProvider
    {
        /// <summary>Get the system default non-unicode code page, or 0 if not available.</summary>
        private static int SystemDefaultCodePage
        {
            get
            {
                // Note: GetACP is not available in the Windows Store Profile, but calling
                // GetCPInfoEx with the value CP_ACP (0) yields the same result.
                Interop.CPINFOEXW cpInfo;
                return Interop.GetCPInfoExW(Interop.CP_ACP, 0, out cpInfo) ?
                    (int)cpInfo.CodePage : 0;
            }
        }
    }
}