// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Tests;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace RemoteInvokeNetfx
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine($"Usage: {typeof(Program).GetTypeInfo().Assembly.GetName().Name} methodName");
                return -1;
            }

            string methodName = args[0];
            string[] additionalArgs = args.Skip(1).ToArray();

            MethodInfo mi = typeof(RemotelyInvokable)
                .GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            if (mi == null || mi.ReturnType != typeof(int) || mi.GetParameters().Length != additionalArgs.Length)
            {
                Console.Error.WriteLine($"Method {methodName} could not be found in class {nameof(RemotelyInvokable)}.");
                return -1;
            }

            return (int)mi.Invoke(null, additionalArgs);
        }
    }
}
