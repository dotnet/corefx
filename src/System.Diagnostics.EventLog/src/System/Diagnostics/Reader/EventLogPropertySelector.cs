// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
** Purpose:
** Public class that encapsulates the information for fast
** access to Event Values of an EventLogRecord. Implements
** the EventPropertyContext abstract class.  An instance of this
** class is constructed and then passed to
** EventLogRecord.GetEventPropertyValues.
**
============================================================*/

using System.Collections.Generic;
using Microsoft.Win32;

namespace System.Diagnostics.Eventing.Reader
{
    /// <summary>
    ///  Encapsulates the information for fast access to Event Values
    ///  of an EventLogRecord.  An instance of this class is constructed
    ///  and then passed to EventLogRecord.GetEventPropertyValues.
    /// </summary>
    public class EventLogPropertySelector : IDisposable
    {
        //
        // access to the data member reference is safe, while
        // invoking methods on it is marked SecurityCritical as appropriate.
        //
        private EventLogHandle _renderContextHandleValues;

        [System.Security.SecurityCritical]
        public EventLogPropertySelector(IEnumerable<string> propertyQueries)
        {
            if (propertyQueries == null)
                throw new ArgumentNullException("propertyQueries");

            string[] paths;

            ICollection<string> coll = propertyQueries as ICollection<string>;
            if (coll != null)
            {
                paths = new string[coll.Count];
                coll.CopyTo(paths, 0);
            }
            else
            {
                List<string> queries;
                queries = new List<string>(propertyQueries);
                paths = queries.ToArray();
            }

            _renderContextHandleValues = NativeWrapper.EvtCreateRenderContext(paths.Length, paths, UnsafeNativeMethods.EvtRenderContextFlags.EvtRenderContextValues);
        }

        internal EventLogHandle Handle
        {
            // just returning reference to security critical type, the methods
            // of that type are protected by SecurityCritical as appropriate.
            get
            {
                return _renderContextHandleValues;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [System.Security.SecuritySafeCritical]
        protected virtual void Dispose(bool disposing)
        {
            if (_renderContextHandleValues != null && !_renderContextHandleValues.IsInvalid)
                _renderContextHandleValues.Dispose();
        }
    }
}
