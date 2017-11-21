using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition
{
    public interface ITrans_CollectionOfStrings
    {
        IEnumerable<string> Values { get; }
    }
}
