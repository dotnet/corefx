using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition
{

    public interface ITrans_SimpleMetadataView
    {
        string String { get; }
        int Int { get; }
        float Float { get; }
        Type Type { get; }
        object Object { get; }
    }
}
