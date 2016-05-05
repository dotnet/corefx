// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.ServiceProcess.Tests
{
    public static class SafeServiceControllerTests
    {
        private const string KeyIsoSvcName = "KEYISO";

        [Fact]
        public static void GetServices()
        {
            bool foundKeyIsoSvc = false;
            bool foundSamSvc = false;
            bool foundOtherSvc = false;
            
            foreach (var service in ServiceController.GetServices())
            {
                // The CNG Key Isolation service (KeyIso) and Security Accounts Manager (SAM) service (SamSs)
                // are both system-critical, so they can be counted on to be registered and running.
                // This gives us a view into whether GetServices is returning expected data.
                switch (service.ServiceName.ToUpperInvariant())
                {
                    case KeyIsoSvcName:
                        foundKeyIsoSvc = true;
                        Assert.False(string.IsNullOrEmpty(service.DisplayName), "string.IsNullOrEmpty(KeyIso.DisplayName)");
                        Assert.True(service.CanStop, "KeyIso.CanStop");
                        Assert.False(service.CanPauseAndContinue, "KeyIso.CanPauseAndContinue");
                        Assert.False(service.CanShutdown, "KeyIso.CanShutdown");
                        Assert.Equal(ServiceType.Win32ShareProcess, service.ServiceType);
                        Assert.Equal(ServiceStartMode.Manual, service.StartType);
                        break;
                    case "SAMSS":
                        foundSamSvc = true;
                        Assert.False(string.IsNullOrEmpty(service.DisplayName), "string.IsNullOrEmpty(SamSs.DisplayName)");
                        Assert.False(service.CanStop, "SamSs.CanStop");
                        Assert.False(service.CanPauseAndContinue, "SamSs.CanPauseAndContinue");
                        Assert.False(service.CanShutdown, "SamSs.CanShutdown");
                        Assert.Equal(ServiceType.Win32ShareProcess, service.ServiceType);
                        Assert.Equal(ServiceStartMode.Automatic, service.StartType);
                        break;
                    default:
                        foundOtherSvc = true;
                        break;
                }
            }

            Assert.True(foundKeyIsoSvc, "foundKeyIsoSvc");
            Assert.True(foundSamSvc, "foundSamSvc");
            Assert.True(foundOtherSvc, "foundOtherSvc");
        }

        [Fact]
        public static void GetDevices()
        {
            ServiceController[] devices = ServiceController.GetDevices();
            Assert.True(devices.Length != 0);

            const ServiceType SERVICE_TYPE_DRIVER =
                ServiceType.FileSystemDriver |
                ServiceType.KernelDriver |
                ServiceType.RecognizerDriver;

            Assert.All(devices, device => Assert.NotEqual(0, (int)(device.ServiceType & SERVICE_TYPE_DRIVER)));
        }

        [Fact]
        public static void WaitForStatusTimeout()
        {
            var controller = new ServiceController(KeyIsoSvcName);
            Assert.Throws<System.ServiceProcess.TimeoutException>(() => controller.WaitForStatus(ServiceControllerStatus.Paused, TimeSpan.Zero));
        }
    }
}
