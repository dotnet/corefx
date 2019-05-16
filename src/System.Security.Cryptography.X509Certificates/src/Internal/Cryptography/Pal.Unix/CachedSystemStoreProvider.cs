// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal sealed class CachedSystemStoreProvider : IStorePal
    {
        private static readonly TimeSpan s_lastWriteRecheckInterval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan s_assumeInvalidInterval = TimeSpan.FromMinutes(5);
        private static readonly Stopwatch s_recheckStopwatch = new Stopwatch();

        // Use non-Value-Tuple so that it's an atomic update.
        private static Tuple<SafeX509StackHandle,SafeX509StackHandle> s_nativeCollections;
        private static DateTime s_directoryCertsLastWrite;
        private static DateTime s_fileCertsLastWrite;
        private static string s_rootStoreDirectory = Interop.Crypto.GetX509RootStorePath();
        private static string s_rootStoreFile = Interop.Crypto.GetX509RootStoreFile();

        private readonly bool _isRoot;

        private CachedSystemStoreProvider(bool isRoot)
        {
            _isRoot = isRoot;
        }

        internal static CachedSystemStoreProvider MachineRoot { get; } =
            new CachedSystemStoreProvider(true);

        internal static CachedSystemStoreProvider MachineIntermediate { get; } =
            new CachedSystemStoreProvider(false);


        public void Dispose()
        {
            // No-op
        }

        public void CloneTo(X509Certificate2Collection collection)
        {
            Tuple<SafeX509StackHandle, SafeX509StackHandle> nativeColls = GetCollections();
            SafeX509StackHandle nativeColl = _isRoot ? nativeColls.Item1 : nativeColls.Item2;

            int count = Interop.Crypto.GetX509StackFieldCount(nativeColl);

            for (int i = 0; i < count; i++)
            {
                X509Certificate2 clone = new X509Certificate2(Interop.Crypto.GetX509StackField(nativeColl, i));
                collection.Add(clone);
            }
        }

        internal static void GetNativeCollections(out SafeX509StackHandle root, out SafeX509StackHandle intermediate)
        {
            Tuple<SafeX509StackHandle, SafeX509StackHandle> nativeColls = GetCollections();
            root = nativeColls.Item1;
            intermediate = nativeColls.Item2;
        }

        public void Add(ICertificatePal cert)
        {
            // These stores can only be opened in ReadOnly mode.
            throw new InvalidOperationException();
        }

        public void Remove(ICertificatePal cert)
        {
            // These stores can only be opened in ReadOnly mode.
            throw new InvalidOperationException();
        }

        public SafeHandle SafeHandle => null;

        private static Tuple<SafeX509StackHandle, SafeX509StackHandle> GetCollections()
        {
            TimeSpan elapsed = s_recheckStopwatch.Elapsed;
            Tuple<SafeX509StackHandle, SafeX509StackHandle> ret = s_nativeCollections;

            if (ret == null || elapsed > s_lastWriteRecheckInterval)
            {
                lock (s_recheckStopwatch)
                {
                    FileInfo fileInfo = null;
                    DirectoryInfo dirInfo = null;

                    if (!string.IsNullOrEmpty(s_rootStoreFile))
                    {
                        try
                        {
                            fileInfo = new FileInfo(s_rootStoreFile);
                        }
                        catch (ArgumentException)
                        {
                            // If SSL_CERT_FILE is set to the empty string, or anything else which gives
                            // "The path is not of a legal form", then the GetX509RootStoreFile value is ignored.
                            //
                            // Rather than do this every reload, just clear out the filename.
                            s_rootStoreFile = null;
                        }
                    }

                    if (!string.IsNullOrEmpty(s_rootStoreDirectory))
                    {
                        try
                        {
                            dirInfo = new DirectoryInfo(s_rootStoreDirectory);
                        }
                        catch (ArgumentException)
                        {
                            // If SSL_CERT_DIR is set to the empty string, or anything else which gives
                            // "The path is not of a legal form", then the GetX509RootStoreFile value is ignored.
                            //
                            // Rather than do this every reload, just clear out the filename.
                            s_rootStoreDirectory = null;
                        }
                    }

                    if (ret == null ||
                        elapsed > s_assumeInvalidInterval ||
                        (fileInfo != null && fileInfo.Exists && fileInfo.LastWriteTimeUtc != s_fileCertsLastWrite) ||
                        (dirInfo != null && dirInfo.Exists && dirInfo.LastWriteTimeUtc != s_directoryCertsLastWrite))
                    {
                        ret = LoadMachineStores(dirInfo, fileInfo);
                    }
                }
            }

            Debug.Assert(ret != null);
            return ret;
        }

        private static Tuple<SafeX509StackHandle, SafeX509StackHandle> LoadMachineStores(
            DirectoryInfo rootStorePath,
            FileInfo rootStoreFile)
        {
            Debug.Assert(
                Monitor.IsEntered(s_recheckStopwatch),
                "LoadMachineStores assumes a lock(s_recheckStopwatch)");

#if PRINT_STORE_RELOAD
            Interop.Sys.PrintF($"Reloading system trust (dir=\"{s_rootStoreDirectory}\", file=\"{s_rootStoreFile}\")\n", "");
#endif

            IEnumerable<FileInfo> trustedCertFiles;
            DateTime newFileTime = default;
            DateTime newDirTime = default;

            if (rootStorePath != null && rootStorePath.Exists)
            {
                trustedCertFiles = rootStorePath.EnumerateFiles();
                newDirTime = rootStorePath.LastWriteTimeUtc;
            }
            else
            {
                trustedCertFiles = Array.Empty<FileInfo>();
            }

            if (rootStoreFile != null && rootStoreFile.Exists)
            {
                trustedCertFiles = trustedCertFiles.Prepend(rootStoreFile);
                newFileTime = rootStoreFile.LastWriteTimeUtc;
            }

            SafeX509StackHandle rootStore = Interop.Crypto.NewX509Stack();
            Interop.Crypto.CheckValidOpenSslHandle(rootStore);
            SafeX509StackHandle intermedStore = Interop.Crypto.NewX509Stack();
            Interop.Crypto.CheckValidOpenSslHandle(intermedStore);

            HashSet<X509Certificate2> uniqueRootCerts = new HashSet<X509Certificate2>();
            HashSet<X509Certificate2> uniqueIntermediateCerts = new HashSet<X509Certificate2>();

            foreach (FileInfo file in trustedCertFiles)
            {
                using (SafeBioHandle fileBio = Interop.Crypto.BioNewFile(file.FullName, "rb"))
                {
                    // The handle may be invalid, for example when we don't have read permission for the file.
                    if (fileBio.IsInvalid)
                    {
                        Interop.Crypto.ErrClearError();
                        continue;
                    }

                    ICertificatePal pal;

                    // Some distros ship with two variants of the same certificate.
                    // One is the regular format ('BEGIN CERTIFICATE') and the other
                    // contains additional AUX-data ('BEGIN TRUSTED CERTIFICATE').
                    // The additional data contains the appropriate usage (e.g. emailProtection, serverAuth, ...).
                    // Because corefx doesn't validate for a specific usage, derived certificates are rejected.
                    // For now, we skip the certificates with AUX data and use the regular certificates.
                    while (OpenSslX509CertificateReader.TryReadX509PemNoAux(fileBio, out pal) ||
                        OpenSslX509CertificateReader.TryReadX509Der(fileBio, out pal))
                    {
                        OpenSslX509CertificateReader typedPal = (OpenSslX509CertificateReader)pal;
                        X509Certificate2 cert = new X509Certificate2(pal);

                        // The HashSets are just used for uniqueness filters, they do not survive this method.
                        if (StringComparer.Ordinal.Equals(cert.Subject, cert.Issuer))
                        {
                            if (uniqueRootCerts.Add(cert))
                            {
                                using (SafeX509Handle tmp = Interop.Crypto.X509UpRef(pal.Handle))
                                {
                                    if (!Interop.Crypto.PushX509StackField(rootStore, tmp))
                                    {
                                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                                    }

                                    // The ownership has been transferred to the stack
                                    tmp.SetHandleAsInvalid();
                                }

                                continue;
                            }
                        }
                        else
                        {
                            if (uniqueIntermediateCerts.Add(cert))
                            {
                                using (SafeX509Handle tmp = Interop.Crypto.X509UpRef(pal.Handle))
                                {
                                    if (!Interop.Crypto.PushX509StackField(intermedStore, tmp))
                                    {
                                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                                    }

                                    // The ownership has been transferred to the stack
                                    tmp.SetHandleAsInvalid();
                                }

                                continue;
                            }
                        }

                        // There's a good chance we'll encounter duplicates on systems that have both one-cert-per-file
                        // and one-big-file trusted certificate stores. Anything that wasn't unique will end up here.
                        cert.Dispose();
                    }
                }
            }

            foreach (X509Certificate2 cert in uniqueRootCerts)
            {
                cert.Dispose();
            }

            foreach (X509Certificate2 cert in uniqueIntermediateCerts)
            {
                cert.Dispose();
            }

            Tuple<SafeX509StackHandle, SafeX509StackHandle> newCollections =
                Tuple.Create(rootStore, intermedStore);

            Debug.Assert(
                Monitor.IsEntered(s_recheckStopwatch),
                "LoadMachineStores assumes a lock(s_recheckStopwatch)");

            Volatile.Write(ref s_nativeCollections, newCollections);
            s_directoryCertsLastWrite = newDirTime;
            s_fileCertsLastWrite = newFileTime;
            s_recheckStopwatch.Restart();
            return newCollections;
        }
    }
}
