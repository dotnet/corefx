// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*============================================================
**
** Class: EventRecordWrittenEventArgs
**
** Purpose: 
** The EventArgs class for an EventLogWatcher notification.
**
============================================================*/


namespace System.Diagnostics.Eventing.Reader
{

    /// <summary>
    /// the custom event handler args.
    /// </summary>
    //[System.Security.Permissions.HostProtection(MayLeakOnAbort = true)]
    public sealed class EventRecordWrittenEventArgs : EventArgs
    {

        private EventRecord _record;
        private Exception _exception;

        internal EventRecordWrittenEventArgs(EventLogRecord record) { _record = record; }
        internal EventRecordWrittenEventArgs(Exception exception) { _exception = exception; }

        /// <summary>
        /// The EventRecord being notified.  
        /// NOTE: If non null, then caller is required to call Dispose().
        /// </summary>
        public EventRecord EventRecord
        {
            get { return _record; }
        }

        /// <summary>
        /// If any error occured during subscription, this will be non-null.
        /// After a notification containing an exception, no more notifications will
        /// be made for this subscription.
        /// </summary>
        public Exception EventException
        {
            get { return _exception; }
        }
    }

}
