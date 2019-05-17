// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Runtime.Augments;

namespace System.Globalization
{
    public partial class HijriCalendar : Calendar
    {
        private static int GetHijriDateAdjustment()
        {
            return WinRTInterop.Callbacks.GetHijriDateAdjustment();
        }
    }
}
