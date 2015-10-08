// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OLEDB.Test.ModuleCore;

namespace System.Xml.Tests
{
    internal class XmlIdeographicCharConvertTests2 : XmlIdeographicCharConvertTests
    {
        #region Constructors and Destructors

        public XmlIdeographicCharConvertTests2()
        {
            for (int i = 0; i < _byte_Ideographic.Length; i = i + 2)
            {
                AddVariation(new CVariation(this, "EncodeNmToken-EncodeLocalNmToken : " + _Expbyte_Ideographic[i / 2], XmlEncodeName));
            }
        }

        #endregion

        #region Public Methods and Operators

        public int XmlEncodeName()
        {
            int i = ((CurVariation.id) - 1) * 2;
            string strEnVal = String.Empty;

            strEnVal = XmlConvert.EncodeNmToken((BitConverter.ToChar(_byte_Ideographic, i)).ToString());
            if (_Expbyte_Ideographic[i / 2] != "_x302A_")
            {
                CError.Compare(strEnVal, _Expbyte_Ideographic[i / 2], "Comparison failed at " + i);
            }
            return TEST_PASS;
        }
        #endregion
    }
}