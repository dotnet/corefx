// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsExtensionDemo.Extension
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    [MetadataAttribute]
    public class SettingAttribute : Attribute
    {
        private readonly string _key;

        public SettingAttribute(string key)
        {
            _key = key;
        }

        public string SettingKey { get { return _key; } }
    }
}
