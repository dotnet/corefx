// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace System.Configuration
{
    internal class ClientConfigPaths
    {
        internal const string UserConfigFilename = "user.config";

        private const string ClickOnceDataDirectory = "DataDirectory";
        private const string ConfigExtension = ".config";
        private const int MaxPath = 260;
        private const int MaxUnicodestringLen = short.MaxValue;
        private const int ErrorInsufficientBuffer = 122;
        private const int MaxLengthToUse = 25;
        private const string FileUriLocal = "file:///";
        private const string FileUriUnc = "file://";
        private const string FileUri = "file:";
        private const string HttpUri = "http://";
        private const string StrongNameDesc = "StrongName";
        private const string UrlDesc = "Url";
        private const string PathDesc = "Path";

        private static readonly char[] s_base32Char =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
            'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
            'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
            'y', 'z', '0', '1', '2', '3', '4', '5'
        };

        private static volatile ClientConfigPaths s_current;
        private static volatile bool s_currentIncludesUserConfig;
        private static volatile SecurityPermission s_serializationPerm;
        private static volatile SecurityPermission s_controlEvidencePerm;

        private readonly bool _includesUserConfig;
        private string _companyName;

        private ClientConfigPaths(string exePath, bool includeUserConfig)
        {
            _includesUserConfig = includeUserConfig;

            Assembly exeAssembly = null;
            string applicationUri;
            string applicationFilename = null;

            // get the assembly and applicationUri for the file
            if (exePath == null)
            {
                // First check if a configuration file has been set for this app domain. If so, we will use that.
                // The CLR would already have normalized this, so no further processing necessary.
                AppDomain domain = AppDomain.CurrentDomain;
                AppDomainSetup setup = domain.SetupInformation;
                ApplicationConfigUri = setup.ConfigurationFile;

                // Now figure out the application path.
                exeAssembly = Assembly.GetEntryAssembly();
                if (exeAssembly != null)
                {
                    HasEntryAssembly = true;
                    applicationUri = exeAssembly.CodeBase;

                    bool isFile = false;

                    if (StringUtil.StartsWithOrdinalIgnoreCase(applicationUri, FileUriLocal))
                    {
                        // If it is a local file URI, convert it to its filename, without invoking Uri class.
                        // example: "file:///C:/WINNT/Microsoft.NET/Framework/v2.0.x86fre/csc.exe"
                        isFile = true;
                        applicationUri = applicationUri.Substring(FileUriLocal.Length);
                    }
                    else
                    {
                        // If it is a UNC file URI, convert it to its filename, without invoking Uri class.
                        // example: "file://server/share/csc.exe"
                        if (StringUtil.StartsWithOrdinalIgnoreCase(applicationUri, FileUriUnc))
                        {
                            isFile = true;
                            applicationUri = applicationUri.Substring(FileUri.Length);
                        }
                    }

                    if (isFile)
                    {
                        applicationUri = applicationUri.Replace('/', '\\');
                        applicationFilename = applicationUri;
                    }
                    else applicationUri = exeAssembly.EscapedCodeBase;
                }
                else
                {
                    StringBuilder sb = new StringBuilder(MaxPath);
                    int noOfTimes = 1;
                    int length;

                    // Iterating by allocating chunk of memory each time we find the length is not sufficient.
                    // Performance should not be an issue for current MAX_PATH length due to this change.
                    while (
                        ((length =
                            UnsafeNativeMethods.GetModuleFileName(new HandleRef(null, IntPtr.Zero), sb, sb.Capacity)) ==
                        sb.Capacity)
                        && (Marshal.GetLastWin32Error() == ErrorInsufficientBuffer)
                        && (sb.Capacity < MaxUnicodestringLen))
                    {
                        noOfTimes += 2; // increasing buffer size by 520 in each iteration - perf.
                        int capacity = noOfTimes * MaxPath < MaxUnicodestringLen
                            ? noOfTimes * MaxPath
                            : MaxUnicodestringLen;
                        sb.EnsureCapacity(capacity);
                    }
                    sb.Length = length;
                    applicationUri = Path.GetFullPath(sb.ToString());
                    applicationFilename = applicationUri;
                }
            }
            else
            {
                applicationUri = Path.GetFullPath(exePath);
                if (!FileUtil.FileExists(applicationUri, false))
                    throw ExceptionUtil.ParameterInvalid("exePath");

                applicationFilename = applicationUri;
            }

            // Fallback if we haven't set the app config file path yet.
            if (ApplicationConfigUri == null) ApplicationConfigUri = applicationUri + ConfigExtension;

            // Set application path
            ApplicationUri = applicationUri;

            // In the case when exePath was explicitly supplied, we will not be able to 
            // construct user.config paths, so quit here.
            if (exePath != null) return;

            // Skip expensive initialization of user config file information if requested.
            if (!_includesUserConfig) return;

            bool isHttp = StringUtil.StartsWithOrdinalIgnoreCase(ApplicationConfigUri, HttpUri);

            SetNamesAndVersion(applicationFilename, exeAssembly, isHttp);

            // Check if this is a clickonce deployed application. If so, point the user config
            // files to the clickonce data directory.
            if (IsClickOnceDeployed(AppDomain.CurrentDomain))
            {
                string dataPath = AppDomain.CurrentDomain.GetData(ClickOnceDataDirectory) as string;
                string versionSuffix = Validate(ProductVersion, false);

                // NOTE: No roaming config for clickonce - not supported.
                if (Path.IsPathRooted(dataPath))
                {
                    LocalConfigDirectory = CombineIfValid(dataPath, versionSuffix);
                    LocalConfigFilename = CombineIfValid(LocalConfigDirectory, UserConfigFilename);
                }
            }
            else if (!isHttp)
            {
                // If we get the config from http, we do not have a roaming or local config directory,
                // as it cannot be edited by the app in those cases because it does not have Full Trust.
                string part1 = Validate(_companyName, true);

                string validAppDomainName = Validate(AppDomain.CurrentDomain.FriendlyName, true);
                string applicationUriLower = !string.IsNullOrEmpty(ApplicationUri)
                    ? ApplicationUri.ToLower(CultureInfo.InvariantCulture)
                    : null;
                string namePrefix = !string.IsNullOrEmpty(validAppDomainName)
                    ? validAppDomainName
                    : Validate(ProductName, true);
                string hashSuffix = GetTypeAndHashSuffix(AppDomain.CurrentDomain, applicationUriLower);

                string part2 = !string.IsNullOrEmpty(namePrefix) && !string.IsNullOrEmpty(hashSuffix)
                    ? namePrefix + hashSuffix
                    : null;

                string part3 = Validate(ProductVersion, false);

                string dirSuffix = CombineIfValid(CombineIfValid(part1, part2), part3);

                string roamingFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (Path.IsPathRooted(roamingFolderPath))
                {
                    RoamingConfigDirectory = CombineIfValid(roamingFolderPath, dirSuffix);
                    RoamingConfigFilename = CombineIfValid(RoamingConfigDirectory, UserConfigFilename);
                }

                string localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (Path.IsPathRooted(localFolderPath))
                {
                    LocalConfigDirectory = CombineIfValid(localFolderPath, dirSuffix);
                    LocalConfigFilename = CombineIfValid(LocalConfigDirectory, UserConfigFilename);
                }
            }
        }

        internal static ClientConfigPaths Current => GetPaths(null, true);

        internal bool HasEntryAssembly { get; }

        internal string ApplicationUri { get; }

        internal string ApplicationConfigUri { get; }

        internal string RoamingConfigFilename { get; }

        internal string RoamingConfigDirectory { get; }

        internal bool HasRoamingConfig => (RoamingConfigFilename != null) || !_includesUserConfig;

        internal string LocalConfigFilename { get; }

        internal string LocalConfigDirectory { get; }

        internal bool HasLocalConfig => (LocalConfigFilename != null) || !_includesUserConfig;

        internal string ProductName { get; private set; }

        internal string ProductVersion { get; private set; }

        private static SecurityPermission ControlEvidencePermission => s_controlEvidencePerm ??
            (s_controlEvidencePerm = new SecurityPermission(SecurityPermissionFlag.ControlEvidence));

        private static SecurityPermission SerializationFormatterPermission => s_serializationPerm ??
            (s_serializationPerm = new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));

        internal static ClientConfigPaths GetPaths(string exePath, bool includeUserConfig)
        {
            ClientConfigPaths result;

            if (exePath == null)
            {
                if ((s_current == null) || (includeUserConfig && !s_currentIncludesUserConfig))
                {
                    s_current = new ClientConfigPaths(null, includeUserConfig);
                    s_currentIncludesUserConfig = includeUserConfig;
                }

                result = s_current;
            }
            else result = new ClientConfigPaths(exePath, includeUserConfig);

            return result;
        }

        internal static void RefreshCurrent()
        {
            s_currentIncludesUserConfig = false;
            s_current = null;
        }

        // Combines path2 with path1 if possible, else returns null.
        private static string CombineIfValid(string path1, string path2)
        {
            string returnPath = null;

            if ((path1 == null) || (path2 == null)) return null;

            try
            {
                string combinedPath = Path.Combine(path1, path2);
                if (combinedPath.Length < MaxPath) returnPath = combinedPath;
            }
            catch { }

            return returnPath;
        }

        // Returns a type and hash suffix based on app domain evidence. The evidence we use, in
        // priority order, is Strong Name, Url and Exe Path. If one of these is found, we compute a 
        // SHA1 hash of it and return a suffix based on that. If none is found, we return null.
        private string GetTypeAndHashSuffix(AppDomain appDomain, string exePath)
        {
            string suffix = null;
            string typeName;

            object evidenceObj = GetEvidenceInfo(appDomain, exePath, out typeName);

            if ((evidenceObj == null) || string.IsNullOrEmpty(typeName)) return null;
            MemoryStream ms = new MemoryStream();

            BinaryFormatter bSer = new BinaryFormatter();
            SerializationFormatterPermission.Assert();
            bSer.Serialize(ms, evidenceObj);
            ms.Position = 0;
            string evidenceHash = GetHash(ms);

            if (!string.IsNullOrEmpty(evidenceHash)) suffix = "_" + typeName + "_" + evidenceHash;

            return suffix;
        }

        // Mostly borrowed from IsolatedStorage, with some modifications
        private static object GetEvidenceInfo(AppDomain appDomain, string exePath, out string typeName)
        {
            ControlEvidencePermission.Assert();
            Evidence evidence = appDomain.Evidence;
            StrongName sn = null;
            Url url = null;

            if (evidence != null)
            {
                IEnumerator e = evidence.GetHostEnumerator();

                while (e.MoveNext())
                {
                    object current = e.Current;

                    StrongName name = current as StrongName;
                    if (name != null)
                    {
                        sn = name;
                        break;
                    }

                    Url temp = current as Url;
                    if (temp != null) url = temp;
                }
            }

            object o = null;

            // The order of preference is StrongName, Url, ExePath.
            if (sn != null)
            {
                o = MakeVersionIndependent(sn);
                typeName = StrongNameDesc;
            }
            else
            {
                if (url != null)
                {
                    // Extract the url string and normalize it to use as evidence
                    o = url.Value.ToUpperInvariant();
                    typeName = UrlDesc;
                }
                else
                {
                    if (exePath != null)
                    {
                        o = exePath;
                        typeName = PathDesc;
                    }
                    else typeName = null;
                }
            }

            return o;
        }

        private static string GetHash(Stream s)
        {
            byte[] hash;

            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                hash = sha1.ComputeHash(s);
            }

            return ToBase32StringSuitableForDirName(hash);
        }

        private bool IsClickOnceDeployed(AppDomain appDomain)
        {
            // NOTE: For perf & servicing reasons, we don't want to introduce a dependency on
            //       System.Deployment.dll here. The following code is an alternative to calling
            //       ApplicationDeployment.IsNetworkDeployed.

            ActivationContext actCtx = appDomain.ActivationContext;

            // Ensures the app is running with a context from the store.
            if ((actCtx == null) || (actCtx.Form != ActivationContext.ContextForm.StoreBounded)) return false;
            string fullAppId = actCtx.Identity.FullName;
            return !string.IsNullOrEmpty(fullAppId);
        }

        private static StrongName MakeVersionIndependent(StrongName sn)
        {
            return new StrongName(sn.PublicKey, sn.Name, new Version(0, 0, 0, 0));
        }

        private void SetNamesAndVersion(string applicationFilename, Assembly exeAssembly, bool isHttp)
        {
            Type mainType = null;

            // Get CompanyName, ProductName, and ProductVersion
            // First try custom attributes on the assembly.
            if (exeAssembly != null)
            {
                object[] attrs = exeAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if ((attrs != null) && (attrs.Length > 0))
                {
                    _companyName = ((AssemblyCompanyAttribute)attrs[0]).Company;
                    _companyName = _companyName?.Trim();
                }

                attrs = exeAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if ((attrs != null) && (attrs.Length > 0))
                {
                    ProductName = ((AssemblyProductAttribute)attrs[0]).Product;
                    ProductName = ProductName?.Trim();
                }

                ProductVersion = exeAssembly.GetName().Version.ToString();
                ProductVersion = ProductVersion?.Trim();
            }

            // If we couldn't get custom attributes, try the Win32 file version
            if (!isHttp &&
                (string.IsNullOrEmpty(_companyName) || string.IsNullOrEmpty(ProductName) ||
                string.IsNullOrEmpty(ProductVersion)))
            {
                string versionInfoFileName = null;

                if (exeAssembly != null)
                {
                    MethodInfo entryPoint = exeAssembly.EntryPoint;
                    if (entryPoint != null)
                    {
                        mainType = entryPoint.ReflectedType;
                        if (mainType != null) versionInfoFileName = mainType.Module.FullyQualifiedName;
                    }
                }

                if (versionInfoFileName == null) versionInfoFileName = applicationFilename;

                if (versionInfoFileName != null)
                {
                    Diagnostics.FileVersionInfo version = Diagnostics.FileVersionInfo.GetVersionInfo(versionInfoFileName);
                    if (version != null)
                    {
                        if (string.IsNullOrEmpty(_companyName))
                        {
                            _companyName = version.CompanyName;
                            _companyName = _companyName?.Trim();
                        }

                        if (string.IsNullOrEmpty(ProductName))
                        {
                            ProductName = version.ProductName;
                            ProductName = ProductName?.Trim();
                        }

                        if (string.IsNullOrEmpty(ProductVersion))
                        {
                            ProductVersion = version.ProductVersion;
                            ProductVersion = ProductVersion?.Trim();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(_companyName) || string.IsNullOrEmpty(ProductName))
            {
                string ns = null;
                if (mainType != null) ns = mainType.Namespace;

                // Desperate measures for product name
                if (string.IsNullOrEmpty(ProductName))
                {
                    // Try the remainder of the namespace
                    if (ns != null)
                    {
                        int lastDot = ns.LastIndexOf(".", StringComparison.Ordinal);
                        if ((lastDot != -1) && (lastDot < ns.Length - 1)) ProductName = ns.Substring(lastDot + 1);
                        else ProductName = ns;

                        ProductName = ProductName.Trim();
                    }

                    // Try the type of the entry assembly
                    if (string.IsNullOrEmpty(ProductName) && (mainType != null)) ProductName = mainType.Name.Trim();

                    // give up, return empty string
                    if (ProductName == null) ProductName = string.Empty;
                }

                // Desperate measures for company name
                if (string.IsNullOrEmpty(_companyName))
                {
                    // Try the first part of the namespace
                    if (ns != null)
                    {
                        int firstDot = ns.IndexOf(".", StringComparison.Ordinal);
                        _companyName = firstDot != -1 ? ns.Substring(0, firstDot) : ns;

                        _companyName = _companyName.Trim();
                    }

                    // If that doesn't work, use the product name
                    if (string.IsNullOrEmpty(_companyName)) _companyName = ProductName;
                }
            }

            // Desperate measures for product version - assume 1.0
            if (string.IsNullOrEmpty(ProductVersion)) ProductVersion = "1.0.0.0";
        }

        // Borrowed from IsolatedStorage
        private static string ToBase32StringSuitableForDirName(byte[] buff)
        {
            StringBuilder sb = new StringBuilder();
            int l, i;

            l = buff.Length;
            i = 0;

            // Create l chars using the last 5 bits of each byte.  
            // Consume 3 MSB bits 5 bytes at a time.

            do
            {
                byte b0 = i < l ? buff[i++] : (byte)0;
                byte b1 = i < l ? buff[i++] : (byte)0;
                byte b2 = i < l ? buff[i++] : (byte)0;
                byte b3 = i < l ? buff[i++] : (byte)0;
                byte b4 = i < l ? buff[i++] : (byte)0;

                // Consume the 5 Least significant bits of each byte
                sb.Append(s_base32Char[b0 & 0x1F]);
                sb.Append(s_base32Char[b1 & 0x1F]);
                sb.Append(s_base32Char[b2 & 0x1F]);
                sb.Append(s_base32Char[b3 & 0x1F]);
                sb.Append(s_base32Char[b4 & 0x1F]);

                // Consume 3 MSB of b0, b1, MSB bits 6, 7 of b3, b4
                sb.Append(s_base32Char[((b0 & 0xE0) >> 5) |
                    ((b3 & 0x60) >> 2)]);

                sb.Append(s_base32Char[((b1 & 0xE0) >> 5) |
                    ((b4 & 0x60) >> 2)]);

                // Consume 3 MSB bits of b2, 1 MSB bit of b3, b4

                b2 >>= 5;

                if ((b3 & 0x80) != 0)
                    b2 |= 0x08;
                if ((b4 & 0x80) != 0)
                    b2 |= 0x10;

                sb.Append(s_base32Char[b2]);
            } while (i < l);

            return sb.ToString();
        }

        // Makes the passed in string suitable to use as a path name by replacing illegal characters
        // with underscores. Additionally, we do two things - replace spaces too with underscores and
        // limit the resultant string's length to MAX_LENGTH_TO_USE if limitSize is true.
        private static string Validate(string str, bool limitSize)
        {
            string validated = str;

            if (string.IsNullOrEmpty(validated)) return validated;

            // First replace all illegal characters with underscores
            foreach (char c in Path.GetInvalidFileNameChars()) validated = validated.Replace(c, '_');

            // Replace all spaces with underscores
            validated = validated.Replace(' ', '_');

            if (limitSize)
            {
                validated = validated.Length > MaxLengthToUse
                    ? validated.Substring(0, MaxLengthToUse)
                    : validated;
            }

            return validated;
        }
    }
}