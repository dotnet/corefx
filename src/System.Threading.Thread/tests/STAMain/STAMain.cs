// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;

namespace STAMain
{
    internal static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            int retValue = 0;

            bool mode = bool.Parse(args[0]);
            if (mode)
            {
                if (Thread.CurrentThread.GetApartmentState().Equals(ApartmentState.STA))
                {
                    retValue = 1;
                }
                
            }
            else
            {
                try
                {
                    Thread.CurrentThread.SetApartmentState(ApartmentState.MTA);
                }
                catch(InvalidOperationException)
                {
                    retValue = 1;
                }
            }

            return retValue;
        }
    }
}
