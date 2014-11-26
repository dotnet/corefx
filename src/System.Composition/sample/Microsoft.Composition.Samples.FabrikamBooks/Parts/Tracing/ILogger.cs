using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FabrikamBooks.Parts.Tracing
{
    public interface ILogger
    {
        void Write(string text);
    }
}