// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class AppleCrypto
    {
        internal static Exception CreateExceptionForOSStatus(int osStatus)
        {
            string msg = GetSecErrorString(osStatus);

            if (msg == null)
            {
                return CreateExceptionForCCError(osStatus, "OSStatus");
            }

            return new AppleCommonCryptoCryptographicException(osStatus, msg);
        }

        internal const int PAL_Error_False = 0;
        internal const int PAL_Error_True = 1;

        internal const int PAL_Error_BadInput = -1;
        internal const int PAL_Error_SeeError = -2;
        internal const int PAL_Error_SeeStatus = -3;
        internal const int PAL_Error_Platform = -4;
        internal const int PAL_Error_UnknownState = -5;
        internal const int PAL_Error_UnknownAlgorithm = -6;

        internal const int PAL_Error_UserTrust = -7;
        internal const int PAL_Error_AdminTrust = -8;
        internal const int PAL_Error_OutItemsNull = -9;
        internal const int PAL_Error_OutItemsEmpty = -10;

    }
}
