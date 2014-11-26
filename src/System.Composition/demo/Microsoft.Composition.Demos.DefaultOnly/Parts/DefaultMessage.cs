// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Composition.Demos.DefaultOnly.Extension;

namespace Microsoft.Composition.Demos.DefaultOnly.Parts
{
    [DefaultExport(typeof(IMessage))]
    public class DefaultMessage : IMessage
    {
        public string Current
        {
            get { return "Hello, World"; }
        }
    }
}
