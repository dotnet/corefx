// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.DefaultOnly.Extension
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DefaultExportAttribute : ExportAttribute
    {
        public DefaultExportAttribute(Type contractType)
            : base(Constants.DefaultContractNamePrefix, contractType)
        { }

        public DefaultExportAttribute(string contractName, Type contractType)
            : base(Constants.DefaultContractNamePrefix + contractName, contractType)
        { }
    }
}
