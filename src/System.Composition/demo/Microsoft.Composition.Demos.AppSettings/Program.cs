// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppSettingsExtensionDemo.Extension;
using AppSettingsExtensionDemo.Parts;

namespace AppSettingsExtensionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ContainerConfiguration()
                .WithAssembly(typeof(Program).Assembly)
                .WithProvider(new AppSettingsExportDescriptorProvider());

            using (var container = configuration.CreateContainer())
            {
                var downloader = container.GetExport<Downloader>();
                downloader.Download();
            }

            Console.ReadKey(true);
        }
    }
}
