// This is auto generated file. Please don’t modify manually.
// The file is generated as part of the build through the ResourceGenerator tool 
// which takes the project resx resource file and generated this source code file.
// By default the tool will use Resources\Strings.resx but projects can customize
// that by overriding the StringResourcesPath property group.
namespace System
{
    internal static partial class SR
    {
#pragma warning disable 0414
        private const string s_resourcesName = "Microsoft.Win32.Registry.resources"; // assembly Name + .resources
#pragma warning restore 0414

#if !DEBUGRESOURCES
        internal static string Arg_RegSubKeyAbsent {
              get { return SR.GetResourceString("Arg_RegSubKeyAbsent", null); }
        }
        internal static string Arg_RegKeyDelHive {
              get { return SR.GetResourceString("Arg_RegKeyDelHive", null); }
        }
        internal static string Arg_RegKeyNoRemoteConnect {
              get { return SR.GetResourceString("Arg_RegKeyNoRemoteConnect", null); }
        }
        internal static string Arg_RegKeyOutOfRange {
              get { return SR.GetResourceString("Arg_RegKeyOutOfRange", null); }
        }
        internal static string Arg_RegKeyNotFound {
              get { return SR.GetResourceString("Arg_RegKeyNotFound", null); }
        }
        internal static string Arg_RegKeyStrLenBug {
              get { return SR.GetResourceString("Arg_RegKeyStrLenBug", null); }
        }
        internal static string Arg_RegValStrLenBug {
              get { return SR.GetResourceString("Arg_RegValStrLenBug", null); }
        }
        internal static string Arg_RegBadKeyKind {
              get { return SR.GetResourceString("Arg_RegBadKeyKind", null); }
        }
        internal static string Arg_RegGetOverflowBug {
              get { return SR.GetResourceString("Arg_RegGetOverflowBug", null); }
        }
        internal static string Arg_RegSetMismatchedKind {
              get { return SR.GetResourceString("Arg_RegSetMismatchedKind", null); }
        }
        internal static string Arg_RegSetBadArrType {
              get { return SR.GetResourceString("Arg_RegSetBadArrType", null); }
        }
        internal static string Arg_RegSetStrArrNull {
              get { return SR.GetResourceString("Arg_RegSetStrArrNull", null); }
        }
        internal static string Arg_RegInvalidKeyName {
              get { return SR.GetResourceString("Arg_RegInvalidKeyName", null); }
        }
        internal static string Arg_DllInitFailure {
              get { return SR.GetResourceString("Arg_DllInitFailure", null); }
        }
        internal static string Arg_EnumIllegalVal {
              get { return SR.GetResourceString("Arg_EnumIllegalVal", null); }
        }
        internal static string Arg_RegSubKeyValueAbsent {
              get { return SR.GetResourceString("Arg_RegSubKeyValueAbsent", null); }
        }
        internal static string Argument_InvalidRegistryOptionsCheck {
              get { return SR.GetResourceString("Argument_InvalidRegistryOptionsCheck", null); }
        }
        internal static string Argument_InvalidRegistryViewCheck {
              get { return SR.GetResourceString("Argument_InvalidRegistryViewCheck", null); }
        }
        internal static string Argument_InvalidRegistryKeyPermissionCheck {
              get { return SR.GetResourceString("Argument_InvalidRegistryKeyPermissionCheck", null); }
        }
        internal static string InvalidOperation_RegRemoveSubKey {
              get { return SR.GetResourceString("InvalidOperation_RegRemoveSubKey", null); }
        }
        internal static string ObjectDisposed_RegKeyClosed {
              get { return SR.GetResourceString("ObjectDisposed_RegKeyClosed", null); }
        }
        internal static string Security_RegistryPermission {
              get { return SR.GetResourceString("Security_RegistryPermission", null); }
        }
        internal static string UnauthorizedAccess_RegistryKeyGeneric_Key {
              get { return SR.GetResourceString("UnauthorizedAccess_RegistryKeyGeneric_Key", null); }
        }
        internal static string UnauthorizedAccess_RegistryNoWrite {
              get { return SR.GetResourceString("UnauthorizedAccess_RegistryNoWrite", null); }
        }
        internal static string UnknownError_Num {
              get { return SR.GetResourceString("UnknownError_Num", null); }
        }
#else
        internal static string Arg_RegSubKeyAbsent {
              get { return SR.GetResourceString("Arg_RegSubKeyAbsent", @"Cannot delete a subkey tree because the subkey does not exist."); }
        }
        internal static string Arg_RegKeyDelHive {
              get { return SR.GetResourceString("Arg_RegKeyDelHive", @"Cannot delete a registry hive's subtree."); }
        }
        internal static string Arg_RegKeyNoRemoteConnect {
              get { return SR.GetResourceString("Arg_RegKeyNoRemoteConnect", @"No remote connection to '{0}' while trying to read the registry."); }
        }
        internal static string Arg_RegKeyOutOfRange {
              get { return SR.GetResourceString("Arg_RegKeyOutOfRange", @"Registry HKEY was out of the legal range."); }
        }
        internal static string Arg_RegKeyNotFound {
              get { return SR.GetResourceString("Arg_RegKeyNotFound", @"The specified registry key does not exist."); }
        }
        internal static string Arg_RegKeyStrLenBug {
              get { return SR.GetResourceString("Arg_RegKeyStrLenBug", @"Registry key names should not be greater than 255 characters."); }
        }
        internal static string Arg_RegValStrLenBug {
              get { return SR.GetResourceString("Arg_RegValStrLenBug", @"Registry value names should not be greater than 16,383 characters."); }
        }
        internal static string Arg_RegBadKeyKind {
              get { return SR.GetResourceString("Arg_RegBadKeyKind", @"The specified RegistryValueKind is an invalid value."); }
        }
        internal static string Arg_RegGetOverflowBug {
              get { return SR.GetResourceString("Arg_RegGetOverflowBug", @"RegistryKey.GetValue does not allow a String that has a length greater than Int32.MaxValue."); }
        }
        internal static string Arg_RegSetMismatchedKind {
              get { return SR.GetResourceString("Arg_RegSetMismatchedKind", @"The type of the value object did not match the specified RegistryValueKind or the object could not be properly converted."); }
        }
        internal static string Arg_RegSetBadArrType {
              get { return SR.GetResourceString("Arg_RegSetBadArrType", @"RegistryKey.SetValue does not support arrays of type '{0}'. Only Byte[] and String[] are supported."); }
        }
        internal static string Arg_RegSetStrArrNull {
              get { return SR.GetResourceString("Arg_RegSetStrArrNull", @"RegistryKey.SetValue does not allow a String[] that contains a null String reference."); }
        }
        internal static string Arg_RegInvalidKeyName {
              get { return SR.GetResourceString("Arg_RegInvalidKeyName", @"Registry key name must start with a valid base key name."); }
        }
        internal static string Arg_DllInitFailure {
              get { return SR.GetResourceString("Arg_DllInitFailure", @"One machine may not have remote administration enabled, or both machines may not be running the remote registry service."); }
        }
        internal static string Arg_EnumIllegalVal {
              get { return SR.GetResourceString("Arg_EnumIllegalVal", @"Illegal enum value: {0}."); }
        }
        internal static string Arg_RegSubKeyValueAbsent {
              get { return SR.GetResourceString("Arg_RegSubKeyValueAbsent", @"No value exists with that name."); }
        }
        internal static string Argument_InvalidRegistryOptionsCheck {
              get { return SR.GetResourceString("Argument_InvalidRegistryOptionsCheck", @"The specified RegistryOptions value is invalid."); }
        }
        internal static string Argument_InvalidRegistryViewCheck {
              get { return SR.GetResourceString("Argument_InvalidRegistryViewCheck", @"The specified RegistryView value is invalid."); }
        }
        internal static string Argument_InvalidRegistryKeyPermissionCheck {
              get { return SR.GetResourceString("Argument_InvalidRegistryKeyPermissionCheck", @"The specified RegistryKeyPermissionCheck value is invalid."); }
        }
        internal static string InvalidOperation_RegRemoveSubKey {
              get { return SR.GetResourceString("InvalidOperation_RegRemoveSubKey", @"Registry key has subkeys and recursive removes are not supported by this method."); }
        }
        internal static string ObjectDisposed_RegKeyClosed {
              get { return SR.GetResourceString("ObjectDisposed_RegKeyClosed", @"Cannot access a closed registry key."); }
        }
        internal static string Security_RegistryPermission {
              get { return SR.GetResourceString("Security_RegistryPermission", @"Requested registry access is not allowed."); }
        }
        internal static string UnauthorizedAccess_RegistryKeyGeneric_Key {
              get { return SR.GetResourceString("UnauthorizedAccess_RegistryKeyGeneric_Key", @"Access to the registry key '{0}' is denied."); }
        }
        internal static string UnauthorizedAccess_RegistryNoWrite {
              get { return SR.GetResourceString("UnauthorizedAccess_RegistryNoWrite", @"Cannot write to the registry key."); }
        }
        internal static string UnknownError_Num {
              get { return SR.GetResourceString("UnknownError_Num", @"Unknown error '{0}'."); }
        }

#endif
    }
}
