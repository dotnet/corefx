// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
    public class CryptoConfig
    {
        private const BindingFlags ConstructorDefault = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;

        private const string ECDsaIdentifier = "ECDsa";

        private static readonly Lazy<IReadOnlyDictionary<string, string>> s_lazyTypeNameToOid = new Lazy<IReadOnlyDictionary<string, string>>(CreateTypeNameToOidMapping);
        private static readonly Lazy<IReadOnlyDictionary<string, string>> s_lazyNameToFullTypeName = new Lazy<IReadOnlyDictionary<string, string>>(CreateNameToFullTypeNameMapping);

        // predefined types, types added by the user and lazy loaded types from full names from different assemblies
        private static readonly Lazy<ConcurrentDictionary<string, Type>> s_lazyTypeNameToType = new Lazy<ConcurrentDictionary<string, Type>>(CreateTypeNameToTypeMapping);

        private static volatile Dictionary<string, string> appOidHT = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static readonly char[] SepArray = { '.' }; // valid ASN.1 separators

        // CoreFx does not support AllowOnlyFipsAlgorithms
        public static bool AllowOnlyFipsAlgorithms => false;

        // Private object for locking instead of locking on a public type for SQL reliability work.
        private static object s_InternalSyncObject = new object();

        public static void AddAlgorithm(Type algorithm, params string[] names)
        {
            if (algorithm == null)
            {
                throw new ArgumentNullException(nameof(algorithm));
            }
            if (!algorithm.IsVisible)
            {
                throw new ArgumentException(SR.Cryptography_AlgorithmTypesMustBeVisible, nameof(algorithm));
            }
            if (names == null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            string[] safeCopy = names.Length == 1 ? new string[1] { names[0] } : names.ToArray();

            // Pre-check the algorithm names for validity so that we don't add a few of the names and then
            // throw an exception if we find an invalid name partway through the list.
            foreach (string name in safeCopy)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException(SR.Cryptography_AddNullOrEmptyName);
                }
            }

            // Everything looks valid, so we're safe to add the name mappings
            foreach (string name in safeCopy)
            {
                s_lazyTypeNameToType.Value[name] = algorithm;
            }
        }

        public static object CreateFromName(string name) => CreateFromName(name, null);

        public static object CreateFromName(string name, params object[] args)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!TryGetTypeFromName(name, out Type type))
            {
                // Special case asking for "ECDsa" since the default map from .NET Framework uses
                // a Windows-only type.
                if ((args == null || args.Length == 1) && name == ECDsaIdentifier)
                {
                    return ECDsa.Create();
                }

                // Still null? Then we didn't find it.
                return null;
            }

            if (args == null)
            {
                args = Array.Empty<object>();
            }

            if (!TryGetConstructors(type, args, out MethodBase[] cons))
            {
                return null;
            }

            // Bind to matching ctor.
            ConstructorInfo rci = Type.DefaultBinder.BindToMethod(ConstructorDefault, cons, ref args, null, null, null, out object state) as ConstructorInfo;

            // Check for ctor we don't like (non-existent, delegate or decorated with declarative linktime demand).
            if (rci == null || typeof(Delegate).IsAssignableFrom(rci.DeclaringType))
            {
                return null;
            }

            // Ctor invoke and allocation.
            object retval = rci.Invoke(ConstructorDefault, Type.DefaultBinder, args, null);

            // Reset any parameter re-ordering performed by the binder.
            if (state != null)
            {
                Type.DefaultBinder.ReorderArgumentArray(ref args, state);
            }

            return retval;
        }

        private static bool TryGetTypeFromName(string name, out Type result)
        {
            if (s_lazyTypeNameToType.Value.TryGetValue(name, out result))
            {
                return true;
            }

            if (s_lazyNameToFullTypeName.Value.TryGetValue(name, out string fullTypeName))
            {
                // it was not registered using simple name, perhaps we can find it using the full name?
                if (s_lazyTypeNameToType.Value.TryGetValue(fullTypeName, out result))
                {
                    return true;
                }

                // try to load it using the full name
                Type loadedUsingFullName = Type.GetType(fullTypeName, false, false);
                if (loadedUsingFullName != null && loadedUsingFullName.IsVisible)
                {
                    // Type.GetType is expensive, we want to cache the result
                    s_lazyTypeNameToType.Value[name] = loadedUsingFullName;
                    s_lazyTypeNameToType.Value[fullTypeName] = loadedUsingFullName;

                    result = loadedUsingFullName;
                    return true;
                }
            }

            // try to load it using the name
            Type loadedUsingName = Type.GetType(name, false, false);
            if (loadedUsingName != null && loadedUsingName.IsVisible)
            {
                // Type.GetType is expensive, we want to cache the result
                s_lazyTypeNameToType.Value[name] = loadedUsingName;

                result = loadedUsingName;
                return true;
            }

            return false;
        }

        private static bool TryGetConstructors(Type type, object[] arguments, out MethodBase[] match)
        {
            match = null;

            // Locate all constructors.
            MethodBase[] constructors = type.GetConstructors(ConstructorDefault);
            if (constructors == null || constructors.Length == 0)
            {
                return false;
            }

            List<MethodBase> candidates = new List<MethodBase>();
            for (int i = 0; i < constructors.Length; i++)
            {
                MethodBase con = constructors[i];
                if (con.GetParameters().Length == arguments.Length)
                {
                    candidates.Add(con);
                }
            }

            if (candidates.Count == 0)
            {
                return false;
            }
            else
            {
                match = candidates.ToArray();
                return true;
            }
        }

        public static void AddOID(string oid, params string[] names)
        {
            if (oid == null)
                throw new ArgumentNullException(nameof(oid));
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            string[] oidNames = new string[names.Length];
            Array.Copy(names, 0, oidNames, 0, oidNames.Length);

            // Pre-check the input names for validity, so that we don't add a few of the names and throw an
            // exception if an invalid name is found further down the array.
            foreach (string name in oidNames)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException(SR.Cryptography_AddNullOrEmptyName);
                }
            }

            // Everything is valid, so we're good to lock the hash table and add the application mappings
            lock (s_InternalSyncObject)
            {
                foreach (string name in oidNames)
                {
                    appOidHT[name] = oid;
                }
            }
        }

        public static string MapNameToOID(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            string oidName;

            // Check to see if we have an application defined mapping
            lock (s_InternalSyncObject)
            {
                if (!appOidHT.TryGetValue(name, out oidName))
                {
                    oidName = null;
                }
            }

            if (string.IsNullOrEmpty(oidName) && !s_lazyTypeNameToOid.Value.TryGetValue(name, out oidName))
            {
                try
                {
                    Oid oid = Oid.FromFriendlyName(name, OidGroup.All);
                    oidName = oid.Value;
                }
                catch (CryptographicException) { }
            }

            return oidName;
        }

        public static byte[] EncodeOID(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            string[] oidString = str.Split(SepArray);
            uint[] oidNums = new uint[oidString.Length];
            for (int i = 0; i < oidString.Length; i++)
            {
                oidNums[i] = unchecked((uint)int.Parse(oidString[i], CultureInfo.InvariantCulture));
            }

            // Handle the first two oidNums special
            if (oidNums.Length < 2)
                throw new CryptographicUnexpectedOperationException(SR.Cryptography_InvalidOID);

            uint firstTwoOidNums = unchecked((oidNums[0] * 40) + oidNums[1]);

            // Determine length of output array
            int encodedOidNumsLength = 2; // Reserve first two bytes for later
            EncodeSingleOidNum(firstTwoOidNums, null, ref encodedOidNumsLength);
            for (int i = 2; i < oidNums.Length; i++)
            {
                EncodeSingleOidNum(oidNums[i], null, ref encodedOidNumsLength);
            }

            // Allocate the array to receive encoded oidNums
            byte[] encodedOidNums = new byte[encodedOidNumsLength];
            int encodedOidNumsIndex = 2;

            // Encode each segment
            EncodeSingleOidNum(firstTwoOidNums, encodedOidNums, ref encodedOidNumsIndex);
            for (int i = 2; i < oidNums.Length; i++)
            {
                EncodeSingleOidNum(oidNums[i], encodedOidNums, ref encodedOidNumsIndex);
            }

            Debug.Assert(encodedOidNumsIndex == encodedOidNumsLength);

            // Final return value is 06 <length> encodedOidNums[]
            if (encodedOidNumsIndex - 2 > 0x7f)
                throw new CryptographicUnexpectedOperationException(SR.Cryptography_Config_EncodedOIDError);

            encodedOidNums[0] = (byte)0x06;
            encodedOidNums[1] = (byte)(encodedOidNumsIndex - 2);

            return encodedOidNums;
        }

        private static IReadOnlyDictionary<string, string> CreateTypeNameToOidMapping()
        {
            const string OID_RSA_SMIMEalgCMS3DESwrap = "1.2.840.113549.1.9.16.3.6";
            const string OID_RSA_MD5 = "1.2.840.113549.2.5";
            const string OID_RSA_RC2CBC = "1.2.840.113549.3.2";
            const string OID_RSA_DES_EDE3_CBC = "1.2.840.113549.3.7";
            const string OID_OIWSEC_desCBC = "1.3.14.3.2.7";
            const string OID_OIWSEC_SHA1 = "1.3.14.3.2.26";
            const string OID_OIWSEC_SHA256 = "2.16.840.1.101.3.4.2.1";
            const string OID_OIWSEC_SHA384 = "2.16.840.1.101.3.4.2.2";
            const string OID_OIWSEC_SHA512 = "2.16.840.1.101.3.4.2.3";
            const string OID_OIWSEC_RIPEMD160 = "1.3.36.3.2.1";

            return new Dictionary<string, string>(35, StringComparer.OrdinalIgnoreCase)
            {
                { "SHA", OID_OIWSEC_SHA1 },
                { "SHA1", OID_OIWSEC_SHA1 },
                { "System.Security.Cryptography.SHA1", OID_OIWSEC_SHA1 },
                { "System.Security.Cryptography.SHA1CryptoServiceProvider", OID_OIWSEC_SHA1 },
                { "System.Security.Cryptography.SHA1Cng", OID_OIWSEC_SHA1 },
                { "System.Security.Cryptography.SHA1Managed", OID_OIWSEC_SHA1 },

                { "SHA256", OID_OIWSEC_SHA256 },
                { "System.Security.Cryptography.SHA256", OID_OIWSEC_SHA256 },
                { "System.Security.Cryptography.SHA256CryptoServiceProvider", OID_OIWSEC_SHA256 },
                { "System.Security.Cryptography.SHA256Cng", OID_OIWSEC_SHA256 },
                { "System.Security.Cryptography.SHA256Managed", OID_OIWSEC_SHA256 },

                { "SHA384", OID_OIWSEC_SHA384 },
                { "System.Security.Cryptography.SHA384", OID_OIWSEC_SHA384 },
                { "System.Security.Cryptography.SHA384CryptoServiceProvider", OID_OIWSEC_SHA384 },
                { "System.Security.Cryptography.SHA384Cng", OID_OIWSEC_SHA384 },
                { "System.Security.Cryptography.SHA384Managed", OID_OIWSEC_SHA384 },

                { "SHA512", OID_OIWSEC_SHA512 },
                { "System.Security.Cryptography.SHA512", OID_OIWSEC_SHA512 },
                { "System.Security.Cryptography.SHA512CryptoServiceProvider", OID_OIWSEC_SHA512 },
                { "System.Security.Cryptography.SHA512Cng", OID_OIWSEC_SHA512 },
                { "System.Security.Cryptography.SHA512Managed", OID_OIWSEC_SHA512 },

                { "RIPEMD160", OID_OIWSEC_RIPEMD160 },
                { "System.Security.Cryptography.RIPEMD160", OID_OIWSEC_RIPEMD160 },
                { "System.Security.Cryptography.RIPEMD160Managed", OID_OIWSEC_RIPEMD160 },

                { "MD5", OID_RSA_MD5 },
                { "System.Security.Cryptography.MD5", OID_RSA_MD5 },
                { "System.Security.Cryptography.MD5CryptoServiceProvider", OID_RSA_MD5 },
                { "System.Security.Cryptography.MD5Managed", OID_RSA_MD5 },

                { "TripleDESKeyWrap", OID_RSA_SMIMEalgCMS3DESwrap },

                { "RC2", OID_RSA_RC2CBC },
                { "System.Security.Cryptography.RC2CryptoServiceProvider", OID_RSA_RC2CBC },

                { "DES", OID_OIWSEC_desCBC },
                { "System.Security.Cryptography.DESCryptoServiceProvider", OID_OIWSEC_desCBC },

                { "TripleDES", OID_RSA_DES_EDE3_CBC },
                { "System.Security.Cryptography.TripleDESCryptoServiceProvider", OID_RSA_DES_EDE3_CBC }
            };
        }

        private static IReadOnlyDictionary<string, string> CreateNameToFullTypeNameMapping()
        {
            const string AssemblyName_Cng = "System.Security.Cryptography.Cng";
            const string AssemblyName_Csp = "System.Security.Cryptography.Csp";
            const string AssemblyName_Pkcs = "System.Security.Cryptography.Pkcs";
            const string AssemblyName_X509Certificates = "System.Security.Cryptography.X509Certificates";

            string SHA1CryptoServiceProviderType = "System.Security.Cryptography.SHA1CryptoServiceProvider, " + AssemblyName_Csp;
            string MD5CryptoServiceProviderType = "System.Security.Cryptography.MD5CryptoServiceProvider," + AssemblyName_Csp;
            string RSACryptoServiceProviderType = "System.Security.Cryptography.RSACryptoServiceProvider, " + AssemblyName_Csp;
            string DSACryptoServiceProviderType = "System.Security.Cryptography.DSACryptoServiceProvider, " + AssemblyName_Csp;
            string DESCryptoServiceProviderType = "System.Security.Cryptography.DESCryptoServiceProvider, " + AssemblyName_Csp;
            string TripleDESCryptoServiceProviderType = "System.Security.Cryptography.TripleDESCryptoServiceProvider, " + AssemblyName_Csp;
            string RC2CryptoServiceProviderType = "System.Security.Cryptography.RC2CryptoServiceProvider, " + AssemblyName_Csp;
            string RNGCryptoServiceProviderType = "System.Security.Cryptography.RNGCryptoServiceProvider, " + AssemblyName_Csp;
            string AesCryptoServiceProviderType = "System.Security.Cryptography.AesCryptoServiceProvider, " + AssemblyName_Csp;

            string ECDsaCngType = "System.Security.Cryptography.ECDsaCng, " + AssemblyName_Cng;

            Dictionary<string, string> nameToFullTypeName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Random number generator
            nameToFullTypeName.Add("RandomNumberGenerator", RNGCryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.RandomNumberGenerator", RNGCryptoServiceProviderType);

            // Hash functions
            nameToFullTypeName.Add("SHA", SHA1CryptoServiceProviderType);
            nameToFullTypeName.Add("SHA1", SHA1CryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.SHA1", SHA1CryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.HashAlgorithm", SHA1CryptoServiceProviderType);

            nameToFullTypeName.Add("MD5", MD5CryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.MD5", MD5CryptoServiceProviderType);

            // Asymmetric algorithms
            nameToFullTypeName.Add("RSA", RSACryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.RSA", RSACryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.AsymmetricAlgorithm", RSACryptoServiceProviderType);

            nameToFullTypeName.Add("DSA", DSACryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.DSA", DSACryptoServiceProviderType);

            // Windows will register the public ECDsaCng type.  Non-Windows gets a special handler.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                nameToFullTypeName.Add(ECDsaIdentifier, ECDsaCngType);
            }

            nameToFullTypeName.Add("ECDsaCng", ECDsaCngType);
            nameToFullTypeName.Add("System.Security.Cryptography.ECDsaCng", ECDsaCngType);

            // Symmetric algorithms
            nameToFullTypeName.Add("DES", DESCryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.DES", DESCryptoServiceProviderType);

            nameToFullTypeName.Add("3DES", TripleDESCryptoServiceProviderType);
            nameToFullTypeName.Add("TripleDES", TripleDESCryptoServiceProviderType);
            nameToFullTypeName.Add("Triple DES", TripleDESCryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.TripleDES", TripleDESCryptoServiceProviderType);

            nameToFullTypeName.Add("RC2", RC2CryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.RC2", RC2CryptoServiceProviderType);

            nameToFullTypeName.Add("AES", AesCryptoServiceProviderType);
            nameToFullTypeName.Add("AesCryptoServiceProvider", AesCryptoServiceProviderType);
            nameToFullTypeName.Add("System.Security.Cryptography.AesCryptoServiceProvider", AesCryptoServiceProviderType);

            // Xml Dsig/ Enc Hash algorithms
            nameToFullTypeName.Add("http://www.w3.org/2000/09/xmldsig#sha1", SHA1CryptoServiceProviderType);
            // Add the other hash algorithms introduced with XML Encryption

            // Xml Encryption symmetric keys
            nameToFullTypeName.Add("http://www.w3.org/2001/04/xmlenc#des-cbc", DESCryptoServiceProviderType);
            nameToFullTypeName.Add("http://www.w3.org/2001/04/xmlenc#tripledes-cbc", TripleDESCryptoServiceProviderType);
            nameToFullTypeName.Add("http://www.w3.org/2001/04/xmlenc#kw-tripledes", TripleDESCryptoServiceProviderType);

            // X509 Extensions (custom decoders)
            // Basic Constraints OID value
            nameToFullTypeName.Add("2.5.29.10", "System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension, " + AssemblyName_X509Certificates);
            nameToFullTypeName.Add("2.5.29.19", "System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension, " + AssemblyName_X509Certificates);
            // Subject Key Identifier OID value
            nameToFullTypeName.Add("2.5.29.14", "System.Security.Cryptography.X509Certificates.X509SubjectKeyIdentifierExtension, " + AssemblyName_X509Certificates);
            // Key Usage OID value
            nameToFullTypeName.Add("2.5.29.15", "System.Security.Cryptography.X509Certificates.X509KeyUsageExtension, " + AssemblyName_X509Certificates);
            // Enhanced Key Usage OID value
            nameToFullTypeName.Add("2.5.29.37", "System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension, " + AssemblyName_X509Certificates);

            // X509Chain class can be overridden to use a different chain engine.
            nameToFullTypeName.Add("X509Chain", "System.Security.Cryptography.X509Certificates.X509Chain, " + AssemblyName_X509Certificates);

            // PKCS9 attributes
            nameToFullTypeName.Add("1.2.840.113549.1.9.3", "System.Security.Cryptography.Pkcs.Pkcs9ContentType, " + AssemblyName_Pkcs);
            nameToFullTypeName.Add("1.2.840.113549.1.9.4", "System.Security.Cryptography.Pkcs.Pkcs9MessageDigest, " + AssemblyName_Pkcs);
            nameToFullTypeName.Add("1.2.840.113549.1.9.5", "System.Security.Cryptography.Pkcs.Pkcs9SigningTime, " + AssemblyName_Pkcs);
            nameToFullTypeName.Add("1.3.6.1.4.1.311.88.2.1", "System.Security.Cryptography.Pkcs.Pkcs9DocumentName, " + AssemblyName_Pkcs);
            nameToFullTypeName.Add("1.3.6.1.4.1.311.88.2.2", "System.Security.Cryptography.Pkcs.Pkcs9DocumentDescription, " + AssemblyName_Pkcs);

            return nameToFullTypeName;

            // Types in Desktop but currently unsupported in CoreFx:
            // string RIPEMD160ManagedType = "System.Security.Cryptography.RIPEMD160Managed" + AssemblyName_Encoding;
            // string ECDiffieHellmanCngType = "System.Security.Cryptography.ECDiffieHellmanCng, " + AssemblyName_Cng;
            // string MD5CngType = "System.Security.Cryptography.MD5Cng, " + AssemblyName_Cng;
            // string SHA1CngType = "System.Security.Cryptography.SHA1Cng, " + AssemblyName_Cng;
            // string SHA256CngType = "System.Security.Cryptography.SHA256Cng, " + AssemblyName_Cng;
            // string SHA384CngType = "System.Security.Cryptography.SHA384Cng, " + AssemblyName_Cng;
            // string SHA512CngType = "System.Security.Cryptography.SHA512Cng, " + AssemblyName_Cng;
            // string SHA256CryptoServiceProviderType = "System.Security.Cryptography.SHA256CryptoServiceProvider, " + AssemblyName_Csp;
            // string SHA384CryptoSerivceProviderType = "System.Security.Cryptography.SHA384CryptoServiceProvider, " + AssemblyName_Csp;
            // string SHA512CryptoServiceProviderType = "System.Security.Cryptography.SHA512CryptoServiceProvider, " + AssemblyName_Csp;
            // string DpapiDataProtectorType = "System.Security.Cryptography.DpapiDataProtector, " + AssemblyRef.SystemSecurity;
        }

        private static ConcurrentDictionary<string, Type> CreateTypeNameToTypeMapping()
        {
            ConcurrentDictionary<string, Type> typeNameToType = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            Type HMACMD5Type = typeof(System.Security.Cryptography.HMACMD5);
            Type HMACSHA1Type = typeof(System.Security.Cryptography.HMACSHA1);
            Type HMACSHA256Type = typeof(System.Security.Cryptography.HMACSHA256);
            Type HMACSHA384Type = typeof(System.Security.Cryptography.HMACSHA384);
            Type HMACSHA512Type = typeof(System.Security.Cryptography.HMACSHA512);
            Type RijndaelManagedType = typeof(System.Security.Cryptography.RijndaelManaged);
            Type AesManagedType = typeof(System.Security.Cryptography.AesManaged);
            Type SHA256DefaultType = typeof(System.Security.Cryptography.SHA256Managed);
            Type SHA384DefaultType = typeof(System.Security.Cryptography.SHA384Managed);
            Type SHA512DefaultType = typeof(System.Security.Cryptography.SHA512Managed);

            typeNameToType["SHA256"] = SHA256DefaultType;
            typeNameToType["SHA-256"] = SHA256DefaultType;
            typeNameToType["System.Security.Cryptography.SHA256"] = SHA256DefaultType;

            typeNameToType["SHA384"] = SHA384DefaultType;
            typeNameToType["SHA-384"] =SHA384DefaultType;
            typeNameToType["System.Security.Cryptography.SHA384"] = SHA384DefaultType;

            typeNameToType["SHA512"] = SHA512DefaultType;
            typeNameToType["SHA-512"] = SHA512DefaultType;
            typeNameToType["System.Security.Cryptography.SHA512"] = SHA512DefaultType;

            // Keyed Hash Algorithms
            typeNameToType["System.Security.Cryptography.HMAC"] = HMACSHA1Type;
            typeNameToType["System.Security.Cryptography.KeyedHashAlgorithm"] = HMACSHA1Type;
            typeNameToType["HMACMD5"] = HMACMD5Type;
            typeNameToType["System.Security.Cryptography.HMACMD5"] = HMACMD5Type;
            typeNameToType["HMACSHA1"] = HMACSHA1Type;
            typeNameToType["System.Security.Cryptography.HMACSHA1"] = HMACSHA1Type;
            typeNameToType["HMACSHA256"] = HMACSHA256Type;
            typeNameToType["System.Security.Cryptography.HMACSHA256"] = HMACSHA256Type;
            typeNameToType["HMACSHA384"] = HMACSHA384Type;
            typeNameToType["System.Security.Cryptography.HMACSHA384"] = HMACSHA384Type;
            typeNameToType["HMACSHA512"] = HMACSHA512Type;
            typeNameToType["System.Security.Cryptography.HMACSHA512"] = HMACSHA512Type;


            typeNameToType["Rijndael"] = RijndaelManagedType;
            typeNameToType["System.Security.Cryptography.Rijndael"] = RijndaelManagedType;
            // Rijndael is the default symmetric cipher because (a) it's the strongest and (b) we know we have an implementation everywhere
            typeNameToType["System.Security.Cryptography.SymmetricAlgorithm"] = RijndaelManagedType;

            typeNameToType["AesManaged"] = AesManagedType;
            typeNameToType["System.Security.Cryptography.AesManaged"] = AesManagedType;

            // Add the other hash algorithms introduced with XML Encryption
            typeNameToType["http://www.w3.org/2001/04/xmlenc#sha256"] = SHA256DefaultType;
            typeNameToType["http://www.w3.org/2001/04/xmlenc#sha512"] = SHA512DefaultType;


            typeNameToType["http://www.w3.org/2001/04/xmlenc#aes128-cbc"] = RijndaelManagedType;
            typeNameToType["http://www.w3.org/2001/04/xmlenc#kw-aes128"] = RijndaelManagedType;
            typeNameToType["http://www.w3.org/2001/04/xmlenc#aes192-cbc"] = RijndaelManagedType;
            typeNameToType["http://www.w3.org/2001/04/xmlenc#kw-aes192"] = RijndaelManagedType;
            typeNameToType["http://www.w3.org/2001/04/xmlenc#aes256-cbc"] = RijndaelManagedType;
            typeNameToType["http://www.w3.org/2001/04/xmlenc#kw-aes256"] = RijndaelManagedType;

            // Xml Dsig HMAC URIs from http://www.w3.org/TR/xmldsig-core/
            typeNameToType["http://www.w3.org/2000/09/xmldsig#hmac-sha1"] = HMACSHA1Type;

            // Xml Dsig-more Uri's as defined in http://www.ietf.org/rfc/rfc4051.txt
            typeNameToType["http://www.w3.org/2001/04/xmldsig-more#sha384"] = SHA384DefaultType;
            typeNameToType["http://www.w3.org/2001/04/xmldsig-more#hmac-md5"] = HMACMD5Type;
            typeNameToType["http://www.w3.org/2001/04/xmldsig-more#hmac-sha256"] = HMACSHA256Type;
            typeNameToType["http://www.w3.org/2001/04/xmldsig-more#hmac-sha384"] = HMACSHA384Type;
            typeNameToType["http://www.w3.org/2001/04/xmldsig-more#hmac-sha512"] = HMACSHA512Type;

            return typeNameToType;

            // Types in Desktop but currently unsupported in CoreFx:
            // Type HMACRIPEMD160Type = typeof(System.Security.Cryptography.HMACRIPEMD160);
            // Type MAC3DESType = typeof(System.Security.Cryptography.MACTripleDES);
            // Type DSASignatureDescriptionType = typeof(System.Security.Cryptography.DSASignatureDescription);
            // Type RSAPKCS1SHA1SignatureDescriptionType = typeof(System.Security.Cryptography.RSAPKCS1SHA1SignatureDescription);
            // Type RSAPKCS1SHA256SignatureDescriptionType = typeof(System.Security.Cryptography.RSAPKCS1SHA256SignatureDescription);
            // Type RSAPKCS1SHA384SignatureDescriptionType = typeof(System.Security.Cryptography.RSAPKCS1SHA384SignatureDescription);
            // Type RSAPKCS1SHA512SignatureDescriptionType = typeof(System.Security.Cryptography.RSAPKCS1SHA512SignatureDescription);
        }

        private static void EncodeSingleOidNum(uint value, byte[] destination, ref int index)
        {
            // Write directly to destination starting at index, and update index based on how many bytes written.
            // If destination is null, just return updated index.
            if (unchecked((int)value) < 0x80)
            {
                if (destination != null)
                {
                    destination[index++] = unchecked((byte)value);
                }
                else
                {
                    index += 1;
                }
            }
            else if (value < 0x4000)
            {
                if (destination != null)
                {
                    destination[index++] = (byte)((value >> 7) | 0x80);
                    destination[index++] = (byte)(value & 0x7f);
                }
                else
                {
                    index += 2;
                }
            }
            else if (value < 0x200000)
            {
                if (destination != null)
                {
                    unchecked
                    {
                        destination[index++] = (byte)((value >> 14) | 0x80);
                        destination[index++] = (byte)((value >> 7) | 0x80);
                        destination[index++] = (byte)(value & 0x7f);
                    }
                }
                else
                {
                    index += 3;
                }
            }
            else if (value < 0x10000000)
            {
                if (destination != null)
                {
                    unchecked
                    {
                        destination[index++] = (byte)((value >> 21) | 0x80);
                        destination[index++] = (byte)((value >> 14) | 0x80);
                        destination[index++] = (byte)((value >> 7) | 0x80);
                        destination[index++] = (byte)(value & 0x7f);
                    }
                }
                else
                {
                    index += 4;
                }
            }
            else
            {
                if (destination != null)
                {
                    unchecked
                    {
                        destination[index++] = (byte)((value >> 28) | 0x80);
                        destination[index++] = (byte)((value >> 21) | 0x80);
                        destination[index++] = (byte)((value >> 14) | 0x80);
                        destination[index++] = (byte)((value >> 7) | 0x80);
                        destination[index++] = (byte)(value & 0x7f);
                    }
                }
                else
                {
                    index += 5;
                }
            }
        }
    }
}
