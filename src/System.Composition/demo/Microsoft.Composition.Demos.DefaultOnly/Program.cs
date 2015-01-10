// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Composition.Demos.DefaultOnly.Extension;
using Microsoft.Composition.Demos.DefaultOnly.Parts;

namespace Microsoft.Composition.Demos.DefaultOnly.Parts
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ContainerConfiguration()
                .WithAssembly(typeof(Program).Assembly)
                .WithProvider(new DefaultExportDescriptorProvider());

            using (var container = configuration.CreateContainer())
            {
                var greeter = container.GetExport<Greeter>();
                greeter.Greet();
            }

            Console.ReadKey();
        }
    }
}
