using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 618 // obsolete types

namespace System.Collections
{
    internal sealed partial class CompatibleComparer
    {
        internal IHashCodeProvider HashCodeProvider => _hcp;

        internal IComparer Comparer => _comparer;
    }
}
