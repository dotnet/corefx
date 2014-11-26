// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlConfigurationDemo.Extension;
using XmlConfigurationDemo.Parts;

namespace XmlConfigurationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ContainerConfiguration()
                .WithPartsFromXml();

            using (var container = configuration.CreateContainer())
            {
                var w = container.GetExport<Window>();
                w.Show();
            }

            Console.ReadKey(true);
        }
    }
}
