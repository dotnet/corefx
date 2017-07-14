// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Diagnostics.Tests
{
    /// <summary>
    /// This class is used as interop between uap and netfx app running in the same app container.
    /// On uap code running here will be running on netfx.
    /// This is a workaround for a limitation of uap RemoteInvoke which does not give us much control over the process.
    /// </summary>
    internal static class RemotelyInvokable
    {
        public static readonly int SuccessExitCode = 42;
        public const int WaitInMS = 30 * 1000;
        public const string TestConsoleApp = "System.Diagnostics.Process.Tests";

        public static int Dummy()
        {
            return SuccessExitCode;
        }

        public static int LongWait()
        {
            Thread.Sleep(WaitInMS);
            return SuccessExitCode;
        }

        public static int ExitWithCode(string exitCodeStr)
        {
            return int.Parse(exitCodeStr);
        }

        public static int ErrorProcessBody()
        {
            Console.Error.WriteLine(TestConsoleApp + " started error stream");
            Console.Error.WriteLine(TestConsoleApp + " closed error stream");
            return SuccessExitCode;
        }

        public static int StreamBody()
        {
            Console.WriteLine(TestConsoleApp + " started");
            Console.WriteLine(TestConsoleApp + " closed");
            return SuccessExitCode;
        }

        public static int ReadLine()
        {
            Console.ReadLine();
            return SuccessExitCode;
        }

        public static int ReadLineWriteIfNull()
        {
            string line = Console.ReadLine();
            Console.WriteLine(line == null ? "NULL" : "NOT_NULL");
            return SuccessExitCode;
        }

        public static int WriteSlowlyByByte()
        {
            var stdout = Console.OpenStandardOutput();
            var bytes = new byte[] { 97, 0 }; //Encoding.Unicode.GetBytes("a");

            for (int i = 0; i != bytes.Length; ++i)
            {
                stdout.WriteByte(bytes[i]);
                stdout.Flush();
                Thread.Sleep(100);
            }
            return SuccessExitCode;
        }

        public static int Write144Lines()
        {
            for (int i = 0; i < 144; i++)
            {
                Console.WriteLine("This is line #" + i + ".");
            }
            return SuccessExitCode;
        }

        public static int ConcatThreeArguments(string one, string two, string three)
        {
            Console.Write(string.Join(",", one, two, three));
            return SuccessExitCode;
        }

        public static int SelfTerminate()
        {
            Process.GetCurrentProcess().Kill();
            throw new ShouldNotBeInvokedException();
        }
    }
}
