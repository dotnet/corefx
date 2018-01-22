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

namespace System.ServiceProcess.Tests
{
    public class TestService : ServiceBase
    {
        private object streamlock = new object();
        
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
            this.Server.WaitForConnectionAsync();
        }

        public NamedPipeServerStream Server { get; set; }

        public static string GetLogPath(string serviceName)
        {
            return typeof(TestService).Assembly.Location + "." + serviceName + ".log";
        }

        protected override void OnContinue()
        {
            WriteLog(nameof(OnContinue));
            base.OnContinue();
            WriteStream("Continue");
        }

        protected override void OnCustomCommand(int command)
        {
            WriteLog(nameof(OnCustomCommand) + " command=" + command);
            base.OnCustomCommand(command);
            WriteStream("executeCommand");
        }

        protected override void OnPause()
        {
            WriteLog(nameof(OnPause));
            base.OnPause();
            WriteStream("Pause");
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            WriteLog(nameof(OnSessionChange) + " change=" + changeDescription.ToString());
            base.OnSessionChange(changeDescription);
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            WriteLog(nameof(OnPowerEvent) + " status=" + powerStatus.ToString());
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnShutdown()
        {
            WriteLog(nameof(OnShutdown));
            base.OnShutdown();
        }

        protected override void OnStart(string[] args)
        {
            File.Delete(GetLogPath(ServiceName));

            WriteLog(nameof(OnStart) + " args=" + string.Join(",", args));
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            Debug.WriteLine("Stopcalled");
            WriteLog(nameof(OnStop));
            base.OnStop();
            Debug.WriteLine("Stopcalled");
            WriteStream("Stop");
        }

        private void WriteLog(string msg)
        {
            File.AppendAllText(GetLogPath(ServiceName), msg + Environment.NewLine);
        }

        private void WriteStream(string msg)
        {
            Debug.WriteLine("LockNotAcquired");
            //lock (streamlock)

            Debug.WriteLine("LockAcquired");
            Server.Flush();
            Byte b = 1;

            Server.WriteByte(b);
            Debug.WriteLine("sent");
            Server.Flush();
            
        }


    }
}
