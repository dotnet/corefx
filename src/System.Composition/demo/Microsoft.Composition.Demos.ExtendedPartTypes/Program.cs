// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Composition.Demos.ExtendedPartTypes.Extension;

namespace Microsoft.Composition.Demos.ExtendedPartTypes
{
    [Export]
    class Program
    {
        static void Main(string[] args)
        {
            // WithExport() demonstrates how a pre-existing instance is used as an export.

            // WithFactoryDelegate() demonstrates how an export can be dynamically
            // provided by calling a method.

            var config = new ContainerConfiguration()
                .WithExport<TextWriter>(Console.Out)
                .WithFactoryDelegate<string>(() => DateTime.Now.ToString(), isShared: true)
                .WithPart<Program>();

            using (var container = config.CreateContainer())
            {
                var p = container.GetExport<Program>();
                p.Run();
                Console.ReadKey();
            }
        }

        private readonly TextWriter _output;
        private readonly string _message;

        [ImportingConstructor]
        public Program(TextWriter output, string message)
        {
            _output = output;
            _message = message;
        }

        public void Run()
        {
            _output.WriteLine(_message);
        }
    }
}
