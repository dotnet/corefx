// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



//------------------------------------------------------------------------------

using System.Diagnostics;
using System.Data.ProviderBase;
using System.Threading;

namespace System.Data.SqlClient
{
    sealed internal class SqlReferenceCollection : DbReferenceCollection
    {
        private sealed class FindLiveCommandContext
        {
            public readonly Func<SqlCommand, bool> Func;

            private TdsParserStateObject stateObj;

            public FindLiveCommandContext() => Func = Predicate;

            public void Setup(TdsParserStateObject state) => stateObj = state;

            public void Clear() => stateObj = null;

            private bool Predicate(SqlCommand command) => command.StateObject == stateObj;
        }

        private sealed class FindLiveReaderContext
        {
            public readonly Func<SqlDataReader, bool> Func;

            private SqlCommand command;

            public FindLiveReaderContext() => Func = Predicate;

            public void Setup(SqlCommand command) => this.command = command;

            public void Clear() => command = null;

            private bool Predicate(SqlDataReader reader) => (!reader.IsClosed) && (command == reader.Command);
        }

        internal const int DataReaderTag = 1;
        internal const int CommandTag = 2;
        internal const int BulkCopyTag = 3;

        private readonly static Func<SqlDataReader, bool> s_hasOpenReaderFunc = HasOpenReaderPredicate;
        private static FindLiveCommandContext cachedFindLiveCommandContext;
        private static FindLiveReaderContext cachedFindLiveReaderContext;

        override public void Add(object value, int tag)
        {
            Debug.Assert(DataReaderTag == tag || CommandTag == tag || BulkCopyTag == tag, "unexpected tag?");
            Debug.Assert(DataReaderTag != tag || value is SqlDataReader, "tag doesn't match object type: SqlDataReader");
            Debug.Assert(CommandTag != tag || value is SqlCommand, "tag doesn't match object type: SqlCommand");
            Debug.Assert(BulkCopyTag != tag || value is SqlBulkCopy, "tag doesn't match object type: SqlBulkCopy");

            base.AddItem(value, tag);
        }

        internal void Deactivate()
        {
            base.Notify(0);
        }

        internal SqlDataReader FindLiveReader(SqlCommand command)
        {
            if (command is null)
            {
                // if null == command, will find first live datareader
                return FindItem<SqlDataReader>(DataReaderTag, s_hasOpenReaderFunc);
            }
            else
            {
                // else will find live datareader associated with the command
                FindLiveReaderContext context = Interlocked.Exchange(ref cachedFindLiveReaderContext, null) ?? new FindLiveReaderContext();
                context.Setup(command);
                SqlDataReader retval = FindItem(DataReaderTag, context.Func);
                context.Clear();
                Interlocked.CompareExchange(ref cachedFindLiveReaderContext, context, null);
                return retval;
            }
        }

        // Finds a SqlCommand associated with the given StateObject
        internal SqlCommand FindLiveCommand(TdsParserStateObject stateObj)
        {
            FindLiveCommandContext context = Interlocked.Exchange(ref cachedFindLiveCommandContext, null) ?? new FindLiveCommandContext();
            context.Setup(stateObj);
            SqlCommand retval = FindItem(CommandTag, context.Func);
            context.Clear();
            Interlocked.CompareExchange(ref cachedFindLiveCommandContext, context, null);
            return retval;
        }

        override protected void NotifyItem(int message, int tag, object value)
        {
            Debug.Assert(0 == message, "unexpected message?");
            Debug.Assert(DataReaderTag == tag || CommandTag == tag || BulkCopyTag == tag, "unexpected tag?");

            if (tag == DataReaderTag)
            {
                Debug.Assert(value is SqlDataReader, "Incorrect object type");
                var rdr = (SqlDataReader)value;
                if (!rdr.IsClosed)
                {
                    rdr.CloseReaderFromConnection();
                }
            }
            else if (tag == CommandTag)
            {
                Debug.Assert(value is SqlCommand, "Incorrect object type");
                ((SqlCommand)value).OnConnectionClosed();
            }
            else if (tag == BulkCopyTag)
            {
                Debug.Assert(value is SqlBulkCopy, "Incorrect object type");
                ((SqlBulkCopy)value).OnConnectionClosed();
            }
        }

        override public void Remove(object value)
        {
            Debug.Assert(value is SqlDataReader || value is SqlCommand || value is SqlBulkCopy, "SqlReferenceCollection.Remove expected a SqlDataReader or SqlCommand or SqlBulkCopy");

            base.RemoveItem(value);
        }

        private static bool HasOpenReaderPredicate(SqlDataReader reader)
        {
            return !reader.IsClosed;
        }


    }
}
