using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Pipes.Tests
{
    public class AnonymousPipeServerStreamAclTests
    {
        [Fact]
        public void Create_NullSecurity()
        {
            using AnonymousPipeServerStream apss = AnonymousPipeServerStreamAcl.Create(PipeDirection.In, HandleInheritability.Inheritable, 0, null);
        }
    }
}
