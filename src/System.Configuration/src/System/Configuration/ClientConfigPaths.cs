// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using System.Reflection;

namespace System.Configuration
{
    internal class ClientConfigPaths
    {
        internal const string UserConfigFilename = "user.config";

        private const string ConfigExtension = ".config";
        private const int MaxPath = 260;
        private const int MaxLengthToUse = 25;
        private const string FileUriLocal = "file:///";
        private const string FileUriUnc = "file://";
        private const string FileUri = "file:";
        private const string HttpUri = "http://";
        private const string StrongNameDesc = "StrongName";
        private const string UrlDesc = "Url";
        private const string PathDesc = "Path";

        private static volatile ClientConfigPaths s_current;
        private static volatile bool s_currentIncludesUserConfig;

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
                // Now figure out the application path.
                exeAssembly = Assembly.GetEntryAssembly();

                if (exeAssembly == null)
                    throw new PlatformNotSupportedException();

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
                else
                {
                    applicationUri = exeAssembly.EscapedCodeBase;
                }
            }
            else
            {
                applicationUri = Path.GetFullPath(exePath);
                if (!File.Exists(applicationUri))
                {
                    throw ExceptionUtil.ParameterInvalid("exePath");
                }

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

            if (isHttp) return;

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
            string hashSuffix = GetTypeAndHashSuffix(applicationUriLower);

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
        private static string GetTypeAndHashSuffix(string exePath)
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            string suffix = null;
            string typeName = null;
            string hash = null;

            if (assembly != null)
            {
                AssemblyName assemblyName = assembly.GetName();
                Uri codeBase = new Uri(assembly.CodeBase);

                hash = IdentityHelper.GetNormalizedStrongNameHash(assemblyName);
                if (hash != null)
                {
                    typeName = StrongNameDesc;
                }
                else
                {
                    hash = IdentityHelper.GetNormalizedUriHash(codeBase);
                    typeName = UrlDesc;
                }
            }
            else if (!string.IsNullOrEmpty(exePath))
            {
                // Fall back on the exe name
                hash = IdentityHelper.GetStrongHashSuitableForObjectName(exePath);
                typeName = PathDesc;
            }

            if (!string.IsNullOrEmpty(hash)) suffix = "_" + typeName + "_" + hash;
            return suffix;
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

            // If we couldn't get custom attributes, fall back on the entry type namespace
            if (!isHttp &&
                (string.IsNullOrEmpty(_companyName) || string.IsNullOrEmpty(ProductName) ||
                string.IsNullOrEmpty(ProductVersion)))
            {
                if (exeAssembly != null)
                {
                    MethodInfo entryPoint = exeAssembly.EntryPoint;
                    if (entryPoint != null)
                    {
                        mainType = entryPoint.ReflectedType;
                    }
                }

                string ns = null;
                if (mainType != null) ns = mainType.Namespace;

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