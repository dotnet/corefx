// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCPInfoExW")]
        private extern static unsafe int GetCPInfoExW(uint CodePage, uint dwFlags, CPINFOEXW* lpCPInfoEx);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private unsafe struct CPINFOEXW
        {
            internal uint MaxCharSize;
            internal fixed byte DefaultChar[2];
            internal fixed byte LeadByte[12];
            internal char UnicodeDefaultChar;
            internal uint CodePage;
            internal fixed byte CodePageName[260];
        }

        internal static unsafe int GetLeadByteRanges(int codePage, byte[] leadyByteRanges)
        {
            int count = 0;
            CPINFOEXW cpInfo;
            if (GetCPInfoExW((uint) codePage, 0, &cpInfo) != 0)
            {
                // we don't care about the last 2 bytes as those are nulls
                for (int i=0; i<10; i+=2)
                {
                    if (leadyByteRanges[i] == 0)
                        break;
                    leadyByteRanges[i] = cpInfo.LeadByte[i];
                    leadyByteRanges[i+1] = cpInfo.LeadByte[i+1];
                    count++;
                }
            }
            return count;
        }

        internal static bool TryGetACPCodePage(out int codePage)
        {
            codePage = 0;
            // Note: GetACP is not available in the Windows Store Profile, but calling
            // GetCPInfoEx with the value CP_ACP (0) yields the same result.
            CPINFOEXW cpInfo;
            unsafe
            {
                CPINFOEXW* lpCPInfoExPtr = &(cpInfo);
                if (GetCPInfoExW(CP_ACP, 0, lpCPInfoExPtr) != 0)
                {
                    codePage = (int)cpInfo.CodePage;
                    return true;
                }
            }

            return false;
        }
    }
}
