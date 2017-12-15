// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Reflection;
using Xunit;

namespace VoidMainWithExitCodeApp
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            PropertyInfo set_ExitCode = typeof(Environment).GetTypeInfo().GetDeclaredProperty("ExitCode");
            MethodInfo Exit = typeof(Environment).GetTypeInfo().GetDeclaredMethod("Exit");
            int retValue = 0;

            int mode = int.Parse(args[0]);
            if (mode == 1)
            {
                if (Thread.CurrentThread.GetApartmentState().ToString().Equals("Unknown"))
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
            Exit.Invoke(null, new object[] { retValue });
        }
    }
}
