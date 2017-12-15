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
        [STAThread]
        static void Main(string[] args)
        {
            PropertyInfo set_ExitCode = typeof(Environment).GetTypeInfo().GetDeclaredProperty("ExitCode");
            MethodInfo Exit = typeof(Environment).GetTypeInfo().GetDeclaredMethod("Exit");
            int retValue = 0;

            bool mode = bool.Parse(args[0]);
            if (mode)
            {
                if (Thread.CurrentThread.GetApartmentState().ToString().Equals("STA"))
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
            Exit.Invoke(null, new object[] { retValue });
        }
    }
}
