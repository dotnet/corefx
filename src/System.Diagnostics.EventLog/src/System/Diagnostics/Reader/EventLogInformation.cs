// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
** Purpose:
** The objects of this class allow access to the run-time
** properties of logs and external log files. An instance of this
** class is obtained from EventLogSession.
**
============================================================*/
using Microsoft.Win32;

namespace System.Diagnostics.Eventing.Reader
{
    /// <summary>
    /// Describes the run-time properties of logs and external log files.  An instance
    /// of this class is obtained from EventLogSession.
    /// </summary>
    public sealed class EventLogInformation
    {
        private DateTime? _creationTime;
        private DateTime? _lastAccessTime;
        private DateTime? _lastWriteTime;
        private long? _fileSize;
        private int? _fileAttributes;
        private long? _recordCount;
        private long? _oldestRecordNumber;
        private bool? _isLogFull;

        [System.Security.SecuritySafeCritical]
        internal EventLogInformation(EventLogSession session, string channelName, PathType pathType)
        {
            EventLogHandle logHandle = NativeWrapper.EvtOpenLog(session.Handle, channelName, pathType);

            using (logHandle)
            {
                _creationTime = (DateTime?)NativeWrapper.EvtGetLogInfo(logHandle, UnsafeNativeMethods.EvtLogPropertyId.EvtLogCreationTime);
                _lastAccessTime = (DateTime?)NativeWrapper.EvtGetLogInfo(logHandle, UnsafeNativeMethods.EvtLogPropertyId.EvtLogLastAccessTime);
                _lastWriteTime = (DateTime?)NativeWrapper.EvtGetLogInfo(logHandle, UnsafeNativeMethods.EvtLogPropertyId.EvtLogLastWriteTime);
                _fileSize = (long?)((ulong?)NativeWrapper.EvtGetLogInfo(logHandle, UnsafeNativeMethods.EvtLogPropertyId.EvtLogFileSize));
                _fileAttributes = (int?)((uint?)NativeWrapper.EvtGetLogInfo(logHandle, UnsafeNativeMethods.EvtLogPropertyId.EvtLogAttributes));
                _recordCount = (long?)((ulong?)NativeWrapper.EvtGetLogInfo(logHandle, UnsafeNativeMethods.EvtLogPropertyId.EvtLogNumberOfLogRecords));
                _oldestRecordNumber = (long?)((ulong?)NativeWrapper.EvtGetLogInfo(logHandle, UnsafeNativeMethods.EvtLogPropertyId.EvtLogOldestRecordNumber));
                _isLogFull = (bool?)NativeWrapper.EvtGetLogInfo(logHandle, UnsafeNativeMethods.EvtLogPropertyId.EvtLogFull);
            }
        }

        public DateTime? CreationTime { get { return _creationTime; } }
        public DateTime? LastAccessTime { get { return _lastAccessTime; } }
        public DateTime? LastWriteTime { get { return _lastWriteTime; } }
        public long? FileSize { get { return _fileSize; } }
        public int? Attributes { get { return _fileAttributes; } }
        public long? RecordCount { get { return _recordCount; } }
        public long? OldestRecordNumber { get { return _oldestRecordNumber; } }
        public bool? IsLogFull { get { return _isLogFull; } }
    }
}
