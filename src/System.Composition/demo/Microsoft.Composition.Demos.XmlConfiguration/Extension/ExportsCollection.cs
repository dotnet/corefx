// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace XmlConfigurationDemo.Extension
{
    public class ExportsCollection : ConfigurationElementCollection, IEnumerable<ExportElement>
    {
        public const string ExportElementName = "export";

        protected override ConfigurationElement CreateNewElement()
        {
            return new ExportElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return element.ElementInformation.Properties[ExportElement.Key].Value;
        }

        public new IEnumerator<ExportElement> GetEnumerator()
        {
            foreach (ExportElement exportElement in ((IEnumerable)this))
                yield return exportElement;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return ExportElementName;
            }
        }
    }
}
