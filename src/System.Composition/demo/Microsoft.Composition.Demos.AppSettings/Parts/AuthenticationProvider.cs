// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppSettingsExtensionDemo.Extension;

namespace AppSettingsExtensionDemo.Parts
{
    public class AuthenticationProvider
    {
        [Export, Setting("username")]
        public string Username { get { return "Nancy"; } }
    }
}
