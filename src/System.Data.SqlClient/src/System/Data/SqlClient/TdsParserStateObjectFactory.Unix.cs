﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.SqlClient.SNI;

namespace System.Data.SqlClient
{
    internal sealed class TdsParserStateObjectFactory
    {

        public static bool useManagedSni = true;

        public static readonly TdsParserStateObjectFactory Singleton = new TdsParserStateObjectFactory();

        public TdsParserStateObject CreateTdsParserStateObject(TdsParser parser)
        {
            return new TdsParserStateObjectManaged(parser);
        }

        internal TdsParserStateObject CreateSessionObject(TdsParser tdsParser, TdsParserStateObject _pMarsPhysicalConObj, bool v)
        {
            return new TdsParserStateObjectManaged(tdsParser, _pMarsPhysicalConObj, true);
        }
    }
}
