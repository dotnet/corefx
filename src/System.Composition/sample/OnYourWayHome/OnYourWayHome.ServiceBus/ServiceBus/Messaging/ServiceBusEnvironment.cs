//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus
{
    using System;

    public static class ServiceBusEnvironment
    {
        private const string StsHostName = "accesscontrol.windows.net";
        private const string RelayHostName = "servicebus.windows.net";

        public static Uri CreateAccessControlUri(string serviceNamespace, string endpointPath)
        {
            if (string.IsNullOrEmpty(serviceNamespace))
            {
                throw new ArgumentException("Service namespace cannot be null or empty", "serviceNamespace");
            }

            return new Uri(String.Format("https://{0}-sb.{1}/{2}", serviceNamespace, StsHostName, endpointPath), UriKind.Absolute);
        }

        public static Uri CreateServiceUri(string serviceNamespace, string servicePath)
        {
            if (string.IsNullOrEmpty(serviceNamespace))
            {
                throw new ArgumentException("Service namespace cannot be null or empty", "serviceNamespace");
            }

            return new Uri(String.Format("https://{0}.{1}/{2}", serviceNamespace, RelayHostName, servicePath), UriKind.Absolute);
        }

        public static Uri CreateDefaultServiceRealmUri(string serviceNamespace)
        {
            if (string.IsNullOrEmpty(serviceNamespace))
            {
                throw new ArgumentException("Service namespace cannot be null or empty", "serviceNamespace");
            }

            return new Uri(String.Format("http://{0}.{1}/", serviceNamespace, RelayHostName), UriKind.Absolute);
        }
    }
}
