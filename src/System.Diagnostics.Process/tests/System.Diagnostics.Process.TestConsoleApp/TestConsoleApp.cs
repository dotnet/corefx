﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO.Pipes;
using System.Threading.Tasks;

namespace System.Diagnostics.ProcessTests
{
    class Program
    {
        private const string Name = "System.Diagnostics.Process.TestConsoleApp.exe";

        static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].Equals("infinite"))
                {
                    // To avoid potential issues with orphaned processes (say, the test exits before
                    // exiting the process), we'll say "infinite" is actually 100 seconds.
                    Sleep(100 * 1000);
                }
                else if (args[0].Equals("error"))
                {
                    Console.Error.WriteLine(Name + " started error stream");
                    Console.Error.WriteLine(Name + " closed error stream");
                    Sleep();
                }
                else if (args[0].Equals("input"))
                {
                    string str = Console.ReadLine();
                }
                else if (args[0].Equals("stream"))
                {
                    Console.WriteLine(Name + " started");
                    Console.WriteLine(Name + " closed");
                    Sleep();
                }
                else if (args[0].Equals("byteAtATime"))
                {
                    var stdout = Console.OpenStandardOutput();
                    var bytes = new byte[] { 97, 0 }; //Encoding.Unicode.GetBytes("a");

                    for (int i = 0; i != bytes.Length; ++i)
                    {
                        stdout.WriteByte(bytes[i]);
                        stdout.Flush();
                        Sleep(100);
                    }
                }
                else if (args[0].Equals("manyOutputLines"))
                {
                    for (int i = 0; i < 144; i++)
                    {
                        Console.WriteLine("This is line #" + i + ".");
                    }
                    // no sleep here
                }
                else
                {
                    Console.WriteLine(string.Join(" ", args));
                }
            }
            return 100;
        }

        private static void Sleep()
        {
            Sleep(100d);
        }

        private static void Sleep(double timeinMs)
        {
            Task.Delay(TimeSpan.FromMilliseconds(timeinMs)).Wait();
        }
    }
}
