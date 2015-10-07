// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OLEDB.Test.ModuleCore;

namespace System.Xml.Tests
{
    public partial class TCMoveToAttributeOrdinal : TCXMLReaderBaseGeneral
    {
        // Type is System.Xml.Tests.TCMoveToAttributeOrdinal
        // Test Case
        public override void AddChildren()
        {
            // for function MoveToAttributeWithGetAttrDoubleQ
            {
                this.AddChild(new CVariation(MoveToAttributeWithGetAttrDoubleQ) { Attribute = new Variation("MoveToAttribute(i) Verify with This[i] - Double Quote") { Pri = 0 } });
            }


            // for function MoveToAttributeWithGetAttrSingleQ
            {
                this.AddChild(new CVariation(MoveToAttributeWithGetAttrSingleQ) { Attribute = new Variation("MoveToAttribute(i) Verify with This[i] - Single Quote") });
            }


            // for function MoveToAttributeWithMoveAttrDoubleQ
            {
                this.AddChild(new CVariation(MoveToAttributeWithMoveAttrDoubleQ) { Attribute = new Variation("MoveToAttribute(i) Verify with GetAttribute(i) - Double Quote") { Pri = 0 } });
            }


            // for function MoveToAttributeWithMoveAttrSingleQ
            {
                this.AddChild(new CVariation(MoveToAttributeWithMoveAttrSingleQ) { Attribute = new Variation("MoveToAttribute(i) Verify with GetAttribute[i] - Single Quote") });
            }


            // for function NegativeOneOrdinal
            {
                this.AddChild(new CVariation(NegativeOneOrdinal) { Attribute = new Variation("MoveToAttribute(i) NegativeOneOrdinal") { Pri = 0 } });
            }


            // for function FieldCountOrdinal
            {
                this.AddChild(new CVariation(FieldCountOrdinal) { Attribute = new Variation("MoveToAttribute(i) FieldCountOrdinal") });
            }


            // for function OrdinalPlusOne
            {
                this.AddChild(new CVariation(OrdinalPlusOne) { Attribute = new Variation("MoveToAttribute(i) OrdinalPlusOne") { Pri = 0 } });
            }


            // for function OrdinalMinusOne
            {
                this.AddChild(new CVariation(OrdinalMinusOne) { Attribute = new Variation("MoveToAttribute(i) OrdinalMinusOne") });
            }
        }
    }
}
