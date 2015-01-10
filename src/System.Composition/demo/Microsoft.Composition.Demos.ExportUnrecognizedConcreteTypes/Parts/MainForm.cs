﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.ExportUnrecognizedConcreteTypes.Parts
{
    [Export]
    class MainForm
    {
        [Import]
        public CustomersView Customers { get; set; }
    }
}
