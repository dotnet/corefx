// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OLEDB.Test.ModuleCore;

namespace System.Xml.Tests
{
    public partial class TCReadToFollowing : TCXMLReaderBaseGeneral
    {
        // Type is System.Xml.Tests.TCReadToFollowing
        // Test Case
        public override void AddChildren()
        {
            // for function v1
            {
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Simple positive test") { Params = new object[] { "DNS" }, Pri = 0 } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Simple positive test") { Params = new object[] { "NNS" }, Pri = 0 } });
                this.AddChild(new CVariation(v1) { Attribute = new Variation("Simple positive test") { Params = new object[] { "NS" }, Pri = 0 } });
            }


            // for function v2
            {
                this.AddChild(new CVariation(v2) { Attribute = new Variation("Read on following with same names") { Params = new object[] { "DNS" }, Pri = 1 } });
                this.AddChild(new CVariation(v2) { Attribute = new Variation("Read on following with same names") { Params = new object[] { "NNS" }, Pri = 1 } });
                this.AddChild(new CVariation(v2) { Attribute = new Variation("Read on following with same names") { Params = new object[] { "NS" }, Pri = 1 } });
            }


            // for function v4
            {
                this.AddChild(new CVariation(v4) { Attribute = new Variation("If name not found, go to eof") { Pri = 1 } });
            }


            // for function v5_1
            {
                this.AddChild(new CVariation(v5_1) { Attribute = new Variation("If localname not found go to eof") { Pri = 1 } });
            }


            // for function v5_2
            {
                this.AddChild(new CVariation(v5_2) { Attribute = new Variation("If uri not found, go to eof") { Pri = 1 } });
            }


            // for function v6
            {
                this.AddChild(new CVariation(v6) { Attribute = new Variation("Read to Following on one level and again to level below it") { Pri = 1 } });
            }


            // for function v7
            {
                this.AddChild(new CVariation(v7) { Attribute = new Variation("Read to Following on one level and again to level below it, with namespace") { Pri = 1 } });
            }


            // for function v8
            {
                this.AddChild(new CVariation(v8) { Attribute = new Variation("Read to Following on one level and again to level below it, with prefix") { Pri = 1 } });
            }


            // for function v9
            {
                this.AddChild(new CVariation(v9) { Attribute = new Variation("Multiple Reads to children and then next siblings, NNS") { Pri = 2 } });
            }


            // for function v10
            {
                this.AddChild(new CVariation(v10) { Attribute = new Variation("Multiple Reads to children and then next siblings, DNS") { Pri = 2 } });
            }


            // for function v11
            {
                this.AddChild(new CVariation(v11) { Attribute = new Variation("Multiple Reads to children and then next siblings, NS") { Pri = 2 } });
            }


            // for function v14
            {
                this.AddChild(new CVariation(v14) { Attribute = new Variation("Only child has namespaces and read to it") { Pri = 2 } });
            }


            // for function v15
            {
                this.AddChild(new CVariation(v15) { Attribute = new Variation("Pass null to both arguments throws ArgumentException") { Pri = 2 } });
            }


            // for function v17
            {
                this.AddChild(new CVariation(v17) { Attribute = new Variation("Different names, same uri works correctly") { Pri = 2 } });
            }
        }
    }
}
