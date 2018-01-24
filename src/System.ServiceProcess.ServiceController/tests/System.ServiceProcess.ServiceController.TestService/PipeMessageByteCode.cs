using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceProcess.Tests
{
    public enum PipeMessageByteCode
    {
        Start = 0,
        Continue = 1,
        Pause = 2,
        Stop = 3,
        OnCustomCommand = 4
    };
}
