// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.DirectoryServices.ActiveDirectory
{
    using System;
    using System.Runtime.InteropServices;

    public class ReplicationCursor
    {
        private string _partition;
        private Guid _invocationID;
        private long _USN;
        private string _serverDN = null;
        private DateTime _syncTime;
        private bool _advanced = false;
        private string _sourceServer = null;

        private DirectoryServer _server = null;

        private ReplicationCursor() { }

        internal ReplicationCursor(DirectoryServer server, string partition, Guid guid, long filter, long time, IntPtr dn)
        {
            _partition = partition;
            _invocationID = guid;
            _USN = filter;

            // convert filetime to DateTime
            _syncTime = DateTime.FromFileTime(time);

            // get the dn
            _serverDN = Marshal.PtrToStringUni(dn);

            _advanced = true;

            _server = server;
        }

        internal ReplicationCursor(DirectoryServer server, string partition, Guid guid, long filter)
        {
            _partition = partition;
            _invocationID = guid;
            _USN = filter;

            _server = server;
        }

        public string PartitionName
        {
            get
            {
                return _partition;
            }
        }

        public Guid SourceInvocationId
        {
            get
            {
                return _invocationID;
            }
        }

        public long UpToDatenessUsn
        {
            get
            {
                return _USN;
            }
        }

        public string SourceServer
        {
            get
            {
                // get the source server name if we are on win2k, or above win2k and serverDN is not NULL (means KCC translation is successful)
                if (!_advanced || (_advanced && _serverDN != null))
                {
                    _sourceServer = Utils.GetServerNameFromInvocationID(_serverDN, SourceInvocationId, _server);
                }

                return _sourceServer;
            }
        }

        public DateTime LastSuccessfulSyncTime
        {
            get
            {
                if (_advanced)
                    return _syncTime;
                else
                {
                    // win2k client machine does not support this
                    if ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 0))
                        throw new PlatformNotSupportedException(Res.GetString(Res.DSNotSupportOnClient));
                    else
                        throw new PlatformNotSupportedException(Res.GetString(Res.DSNotSupportOnDC));
                }
            }
        }
    }
}
