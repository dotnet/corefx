// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace System.Net
{
    internal static partial class CertificateValidationPal
    {
        private static readonly object s_syncObject = new object();

        private static volatile X509Store s_myCertStoreEx;
        private static volatile X509Store s_myMachineCertStoreEx;

        static partial void CheckSupportsStore(StoreLocation storeLocation, ref bool hasSupport);

        internal static X509Store EnsureStoreOpened(bool isMachineStore)
        {
            X509Store store = isMachineStore ? s_myMachineCertStoreEx : s_myCertStoreEx;

            // TODO #3862 Investigate if this can be switched to either the static or Lazy<T> patterns.
            if (store == null)
            {
                StoreLocation storeLocation = isMachineStore ? StoreLocation.LocalMachine : StoreLocation.CurrentUser;

                // On Windows and OSX CheckSupportsStore is not defined, so the call is eliminated and the
                // if should be folded out.
                //
                // On Unix it will prevent the lock from being held and released over and over for the LocalMachine store.
                bool supportsStore = true;
                CheckSupportsStore(storeLocation, ref supportsStore);

                if (!supportsStore)
                {
                    return null;
                }

                lock (s_syncObject)
                {
                    store = isMachineStore ? s_myMachineCertStoreEx : s_myCertStoreEx;

                    if (store == null)
                    {
                        try
                        {
                            // NOTE: that if this call fails we won't keep track and the next time we enter we will try to open the store again.
                            store = OpenStore(storeLocation);

                            if (NetEventSource.IsEnabled)
                                NetEventSource.Info(null, $"storeLocation: {storeLocation} returned store {store}");

                            if (isMachineStore)
                            {
                                s_myMachineCertStoreEx = store;
                            }
                            else
                            {
                                s_myCertStoreEx = store;
                            }
                        }
                        catch (Exception exception)
                        {
                            if (exception is CryptographicException || exception is SecurityException)
                            {
                                NetEventSource.Fail(null,
                                    $"Failed to open cert store, location: {storeLocation} exception: {exception}");
                                return null;
                            }

                            if (NetEventSource.IsEnabled)
                                NetEventSource.Error(null, SR.Format(SR.net_log_open_store_failed, storeLocation, exception));

                            throw;
                        }
                    }
                }
            }

            return store;
        }
    }
}
