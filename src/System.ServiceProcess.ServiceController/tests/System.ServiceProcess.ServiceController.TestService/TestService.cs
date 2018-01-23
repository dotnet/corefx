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
    public class TestService : ServiceBase, IDisposable
    {
        private bool _disposed;

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
            WriteStream(PipeMessageByteCode.OnCustomCommand, command);
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
            if (args.Length == 4 && args[0] == "StartWithArguments")
            {
                Debug.Assert(args[1] == "a");
                Debug.Assert(args[2] == "b");
                Debug.Assert(args[3] == "c");
                Server.WaitForConnectionAsync();
                WriteStream(PipeMessageByteCode.Start);
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            WriteStream(PipeMessageByteCode.Stop);
        }

        private void WriteStream(PipeMessageByteCode code, int command = 0)
        {
            if (code == PipeMessageByteCode.OnCustomCommand)
                Server.WriteByte((byte)command);
            else
                Server.WriteByte((byte)code);
        }

        public new void Dispose()
        {
            if (!_disposed)
            {
                Server.Dispose();
                _disposed = true;
                base.Dispose();
            }
        }

        public enum PipeMessageByteCode
        {
            Start = 0,
            Continue = 1,
            Pause = 2,
            Stop = 3,
            OnCustomCommand = 4
        };
    }
}
