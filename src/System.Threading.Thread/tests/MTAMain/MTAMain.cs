// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;

namespace MTAMain
{
    internal static class Program
    {
        [MTAThread]
        static int Main(string[] args)
        {
            string mode = args[0];
            int retValue = 1;
            Thread curThread = Thread.CurrentThread;

            if (mode == "GetApartmentState")
            {
                if (curThread.GetApartmentState() == ApartmentState.MTA)
                {
                    curThread.SetApartmentState(ApartmentState.MTA);
                    retValue = 0;
                }

            }
            else
            {
                try
                {
                    curThread.SetApartmentState(ApartmentState.STA);
                }
                catch (InvalidOperationException)
                {
                    retValue = 0;
                }
                catch (PlatformNotSupportedException) {}
            }

            return retValue;
        }
    }
}
