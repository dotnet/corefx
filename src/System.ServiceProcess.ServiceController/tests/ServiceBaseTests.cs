// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using Xunit;

/// <summary>
/// NOTE: All tests checking the output file should always call Stop before checking because Stop will flush the file to disk.
/// </summary>
namespace System.ServiceProcess.Tests
{
    [OuterLoop(/* Modifies machine state */)]
    public class ServiceBaseTests : IDisposable
    {
        private readonly TestServiceProvider _testService;

        private static readonly Lazy<bool> s_isElevated = new Lazy<bool>(() => AdminHelpers.IsProcessElevated());
        protected static bool IsProcessElevated => s_isElevated.Value;
        protected static bool IsElevatedAndSupportsEventLogs => IsProcessElevated && PlatformDetection.IsNotWindowsNanoServer;

        private bool _disposed;

        public ServiceBaseTests()
        {
            _testService = new TestServiceProvider();
        }

        private void AssertExpectedProperties(ServiceController testServiceController)
        {
            var comparer = PlatformDetection.IsFullFramework ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal; // Full framework upper cases the name
            Assert.Equal(_testService.TestServiceName, testServiceController.ServiceName, comparer);
            Assert.Equal(_testService.TestServiceDisplayName, testServiceController.DisplayName);
            Assert.Equal(_testService.TestMachineName, testServiceController.MachineName);
            Assert.Equal(ServiceType.Win32OwnProcess, testServiceController.ServiceType);
            Assert.True(testServiceController.CanPauseAndContinue);
            Assert.True(testServiceController.CanStop);
            Assert.True(testServiceController.CanShutdown);
        }

        //[Fact]
        // To cleanup lingering Test Services uncomment the Fact attribute and run the following command
        //   msbuild /t:rebuildandtest /p:XunitMethodName=System.ServiceProcess.Tests.ServiceBaseTests.Cleanup
        // Remember to comment out the Fact again before running tests otherwise it will cleanup tests running in parallel
        // and casue them to fail.
        public void Cleanup()
        {
            string currentService = "";
            foreach (ServiceController controller in ServiceController.GetServices())
            {
                try
                {
                    currentService = controller.DisplayName;
                    if (controller.DisplayName.StartsWith("Test Service"))
                    {
                        Console.WriteLine("Trying to clean-up " + currentService);
                        TestServiceInstaller deleteService = new TestServiceInstaller()
                        {
                            ServiceName = controller.ServiceName
                        };
                        deleteService.RemoveService();
                        Console.WriteLine("Cleaned up " + currentService);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed " + ex.Message);
                }
            }
        }

        [ConditionalFact(nameof(IsProcessElevated))]
        public void TestOnStartThenStop()
        {
            string serviceName = _testService.TestServiceName;
            using (var client = new NamedPipeClientStream(".", serviceName, PipeDirection.In))
            {
                client.Connect();
                var controller = new ServiceController(serviceName);
                AssertExpectedProperties(controller);

                controller.Stop();
                Assert.Equal((int)PipeMessageByteCode.Stop, client.ReadByte());
            }
        }

        [ConditionalFact(nameof(IsProcessElevated))]
        public void TestOnStartWithArgsThenStop()
        {
            string serviceName = _testService.TestServiceName;
            var controller = new ServiceController(serviceName);

            using (var client = new NamedPipeClientStream(".", serviceName, PipeDirection.In))
            {
                client.Connect();
                AssertExpectedProperties(controller);

                controller.Stop();
                Assert.Equal((int)PipeMessageByteCode.Stop, client.ReadByte());

                controller.WaitForStatus(ServiceControllerStatus.Stopped);
                controller.Start(new string[] { "a", "b", "c" });
            }

            using (var client = new NamedPipeClientStream(".", serviceName, PipeDirection.In))
            {
                client.Connect();                
                Assert.Equal((int)PipeMessageByteCode.Start, client.ReadByte());
                controller.WaitForStatus(ServiceControllerStatus.Running);

                controller.Stop();
                Assert.Equal((int)PipeMessageByteCode.Stop, client.ReadByte());
                controller.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        [ConditionalFact(nameof(IsProcessElevated))]
        public void TestOnPauseThenStop()
        {
            string serviceName = _testService.TestServiceName;
            using (var client = new NamedPipeClientStream(".", serviceName, PipeDirection.In))
            {
                client.Connect();
                var controller = new ServiceController(serviceName);
                AssertExpectedProperties(controller);

                controller.Pause();
                Assert.Equal((int)PipeMessageByteCode.Pause, client.ReadByte());
                controller.WaitForStatus(ServiceControllerStatus.Paused);

                controller.Stop();
                Assert.Equal((int)PipeMessageByteCode.Stop, client.ReadByte());
            }
        }

        [ConditionalFact(nameof(IsProcessElevated))]
        public void TestOnPauseAndContinueThenStop()
        {
            string serviceName = _testService.TestServiceName;
            using (var client = new NamedPipeClientStream(".", serviceName, PipeDirection.In))
            {
                client.Connect();
                var controller = new ServiceController(serviceName);
                AssertExpectedProperties(controller);

                controller.Pause();
                Assert.Equal((int)PipeMessageByteCode.Pause, client.ReadByte());
                controller.WaitForStatus(ServiceControllerStatus.Paused);

                controller.Continue();
                Assert.Equal((int)PipeMessageByteCode.Continue, client.ReadByte());

                controller.WaitForStatus(ServiceControllerStatus.Running);
                controller.Stop();
                Assert.Equal((int)PipeMessageByteCode.Stop, client.ReadByte());
            }
        }

        [ConditionalFact(nameof(IsProcessElevated))]
        public void TestOnExecuteCustomCommand()
        {
            var serviceName = _testService.TestServiceName;
            using(var client = new NamedPipeClientStream(".", serviceName, PipeDirection.In))
            {        
                client.Connect();
                var controller = new ServiceController(serviceName);
                AssertExpectedProperties(controller);

                controller.ExecuteCommand(128);
                Assert.Equal(128, client.ReadByte());

                controller.Stop();
                Assert.Equal((int)PipeMessageByteCode.Stop, client.ReadByte());
            }
        }

        [ConditionalFact(nameof(IsProcessElevated))]
        public void TestOnContinueBeforePause()
        {
            var serviceName = _testService.TestServiceName;
            using (var client = new NamedPipeClientStream(".", serviceName, PipeDirection.In))
            {
                client.Connect();
                var controller = new ServiceController(serviceName);
                AssertExpectedProperties(controller);

                controller.Continue();
                controller.WaitForStatus(ServiceControllerStatus.Running);
                
                controller.Stop();
                Assert.Equal((int)PipeMessageByteCode.Stop, client.ReadByte());
                controller.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        [ConditionalFact(nameof(IsElevatedAndSupportsEventLogs))]
        public void LogWritten()
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                ServiceBase sb = new ServiceBase() { ServiceName = nameof(LogWritten) + Guid.NewGuid().ToString() };
                Assert.False(EventLog.SourceExists(sb.ServiceName));
                try
                {
                    ServiceBase.Run(sb);
                    eventLog.Source = sb.ServiceName;
                    Assert.True(EventLog.SourceExists(sb.ServiceName));
                }
                finally
                {
                    sb.Stop();
                    EventLog.DeleteEventSource(sb.ServiceName);
                }
            } 
        }

        [ConditionalFact(nameof(IsElevatedAndSupportsEventLogs))]
        public void LogWritten_AutoLog_False()
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                ServiceBase sb = new ServiceBase() { ServiceName = nameof(LogWritten) + Guid.NewGuid().ToString(), AutoLog = false };
                Assert.False(EventLog.SourceExists(sb.ServiceName));
                try
                {
                    ServiceBase.Run(sb);
                    Assert.False(EventLog.SourceExists(sb.ServiceName));
                }
                finally
                {
                    sb.Stop();
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _testService.DeleteTestServices();
                _disposed = true;
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
