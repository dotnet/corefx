// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    partial class ProcessTestBase
    {
        protected const string NetfxRunnerName = "RemoteInvokeNetfx";
        protected static readonly string RunnerName = $"{NetfxRunnerName}.exe";

        protected Process CreateProcessLong()
        {
            return CreateNetfxProcess(RemotelyInvokable.LongWait);
        }

        protected Process CreateProcessPortable(Func<int> func)
        {
            return CreateNetfxProcess(func);
        }

        protected Process CreateProcessPortable(Func<string, int> func, string arg)
        {
            return CreateNetfxProcess(func, arg);
        }

        protected Process CreateNetfxProcess(Func<int> func)
        {
            return CreateNetfxProcess(func.Method, Array.Empty<string>());
        }

        protected Process CreateNetfxProcess(Func<string, int> func, string arg)
        {
            return CreateNetfxProcess(func.Method, new string[] { arg });
        }

        protected Process CreateNetfxProcess(MethodInfo method, string[] args)
        {
            if (method.DeclaringType != typeof(RemotelyInvokable))
            {
                throw new Exception($"Method needs to be defined in {nameof(RemotelyInvokable)} class.");
            }

            return new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = RunnerName,
                    Arguments = $"{method.Name} {PasteArguments.Paste(args, false)}"
                }
            };
        }
    }
}
