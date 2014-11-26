// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace XmlConfigurationDemo.Extension
{
    public class ExportElement : ConfigurationElement
    {
        public const string ContractTypeAttributeName = "contractType";
        public const string Key = ContractTypeAttributeName;

        [ConfigurationProperty(ContractTypeAttributeName, IsRequired = true)]
        public string ContractType { get { return (string)this[ContractTypeAttributeName]; } }
    }
}
