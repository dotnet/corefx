﻿// This is auto generated file. Please don’t modify manually.
// The file is generated as part of the build through the ResourceGenerator tool 
// which takes the project resx resource file and generated this source code file.
// By default the tool will use Resources\Strings.resx but projects can customize
// that by overriding the StringResourcesPath property group.
namespace System
{
    internal static partial class SR
    {
#pragma warning disable 0414
        private const string s_resourcesName = "System.IO.MemoryMappedFiles.resources"; // assembly Name + .resources
#pragma warning restore 0414

#if !DEBUGRESOURCES
        internal static string ArgumentOutOfRange_NeedNonNegNum {
              get { return SR.GetResourceString("ArgumentOutOfRange_NeedNonNegNum", null); }
        }
        internal static string IO_FileNotFound {
              get { return SR.GetResourceString("IO_FileNotFound", null); }
        }
        internal static string IO_FileNotFound_FileName {
              get { return SR.GetResourceString("IO_FileNotFound_FileName", null); }
        }
        internal static string IO_AlreadyExists_Name {
              get { return SR.GetResourceString("IO_AlreadyExists_Name", null); }
        }
        internal static string IO_FileExists_Name {
              get { return SR.GetResourceString("IO_FileExists_Name", null); }
        }
        internal static string IO_NoPermissionToDirectoryName {
              get { return SR.GetResourceString("IO_NoPermissionToDirectoryName", null); }
        }
        internal static string IO_SharingViolation_File {
              get { return SR.GetResourceString("IO_SharingViolation_File", null); }
        }
        internal static string IO_SharingViolation_NoFileName {
              get { return SR.GetResourceString("IO_SharingViolation_NoFileName", null); }
        }
        internal static string IO_DriveNotFound_Drive {
              get { return SR.GetResourceString("IO_DriveNotFound_Drive", null); }
        }
        internal static string IO_PathNotFound_Path {
              get { return SR.GetResourceString("IO_PathNotFound_Path", null); }
        }
        internal static string IO_PathNotFound_NoPathName {
              get { return SR.GetResourceString("IO_PathNotFound_NoPathName", null); }
        }
        internal static string IO_PathTooLong {
              get { return SR.GetResourceString("IO_PathTooLong", null); }
        }
        internal static string UnauthorizedAccess_IODenied_Path {
              get { return SR.GetResourceString("UnauthorizedAccess_IODenied_Path", null); }
        }
        internal static string UnauthorizedAccess_IODenied_NoPathName {
              get { return SR.GetResourceString("UnauthorizedAccess_IODenied_NoPathName", null); }
        }
        internal static string Argument_MapNameEmptyString {
              get { return SR.GetResourceString("Argument_MapNameEmptyString", null); }
        }
        internal static string Argument_EmptyFile {
              get { return SR.GetResourceString("Argument_EmptyFile", null); }
        }
        internal static string Argument_NewMMFWriteAccessNotAllowed {
              get { return SR.GetResourceString("Argument_NewMMFWriteAccessNotAllowed", null); }
        }
        internal static string Argument_ReadAccessWithLargeCapacity {
              get { return SR.GetResourceString("Argument_ReadAccessWithLargeCapacity", null); }
        }
        internal static string Argument_NewMMFAppendModeNotAllowed {
              get { return SR.GetResourceString("Argument_NewMMFAppendModeNotAllowed", null); }
        }
        internal static string ArgumentNull_MapName {
              get { return SR.GetResourceString("ArgumentNull_MapName", null); }
        }
        internal static string ArgumentNull_FileStream {
              get { return SR.GetResourceString("ArgumentNull_FileStream", null); }
        }
        internal static string ArgumentOutOfRange_CapacityLargerThanLogicalAddressSpaceNotAllowed {
              get { return SR.GetResourceString("ArgumentOutOfRange_CapacityLargerThanLogicalAddressSpaceNotAllowed", null); }
        }
        internal static string ArgumentOutOfRange_FileLengthTooBig {
              get { return SR.GetResourceString("ArgumentOutOfRange_FileLengthTooBig", null); }
        }
        internal static string ArgumentOutOfRange_NeedPositiveNumber {
              get { return SR.GetResourceString("ArgumentOutOfRange_NeedPositiveNumber", null); }
        }
        internal static string ArgumentOutOfRange_PositiveOrDefaultCapacityRequired {
              get { return SR.GetResourceString("ArgumentOutOfRange_PositiveOrDefaultCapacityRequired", null); }
        }
        internal static string ArgumentOutOfRange_PositiveOrDefaultSizeRequired {
              get { return SR.GetResourceString("ArgumentOutOfRange_PositiveOrDefaultSizeRequired", null); }
        }
        internal static string ArgumentOutOfRange_CapacityGEFileSizeRequired {
              get { return SR.GetResourceString("ArgumentOutOfRange_CapacityGEFileSizeRequired", null); }
        }
        internal static string IO_NotEnoughMemory {
              get { return SR.GetResourceString("IO_NotEnoughMemory", null); }
        }
        internal static string InvalidOperation_CantCreateFileMapping {
              get { return SR.GetResourceString("InvalidOperation_CantCreateFileMapping", null); }
        }
        internal static string InvalidOperation_ViewIsNull {
              get { return SR.GetResourceString("InvalidOperation_ViewIsNull", null); }
        }
        internal static string NotSupported_MMViewStreamsFixedLength {
              get { return SR.GetResourceString("NotSupported_MMViewStreamsFixedLength", null); }
        }
        internal static string NotSupported_UnreadableStream {
              get { return SR.GetResourceString("NotSupported_UnreadableStream", null); }
        }
        internal static string NotSupported_UnwritableStream {
              get { return SR.GetResourceString("NotSupported_UnwritableStream", null); }
        }
        internal static string ObjectDisposed_ViewAccessorClosed {
              get { return SR.GetResourceString("ObjectDisposed_ViewAccessorClosed", null); }
        }
        internal static string ObjectDisposed_StreamIsClosed {
              get { return SR.GetResourceString("ObjectDisposed_StreamIsClosed", null); }
        }
        internal static string UnknownError_Num {
              get { return SR.GetResourceString("UnknownError_Num", null); }
        }
        internal static string PlatformNotSupported_NamedMaps {
              get { return SR.GetResourceString("PlatformNotSupported_NamedMaps", null); }
        }
#else
        internal static string ArgumentOutOfRange_NeedNonNegNum {
              get { return SR.GetResourceString("ArgumentOutOfRange_NeedNonNegNum", @"Non negative number is required."); }
        }
        internal static string IO_FileNotFound {
              get { return SR.GetResourceString("IO_FileNotFound", @"Unable to find the specified file."); }
        }
        internal static string IO_FileNotFound_FileName {
              get { return SR.GetResourceString("IO_FileNotFound_FileName", @"Could not find file '{0}'."); }
        }
        internal static string IO_AlreadyExists_Name {
              get { return SR.GetResourceString("IO_AlreadyExists_Name", @"Cannot create \""{0}\"" because a file or directory with the same name already exists."); }
        }
        internal static string IO_FileExists_Name {
              get { return SR.GetResourceString("IO_FileExists_Name", @"The file '{0}' already exists."); }
        }
        internal static string IO_NoPermissionToDirectoryName {
              get { return SR.GetResourceString("IO_NoPermissionToDirectoryName", @"<Path discovery permission to the specified directory was denied.>"); }
        }
        internal static string IO_SharingViolation_File {
              get { return SR.GetResourceString("IO_SharingViolation_File", @"The process cannot access the file '{0}' because it is being used by another process."); }
        }
        internal static string IO_SharingViolation_NoFileName {
              get { return SR.GetResourceString("IO_SharingViolation_NoFileName", @"The process cannot access the file because it is being used by another process."); }
        }
        internal static string IO_DriveNotFound_Drive {
              get { return SR.GetResourceString("IO_DriveNotFound_Drive", @"Could not find the drive '{0}'. The drive might not be ready or might not be mapped."); }
        }
        internal static string IO_PathNotFound_Path {
              get { return SR.GetResourceString("IO_PathNotFound_Path", @"Could not find a part of the path '{0}'."); }
        }
        internal static string IO_PathNotFound_NoPathName {
              get { return SR.GetResourceString("IO_PathNotFound_NoPathName", @"Could not find a part of the path."); }
        }
        internal static string IO_PathTooLong {
              get { return SR.GetResourceString("IO_PathTooLong", @"The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters."); }
        }
        internal static string UnauthorizedAccess_IODenied_Path {
              get { return SR.GetResourceString("UnauthorizedAccess_IODenied_Path", @"Access to the path '{0}' is denied."); }
        }
        internal static string UnauthorizedAccess_IODenied_NoPathName {
              get { return SR.GetResourceString("UnauthorizedAccess_IODenied_NoPathName", @"Access to the path is denied."); }
        }
        internal static string Argument_MapNameEmptyString {
              get { return SR.GetResourceString("Argument_MapNameEmptyString", @"Map name cannot be an empty string."); }
        }
        internal static string Argument_EmptyFile {
              get { return SR.GetResourceString("Argument_EmptyFile", @"A positive capacity must be specified for a Memory Mapped File backed by an empty file."); }
        }
        internal static string Argument_NewMMFWriteAccessNotAllowed {
              get { return SR.GetResourceString("Argument_NewMMFWriteAccessNotAllowed", @"MemoryMappedFileAccess.Write is not permitted when creating new memory mapped files. Use MemoryMappedFileAccess.ReadWrite instead."); }
        }
        internal static string Argument_ReadAccessWithLargeCapacity {
              get { return SR.GetResourceString("Argument_ReadAccessWithLargeCapacity", @"When specifying MemoryMappedFileAccess.Read access, the capacity must not be larger than the file size."); }
        }
        internal static string Argument_NewMMFAppendModeNotAllowed {
              get { return SR.GetResourceString("Argument_NewMMFAppendModeNotAllowed", @"FileMode.Append is not permitted when creating new memory mapped files. Instead, use MemoryMappedFileView to ensure write-only access within a specified region."); }
        }
        internal static string ArgumentNull_MapName {
              get { return SR.GetResourceString("ArgumentNull_MapName", @"Map name cannot be null."); }
        }
        internal static string ArgumentNull_FileStream {
              get { return SR.GetResourceString("ArgumentNull_FileStream", @"fileStream cannot be null."); }
        }
        internal static string ArgumentOutOfRange_CapacityLargerThanLogicalAddressSpaceNotAllowed {
              get { return SR.GetResourceString("ArgumentOutOfRange_CapacityLargerThanLogicalAddressSpaceNotAllowed", @"The capacity cannot be greater than the size of the system's logical address space."); }
        }
        internal static string ArgumentOutOfRange_FileLengthTooBig {
              get { return SR.GetResourceString("ArgumentOutOfRange_FileLengthTooBig", @"Specified file length was too large for the file system."); }
        }
        internal static string ArgumentOutOfRange_NeedPositiveNumber {
              get { return SR.GetResourceString("ArgumentOutOfRange_NeedPositiveNumber", @"A positive number is required."); }
        }
        internal static string ArgumentOutOfRange_PositiveOrDefaultCapacityRequired {
              get { return SR.GetResourceString("ArgumentOutOfRange_PositiveOrDefaultCapacityRequired", @"The capacity must be greater than or equal to 0. 0 represents the the size of the file being mapped."); }
        }
        internal static string ArgumentOutOfRange_PositiveOrDefaultSizeRequired {
              get { return SR.GetResourceString("ArgumentOutOfRange_PositiveOrDefaultSizeRequired", @"The size must be greater than or equal to 0. If 0 is specified, the view extends from the specified offset to the end of the file mapping."); }
        }
        internal static string ArgumentOutOfRange_CapacityGEFileSizeRequired {
              get { return SR.GetResourceString("ArgumentOutOfRange_CapacityGEFileSizeRequired", @"The capacity may not be smaller than the file size."); }
        }
        internal static string IO_NotEnoughMemory {
              get { return SR.GetResourceString("IO_NotEnoughMemory", @"Not enough memory to map view."); }
        }
        internal static string InvalidOperation_CantCreateFileMapping {
              get { return SR.GetResourceString("InvalidOperation_CantCreateFileMapping", @"Cannot create file mapping."); }
        }
        internal static string InvalidOperation_ViewIsNull {
              get { return SR.GetResourceString("InvalidOperation_ViewIsNull", @"The underlying MemoryMappedView object is null."); }
        }
        internal static string NotSupported_MMViewStreamsFixedLength {
              get { return SR.GetResourceString("NotSupported_MMViewStreamsFixedLength", @"MemoryMappedViewStreams are fixed length."); }
        }
        internal static string NotSupported_UnreadableStream {
              get { return SR.GetResourceString("NotSupported_UnreadableStream", @"Stream does not support reading."); }
        }
        internal static string NotSupported_UnwritableStream {
              get { return SR.GetResourceString("NotSupported_UnwritableStream", @"Stream does not support writing."); }
        }
        internal static string ObjectDisposed_ViewAccessorClosed {
              get { return SR.GetResourceString("ObjectDisposed_ViewAccessorClosed", @"Cannot access a closed accessor."); }
        }
        internal static string ObjectDisposed_StreamIsClosed {
              get { return SR.GetResourceString("ObjectDisposed_StreamIsClosed", @"Cannot access a closed Stream."); }
        }
        internal static string UnknownError_Num {
              get { return SR.GetResourceString("UnknownError_Num", @"Unknown error '{0}'."); }
        }
        internal static string PlatformNotSupported_NamedMaps {
              get { return SR.GetResourceString("PlatformNotSupported_NamedMaps", @"Named maps are not supported."); }
        }

#endif
    }
}
