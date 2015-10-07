﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OLEDB.Test.ModuleCore;

namespace System.Xml.Tests
{
    public partial class TCCloseInput : TCXMLReaderBaseGeneral
    {
        // Type is System.Xml.Tests.TCCloseInput
        // Test Case
        public override void AddChildren()
        {
            // for function v1
            {
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Default Values") { Priority = 0 } });
            }
        }
    }
}
