// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Convention;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopStyleLifetimeDemo.Parts;

namespace DesktopStyleLifetimeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var conventions = new ConventionBuilder();
            conventions.ForTypesMatching(t => t.Namespace == typeof(Application).Namespace)
                .Export();

            var configuration = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithAssembly(typeof(Program).Assembly);

            using (var container = configuration.CreateContainer())
            {
                var application = container.GetExport<Application>();
                application.Run("SuperIDE");
            }

            Console.ReadKey();
        }
    }
}
