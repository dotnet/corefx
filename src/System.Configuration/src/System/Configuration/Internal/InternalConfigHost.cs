// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;

namespace System.Configuration.Internal
{
    internal sealed class InternalConfigHost : IInternalConfigHost
    {
        private const FileAttributes InvalidAttributesForWrite = FileAttributes.ReadOnly | FileAttributes.Hidden;
        private IInternalConfigRoot _configRoot;

        void IInternalConfigHost.Init(IInternalConfigRoot configRoot, params object[] hostInitParams)
        {
            _configRoot = configRoot;
        }

        void IInternalConfigHost.InitForConfiguration(ref string locationSubPath, out string configPath,
            out string locationConfigPath,
            IInternalConfigRoot configRoot, params object[] hostInitConfigurationParams)
        {
            _configRoot = configRoot;
            configPath = null;
            locationConfigPath = null;
        }

        bool IInternalConfigHost.IsConfigRecordRequired(string configPath)
        {
            return true;
        }

        bool IInternalConfigHost.IsInitDelayed(IInternalConfigRecord configRecord)
        {
            return false;
        }

        void IInternalConfigHost.RequireCompleteInit(IInternalConfigRecord configRecord) { }

        public bool IsSecondaryRoot(string configPath)
        {
            // In the default there are no secondary root's
            return false;
        }

        string IInternalConfigHost.GetStreamName(string configPath)
        {
            throw ExceptionUtil.UnexpectedError("IInternalConfigHost.GetStreamName");
        }

        string IInternalConfigHost.GetStreamNameForConfigSource(string streamName, string configSource)
        {
            return StaticGetStreamNameForConfigSource(streamName, configSource);
        }

        object IInternalConfigHost.GetStreamVersion(string streamName)
        {
            return StaticGetStreamVersion(streamName);
        }

        Stream IInternalConfigHost.OpenStreamForRead(string streamName)
        {
            return ((IInternalConfigHost)this).OpenStreamForRead(streamName, false);
        }

        Stream IInternalConfigHost.OpenStreamForRead(string streamName, bool assertPermissions)
        {
            Stream stream;
            bool revertAssert = false;

            // Runtime config: assert access to the file
            // Designtime config: require caller to have all required permissions
            //
            // assertPermissions: if true, we'll assert permission.  Used by ClientSettingsConfigurationHost.
            if (assertPermissions || !_configRoot.IsDesignTime)
            {
                new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, streamName)
                    .Assert();
                revertAssert = true;
            }

            try
            {
                stream = StaticOpenStreamForRead(streamName);
            }
            finally
            {
                if (revertAssert) CodeAccessPermission.RevertAssert();
            }

            return stream;
        }


        Stream IInternalConfigHost.OpenStreamForWrite(string streamName, string templateStreamName,
            ref object writeContext)
        {
            return ((IInternalConfigHost)this).OpenStreamForWrite(streamName, templateStreamName, ref writeContext,
                false);
        }


        Stream IInternalConfigHost.OpenStreamForWrite(string streamName, string templateStreamName,
            ref object writeContext, bool assertPermissions)
        {
            return StaticOpenStreamForWrite(streamName, templateStreamName, ref writeContext, assertPermissions);
        }

        void IInternalConfigHost.WriteCompleted(string streamName, bool success, object writeContext)
        {
            ((IInternalConfigHost)this).WriteCompleted(streamName, success, writeContext, false);
        }

        void IInternalConfigHost.WriteCompleted(string streamName, bool success, object writeContext,
            bool assertPermissions)
        {
            StaticWriteCompleted(streamName, success, writeContext, assertPermissions);
        }

        void IInternalConfigHost.DeleteStream(string streamName)
        {
            StaticDeleteStream(streamName);
        }

        bool IInternalConfigHost.IsFile(string streamName)
        {
            return StaticIsFile(streamName);
        }

        // change notification support - runtime only
        bool IInternalConfigHost.SupportsChangeNotifications => false;

        object IInternalConfigHost.StartMonitoringStreamForChanges(string streamName, StreamChangeCallback callback)
        {
            throw ExceptionUtil.UnexpectedError("IInternalConfigHost.StartMonitoringStreamForChanges");
        }

        void IInternalConfigHost.StopMonitoringStreamForChanges(string streamName, StreamChangeCallback callback)
        {
            throw ExceptionUtil.UnexpectedError("IInternalConfigHost.StopMonitoringStreamForChanges");
        }

        // RefreshConfig support - runtime only
        bool IInternalConfigHost.SupportsRefresh => false;

        // path support
        bool IInternalConfigHost.SupportsPath => false;

        bool IInternalConfigHost.IsDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition,
            ConfigurationAllowExeDefinition allowExeDefinition)
        {
            return true;
        }

        void IInternalConfigHost.VerifyDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition,
            ConfigurationAllowExeDefinition allowExeDefinition, IConfigErrorInfo errorInfo)
        { }

        // Do we support location tags?
        bool IInternalConfigHost.SupportsLocation => false;

        bool IInternalConfigHost.IsAboveApplication(string configPath)
        {
            throw ExceptionUtil.UnexpectedError("IInternalConfigHost.IsAboveApplication");
        }

        string IInternalConfigHost.GetConfigPathFromLocationSubPath(string configPath, string locationSubPath)
        {
            throw ExceptionUtil.UnexpectedError("IInternalConfigHost.GetConfigPathFromLocationSubPath");
        }

        bool IInternalConfigHost.IsLocationApplicable(string configPath)
        {
            throw ExceptionUtil.UnexpectedError("IInternalConfigHost.IsLocationApplicable");
        }

        bool IInternalConfigHost.IsTrustedConfigPath(string configPath)
        {
            throw ExceptionUtil.UnexpectedError("IInternalConfigHost.IsTrustedConfigPath");
        }

        // Default implementation: ensure that the caller has full trust.
        bool IInternalConfigHost.IsFullTrustSectionWithoutAptcaAllowed(IInternalConfigRecord configRecord)
        {
            return TypeUtil.IsCallerFullTrust;
        }

        // security support
        void IInternalConfigHost.GetRestrictedPermissions(IInternalConfigRecord configRecord,
            out PermissionSet permissionSet, out bool isHostReady)
        {
            permissionSet = null;
            isHostReady = true;
        }

        IDisposable IInternalConfigHost.Impersonate()
        {
            return null;
        }

        // prefetch support
        bool IInternalConfigHost.PrefetchAll(string configPath, string streamName)
        {
            return false;
        }

        bool IInternalConfigHost.PrefetchSection(string sectionGroupName, string sectionName)
        {
            return false;
        }

        object IInternalConfigHost.CreateDeprecatedConfigContext(string configPath)
        {
            throw ExceptionUtil.UnexpectedError("IInternalConfigHost.CreateDeprecatedConfigContext");
        }

        object IInternalConfigHost.CreateConfigurationContext(string configPath, string locationSubPath)
        {
            throw ExceptionUtil.UnexpectedError("IInternalConfigHost.CreateConfigurationContext");
        }

        string IInternalConfigHost.DecryptSection(string encryptedXml, ProtectedConfigurationProvider protectionProvider,
            ProtectedConfigurationSection protectedConfigSection)
        {
            return ProtectedConfigurationSection.DecryptSection(encryptedXml, protectionProvider);
        }

        string IInternalConfigHost.EncryptSection(string clearTextXml, ProtectedConfigurationProvider protectionProvider,
            ProtectedConfigurationSection protectedConfigSection)
        {
            return ProtectedConfigurationSection.EncryptSection(clearTextXml, protectionProvider);
        }

        Type IInternalConfigHost.GetConfigType(string typeName, bool throwOnError)
        {
            return Type.GetType(typeName, throwOnError);
        }

        string IInternalConfigHost.GetConfigTypeName(Type t)
        {
            return t.AssemblyQualifiedName;
        }

        bool IInternalConfigHost.IsRemote => false;

        internal static string StaticGetStreamNameForConfigSource(string streamName, string configSource)
        {
            // RemoteWebConfigurationHost also redirects GetStreamNameForConfigSource to this
            // method, and that means streamName is referring to a path that's on the remote
            // machine.  The problem is that Path.GetFullPath will demand FileIOPermission on
            // that file and *assume* the file is referring to one on the local machine.
            // This can be a potential problem for RemoteWebConfigurationHost.  However, since
            // we Assert the PathDiscovery permission at this method, so this problem is handled
            // and we're okay.  But in the future if we modify this method to handle anything
            // that assumes streamName is a local path, then RemoteWebConfigurationHost has to
            // override GetStreamNameForConfigSource.

            // don't allow relative paths for stream name
            if (!Path.IsPathRooted(streamName)) throw ExceptionUtil.ParameterInvalid("streamName");

            // get the path part of the original stream
            streamName = Path.GetFullPath(streamName);
            string dirStream = UrlPath.GetDirectoryOrRootName(streamName);

            // combine with the new config source
            string result = Path.Combine(dirStream, configSource);
            result = Path.GetFullPath(result);

            // ensure the result is in or under the directory of the original source
            string dirResult = UrlPath.GetDirectoryOrRootName(result);
            if (!UrlPath.IsEqualOrSubdirectory(dirStream, dirResult))
                throw new ArgumentException(string.Format(SR.Config_source_not_under_config_dir, configSource));

            return result;
        }

        internal static object StaticGetStreamVersion(string streamName)
        {
            bool exists = false;
            long fileSize = 0;
            DateTime utcCreationTime = DateTime.MinValue;
            DateTime utcLastWriteTime = DateTime.MinValue;

            UnsafeNativeMethods.WIN32_FILE_ATTRIBUTE_DATA data;
            if (
                UnsafeNativeMethods.GetFileAttributesEx(streamName, UnsafeNativeMethods.GetFileExInfoStandard, out data) &&
                ((data.fileAttributes & (int)FileAttributes.Directory) == 0))
            {
                exists = true;
                fileSize = ((long)(uint)data.fileSizeHigh << 32) | (uint)data.fileSizeLow;
                utcCreationTime =
                    DateTime.FromFileTimeUtc(((long)data.ftCreationTimeHigh << 32) | (long)data.ftCreationTimeLow);
                utcLastWriteTime =
                    DateTime.FromFileTimeUtc(((long)data.ftLastWriteTimeHigh << 32) | (long)data.ftLastWriteTimeLow);
            }

            return new FileVersion(exists, fileSize, utcCreationTime, utcLastWriteTime);
        }

        // default impl treats name as a file name
        // null means stream doesn't exist for this name
        internal static Stream StaticOpenStreamForRead(string streamName)
        {
            if (string.IsNullOrEmpty(streamName))
                throw ExceptionUtil.UnexpectedError("InternalConfigHost::StaticOpenStreamForRead");

            return !FileUtil.FileExists(streamName, true)
                ? null
                : new FileStream(streamName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        // This method doesn't really open the streamName for write.  Instead, using WriteFileContext
        // it opens a stream on a temporary file created in the same directory as streamName.
        //
        // Parameters:
        //  assertPermissions - If true, then we'll assert all required permissions.  Used by ClientSettingsConfigurationHost.
        //                      to allow low-trust apps to use ClientSettingsStore.
        internal static Stream StaticOpenStreamForWrite(string streamName, string templateStreamName,
            ref object writeContext, bool assertPermissions)
        {
            bool revertAssert = false;

            if (string.IsNullOrEmpty(streamName))
                throw new ConfigurationErrorsException(SR.Config_no_stream_to_write);

            // Create directory if it does not exist.
            // Ignore errors, allow any failure to come when trying to open the file.
            string dir = Path.GetDirectoryName(streamName);
            try
            {
                if (!Directory.Exists(dir))
                {
                    // TODO: This Assert( ) should be moved to before we call
                    // Directory.Exists because we
                    // discovered that under partial trust Directory.Exists will
                    // always return false.  But for Whidbey RTM it's okay to not
                    // fix it because Directory.CreateDirectory will not throw if
                    // the directory already exists.
                    if (assertPermissions)
                    {
                        new FileIOPermission(PermissionState.Unrestricted).Assert();
                        revertAssert = true;
                    }

                    Directory.CreateDirectory(dir);
                }
            }
            catch { }
            finally
            {
                if (revertAssert) CodeAccessPermission.RevertAssert();
            }

            Stream stream;
            WriteFileContext writeFileContext = null;
            revertAssert = false;

            if (assertPermissions)
            {
                // If we're asked to assert permission, we will assert allAccess on the directory (instead of just the file).
                // We need to assert for the whole directory because WriteFileContext will call TempFileCollection.AddExtension,
                // which will generate a temporary file and make a AllAccess Demand on that file.
                // Since we don't know the name of the temporary file right now, we need to assert for the whole dir.
                new FileIOPermission(FileIOPermissionAccess.AllAccess, dir).Assert();
                revertAssert = true;
            }

            try
            {
                writeFileContext = new WriteFileContext(streamName, templateStreamName);

                if (File.Exists(streamName))
                {
                    FileInfo fi = new FileInfo(streamName);
                    FileAttributes attrs = fi.Attributes;
                    if ((int)(attrs & InvalidAttributesForWrite) != 0)
                        throw new IOException(string.Format(SR.Config_invalid_attributes_for_write, streamName));
                }

                try
                {
                    stream = new FileStream(writeFileContext.TempNewFilename, FileMode.Create, FileAccess.Write,
                        FileShare.Read);
                }
                catch (Exception e)
                {
                    // Wrap all exceptions so that we provide a meaningful filename - otherwise the end user
                    // will just see the temporary file name, which is meaningless.
                    throw new ConfigurationErrorsException(string.Format(SR.Config_write_failed, streamName), e);
                }
            }
            catch
            {
                writeFileContext?.Complete(streamName, false);
                throw;
            }
            finally
            {
                if (revertAssert) CodeAccessPermission.RevertAssert();
            }

            writeContext = writeFileContext;
            return stream;
        }

        // Parameters:
        //  assertPermissions - If true, then we'll assert all required permissions.  Used by ClientSettingsConfigurationHost.
        //                      to allow low-trust apps to use ClientSettingsStore.
        internal static void StaticWriteCompleted(string streamName, bool success, object writeContext,
            bool assertPermissions)
        {
            WriteFileContext writeFileContext = (WriteFileContext)writeContext;
            bool revertAssert = false;

            if (assertPermissions)
            {
                // If asked to assert permissions, we will assert allAccess on the streamName, the temporary file 
                // created by WriteContext, and also the directory itself.  The last one is needed because 
                // WriteFileContext will call TempFileCollection.Dispose, which will remove a .tmp file it created.
                string dir = Path.GetDirectoryName(streamName);
                string[] filePaths = { streamName, writeFileContext.TempNewFilename, dir };
                FileIOPermission fileIOPerm = new FileIOPermission(FileIOPermissionAccess.AllAccess,
                    AccessControlActions.View | AccessControlActions.Change, filePaths);
                fileIOPerm.Assert();
                revertAssert = true;
            }

            try
            {
                writeFileContext.Complete(streamName, success);
            }
            finally
            {
                if (revertAssert) CodeAccessPermission.RevertAssert();
            }
        }

        internal static void StaticDeleteStream(string streamName)
        {
            File.Delete(streamName);
        }

        // ConfigurationErrorsException support
        internal static bool StaticIsFile(string streamName)
        {
            // We want to avoid loading configuration before machine.config
            // is instantiated. Referencing the Uri class will cause config
            // to be loaded, so we use Path.IsPathRooted.
            return Path.IsPathRooted(streamName);
        }
    }
}