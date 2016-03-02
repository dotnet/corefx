// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Principal;
using System.Diagnostics.Contracts;

namespace System.Security.AccessControl
{
    internal static class Win32
    {
        internal const System.Int32 TRUE = 1;

        //
        // Wrapper around advapi32.ConvertSecurityDescriptorToStringSecurityDescriptorW
        //

        internal static int ConvertSdToSddl(
            byte[] binaryForm,
            int requestedRevision,
            SecurityInfos si,
            out string resultSddl)
        {
            int errorCode;
            IntPtr ByteArray;
            uint ByteArraySize = 0;

            if (TRUE != Interop.mincore.ConvertSdToStringSd(binaryForm, (uint)requestedRevision, (uint)si, out ByteArray, ref ByteArraySize))
            {
                errorCode = Marshal.GetLastWin32Error();
                goto Error;
            }

            //
            // Extract data from the returned pointer
            //

            resultSddl = Marshal.PtrToStringUni(ByteArray);

            //
            // Now is a good time to get rid of the returned pointer
            //

            Interop.mincore_obsolete.LocalFree(ByteArray);

            return 0;

        Error:

            resultSddl = null;

            if (errorCode == Interop.mincore.Errors.ERROR_NOT_ENOUGH_MEMORY)
            {
                throw new OutOfMemoryException();
            }

            return errorCode;
        }

        //
        // Wrapper around advapi32.GetSecurityInfo
        //

        internal static int GetSecurityInfo(
            ResourceType resourceType,
            string name,
            SafeHandle handle,
            AccessControlSections accessControlSections,
            out RawSecurityDescriptor resultSd
            )
        {
            resultSd = null;

            int errorCode;
            IntPtr SidOwner, SidGroup, Dacl, Sacl, ByteArray;
            SecurityInfos SecurityInfos = 0;
            Privilege privilege = null;

            if ((accessControlSections & AccessControlSections.Owner) != 0)
            {
                SecurityInfos |= SecurityInfos.Owner;
            }

            if ((accessControlSections & AccessControlSections.Group) != 0)
            {
                SecurityInfos |= SecurityInfos.Group;
            }

            if ((accessControlSections & AccessControlSections.Access) != 0)
            {
                SecurityInfos |= SecurityInfos.DiscretionaryAcl;
            }

            if ((accessControlSections & AccessControlSections.Audit) != 0)
            {
                SecurityInfos |= SecurityInfos.SystemAcl;
                privilege = new Privilege(Privilege.Security);
            }

            try
            {
                if (privilege != null)
                {
                    try
                    {
                        privilege.Enable();
                    }
                    catch (PrivilegeNotHeldException)
                    {
                        // we will ignore this exception and press on just in case this is a remote resource
                    }
                }

                if (name != null)
                {
                    errorCode = (int)Interop.mincore.GetSecurityInfoByName(name, (uint)resourceType, (uint)SecurityInfos, out SidOwner, out SidGroup, out Dacl, out Sacl, out ByteArray);
                }
                else if (handle != null)
                {
                    if (handle.IsInvalid)
                    {
                        throw new ArgumentException(
                            SR.Argument_InvalidSafeHandle,
nameof(handle));
                    }
                    else
                    {
                        errorCode = (int)Interop.mincore.GetSecurityInfoByHandle(handle, (uint)resourceType, (uint)SecurityInfos, out SidOwner, out SidGroup, out Dacl, out Sacl, out ByteArray);
                    }
                }
                else
                {
                    // both are null, shouldn't happen
                    // Changing from SystemException to ArgumentException as this code path is indicative of a null name argument
                    // as well as an accessControlSections argument with an audit flag
                    throw new ArgumentException();
                }

                if (errorCode == Interop.mincore.Errors.ERROR_SUCCESS && IntPtr.Zero.Equals(ByteArray))
                {
                    //
                    // This means that the object doesn't have a security descriptor. And thus we throw
                    // a specific exception for the caller to catch and handle properly.
                    //
                    throw new InvalidOperationException(SR.InvalidOperation_NoSecurityDescriptor);
                }
                else if (errorCode == Interop.mincore.Errors.ERROR_NOT_ALL_ASSIGNED ||
                         errorCode == Interop.mincore.Errors.ERROR_PRIVILEGE_NOT_HELD)
                {
                    throw new PrivilegeNotHeldException(Privilege.Security);
                }
                else if (errorCode == Interop.mincore.Errors.ERROR_ACCESS_DENIED ||
                    errorCode == Interop.mincore.Errors.ERROR_CANT_OPEN_ANONYMOUS)
                {
                    throw new UnauthorizedAccessException();
                }

                if (errorCode != Interop.mincore.Errors.ERROR_SUCCESS)
                {
                    goto Error;
                }
            }
            catch
            {
                // protection against exception filter-based luring attacks
                if (privilege != null)
                {
                    privilege.Revert();
                }
                throw;
            }
            finally
            {
                if (privilege != null)
                {
                    privilege.Revert();
                }
            }

            //
            // Extract data from the returned pointer
            //

            uint Length = Interop.mincore.GetSecurityDescriptorLength(ByteArray);

            byte[] BinaryForm = new byte[Length];

            Marshal.Copy(ByteArray, BinaryForm, 0, (int)Length);

            Interop.mincore_obsolete.LocalFree(ByteArray);

            resultSd = new RawSecurityDescriptor(BinaryForm, 0);

            return Interop.mincore.Errors.ERROR_SUCCESS;

        Error:

            if (errorCode == Interop.mincore.Errors.ERROR_NOT_ENOUGH_MEMORY)
            {
                throw new OutOfMemoryException();
            }

            return errorCode;
        }

        //
        // Wrapper around advapi32.SetNamedSecurityInfoW and advapi32.SetSecurityInfo
        //

        internal static int SetSecurityInfo(
            ResourceType type,
            string name,
            SafeHandle handle,
            SecurityInfos securityInformation,
            SecurityIdentifier owner,
            SecurityIdentifier group,
            GenericAcl sacl,
            GenericAcl dacl)
        {
            int errorCode;
            int Length;
            byte[] OwnerBinary = null, GroupBinary = null, SaclBinary = null, DaclBinary = null;
            Privilege securityPrivilege = null;

            if (owner != null)
            {
                Length = owner.BinaryLength;
                OwnerBinary = new byte[Length];
                owner.GetBinaryForm(OwnerBinary, 0);
            }

            if (group != null)
            {
                Length = group.BinaryLength;
                GroupBinary = new byte[Length];
                group.GetBinaryForm(GroupBinary, 0);
            }

            if (dacl != null)
            {
                Length = dacl.BinaryLength;
                DaclBinary = new byte[Length];
                dacl.GetBinaryForm(DaclBinary, 0);
            }

            if (sacl != null)
            {
                Length = sacl.BinaryLength;
                SaclBinary = new byte[Length];
                sacl.GetBinaryForm(SaclBinary, 0);
            }

            if ((securityInformation & SecurityInfos.SystemAcl) != 0)
            {
                //
                // Enable security privilege if trying to set a SACL. 
                // Note: even setting it by handle needs this privilege enabled!
                //

                securityPrivilege = new Privilege(Privilege.Security);
            }

            try
            {
                if (securityPrivilege != null)
                {
                    try
                    {
                        securityPrivilege.Enable();
                    }
                    catch (PrivilegeNotHeldException)
                    {
                        // we will ignore this exception and press on just in case this is a remote resource
                    }
                }

                if (name != null)
                {
                    errorCode = (int)Interop.mincore.SetSecurityInfoByName(name, (uint)type, (uint)securityInformation, OwnerBinary, GroupBinary, DaclBinary, SaclBinary);
                }
                else if (handle != null)
                {
                    if (handle.IsInvalid)
                    {
                        throw new ArgumentException(
                            SR.Argument_InvalidSafeHandle,
nameof(handle));
                    }
                    else
                    {
                        errorCode = (int)Interop.mincore.SetSecurityInfoByHandle(handle, (uint)type, (uint)securityInformation, OwnerBinary, GroupBinary, DaclBinary, SaclBinary);
                    }
                }
                else
                {
                    // both are null, shouldn't happen
                    Contract.Assert(false, "Internal error: both name and handle are null");
                    throw new ArgumentException();
                }

                if (errorCode == Interop.mincore.Errors.ERROR_NOT_ALL_ASSIGNED ||
                    errorCode == Interop.mincore.Errors.ERROR_PRIVILEGE_NOT_HELD)
                {
                    throw new PrivilegeNotHeldException(Privilege.Security);
                }
                else if (errorCode == Interop.mincore.Errors.ERROR_ACCESS_DENIED ||
                    errorCode == Interop.mincore.Errors.ERROR_CANT_OPEN_ANONYMOUS)
                {
                    throw new UnauthorizedAccessException();
                }
                else if (errorCode != Interop.mincore.Errors.ERROR_SUCCESS)
                {
                    goto Error;
                }
            }
            catch
            {
                // protection against exception filter-based luring attacks
                if (securityPrivilege != null)
                {
                    securityPrivilege.Revert();
                }
                throw;
            }
            finally
            {
                if (securityPrivilege != null)
                {
                    securityPrivilege.Revert();
                }
            }

            return 0;

        Error:

            if (errorCode == Interop.mincore.Errors.ERROR_NOT_ENOUGH_MEMORY)
            {
                throw new OutOfMemoryException();
            }

            return errorCode;
        }
    }
}
