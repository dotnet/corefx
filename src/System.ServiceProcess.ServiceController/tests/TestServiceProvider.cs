﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading.Tasks;

namespace System.ServiceProcess.Tests
{
    internal sealed class TestServiceProvider
    {
        private const int readTimeout = 60000;

        private static readonly Lazy<bool> s_runningWithElevatedPrivileges = new Lazy<bool>(
            () => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator));

        private NamedPipeClientStream _client;

        public static bool RunningWithElevatedPrivileges
        {
            get { return s_runningWithElevatedPrivileges.Value; }
        }

        public NamedPipeClientStream Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new NamedPipeClientStream(".", TestServiceName, PipeDirection.In);
                }
                return _client;
            }
            set
            {
                if (value == null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
        }

        public readonly string TestServiceAssembly = typeof(TestService).Assembly.Location;
        public readonly string TestMachineName;
        public readonly TimeSpan ControlTimeout;
        public readonly string TestServiceName;
        public readonly string TestServiceDisplayName;

        private readonly TestServiceProvider _dependentServices;
        public TestServiceProvider()
        {
            TestMachineName = ".";
            ControlTimeout = TimeSpan.FromSeconds(120);
            TestServiceName = Guid.NewGuid().ToString();
            TestServiceDisplayName = "Test Service " + TestServiceName;

            _dependentServices = new TestServiceProvider(TestServiceName + ".Dependent");

            // Create the service
            CreateTestServices();
        }

        public TestServiceProvider(string serviceName)
        {
            TestMachineName = ".";
            ControlTimeout = TimeSpan.FromSeconds(120);
            TestServiceName = serviceName;
            TestServiceDisplayName = "Test Service " + TestServiceName;

            // Create the service
            CreateTestServices();
        }

        public async Task<byte> ReadPipeAsync()
        {
            Task readTask;
            byte[] received = new byte[] { 0 };
            readTask = Client.ReadAsync(received, 0, 1);
            await readTask.TimeoutAfter(readTimeout).ConfigureAwait(false);
            return received[0];
        }

        public byte GetByte() => ReadPipeAsync().Result;

        private void CreateTestServices()
        {
            TestServiceInstaller testServiceInstaller = new TestServiceInstaller();

            testServiceInstaller.ServiceName = TestServiceName;
            testServiceInstaller.DisplayName = TestServiceDisplayName;
            testServiceInstaller.Description = "__Dummy Test Service__";

            if (_dependentServices != null)
            {
                testServiceInstaller.ServicesDependedOn = new string[] { _dependentServices.TestServiceName };
            }

            var comparer = PlatformDetection.IsFullFramework ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal; // Full framework upper cases the name
            string processName = Process.GetCurrentProcess().MainModule.FileName;
            string entryPointName = typeof(TestService).Assembly.Location;
            string arguments = TestServiceName;

            // if process and entry point aren't the same then we are running hosted so pass
            // in the entrypoint as the first argument
            if (!PlatformDetection.IsFullFramework)
            {
                arguments = $"\"{entryPointName}\" {arguments}";
            }
            else
            {
                processName = entryPointName;
            }

            testServiceInstaller.ServiceCommandLine = $"\"{processName}\" {arguments}";

            testServiceInstaller.Install();
        }

        public void DeleteTestServices()
        {
            try
            {
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }

                TestServiceInstaller testServiceInstaller = new TestServiceInstaller();
                testServiceInstaller.ServiceName = TestServiceName;
                testServiceInstaller.RemoveService();
            }
            finally
            {
                // Lets be sure to try and clean up dependenct services even if something goes
                // wrong with the full removal of the other service.
                if (_dependentServices != null)
                {
                    _dependentServices.DeleteTestServices();
                }
            }
        }
    }
}
