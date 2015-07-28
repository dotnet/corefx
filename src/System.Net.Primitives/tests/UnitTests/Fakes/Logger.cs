// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace NetPrimitivesUnitTests
{
    public static class Logger
    {
        public static void LogInformation(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }
    }
}
