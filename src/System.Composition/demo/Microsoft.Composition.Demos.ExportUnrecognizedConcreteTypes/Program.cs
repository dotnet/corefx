// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Composition.Demos.ExportUnrecognizedConcreteTypes.Extension;
using Microsoft.Composition.Demos.ExportUnrecognizedConcreteTypes.Parts;

namespace Microsoft.Composition.Demos.ExportUnrecognizedConcreteTypes
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ContainerConfiguration()
                .WithPart<MainForm>()
                .WithProvider(new UnrecognizedConcreteTypeSource());

            using (var container = configuration.CreateContainer())
            {
                var form = container.GetExport<MainForm>();
                form.Customers.Render();
            }

            Console.ReadKey();
        }
    }
}
