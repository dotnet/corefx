// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogEntryCollectionTests
    {
        private const string message = "EntryCollectionMessage";

        [ConditionalFact(typeof(Helpers), nameof(Helpers.IsElevatedAndSupportsEventLogs))]
        public void CopyingEventLogEntryCollection()
        {
            string log = "CopyCollection";
            string source = "Source_" + nameof(CopyingEventLogEntryCollection);

            try
            {
                EventLog.CreateEventSource(source, log);
                using (EventLog eventLog = new EventLog())
                {
                    eventLog.Source = source;
                    bool entryWritten = true;
                    while (entryWritten)
                    {
                        try
                        {
                            eventLog.WriteEntry(message);
                            eventLog.WriteEntry("Further Testing");
                            entryWritten = false;
                        }
                        catch (Win32Exception)
                        {
                            Thread.Sleep(100);
                            eventLog.Clear();
                        }
                    }

                    EventLogEntryCollection entryCollection = eventLog.Entries;
                    EventLogEntry[] entryCollectionCopied = new EventLogEntry[entryCollection.Count];

                    bool entryCopied = true;
                    while (entryCopied)
                    {
                        try
                        {
                            entryCollection.CopyTo(entryCollectionCopied, 0);
                            entryCopied = false;
                        }
                        catch (Win32Exception)
                        {
                            Thread.Sleep(100);
                            eventLog.Clear();
                        }
                    }

                    bool entryRetrived = true;
                    while (entryRetrived)
                    {
                        try
                        {
                            int i = 0;
                            foreach (EventLogEntry entry in entryCollection)
                            {
                                Assert.Equal(entry.Message, entryCollectionCopied[i].Message);
                                i += 1;
                            }
                            entryRetrived = false;
                        }
                        catch (Win32Exception)
                        {
                            Thread.Sleep(100);
                        }
                    }

                }
            }
            finally
            {
                EventLog.DeleteEventSource(source);
                EventLog.Delete(log);
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.IsElevatedAndSupportsEventLogs))]
        public void CheckingEntryEqualityWithNull()
        {
            string log = "NullTest";
            string source = "Source_" + nameof(CheckingEntryEqualityWithNull);

            try
            {
                EventLog.CreateEventSource(source, log);
                using (EventLog eventLog = new EventLog())
                {
                    eventLog.Source = source;
                    bool entryWritten = true;
                    while (entryWritten)
                    {
                        try
                        {
                            eventLog.WriteEntry(message);
                            EventLogEntry entry = eventLog.Entries[eventLog.Entries.Count - 1];
                            Assert.False(entry.Equals(null));
                            entryWritten = false;
                        }
                        catch (Win32Exception)
                        {
                            Thread.Sleep(100);
                            eventLog.Clear();
                        }
                    }
                }
            }
            finally
            {
                EventLog.DeleteEventSource(source);
                EventLog.Delete(log);
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.IsElevatedAndSupportsEventLogs))]
        public void CheckingEntryEqualityAndIndex()
        {
            string log = "IndexTest";
            string source = "Source_" + nameof(CheckingEntryEqualityAndIndex);

            try
            {
                EventLog.CreateEventSource(source, log);
                using (EventLog eventLog = new EventLog())
                {
                    eventLog.Source = source;
                    bool entryWritten = true;
                    while (entryWritten)
                    {
                        try
                        {
                            eventLog.WriteEntry(message);
                            EventLogEntry entry = eventLog.Entries[eventLog.Entries.Count - 1];
                            Assert.True(entry.Equals(entry));
                            eventLog.WriteEntry(message);
                            EventLogEntry secondEntry = eventLog.Entries[eventLog.Entries.Count - 1];
                            Assert.Equal(entry.Index + 1, secondEntry.Index);
                            entryWritten = false;
                        }
                        catch (Win32Exception)
                        {
                            Thread.Sleep(100);
                            eventLog.Clear();
                        }
                    }
                }
            }
            finally
            {
                EventLog.DeleteEventSource(source);
                EventLog.Delete(log);
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.IsElevatedAndSupportsEventLogs))]
        public void CheckingEntryInEquality()
        {
            string log = "InEqualityTest";
            string source = "Source_" + nameof(CheckingEntryInEquality);

            try
            {
                EventLog.CreateEventSource(source, log);
                using (EventLog eventLog = new EventLog())
                {
                    eventLog.Source = source;
                    bool entryWritten = true;
                    while (entryWritten)
                    {
                        try
                        {
                            eventLog.WriteEntry(message);
                            eventLog.WriteEntry(message);
                            EventLogEntry entry = eventLog.Entries[eventLog.Entries.Count - 1];
                            EventLogEntry secondEntry = eventLog.Entries[eventLog.Entries.Count - 2];
                            Assert.False(entry.Equals(secondEntry));
                            entryWritten = false;
                        }
                        catch (Win32Exception)
                        {
                            Thread.Sleep(100);
                            eventLog.Clear();
                        }
                    }
                }
            }
            finally
            {
                EventLog.DeleteEventSource(source);
                EventLog.Delete(log);
            }
        }
    }
}
