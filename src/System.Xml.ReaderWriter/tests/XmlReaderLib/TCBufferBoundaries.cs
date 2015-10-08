﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OLEDB.Test.ModuleCore;

namespace System.Xml.Tests
{
    public partial class TCBufferBoundaries : TCXMLReaderBaseGeneral
    {
        // Type is System.Xml.Tests.TCBufferBoundaries
        // Test Case
        public override void AddChildren()
        {
            // for function v1
            {
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4090", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4093", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4101", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4092", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4102", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4088", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4089", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4091", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4000", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4096", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4097", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4098", "4096" } } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Test PI Buffer Boundaries with variable byte boundary") { Params = new object[] { "4099", "4096" } } });
            }
        }
    }
}
