// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.SqlClient.SNI;

namespace System.Data.SqlClient
{
    internal sealed class TdsParserStateObjectFactory
    {

        private const string UseLegacyNetworkingOnWindows = "System.Data.SqlClient.UseLegacyNetworkingOnWindows";

        static TdsParserStateObjectFactory()
        {
            // If The appcontext switch is set then Use Managed SNI based on the value. Otherwise Managed SNI should always be used.
            UseManagedSni = AppContext.TryGetSwitch(UseLegacyNetworkingOnWindows, out shouldUseLegacyNetorking) ? !shouldUseLegacyNetorking : true;
        }

        private static bool shouldUseLegacyNetorking;

        public static bool UseManagedSni
        {
            get; private set;
        }

        public static readonly TdsParserStateObjectFactory Singleton = new TdsParserStateObjectFactory();

        public EncryptionOptions EncryptionOptions
        {
            get
            {
                return UseManagedSni ? SNI.SNILoadHandle.SingletonInstance.Options : SNILoadHandle.SingletonInstance.Options;
            }
        }

        public uint SNIStatus
        {
            get
            {
                return UseManagedSni ? SNI.SNILoadHandle.SingletonInstance.Status : SNILoadHandle.SingletonInstance.Status;
            }
        }

        public TdsParserStateObject CreateTdsParserStateObject(TdsParser parser)
        {
            if (UseManagedSni)
            {
                return new TdsParserStateObjectManaged(parser);
            }
            else
            {
                return new TdsParserStateObjectNative(parser);
            }
        }

        internal TdsParserStateObject CreateSessionObject(TdsParser tdsParser, TdsParserStateObject _pMarsPhysicalConObj, bool v)
        {
            if (TdsParserStateObjectFactory.UseManagedSni)
            {
                return new TdsParserStateObjectManaged(tdsParser, _pMarsPhysicalConObj, true);
            }
            else
            {
                return new TdsParserStateObjectNative(tdsParser, _pMarsPhysicalConObj, true);
            }
        }
    }
}
