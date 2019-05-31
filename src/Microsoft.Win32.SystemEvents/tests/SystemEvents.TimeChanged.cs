﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using static Interop;

namespace Microsoft.Win32.SystemEventsTests
{
    public class TimeChangedTests : GenericEventTests
    {
        protected override int MessageId => User32.WM_TIMECHANGE;

        protected override event EventHandler Event
        {
            add
            {
                SystemEvents.TimeChanged += value;
            }
            remove
            {
                SystemEvents.TimeChanged -= value;
            }
        }
    }
}
