// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;

namespace Unknown
{
    internal static class Program
    {
        static int Main(string[] args)
        {
            int retValue = 0;

            int mode = int.Parse(args[0]);
            if (mode == 1)
            {
                if (Thread.CurrentThread.GetApartmentState().Equals(ApartmentState.Unknown))
                {
                    retValue = 1;
                }
            }
            else if (mode == 2)
            {
                try
                {
                    Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
                    retValue = 1;
                }
                catch (InvalidOperationException){ }
            }
            else
            {
                try
                {
                    Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
                    retValue = 1;
                }
                catch (InvalidOperationException){ }
            }

            return retValue;
        }
    }
}
