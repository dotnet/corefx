// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Data.SqlClient.SNI;


using MSS = Microsoft.SqlServer.Server;

namespace System.Data.SqlClient
{
    sealed internal partial class TdsParser
    {
        private static bool s_fSSPILoaded = false; // bool to indicate whether library has been loaded

        internal void PostReadAsyncForMars()
        {
            if (TdsParserStateObjectFactory.useManagedSni)
                return;

            // HACK HACK HACK - for Async only
            // Have to post read to initialize MARS - will get callback on this when connection goes
            // down or is closed.

            IntPtr temp = IntPtr.Zero;
            uint error = TdsEnums.SNI_SUCCESS;

            try { }
            finally
            {
                _pMarsPhysicalConObj.IncrementPendingCallbacks();

                error = SNINativeMethodWrapper.SNIReadAsync((SNIHandle)_pMarsPhysicalConObj.HandleObject, ref temp);

                if (temp != IntPtr.Zero)
                {
                    // Be sure to release packet, otherwise it will be leaked by native.
                    SNINativeMethodWrapper.SNIPacketRelease(temp);
                }
            }
            Debug.Assert(IntPtr.Zero == temp, "unexpected syncReadPacket without corresponding SNIPacketRelease");
            if (TdsEnums.SNI_SUCCESS_IO_PENDING != error)
            {
                Debug.Assert(TdsEnums.SNI_SUCCESS != error, "Unexpected successful read async on physical connection before enabling MARS!");
                _physicalStateObj.AddError(ProcessSNIError(_physicalStateObj));
                ThrowExceptionAndWarning(_physicalStateObj);
            }
                
        }

        private void LoadSSPILibrary()
        {
            // Outer check so we don't acquire lock once it's loaded.
            if (!s_fSSPILoaded)
            {
                lock (s_tdsParserLock)
                {
                    // re-check inside lock
                    if (!s_fSSPILoaded)
                    {
                        // use local for ref param to defer setting s_maxSSPILength until we know the call succeeded.
                        UInt32 maxLength = 0;

                        if (0 != SNINativeMethodWrapper.SNISecInitPackage(ref maxLength))
                            SSPIError(SQLMessage.SSPIInitializeError(), TdsEnums.INIT_SSPI_PACKAGE);

                        s_maxSSPILength = maxLength;
                        s_fSSPILoaded = true;
                    }
                }
            }

            if (s_maxSSPILength > Int32.MaxValue)
            {
                throw SQL.InvalidSSPIPacketSize();   // SqlBu 332503
            }
        }

        private SNIErrorDetails GetSniErrorDetails()
        {
            SNIErrorDetails details = new SNIErrorDetails();

            if (TdsParserStateObjectFactory.useManagedSni)
            {
                SNIError sniError = SNIProxy.Singleton.GetLastError();
                details.sniErrorNumber = sniError.sniError;
                details.errorMessage = sniError.errorMessage;
                details.win32ErrorCode = sniError.nativeError;
                details.provider = (int)sniError.provider;
                details.lineNumber = sniError.lineNumber;
                details.function = sniError.function;
                details.exception = sniError.exception;
            }
            else
            {
                SNINativeMethodWrapper.SNI_Error sniError;
                SNINativeMethodWrapper.SNIGetLastError(out sniError);
                details.sniErrorNumber = sniError.sniError;
                details.errorMessage = sniError.errorMessage;
                details.win32ErrorCode = sniError.nativeError;
                details.provider = (int)sniError.provider;
                details.lineNumber = sniError.lineNumber;
                details.function = sniError.function;
            }
            return details;

        }

    }    // tdsparser
}//namespace