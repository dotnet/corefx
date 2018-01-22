// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace System.ServiceProcess.Tests
{
    public class TestService : ServiceBase
    {
        private object streamlock = new object();
        private Task waitConnectTask;

        public TestService(string serviceName)
        {
            this.ServiceName = serviceName;
            Console.WriteLine("constructor");

            // Enable all the events
            this.CanPauseAndContinue = true;
            this.CanStop = true;
            this.CanShutdown = true;

            // We cannot easily test these so disable the events
            this.CanHandleSessionChangeEvent = false;
            this.CanHandlePowerEvent = false;

            this.Server = new NamedPipeServerStream(serviceName);
            this.waitConnectTask = this.Server.WaitForConnectionAsync();
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
                Debug.WriteLine(Server.IsConnected);
                Server.WaitForConnectionAsync();
                Debug.WriteLine(Server.IsConnected);
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
            //lock (streamlock)
            byte b = (byte)code;
            Server.Flush();
            Server.WriteByte(b);
            Server.Flush();
        }
        public enum PipeMessageByteCode { Start, Continue, Pause, Stop, OnCustomCommand };

    }

    
}
