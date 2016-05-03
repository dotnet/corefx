// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace Microsoft.Win32
{
    /// <summary>
    /// Provides <see cref="RegistryKey" /> objects that represent the root keys
    /// in the Windows registry, and static methods to access key/value pairs.
    /// </summary>
    public static partial class Registry
    {
        /// <summary>
        /// Defines the types (or classes) of documents and the properties associated with those types.
        /// This field reads the Windows registry base key HKEY_CLASSES_ROOT.
        /// </summary>
        public static readonly Microsoft.Win32.RegistryKey ClassesRoot;
        /// <summary>
        /// Contains configuration information pertaining to the hardware that is not specific to the user.
        /// This field reads the Windows registry base key HKEY_CURRENT_CONFIG.
        /// </summary>
        public static readonly Microsoft.Win32.RegistryKey CurrentConfig;
        /// <summary>
        /// Contains information about the current user preferences. This field reads the Windows registry
        /// base key HKEY_CURRENT_USER
        /// </summary>
        public static readonly Microsoft.Win32.RegistryKey CurrentUser;
        /// <summary>
        /// Contains the configuration data for the local machine. This field reads the Windows registry
        /// base key HKEY_LOCAL_MACHINE.
        /// </summary>
        public static readonly Microsoft.Win32.RegistryKey LocalMachine;
        /// <summary>
        /// Contains performance information for software components. This field reads the Windows registry
        /// base key HKEY_PERFORMANCE_DATA.
        /// </summary>
        public static readonly Microsoft.Win32.RegistryKey PerformanceData;
        /// <summary>
        /// Contains information about the default user configuration. This field reads the Windows registry
        /// base key HKEY_USERS.
        /// </summary>
        public static readonly Microsoft.Win32.RegistryKey Users;
        /// <summary>
        /// Retrieves the value associated with the specified name, in the specified registry key. If the
        /// name is not found in the specified key, returns a default value that you provide, or null if the
        /// specified key does not exist.
        /// </summary>
        /// <param name="keyName">
        /// The full registry path of the key, beginning with a valid registry root, such as "HKEY_CURRENT_USER".
        /// </param>
        /// <param name="valueName">The name of the name/value pair.</param>
        /// <param name="defaultValue">The value to return if <paramref name="valueName" /> does not exist.</param>
        /// <returns>
        /// null if the subkey specified by <paramref name="keyName" /> does not exist; otherwise, the
        /// value associated with <paramref name="valueName" />, or <paramref name="defaultValue" /> if
        /// <paramref name="valueName" /> is not found.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read from the registry key.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The <see cref="RegistryKey" /> that contains the specified value has been
        /// marked for deletion.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="keyName" /> does not begin with a valid registry root.
        /// </exception>
        public static object GetValue(string keyName, string valueName, object defaultValue) { return default(object); }
        /// <summary>
        /// Sets the specified name/value pair on the specified registry key. If the specified key does
        /// not exist, it is created.
        /// </summary>
        /// <param name="keyName">
        /// The full registry path of the key, beginning with a valid registry root, such as "HKEY_CURRENT_USER".
        /// </param>
        /// <param name="valueName">The name of the name/value pair.</param>
        /// <param name="value">The value to be stored.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="value" /> is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="keyName" /> does not begin with a valid registry root. -or-<paramref name="keyName" />
        /// is longer than the maximum length allowed (255 characters).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The <see cref="RegistryKey" /> is read-only, and thus cannot be written
        /// to; for example, it is a root-level node.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to create or modify registry keys.
        /// </exception>
        public static void SetValue(string keyName, string valueName, object value) { }
        /// <summary>
        /// Sets the name/value pair on the specified registry key, using the specified registry data type.
        /// If the specified key does not exist, it is created.
        /// </summary>
        /// <param name="keyName">
        /// The full registry path of the key, beginning with a valid registry root, such as "HKEY_CURRENT_USER".
        /// </param>
        /// <param name="valueName">The name of the name/value pair.</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="valueKind">The registry data type to use when storing the data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="value" /> is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="keyName" /> does not begin with a valid registry root.-or-<paramref name="keyName" />
        /// is longer than the maximum length allowed (255 characters).-or- The type of <paramref name="value" />
        /// did not match the registry data type specified by <paramref name="valueKind" />, therefore
        /// the data could not be converted properly.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The <see cref="RegistryKey" /> is read-only, and thus cannot be written
        /// to; for example, it is a root-level node, or the key has not been opened with write access.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to create or modify registry keys.
        /// </exception>
        public static void SetValue(string keyName, string valueName, object value, Microsoft.Win32.RegistryValueKind valueKind) { }
    }
    /// <summary>
    /// Represents the possible values for a top-level node on a foreign machine.
    /// </summary>
    public enum RegistryHive
    {
        /// <summary>
        /// Represents the HKEY_CLASSES_ROOT base key on another computer. This value can be passed to
        /// the <see cref="RegistryKey.OpenRemoteBaseKey(RegistryHive,System.String)" />
        /// method, to open this node remotely.
        /// </summary>
        ClassesRoot = -2147483648,
        /// <summary>
        /// Represents the HKEY_CURRENT_CONFIG base key on another computer. This value can be passed
        /// to the <see cref="RegistryKey.OpenRemoteBaseKey(RegistryHive,System.String)" />
        /// method, to open this node remotely.
        /// </summary>
        CurrentConfig = -2147483643,
        /// <summary>
        /// Represents the HKEY_CURRENT_USER base key on another computer. This value can be passed to
        /// the <see cref="RegistryKey.OpenRemoteBaseKey(RegistryHive,System.String)" />
        /// method, to open this node remotely.
        /// </summary>
        CurrentUser = -2147483647,
        /// <summary>
        /// Represents the HKEY_LOCAL_MACHINE base key on another computer. This value can be passed to
        /// the <see cref="RegistryKey.OpenRemoteBaseKey(RegistryHive,System.String)" />
        /// method, to open this node remotely.
        /// </summary>
        LocalMachine = -2147483646,
        /// <summary>
        /// Represents the HKEY_PERFORMANCE_DATA base key on another computer. This value can be passed
        /// to the <see cref="RegistryKey.OpenRemoteBaseKey(RegistryHive,System.String)" />
        /// method, to open this node remotely.
        /// </summary>
        PerformanceData = -2147483644,
        /// <summary>
        /// Represents the HKEY_USERS base key on another computer. This value can be passed to the
        /// <see cref="RegistryKey.OpenRemoteBaseKey(RegistryHive,System.String)" />
        /// method, to open this node remotely.
        /// </summary>
        Users = -2147483645,
    }
    /// <summary>
    /// Represents a key-level node in the Windows registry. This class is a registry encapsulation.
    /// </summary>
    public sealed partial class RegistryKey : System.IDisposable
    {
        internal RegistryKey() { }
        /// <summary>
        /// Gets a <see cref="SafeHandles.SafeRegistryHandle" /> object that represents
        /// the registry key that the current <see cref="RegistryKey" /> object encapsulates.
        /// </summary>
        /// <returns>
        /// The handle to the registry key.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        /// The registry key is closed. Closed keys cannot be accessed.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        /// <exception cref="System.IO.IOException">A system error occurred, such as deletion of the current key.</exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read the key.
        /// </exception>
        public Microsoft.Win32.SafeHandles.SafeRegistryHandle Handle {[System.Security.SecurityCriticalAttribute]get { return default(Microsoft.Win32.SafeHandles.SafeRegistryHandle); } }
        /// <summary>
        /// Retrieves the name of the key.
        /// </summary>
        /// <returns>
        /// The absolute (qualified) name of the key.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> is closed (closed keys cannot be accessed).
        /// </exception>
        public string Name { get { return default(string); } }
        /// <summary>
        /// Retrieves the count of subkeys of the current key.
        /// </summary>
        /// <returns>
        /// The number of subkeys of the current key.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have read permission for the key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> being manipulated is closed (closed keys
        /// cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// A system error occurred, for example the current key has been deleted.
        /// </exception>
        public int SubKeyCount { get { return default(int); } }
        /// <summary>
        /// Retrieves the count of values in the key.
        /// </summary>
        /// <returns>
        /// The number of name/value pairs in the key.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have read permission for the key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> being manipulated is closed (closed keys
        /// cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// A system error occurred, for example the current key has been deleted.
        /// </exception>
        public int ValueCount { get { return default(int); } }
        /// <summary>
        /// Gets the view that was used to create the registry key.
        /// </summary>
        /// <returns>
        /// The view that was used to create the registry key.-or-<see cref="RegistryView.Default" />,
        /// if no view was used.
        /// </returns>
        public Microsoft.Win32.RegistryView View { get { return default(Microsoft.Win32.RegistryView); } }
        /// <summary>
        /// Creates a new subkey or opens an existing subkey for write access.
        /// </summary>
        /// <param name="subkey">The name or path of the subkey to create or open. This string is not case-sensitive.</param>
        /// <returns>
        /// The newly created subkey, or null if the operation failed. If a zero-length string is specified
        /// for <paramref name="subkey" />, the current <see cref="RegistryKey" /> object
        /// is returned.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="subkey" /> is null.</exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to create or open the registry key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> on which this method is being invoked is
        /// closed (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The <see cref="RegistryKey" /> cannot be written to; for example, it was
        /// not opened as a writable key , or the user does not have the necessary access rights.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The nesting level exceeds 510.-or-A system error occurred, such as deletion of the key, or
        /// an attempt to create a key in the <see cref="Registry.LocalMachine" /> root.
        /// </exception>
        public Microsoft.Win32.RegistryKey CreateSubKey(string subkey) { return default(Microsoft.Win32.RegistryKey); }
        /// <summary>
        /// Creates a new subkey or opens an existing subkey with the specified access.
        /// </summary>
        /// <param name="subkey">The name or path of the subkey to create or open. This string is not case-sensitive.</param>
        /// <param name="writable">true to indicate the new subkey is writable; otherwise, false.</param>
        /// <returns>
        /// The newly created subkey, or null if the operation failed. If a zero-length string is specified
        /// for <paramref name="subkey" />, the current <see cref="RegistryKey" /> object
        /// is returned.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="subkey" /> is null.</exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to create or open the registry key.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The current <see cref="RegistryKey" /> cannot be written to; for example,
        /// it was not opened as a writable key, or the user does not have the necessary access rights.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The nesting level exceeds 510.-or-A system error occurred, such as deletion of the key, or
        /// an attempt to create a key in the <see cref="Registry.LocalMachine" /> root.
        /// </exception>
        public Microsoft.Win32.RegistryKey CreateSubKey(string subkey, bool writable) { return default(Microsoft.Win32.RegistryKey); }
        /// <summary>
        /// Creates a new subkey or opens an existing subkey with the specified access.
        /// </summary>
        /// <param name="subkey">The name or path of the subkey to create or open. This string is not case-sensitive.</param>
        /// <param name="writable">true to indicate the new subkey is writable; otherwise, false.</param>
        /// <param name="options">The registry option to use.</param>
        /// <returns>
        /// The newly created subkey, or null if the operation failed. If a zero-length string is specified
        /// for <paramref name="subkey" />, the current <see cref="RegistryKey" /> object
        /// is returned.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="subkey" /> is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="options" /> does not specify a valid Option
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to create or open the registry key.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The current <see cref="RegistryKey" /> cannot be written to; for example,
        /// it was not opened as a writable key, or the user does not have the necessary access rights.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The nesting level exceeds 510.-or-A system error occurred, such as deletion of the key, or
        /// an attempt to create a key in the <see cref="Registry.LocalMachine" /> root.
        /// </exception>
        public Microsoft.Win32.RegistryKey CreateSubKey(string subkey, bool writable, Microsoft.Win32.RegistryOptions options) { return default(Microsoft.Win32.RegistryKey); }
        /// <summary>
        /// Deletes the specified subkey.
        /// </summary>
        /// <param name="subkey">The name of the subkey to delete. This string is not case-sensitive.</param>
        /// <exception cref="System.InvalidOperationException">The <paramref name="subkey" /> has child subkeys</exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="subkey" /> parameter does not specify a valid registry key
        /// </exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="subkey" /> is null</exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to delete the key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> being manipulated is closed (closed keys
        /// cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        public void DeleteSubKey(string subkey) { }
        /// <summary>
        /// Deletes the specified subkey, and specifies whether an exception is raised if the subkey is
        /// not found.
        /// </summary>
        /// <param name="subkey">The name of the subkey to delete. This string is not case-sensitive.</param>
        /// <param name="throwOnMissingSubKey">
        /// Indicates whether an exception should be raised if the specified subkey cannot be found. If
        /// this argument is true and the specified subkey does not exist, an exception is raised. If this argument
        /// is false and the specified subkey does not exist, no action is taken.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="subkey" /> has child subkeys.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="subkey" /> does not specify a valid registry key, and <paramref name="throwOnMissingSubKey" />
        /// is true.
        /// </exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="subkey" /> is null.</exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to delete the key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> being manipulated is closed (closed keys
        /// cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        public void DeleteSubKey(string subkey, bool throwOnMissingSubKey) { }
        /// <summary>
        /// Deletes a subkey and any child subkeys recursively.
        /// </summary>
        /// <param name="subkey">The subkey to delete. This string is not case-sensitive.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="subkey" /> is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// Deletion of a root hive is attempted.-or-<paramref name="subkey" /> does not specify a valid
        /// registry subkey.
        /// </exception>
        /// <exception cref="System.IO.IOException">An I/O error has occurred.</exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to delete the key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> being manipulated is closed (closed keys
        /// cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        public void DeleteSubKeyTree(string subkey) { }
        /// <summary>
        /// Deletes the specified subkey and any child subkeys recursively, and specifies whether an exception
        /// is raised if the subkey is not found.
        /// </summary>
        /// <param name="subkey">The name of the subkey to delete. This string is not case-sensitive.</param>
        /// <param name="throwOnMissingSubKey">
        /// Indicates whether an exception should be raised if the specified subkey cannot be found. If
        /// this argument is true and the specified subkey does not exist, an exception is raised. If this argument
        /// is false and the specified subkey does not exist, no action is taken.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// An attempt was made to delete the root hive of the tree.-or-<paramref name="subkey" /> does
        /// not specify a valid registry subkey, and <paramref name="throwOnMissingSubKey" /> is true.
        /// </exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="subkey" /> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> is closed (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to delete the key.
        /// </exception>
        public void DeleteSubKeyTree(string subkey, bool throwOnMissingSubKey) { }
        /// <summary>
        /// Deletes the specified value from this key.
        /// </summary>
        /// <param name="name">The name of the value to delete.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="name" /> is not a valid reference to a value.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to delete the value.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> being manipulated is closed (closed keys
        /// cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The <see cref="RegistryKey" /> being manipulated is read-only.
        /// </exception>
        public void DeleteValue(string name) { }
        /// <summary>
        /// Deletes the specified value from this key, and specifies whether an exception is raised if
        /// the value is not found.
        /// </summary>
        /// <param name="name">The name of the value to delete.</param>
        /// <param name="throwOnMissingValue">
        /// Indicates whether an exception should be raised if the specified value cannot be found. If
        /// this argument is true and the specified value does not exist, an exception is raised. If this argument
        /// is false and the specified value does not exist, no action is taken.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="name" /> is not a valid reference to a value and <paramref name="throwOnMissingValue" />
        /// is true. -or- <paramref name="name" /> is null.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to delete the value.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> being manipulated is closed (closed keys
        /// cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The <see cref="RegistryKey" /> being manipulated is read-only.
        /// </exception>
        public void DeleteValue(string name, bool throwOnMissingValue) { }
        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="RegistryKey" />
        /// class.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Writes all the attributes of the specified open registry key into the registry.
        /// </summary>
        public void Flush() { }
        /// <summary>
        /// Creates a registry key from a specified handle.
        /// </summary>
        /// <param name="handle">The handle to the registry key.</param>
        /// <returns>
        /// A registry key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="handle" /> is null.</exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to perform this action.
        /// </exception>
        [System.Security.SecurityCriticalAttribute]
        public static Microsoft.Win32.RegistryKey FromHandle(Microsoft.Win32.SafeHandles.SafeRegistryHandle handle) { return default(Microsoft.Win32.RegistryKey); }
        /// <summary>
        /// Creates a registry key from a specified handle and registry view setting.
        /// </summary>
        /// <param name="handle">The handle to the registry key.</param>
        /// <param name="view">The registry view to use.</param>
        /// <returns>
        /// A registry key.
        /// </returns>
        /// <exception cref="System.ArgumentException"><paramref name="view" /> is invalid.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="handle" /> is null.</exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to perform this action.
        /// </exception>
        [System.Security.SecurityCriticalAttribute]
        public static Microsoft.Win32.RegistryKey FromHandle(Microsoft.Win32.SafeHandles.SafeRegistryHandle handle, Microsoft.Win32.RegistryView view) { return default(Microsoft.Win32.RegistryKey); }
        /// <summary>
        /// Retrieves an array of strings that contains all the subkey names.
        /// </summary>
        /// <returns>
        /// An array of strings that contains the names of the subkeys for the current key.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read from the key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> being manipulated is closed (closed keys
        /// cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// A system error occurred, for example the current key has been deleted.
        /// </exception>
        public string[] GetSubKeyNames() { return default(string[]); }
        /// <summary>
        /// Retrieves the value associated with the specified name. Returns null if the name/value pair
        /// does not exist in the registry.
        /// </summary>
        /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
        /// <returns>
        /// The value associated with <paramref name="name" />, or null if <paramref name="name" /> is
        /// not found.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read from the registry key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> that contains the specified value is closed
        /// (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The <see cref="RegistryKey" /> that contains the specified value has been
        /// marked for deletion.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        public object GetValue(string name) { return default(object); }
        /// <summary>
        /// Retrieves the value associated with the specified name. If the name is not found, returns the
        /// default value that you provide.
        /// </summary>
        /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
        /// <param name="defaultValue">The value to return if <paramref name="name" /> does not exist.</param>
        /// <returns>
        /// The value associated with <paramref name="name" />, with any embedded environment variables
        /// left unexpanded, or <paramref name="defaultValue" /> if <paramref name="name" /> is not found.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read from the registry key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> that contains the specified value is closed
        /// (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The <see cref="RegistryKey" /> that contains the specified value has been
        /// marked for deletion.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        public object GetValue(string name, object defaultValue) { return default(object); }
        /// <summary>
        /// Retrieves the value associated with the specified name and retrieval options. If the name is
        /// not found, returns the default value that you provide.
        /// </summary>
        /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
        /// <param name="defaultValue">The value to return if <paramref name="name" /> does not exist.</param>
        /// <param name="options">
        /// One of the enumeration values that specifies optional processing of the retrieved value.
        /// </param>
        /// <returns>
        /// The value associated with <paramref name="name" />, processed according to the specified
        /// <paramref name="options" />, or <paramref name="defaultValue" /> if <paramref name="name" /> is not
        /// found.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read from the registry key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> that contains the specified value is closed
        /// (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The <see cref="RegistryKey" /> that contains the specified value has been
        /// marked for deletion.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="options" /> is not a valid <see cref="RegistryValueOptions" />
        /// value; for example, an invalid value is cast to <see cref="RegistryValueOptions" />.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        public object GetValue(string name, object defaultValue, Microsoft.Win32.RegistryValueOptions options) { return default(object); }
        /// <summary>
        /// Retrieves the registry data type of the value associated with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the value whose registry data type is to be retrieved. This string is not case-sensitive.
        /// </param>
        /// <returns>
        /// The registry data type of the value associated with <paramref name="name" />.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read from the registry key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> that contains the specified value is closed
        /// (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The subkey that contains the specified value does not exist.-or-The name/value pair specified
        /// by <paramref name="name" /> does not exist.This exception is not thrown on Windows 95, Windows
        /// 98, or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        public Microsoft.Win32.RegistryValueKind GetValueKind(string name) { return default(Microsoft.Win32.RegistryValueKind); }
        /// <summary>
        /// Retrieves an array of strings that contains all the value names associated with this key.
        /// </summary>
        /// <returns>
        /// An array of strings that contains the value names for the current key.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read from the registry key.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" />  being manipulated is closed (closed keys
        /// cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// A system error occurred; for example, the current key has been deleted.
        /// </exception>
        public string[] GetValueNames() { return default(string[]); }
        /// <summary>
        /// Opens a new <see cref="RegistryKey" /> that represents the requested key
        /// on the local machine with the specified view.
        /// </summary>
        /// <param name="hKey">The HKEY to open.</param>
        /// <param name="view">The registry view to use.</param>
        /// <returns>
        /// The requested registry key.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="hKey" /> or <paramref name="view" /> is invalid.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The user does not have the necessary registry rights.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to perform this action.
        /// </exception>
        public static Microsoft.Win32.RegistryKey OpenBaseKey(Microsoft.Win32.RegistryHive hKey, Microsoft.Win32.RegistryView view) { return default(Microsoft.Win32.RegistryKey); }
        /// <summary>
        /// Retrieves a subkey as read-only.
        /// </summary>
        /// <param name="name">The name or path of the subkey to open as read-only.</param>
        /// <returns>
        /// The subkey requested, or null if the operation failed.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name" /> is null</exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> is closed (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to read the registry key.
        /// </exception>
        public Microsoft.Win32.RegistryKey OpenSubKey(string name) { return default(Microsoft.Win32.RegistryKey); }
        /// <summary>
        /// Retrieves a specified subkey, and specifies whether write access is to be applied to the key.
        /// </summary>
        /// <param name="name">Name or path of the subkey to open.</param>
        /// <param name="writable">Set to true if you need write access to the key.</param>
        /// <returns>
        /// The subkey requested, or null if the operation failed.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> is closed (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to access the registry key in the specified
        /// mode.
        /// </exception>
        public Microsoft.Win32.RegistryKey OpenSubKey(string name, bool writable) { return default(Microsoft.Win32.RegistryKey); }
        /// <summary>
        /// Retrieves a subkey with the specified name and .Available starting in .NET Framework 4.6
        /// </summary>
        /// <param name="name">The name or path of the subkey to create or open.</param>
        /// <param name="rights">The rights for the registry key.</param>
        /// <returns>
        /// The subkey requested, or null if the operation failed.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> is closed (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to access the registry key in the specified
        /// mode.
        /// </exception>
        public Microsoft.Win32.RegistryKey OpenSubKey(string name, System.Security.AccessControl.RegistryRights rights) { return default(Microsoft.Win32.RegistryKey); }
        /// <summary>
        /// Sets the specified name/value pair.
        /// </summary>
        /// <param name="name">The name of the value to store.</param>
        /// <param name="value">The data to be stored.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="value" /> is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="value" /> is an unsupported data type.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> that contains the specified value is closed
        /// (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The <see cref="RegistryKey" /> is read-only, and cannot be written to; for
        /// example, the key has not been opened with write access. -or-The <see cref="RegistryKey" />
        /// object represents a root-level node, and the operating system is Windows Millennium Edition
        /// or Windows 98.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to create or modify registry keys.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The <see cref="RegistryKey" /> object represents a root-level node, and
        /// the operating system is Windows 2000, Windows XP, or Windows Server 2003.
        /// </exception>
        public void SetValue(string name, object value) { }
        /// <summary>
        /// Sets the value of a name/value pair in the registry key, using the specified registry data
        /// type.
        /// </summary>
        /// <param name="name">The name of the value to be stored.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="valueKind">The registry data type to use when storing the data.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="value" /> is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// The type of <paramref name="value" /> did not match the registry data type specified by
        /// <paramref name="valueKind" />, therefore the data could not be converted properly.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> that contains the specified value is closed
        /// (closed keys cannot be accessed).
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        /// The <see cref="RegistryKey" /> is read-only, and cannot be written to; for
        /// example, the key has not been opened with write access.-or-The <see cref="RegistryKey" />
        /// object represents a root-level node, and the operating system is Windows Millennium Edition
        /// or Windows 98.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The user does not have the permissions required to create or modify registry keys.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// The <see cref="RegistryKey" /> object represents a root-level node, and
        /// the operating system is Windows 2000, Windows XP, or Windows Server 2003.
        /// </exception>
        public void SetValue(string name, object value, Microsoft.Win32.RegistryValueKind valueKind) { }
        /// <summary>
        /// Retrieves a string representation of this key.
        /// </summary>
        /// <returns>
        /// A string representing the key. If the specified key is invalid (cannot be found) then null
        /// is returned.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        /// The <see cref="RegistryKey" /> being accessed is closed (closed keys cannot
        /// be accessed).
        /// </exception>
        public override string ToString() { return default(string); }
    }
    /// <summary>
    /// Specifies options to use when creating a registry key.
    /// </summary>
    [System.FlagsAttribute]
    public enum RegistryOptions
    {
        /// <summary>
        /// A non-volatile key. This is the default.
        /// </summary>
        None = 0,
        /// <summary>
        /// A volatile key. The information is stored in memory and is not preserved when the corresponding
        /// registry hive is unloaded.
        /// </summary>
        Volatile = 1,
    }
    /// <summary>
    /// Specifies the data types to use when storing values in the registry, or identifies the data
    /// type of a value in the registry.
    /// </summary>
    public enum RegistryValueKind
    {
        /// <summary>
        /// Binary data in any form. This value is equivalent to the Win32 API registry data type REG_BINARY.
        /// </summary>
        Binary = 3,
        /// <summary>
        /// A 32-bit binary number. This value is equivalent to the Win32 API registry data type REG_DWORD.
        /// </summary>
        DWord = 4,
        /// <summary>
        /// A null-terminated string that contains unexpanded references to environment variables, such
        /// as %PATH%, that are expanded when the value is retrieved. This value is equivalent to the Win32 API
        /// registry data type REG_EXPAND_SZ.
        /// </summary>
        ExpandString = 2,
        /// <summary>
        /// An array of null-terminated strings, terminated by two null characters. This value is equivalent
        /// to the Win32 API registry data type REG_MULTI_SZ.
        /// </summary>
        MultiString = 7,
        /// <summary>
        /// No data type.
        /// </summary>
        None = -1,
        /// <summary>
        /// A 64-bit binary number. This value is equivalent to the Win32 API registry data type REG_QWORD.
        /// </summary>
        QWord = 11,
        /// <summary>
        /// A null-terminated string. This value is equivalent to the Win32 API registry data type REG_SZ.
        /// </summary>
        String = 1,
        /// <summary>
        /// An unsupported registry data type. For example, the Microsoft Win32 API registry data type
        /// REG_RESOURCE_LIST is unsupported. Use this value to specify that the
        /// <see cref="RegistryKey.SetValue(System.String,System.Object)" /> method should determine the appropriate registry data type when storing a name/value pair.
        /// </summary>
        Unknown = 0,
    }
    /// <summary>
    /// Specifies optional behavior when retrieving name/value pairs from a registry key.
    /// </summary>
    [System.FlagsAttribute]
    public enum RegistryValueOptions
    {
        /// <summary>
        /// A value of type <see cref="RegistryValueKind.ExpandString" /> is retrieved
        /// without expanding its embedded environment variables.
        /// </summary>
        DoNotExpandEnvironmentNames = 1,
        /// <summary>
        /// No optional behavior is specified.
        /// </summary>
        None = 0,
    }
    /// <summary>
    /// Specifies which registry view to target on a 64-bit operating system.
    /// </summary>
    public enum RegistryView
    {
        /// <summary>
        /// The default view.
        /// </summary>
        Default = 0,
        /// <summary>
        /// The 32-bit view.
        /// </summary>
        Registry32 = 512,
        /// <summary>
        /// The 64-bit view.
        /// </summary>
        Registry64 = 256,
    }
}
namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// Represents a safe handle to the Windows registry.
    /// </summary>
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class SafeRegistryHandle : System.Runtime.InteropServices.SafeHandle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeRegistryHandle" />
        /// class.
        /// </summary>
        /// <param name="preexistingHandle">An object that represents the pre-existing handle to use.</param>
        /// <param name="ownsHandle">
        /// true to reliably release the handle during the finalization phase; false to prevent reliable
        /// release.
        /// </param>
        [System.Security.SecurityCriticalAttribute]
        public SafeRegistryHandle(System.IntPtr preexistingHandle, bool ownsHandle) : base(default(System.IntPtr), default(bool)) { }
        [System.Security.SecurityCriticalAttribute]
        protected override bool ReleaseHandle() { return default(bool); }
    }
}
namespace System.Security.AccessControl
{
    /// <summary>
    /// Specifies the access control rights that can be applied to registry objects.
    /// </summary>
    [System.FlagsAttribute]
    public enum RegistryRights
    {
        /// <summary>
        /// The right to change the access rules and audit rules associated with a registry key.
        /// </summary>
        ChangePermissions = 262144,
        /// <summary>
        /// Reserved for system use.
        /// </summary>
        CreateLink = 32,
        /// <summary>
        /// The right to create subkeys of a registry key.
        /// </summary>
        CreateSubKey = 4,
        /// <summary>
        /// The right to delete a registry key.
        /// </summary>
        Delete = 65536,
        /// <summary>
        /// The right to list the subkeys of a registry key.
        /// </summary>
        EnumerateSubKeys = 8,
        /// <summary>
        /// Same as <see cref="ReadKey" />.
        /// </summary>
        ExecuteKey = 131097,
        /// <summary>
        /// The right to exert full control over a registry key, and to modify its access rules and audit
        /// rules.
        /// </summary>
        FullControl = 983103,
        /// <summary>
        /// The right to request notification of changes on a registry key.
        /// </summary>
        Notify = 16,
        /// <summary>
        /// The right to query the name/value pairs in a registry key.
        /// </summary>
        QueryValues = 1,
        /// <summary>
        /// The right to query the name/value pairs in a registry key, to request notification of changes,
        /// to enumerate its subkeys, and to read its access rules and audit rules.
        /// </summary>
        ReadKey = 131097,
        /// <summary>
        /// The right to open and copy the access rules and audit rules for a registry key.
        /// </summary>
        ReadPermissions = 131072,
        /// <summary>
        /// The right to create, delete, or set name/value pairs in a registry key.
        /// </summary>
        SetValue = 2,
        /// <summary>
        /// The right to change the owner of a registry key.
        /// </summary>
        TakeOwnership = 524288,
        /// <summary>
        /// The right to create, delete, and set the name/value pairs in a registry key, to create or delete
        /// subkeys, to request notification of changes, to enumerate its subkeys, and to read its access
        /// rules and audit rules.
        /// </summary>
        WriteKey = 131078,
    }
}
