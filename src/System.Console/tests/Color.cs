// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

public class Color
{
    [Fact]
    public static void InvalidColors()
    {
        Assert.Throws<ArgumentException>(() => Console.BackgroundColor = (ConsoleColor)42);
        Assert.Throws<ArgumentException>(() => Console.ForegroundColor = (ConsoleColor)42);
    }

    [Fact]
    public static void RoundtrippingColor()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.BackgroundColor = Console.BackgroundColor;
            Console.ForegroundColor = Console.ForegroundColor;
            // Changing color on Windows doesn't have effect in some testing environments
            // when there is no associated console, such as when run under a profiler like 
            // our code coverage tools, so we don't assert that the change took place and 
            // simple ensure that getting/setting doesn't throw.
        }
        else
        {
            Assert.Throws<PlatformNotSupportedException>(() => Console.BackgroundColor);
            Assert.Throws<PlatformNotSupportedException>(() => Console.ForegroundColor);
        }
    }

    [Fact]
    [PlatformSpecific(PlatformID.AnyUnix)]
    public static void RedirectedOutputDoesNotUseAnsiSequences()
    {
        // Make sure that redirecting to a memory stream causes Console not to write out the ANSI sequences

        Helpers.RunInRedirectedOutput((data) => 
        {
            Console.Write('1');
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write('2');
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write('3');
            Console.ResetColor();
            Console.Write('4');

            const char Esc = (char)0x1B;
            Assert.Equal(0, Encoding.UTF8.GetString(data.ToArray()).ToCharArray().Count(c => c == Esc));
            Assert.Equal("1234", Encoding.UTF8.GetString(data.ToArray()));
        });
    }

    //[Fact] // the CI system redirects stdout, so we can't easily test non-redirected behavior
    [PlatformSpecific(PlatformID.AnyUnix)]
    public static void NonRedirectedOutputDoesUseAnsiSequences()
    {
        // Make sure that when writing out to a UnixConsoleStream, the ANSI escape sequences are properly
        // written out.
        Helpers.RunInNonRedirectedOutput((data) =>
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ResetColor();
            const char Esc = (char)0x1B;
            Assert.Equal(3, Encoding.UTF8.GetString(data.ToArray()).ToCharArray().Count(c => c == Esc));
        });
    }
}
