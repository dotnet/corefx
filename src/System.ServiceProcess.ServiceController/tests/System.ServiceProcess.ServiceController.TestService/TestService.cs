// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.IO.Pipes;

namespace System.ServiceProcess.Tests
{
    public class TestService : ServiceBase
    {
        public TestService(string serviceName)
        {
            this.ServiceName = serviceName;

            // Enable all the events
            this.CanPauseAndContinue = true;
            this.CanStop = true;
            this.CanShutdown = true;

            // We cannot easily test these so disable the events
            this.CanHandleSessionChangeEvent = false;
            this.CanHandlePowerEvent = false;

            this.Server = new NamedPipeServerStream(serviceName);
            this.Server.WaitForConnectionAsync();
        }

        public NamedPipeServerStream Server { get; set; }

        protected override void OnContinue()
        {
            base.OnContinue();
            WriteStream(PipeMessageByteCode.Continue);
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);
            WriteStream(PipeMessageByteCode.OnCustomCommand);
        }

        protected override void OnPause()
        {
            base.OnPause();
            WriteStream(PipeMessageByteCode.Pause);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            if (args.Length == 3)
            {
                Server.WaitForConnectionAsync();
                WriteStream(PipeMessageByteCode.Start);
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            WriteStream(PipeMessageByteCode.Stop);
        }

        private void WriteStream(PipeMessageByteCode code)
        {
            byte data = (byte)code;
            Server.WriteByte(data);
        }

        public enum PipeMessageByteCode { Start, Continue, Pause, Stop, OnCustomCommand };
    }

    
}
