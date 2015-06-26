

namespace System.Net.NetworkInformation
{

    using System.Net;
    using System.Runtime.InteropServices;

    internal class HostInformation
    {
        private static FIXED_INFO fixedInfo;
        private static bool fixedInfoInitialized = false;

        //changing these require a reboot, so we'll cache them instead.
        private static volatile string hostName = null;
        private static volatile string domainName = null;

        private static object syncObject = new object();

        internal static FIXED_INFO GetFixedInfo()
        {
            uint size = 0;
            SafeLocalFree buffer = null;
            FIXED_INFO fixedInfo = new FIXED_INFO();

            //first we need to get the size of the buffer
            uint result = UnsafeCommonNativeMethods.GetNetworkParams(SafeLocalFree.Zero, ref size);

            while (result == IpHelperErrors.ErrorBufferOverflow)
            {
                try
                {
                    //now we allocate the buffer and read the network parameters.
                    buffer = SafeLocalFree.LocalAlloc((int)size);
                    result = UnsafeCommonNativeMethods.GetNetworkParams(buffer, ref size);
                    if (result == IpHelperErrors.Success)
                    {
                        fixedInfo = Marshal.PtrToStructure<FIXED_INFO>(buffer.DangerousGetHandle());
                    }
                }
                finally
                {
                    if (buffer != null)
                    {
                        buffer.Dispose();
                    }
                }
            }

            //if the result include there being no information, we'll still throw
            if (result != IpHelperErrors.Success)
            {
                throw new NetworkInformationException((int)result);
            }
            return fixedInfo;
        }


        internal static FIXED_INFO FixedInfo
        {
            get
            {
                if (!fixedInfoInitialized)
                {
                    lock (syncObject)
                    {
                        if (!fixedInfoInitialized)
                        {
                            fixedInfo = GetFixedInfo();
                            fixedInfoInitialized = true;
                        }
                    }
                }
                return fixedInfo;
            }
        }

        /// <summary>Specifies the host name for the local computer.</summary>
        internal static string HostName
        {
            get
            {
                if (hostName == null)
                {
                    lock (syncObject)
                    {
                        if (hostName == null)
                        {
                            hostName = FixedInfo.hostName;
                            domainName = FixedInfo.domainName;
                        }
                    }
                }
                return hostName;
            }
        }
        /// <summary>Specifies the domain in which the local computer is registered.</summary>
        internal static string DomainName
        {
            get
            {
                if (domainName == null)
                {
                    lock (syncObject)
                    {
                        if (domainName == null)
                        {
                            hostName = FixedInfo.hostName;
                            domainName = FixedInfo.domainName;
                        }
                    }
                }
                return domainName;
            }
        }
    }
}

