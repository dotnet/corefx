// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStyleLifetimeDemo.Extension;

namespace WebStyleLifetimeDemo.Parts
{
    [Export, Shared(Boundaries.DataConsistency)]
    public class DatabaseConnection : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("Disposing database connection");
        }
    }
}
