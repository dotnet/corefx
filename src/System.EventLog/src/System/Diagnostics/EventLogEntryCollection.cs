// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Diagnostics
{
    public class EventLogEntryCollection : ICollection
    {
        private readonly EventLogInternal log;

        internal EventLogEntryCollection(EventLogInternal log)
        {
            this.log = log;
        }

        public int Count
        {
            get
            {
                return log.EntryCount;
            }
        }

        public virtual EventLogEntry this[int index]
        {
            get
            {
                return log.GetEntryAt(index);
            }
        }

        public void CopyTo(EventLogEntry[] entries, int index)
        {
            ((ICollection)this).CopyTo((Array)entries, index);
        }

        public IEnumerator GetEnumerator()
        {
            return new EntriesEnumerator(this);
        }

        internal EventLogEntry GetEntryAtNoThrow(int index)
        {
            return log.GetEntryAtNoThrow(index);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            EventLogEntry[] entries = log.GetAllEntries();
            Array.Copy(entries, 0, array, index, entries.Length);
        }

        private class EntriesEnumerator : IEnumerator
        {
            private EventLogEntryCollection entries;
            private int num = -1;
            private EventLogEntry cachedEntry = null;

            internal EntriesEnumerator(EventLogEntryCollection entries)
            {
                this.entries = entries;
            }

            public object Current
            {
                get
                {
                    if (cachedEntry == null)
                        throw new InvalidOperationException(SR.NoCurrentEntry);

                    return cachedEntry;
                }
            }

            public bool MoveNext()
            {
                num++;
                cachedEntry = entries.GetEntryAtNoThrow(num);

                return cachedEntry != null;
            }

            public void Reset()
            {
                num = -1;
            }
        }
    }
}
