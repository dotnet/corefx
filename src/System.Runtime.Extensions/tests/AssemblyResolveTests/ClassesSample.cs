using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyResolveTests
{
    public class PublicClassSample
    {
        public PublicClassSample() { }
        public PublicClassSample(int param) { }
    }

    class PrivateClassSample
    {
        public PrivateClassSample() { }
        public PrivateClassSample(int param) { }
    }

    public class PublicClassNoDefaultConstructorSample
    {
        public PublicClassNoDefaultConstructorSample(int param) { }
    }
}
