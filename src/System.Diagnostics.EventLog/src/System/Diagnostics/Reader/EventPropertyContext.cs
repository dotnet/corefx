// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
** Purpose:
** This public abstract class defines the methods / properties
** for a context object used to access a set of Data Values from
** an EventRecord.
**
============================================================*/

namespace System.Diagnostics.Eventing.Reader
{
    public abstract class EventPropertyContext : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
