// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace XmlConfigurationDemo.Extension
{
    public class PartElement : ConfigurationElement
    {
        public const string TypeAttributeName = "type";
        public const string Key = TypeAttributeName;

        public const string ExportsPropertyName = "exports";

        [ConfigurationProperty(TypeAttributeName, IsRequired = true)]
        public string Type { get { return (string)this[TypeAttributeName]; } }

        [ConfigurationProperty(ExportsPropertyName, IsRequired = false)]
        public ExportsCollection Exports { get { return (ExportsCollection)this[ExportsPropertyName]; } }
    }
}
