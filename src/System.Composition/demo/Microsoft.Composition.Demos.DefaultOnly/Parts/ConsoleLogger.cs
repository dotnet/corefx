// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.DefaultOnly.Parts
{
    [Export(typeof(ILogger))]
    public class ConsoleLogger : ILogger
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
