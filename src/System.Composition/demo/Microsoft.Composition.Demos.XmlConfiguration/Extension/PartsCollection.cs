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
    public class PartsCollection : ConfigurationElementCollection, IEnumerable<PartElement>
    {
        public const string PartElementName = "part";

        protected override ConfigurationElement CreateNewElement()
        {
            return new PartElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return element.ElementInformation.Properties[PartElement.Key].Value;
        }

        public new IEnumerator<PartElement> GetEnumerator()
        {
            foreach (PartElement partElement in ((IEnumerable)this))
                yield return partElement;
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
                return PartElementName;
            }
        }
    }
}
