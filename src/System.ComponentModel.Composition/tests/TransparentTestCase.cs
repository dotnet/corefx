using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace System.ComponentModel.Composition
{
    static public class TransparentTestCase
    {
        static public int GetMetadataView_IMetadataViewWithDefaultedIntInTranparentType(ITrans_MetadataViewWithDefaultedInt view)
        {
            return view.MyInt;
        }
    }
}
