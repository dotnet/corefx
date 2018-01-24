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
using System.Threading.Tasks;

namespace System.ServiceProcess.Tests
{
    public class TestService : ServiceBase
    {
        private bool _disposed;
        private Task serverStarted;

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
            serverStarted = this.Server.WaitForConnectionAsync();
        }

        private NamedPipeServerStream Server { get; }

        protected override void OnContinue()
        {
            base.OnContinue();
            var task = Task.Run(() => WriteStreamAsync(PipeMessageByteCode.Continue));
            task.Wait();
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);
            var task = Task.Run(() => WriteStreamAsync(PipeMessageByteCode.OnCustomCommand, command));
            task.Wait();
        }

        protected override void OnPause()
        {
            base.OnPause();
            var task = Task.Run(() => WriteStreamAsync(PipeMessageByteCode.Pause));
            task.Wait();
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
                var task = Task.Run(() => WriteStreamAsync(PipeMessageByteCode.Start));
                task.Wait();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            var task = Task.Run(() => WriteStreamAsync(PipeMessageByteCode.Stop));
            task.Wait();
        }

        private async Task WriteStreamAsync(PipeMessageByteCode code, int command = 0)
        {
            Task writeCompleted;
            if (serverStarted.IsCompletedSuccessfully)
            {
                if (code == PipeMessageByteCode.OnCustomCommand)
                {
                    writeCompleted = Server.WriteAsync(new byte[] { (byte)command }, 0, 1);
                    await writeCompleted.TimeoutAfter(60000);
                }
                else
                {
                    writeCompleted = Server.WriteAsync(new byte[] { (byte)code }, 0, 1);
                    await writeCompleted.TimeoutAfter(60000);
                }
            }
            else
            {
                throw new TimeoutException($"Connection timed out after 30 seconds");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Server.Dispose();
                _disposed = true;
                base.Dispose();
            }
        }
    }
}
