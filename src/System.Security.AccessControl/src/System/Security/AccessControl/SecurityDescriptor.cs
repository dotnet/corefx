// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Classes:  Security Descriptor family of classes
**
**
===========================================================*/

using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace System.Security.AccessControl
{
    [Flags]

    public enum ControlFlags
    {
        None = 0x0000,
        OwnerDefaulted = 0x0001, // set by RM only
        GroupDefaulted = 0x0002, // set by RM only
        DiscretionaryAclPresent = 0x0004, // set by RM or user, 'off' means DACL is null
        DiscretionaryAclDefaulted = 0x0008, // set by RM only
        SystemAclPresent = 0x0010, // same as DiscretionaryAclPresent
        SystemAclDefaulted = 0x0020, // sams as DiscretionaryAclDefaulted
        DiscretionaryAclUntrusted = 0x0040, // ignore this one
        ServerSecurity = 0x0080, // ignore this one
        DiscretionaryAclAutoInheritRequired = 0x0100, // ignore this one
        SystemAclAutoInheritRequired = 0x0200, // ignore this one
        DiscretionaryAclAutoInherited = 0x0400, // set by RM only
        SystemAclAutoInherited = 0x0800, // set by RM only
        DiscretionaryAclProtected = 0x1000, // when set, RM will stop inheriting
        SystemAclProtected = 0x2000, // when set, RM will stop inheriting
        RMControlValid = 0x4000, // the reserved 8 bits have some meaning
        SelfRelative = 0x8000, // must always be on
    }

    public abstract class GenericSecurityDescriptor
    {
        #region Protected Members

        //
        // Pictorially the structure of a security descriptor is as follows:
        //
        //       3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
        //       1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
        //      +---------------------------------------------------------------+
        //      |            Control            |Reserved1 (SBZ)|   Revision    |
        //      +---------------------------------------------------------------+
        //      |                            Owner                              |
        //      +---------------------------------------------------------------+
        //      |                            Group                              |
        //      +---------------------------------------------------------------+
        //      |                            Sacl                               |
        //      +---------------------------------------------------------------+
        //      |                            Dacl                               |
        //      +---------------------------------------------------------------+
        //

        internal const int HeaderLength = 20;
        internal const int OwnerFoundAt = 4;
        internal const int GroupFoundAt = 8;
        internal const int SaclFoundAt = 12;
        internal const int DaclFoundAt = 16;

        #endregion

        #region Private Methods

        //
        // Stores an integer in big-endian format into an array at a given offset
        //

        private static void MarshalInt(byte[] binaryForm, int offset, int number)
        {
            binaryForm[offset + 0] = (byte)(number >> 0);
            binaryForm[offset + 1] = (byte)(number >> 8);
            binaryForm[offset + 2] = (byte)(number >> 16);
            binaryForm[offset + 3] = (byte)(number >> 24);
        }

        //
        // Retrieves an integer stored in big-endian format at a given offset in an array
        //

        internal static int UnmarshalInt(byte[] binaryForm, int offset)
        {
            return (int)(
                (binaryForm[offset + 0] << 0) +
                (binaryForm[offset + 1] << 8) +
                (binaryForm[offset + 2] << 16) +
                (binaryForm[offset + 3] << 24));
        }

        #endregion

        #region Constructors

        protected GenericSecurityDescriptor()
        { }

        #endregion

        #region Protected Properties

        //
        // Marshaling logic requires calling into the derived
        // class to obtain pointers to SACL and DACL
        //

        internal abstract GenericAcl GenericSacl { get; }
        internal abstract GenericAcl GenericDacl { get; }
        private bool IsCraftedAefaDacl
        {
            get
            {
                return (GenericDacl is DiscretionaryAcl) && (GenericDacl as DiscretionaryAcl).EveryOneFullAccessForNullDacl;
            }
        }

        #endregion

        #region Public Properties

        public static bool IsSddlConversionSupported()
        {
            return true; // SDDL to binary conversions are supported on Windows 2000 and higher
        }

        public static byte Revision
        {
            get { return 1; }
        }

        //
        // Allows retrieving and setting the control bits for this security descriptor
        //

        public abstract ControlFlags ControlFlags { get; }

        //
        // Allows retrieving and setting the owner SID for this security descriptor
        //

        public abstract SecurityIdentifier Owner { get; set; }

        //
        // Allows retrieving and setting the group SID for this security descriptor
        //

        public abstract SecurityIdentifier Group { get; set; }

        //
        // Retrieves the length of the binary representation
        // of the security descriptor
        //

        public int BinaryLength
        {
            get
            {
                int result = HeaderLength;

                if (Owner != null)
                {
                    result += Owner.BinaryLength;
                }

                if (Group != null)
                {
                    result += Group.BinaryLength;
                }

                if ((ControlFlags & ControlFlags.SystemAclPresent) != 0 &&
                    GenericSacl != null)
                {
                    result += GenericSacl.BinaryLength;
                }

                if ((ControlFlags & ControlFlags.DiscretionaryAclPresent) != 0 &&
                    GenericDacl != null && !IsCraftedAefaDacl)
                {
                    result += GenericDacl.BinaryLength;
                }

                return result;
            }
        }

        #endregion

        #region Public Methods

        //
        // Converts the security descriptor to its SDDL form
        //

        public string GetSddlForm(AccessControlSections includeSections)
        {
            byte[] binaryForm = new byte[BinaryLength];
            string resultSddl;
            int error;

            GetBinaryForm(binaryForm, 0);

            SecurityInfos flags = 0;

            if ((includeSections & AccessControlSections.Owner) != 0)
            {
                flags |= SecurityInfos.Owner;
            }

            if ((includeSections & AccessControlSections.Group) != 0)
            {
                flags |= SecurityInfos.Group;
            }

            if ((includeSections & AccessControlSections.Audit) != 0)
            {
                flags |= SecurityInfos.SystemAcl;
            }

            if ((includeSections & AccessControlSections.Access) != 0)
            {
                flags |= SecurityInfos.DiscretionaryAcl;
            }

            error = Win32.ConvertSdToSddl(binaryForm, 1, flags, out resultSddl);

            if (error == Interop.mincore.Errors.ERROR_INVALID_PARAMETER ||
                error == Interop.mincore.Errors.ERROR_UNKNOWN_REVISION)
            {
                //
                // Indicates that the marshaling logic in GetBinaryForm is busted
                //

                Contract.Assert(false, "binaryForm produced invalid output");
                throw new InvalidOperationException();
            }
            else if (error != Interop.mincore.Errors.ERROR_SUCCESS)
            {
                Contract.Assert(false, string.Format(CultureInfo.InvariantCulture, "Win32.ConvertSdToSddl returned {0}", error));
                throw new InvalidOperationException();
            }

            return resultSddl;
        }

        //
        // Converts the security descriptor to its binary form
        //

        public void GetBinaryForm(byte[] binaryForm, int offset)
        {
            if (binaryForm == null)
            {
                throw new ArgumentNullException(nameof(binaryForm));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset),
                    SR.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (binaryForm.Length - offset < BinaryLength)
            {
                throw new ArgumentOutOfRangeException(
nameof(binaryForm),
                    SR.ArgumentOutOfRange_ArrayTooSmall);
            }
            Contract.EndContractBlock();

            //
            // the offset will grow as we go for each additional field (owner, group,
            // acl, etc) being written. But for each of such fields, we must use the
            // original offset as passed in, not the growing offset
            //

            int originalOffset = offset;

            //
            // Populate the header
            //

            int length = BinaryLength;

            byte rmControl =
                ((this is RawSecurityDescriptor) &&
                 ((ControlFlags & ControlFlags.RMControlValid) != 0)) ? ((this as RawSecurityDescriptor).ResourceManagerControl) : (byte)0;

            // if the DACL is our internally crafted NULL replacement, then let us turn off this control
            int materializedControlFlags = (int)ControlFlags;
            if (IsCraftedAefaDacl)
            {
                unchecked { materializedControlFlags &= ~((int)ControlFlags.DiscretionaryAclPresent); }
            }

            binaryForm[offset + 0] = Revision;
            binaryForm[offset + 1] = rmControl;
            binaryForm[offset + 2] = (byte)((int)materializedControlFlags >> 0);
            binaryForm[offset + 3] = (byte)((int)materializedControlFlags >> 8);

            //
            // Compute offsets at which owner, group, SACL and DACL are stored
            //

            int ownerOffset, groupOffset, saclOffset, daclOffset;

            ownerOffset = offset + OwnerFoundAt;
            groupOffset = offset + GroupFoundAt;
            saclOffset = offset + SaclFoundAt;
            daclOffset = offset + DaclFoundAt;

            offset += HeaderLength;

            //
            // Marhsal the Owner SID into place
            //

            if (Owner != null)
            {
                MarshalInt(binaryForm, ownerOffset, offset - originalOffset);
                Owner.GetBinaryForm(binaryForm, offset);
                offset += Owner.BinaryLength;
            }
            else
            {
                //
                // If Owner SID is null, store 0 in the offset field
                //

                MarshalInt(binaryForm, ownerOffset, 0);
            }

            //
            // Marshal the Group SID into place
            //

            if (Group != null)
            {
                MarshalInt(binaryForm, groupOffset, offset - originalOffset);
                Group.GetBinaryForm(binaryForm, offset);
                offset += Group.BinaryLength;
            }
            else
            {
                //
                // If Group SID is null, store 0 in the offset field
                //

                MarshalInt(binaryForm, groupOffset, 0);
            }

            //
            // Marshal the SACL into place, if present
            //

            if ((ControlFlags & ControlFlags.SystemAclPresent) != 0 &&
                GenericSacl != null)
            {
                MarshalInt(binaryForm, saclOffset, offset - originalOffset);
                GenericSacl.GetBinaryForm(binaryForm, offset);
                offset += GenericSacl.BinaryLength;
            }
            else
            {
                //
                // If SACL is null or not present, store 0 in the offset field
                //

                MarshalInt(binaryForm, saclOffset, 0);
            }

            //
            // Marshal the DACL into place, if present
            //

            if ((ControlFlags & ControlFlags.DiscretionaryAclPresent) != 0 &&
                GenericDacl != null && !IsCraftedAefaDacl)
            {
                MarshalInt(binaryForm, daclOffset, offset - originalOffset);
                GenericDacl.GetBinaryForm(binaryForm, offset);
                offset += GenericDacl.BinaryLength;
            }
            else
            {
                //
                // If DACL is null or not present, store 0 in the offset field
                //

                MarshalInt(binaryForm, daclOffset, 0);
            }
        }
        #endregion
    }


    public sealed class RawSecurityDescriptor : GenericSecurityDescriptor
    {
        #region Private Members

        private SecurityIdentifier _owner;
        private SecurityIdentifier _group;
        private ControlFlags _flags;
        private RawAcl _sacl;
        private RawAcl _dacl;
        private byte _rmControl; // the not-so-reserved SBZ1 field

        #endregion

        #region Protected Properties

        internal override GenericAcl GenericSacl
        {
            get { return _sacl; }
        }

        internal override GenericAcl GenericDacl
        {
            get { return _dacl; }
        }

        #endregion

        #region Private methods

        private void CreateFromParts(ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, RawAcl systemAcl, RawAcl discretionaryAcl)
        {
            SetFlags(flags);
            Owner = owner;
            Group = group;
            SystemAcl = systemAcl;
            DiscretionaryAcl = discretionaryAcl;
            ResourceManagerControl = 0;
        }

        #endregion

        #region Constructors

        //
        // Creates a security descriptor explicitly
        //

        public RawSecurityDescriptor(ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, RawAcl systemAcl, RawAcl discretionaryAcl)
            : base()
        {
            CreateFromParts(flags, owner, group, systemAcl, discretionaryAcl);
        }

        //
        // Creates a security descriptor from an SDDL string
        //

        public RawSecurityDescriptor(string sddlForm)
            : this(BinaryFormFromSddlForm(sddlForm), 0)
        {
        }

        //
        // Creates a security descriptor from its binary representation
        // Important: the representation must be in self-relative format
        //

        public RawSecurityDescriptor(byte[] binaryForm, int offset)
            : base()
        {
            //
            // The array passed in must be valid
            //

            if (binaryForm == null)
            {
                throw new ArgumentNullException(nameof(binaryForm));
            }

            if (offset < 0)
            {
                //
                // Offset must not be negative
                //

                throw new ArgumentOutOfRangeException(nameof(offset),
                     SR.ArgumentOutOfRange_NeedNonNegNum);
            }

            //
            // At least make sure the header is in place
            //

            if (binaryForm.Length - offset < HeaderLength)
            {
                throw new ArgumentOutOfRangeException(
nameof(binaryForm),
                     SR.ArgumentOutOfRange_ArrayTooSmall);
            }

            //
            // We only understand revision-1 security descriptors
            //

            if (binaryForm[offset + 0] != Revision)
            {
                throw new ArgumentOutOfRangeException(nameof(binaryForm),
                     SR.AccessControl_InvalidSecurityDescriptorRevision);
            }
            Contract.EndContractBlock();


            ControlFlags flags;
            SecurityIdentifier owner, group;
            RawAcl sacl, dacl;
            byte rmControl;

            //
            // Extract the ResourceManagerControl field
            //

            rmControl = binaryForm[offset + 1];

            //
            // Extract the control flags
            //

            flags = (ControlFlags)((binaryForm[offset + 2] << 0) + (binaryForm[offset + 3] << 8));

            //
            // Make sure that the input is in self-relative format
            //

            if ((flags & ControlFlags.SelfRelative) == 0)
            {
                throw new ArgumentException(
                     SR.AccessControl_InvalidSecurityDescriptorSelfRelativeForm,
nameof(binaryForm));
            }

            //
            // Extract the owner SID
            //

            int ownerOffset = UnmarshalInt(binaryForm, offset + OwnerFoundAt);

            if (ownerOffset != 0)
            {
                owner = new SecurityIdentifier(binaryForm, offset + ownerOffset);
            }
            else
            {
                owner = null;
            }

            //
            // Extract the group SID
            //

            int groupOffset = UnmarshalInt(binaryForm, offset + GroupFoundAt);

            if (groupOffset != 0)
            {
                group = new SecurityIdentifier(binaryForm, offset + groupOffset);
            }
            else
            {
                group = null;
            }

            //
            // Extract the SACL
            //

            int saclOffset = UnmarshalInt(binaryForm, offset + SaclFoundAt);

            if (((flags & ControlFlags.SystemAclPresent) != 0) &&
                saclOffset != 0)
            {
                sacl = new RawAcl(binaryForm, offset + saclOffset);
            }
            else
            {
                sacl = null;
            }

            //
            // Extract the DACL
            //

            int daclOffset = UnmarshalInt(binaryForm, offset + DaclFoundAt);

            if (((flags & ControlFlags.DiscretionaryAclPresent) != 0) &&
                daclOffset != 0)
            {
                dacl = new RawAcl(binaryForm, offset + daclOffset);
            }
            else
            {
                dacl = null;
            }

            //
            // Create the resulting security descriptor
            //

            CreateFromParts(flags, owner, group, sacl, dacl);

            //
            // In the offchance that the flags indicate that the rmControl
            // field is meaningful, remember what was there.
            //

            if ((flags & ControlFlags.RMControlValid) != 0)
            {
                ResourceManagerControl = rmControl;
            }
        }

        #endregion

        #region Static Methods

        private static byte[] BinaryFormFromSddlForm(string sddlForm)
        {
            if (sddlForm == null)
            {
                throw new ArgumentNullException(nameof(sddlForm));
            }
            Contract.EndContractBlock();

            int error;
            IntPtr byteArray = IntPtr.Zero;
            uint byteArraySize = 0;
            const System.Int32 TRUE = 1;
            byte[] binaryForm = null;

            try
            {
                if (TRUE != Interop.mincore.ConvertStringSdToSd(
                        sddlForm,
                        GenericSecurityDescriptor.Revision,
                        out byteArray,
                        ref byteArraySize))
                {
                    error = Marshal.GetLastWin32Error();

                    if (error == Interop.mincore.Errors.ERROR_INVALID_PARAMETER ||
                        error == Interop.mincore.Errors.ERROR_INVALID_ACL ||
                        error == Interop.mincore.Errors.ERROR_INVALID_SECURITY_DESCR ||
                        error == Interop.mincore.Errors.ERROR_UNKNOWN_REVISION)
                    {
                        throw new ArgumentException(
                             SR.ArgumentException_InvalidSDSddlForm,
nameof(sddlForm));
                    }
                    else if (error == Interop.mincore.Errors.ERROR_NOT_ENOUGH_MEMORY)
                    {
                        throw new OutOfMemoryException();
                    }
                    else if (error == Interop.mincore.Errors.ERROR_INVALID_SID)
                    {
                        throw new ArgumentException(
                             SR.AccessControl_InvalidSidInSDDLString,
nameof(sddlForm));
                    }
                    else if (error != Interop.mincore.Errors.ERROR_SUCCESS)
                    {
                        Contract.Assert(false, string.Format(CultureInfo.InvariantCulture, "Unexpected error out of Win32.ConvertStringSdToSd: {0}", error));
                        // TODO : This should be a Win32Exception once that type is available
                        throw new Exception();
                    }
                }

                binaryForm = new byte[byteArraySize];

                //
                // Extract the data from the returned pointer
                //

                Marshal.Copy(byteArray, binaryForm, 0, (int)byteArraySize);
            }
            finally
            {
                //
                // Now is a good time to get rid of the returned pointer
                //
                if (byteArray != IntPtr.Zero)
                {
                    Interop.mincore_obsolete.LocalFree(byteArray);
                }
            }

            return binaryForm;
        }

        #endregion

        #region Public Properties

        //
        // Allows retrieving the control bits for this security descriptor
        // Important: Special checks must be applied when setting flags and not
        // all flags can be set (for instance, we only deal with self-relative
        // security descriptors), thus flags can be set through other methods.
        //

        public override ControlFlags ControlFlags
        {
            get
            {
                return _flags;
            }
        }

        //
        // Allows retrieving and setting the owner SID for this security descriptor
        //

        public override SecurityIdentifier Owner
        {
            get
            {
                return _owner;
            }

            set
            {
                _owner = value;
            }
        }

        //
        // Allows retrieving and setting the group SID for this security descriptor
        //

        public override SecurityIdentifier Group
        {
            get
            {
                return _group;
            }

            set
            {
                _group = value;
            }
        }

        //
        // Allows retrieving and setting the SACL for this security descriptor
        //

        public RawAcl SystemAcl
        {
            get
            {
                return _sacl;
            }

            set
            {
                _sacl = value;
            }
        }

        //
        // Allows retrieving and setting the DACL for this security descriptor
        //

        public RawAcl DiscretionaryAcl
        {
            get
            {
                return _dacl;
            }

            set
            {
                _dacl = value;
            }
        }

        //
        // CORNER CASE (LEGACY)
        // The ostensibly "reserved" field in the Security Descriptor header
        // can in fact be used by obscure resource managers which in this
        // case must set the RMControlValid flag.
        //

        public byte ResourceManagerControl
        {
            get
            {
                return _rmControl;
            }

            set
            {
                _rmControl = value;
            }
        }


        #endregion

        #region Public Methods

        public void SetFlags(ControlFlags flags)
        {
            //
            // We can not deal with non-self-relative descriptors
            // so just forget about it
            //

            _flags = (flags | ControlFlags.SelfRelative);
        }
        #endregion
    }


    public sealed class CommonSecurityDescriptor : GenericSecurityDescriptor
    {
        #region Private Members

        bool _isContainer;
        bool _isDS;
        private RawSecurityDescriptor _rawSd;
        private SystemAcl _sacl;
        private DiscretionaryAcl _dacl;


        #endregion

        #region Private Methods

        private void CreateFromParts(bool isContainer, bool isDS, ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, SystemAcl systemAcl, DiscretionaryAcl discretionaryAcl)
        {
            if (systemAcl != null &&
                systemAcl.IsContainer != isContainer)
            {
                throw new ArgumentException(
                     isContainer ?
                        SR.AccessControl_MustSpecifyContainerAcl :
                        SR.AccessControl_MustSpecifyLeafObjectAcl,
nameof(systemAcl));
            }

            if (discretionaryAcl != null &&
                discretionaryAcl.IsContainer != isContainer)
            {
                throw new ArgumentException(
                     isContainer ?
                        SR.AccessControl_MustSpecifyContainerAcl :
                        SR.AccessControl_MustSpecifyLeafObjectAcl,
nameof(discretionaryAcl));
            }

            _isContainer = isContainer;

            if (systemAcl != null &&
                systemAcl.IsDS != isDS)
            {
                throw new ArgumentException(
                     isDS ?
                        SR.AccessControl_MustSpecifyDirectoryObjectAcl :
                        SR.AccessControl_MustSpecifyNonDirectoryObjectAcl,
nameof(systemAcl));
            }

            if (discretionaryAcl != null &&
                discretionaryAcl.IsDS != isDS)
            {
                throw new ArgumentException(
                    isDS ?
                        SR.AccessControl_MustSpecifyDirectoryObjectAcl :
                        SR.AccessControl_MustSpecifyNonDirectoryObjectAcl,
nameof(discretionaryAcl));
            }

            _isDS = isDS;

            _sacl = systemAcl;

            //
            // Replace null DACL with an allow-all for everyone DACL
            //

            if (discretionaryAcl == null)
            {
                //
                // to conform to native behavior, we will add allow everyone ace for DACL
                //

                discretionaryAcl = DiscretionaryAcl.CreateAllowEveryoneFullAccess(_isDS, _isContainer);
            }

            _dacl = discretionaryAcl;

            //
            // DACL is never null. So always set the flag bit on
            //

            ControlFlags actualFlags = flags | ControlFlags.DiscretionaryAclPresent;

            //
            // Keep SACL and the flag bit in sync.
            //

            if (systemAcl == null)
            {
                unchecked { actualFlags &= ~(ControlFlags.SystemAclPresent); }
            }
            else
            {
                actualFlags |= (ControlFlags.SystemAclPresent);
            }

            _rawSd = new RawSecurityDescriptor(actualFlags, owner, group, systemAcl == null ? null : systemAcl.RawAcl, discretionaryAcl.RawAcl);
        }

        #endregion

        #region Constructors

        //
        // Creates a security descriptor explicitly
        //

        public CommonSecurityDescriptor(bool isContainer, bool isDS, ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, SystemAcl systemAcl, DiscretionaryAcl discretionaryAcl)
        {
            CreateFromParts(isContainer, isDS, flags, owner, group, systemAcl, discretionaryAcl);
        }

        private CommonSecurityDescriptor(bool isContainer, bool isDS, ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, RawAcl systemAcl, RawAcl discretionaryAcl)
            : this(isContainer, isDS, flags, owner, group, systemAcl == null ? null : new SystemAcl(isContainer, isDS, systemAcl), discretionaryAcl == null ? null : new DiscretionaryAcl(isContainer, isDS, discretionaryAcl))
        {
        }

        public CommonSecurityDescriptor(bool isContainer, bool isDS, RawSecurityDescriptor rawSecurityDescriptor)
            : this(isContainer, isDS, rawSecurityDescriptor, false)
        {
        }

        internal CommonSecurityDescriptor(bool isContainer, bool isDS, RawSecurityDescriptor rawSecurityDescriptor, bool trusted)
        {
            if (rawSecurityDescriptor == null)
            {
                throw new ArgumentNullException(nameof(rawSecurityDescriptor));
            }
            Contract.EndContractBlock();

            CreateFromParts(
                isContainer,
                isDS,
                rawSecurityDescriptor.ControlFlags,
                rawSecurityDescriptor.Owner,
                rawSecurityDescriptor.Group,
                rawSecurityDescriptor.SystemAcl == null ? null : new SystemAcl(isContainer, isDS, rawSecurityDescriptor.SystemAcl, trusted),
                rawSecurityDescriptor.DiscretionaryAcl == null ? null : new DiscretionaryAcl(isContainer, isDS, rawSecurityDescriptor.DiscretionaryAcl, trusted));
        }

        //
        // Create a security descriptor from an SDDL string
        //

        public CommonSecurityDescriptor(bool isContainer, bool isDS, string sddlForm)
            : this(isContainer, isDS, new RawSecurityDescriptor(sddlForm), true)
        {
        }

        //
        // Create a security descriptor from its binary representation
        //

        public CommonSecurityDescriptor(bool isContainer, bool isDS, byte[] binaryForm, int offset)
            : this(isContainer, isDS, new RawSecurityDescriptor(binaryForm, offset), true)
        {
        }

        #endregion

        #region Protected Properties

        internal sealed override GenericAcl GenericSacl
        {
            get { return _sacl; }
        }

        internal sealed override GenericAcl GenericDacl
        {
            get { return _dacl; }
        }

        #endregion

        #region Public Properties

        public bool IsContainer
        {
            get { return _isContainer; }
        }

        public bool IsDS
        {
            get { return _isDS; }
        }


        //
        // Allows retrieving the control bits for this security descriptor
        //

        public override ControlFlags ControlFlags
        {
            get
            {
                return _rawSd.ControlFlags;
            }
        }

        //
        // Allows retrieving and setting the owner SID for this security descriptor
        //

        public override SecurityIdentifier Owner
        {
            get
            {
                return _rawSd.Owner;
            }

            set
            {
                _rawSd.Owner = value;
            }
        }

        //
        // Allows retrieving and setting the group SID for this security descriptor
        //

        public override SecurityIdentifier Group
        {
            get
            {
                return _rawSd.Group;
            }

            set
            {
                _rawSd.Group = value;
            }
        }


        public SystemAcl SystemAcl
        {
            get
            {
                return _sacl;
            }

            set
            {
                if (value != null)
                {
                    if (value.IsContainer != this.IsContainer)
                    {
                        throw new ArgumentException(
                             this.IsContainer ?
                                SR.AccessControl_MustSpecifyContainerAcl :
                                SR.AccessControl_MustSpecifyLeafObjectAcl,
nameof(value));
                    }

                    if (value.IsDS != this.IsDS)
                    {
                        throw new ArgumentException(
                            this.IsDS ?
                                SR.AccessControl_MustSpecifyDirectoryObjectAcl :
                                SR.AccessControl_MustSpecifyNonDirectoryObjectAcl,
nameof(value));
                    }
                }

                _sacl = value;

                if (_sacl != null)
                {
                    _rawSd.SystemAcl = _sacl.RawAcl;
                    AddControlFlags(ControlFlags.SystemAclPresent);
                }
                else
                {
                    _rawSd.SystemAcl = null;
                    RemoveControlFlags(ControlFlags.SystemAclPresent);
                }
            }
        }

        //
        // Allows retrieving and setting the DACL for this security descriptor
        //

        public DiscretionaryAcl DiscretionaryAcl
        {
            get
            {
                return _dacl;
            }

            set
            {
                if (value != null)
                {
                    if (value.IsContainer != this.IsContainer)
                    {
                        throw new ArgumentException(
                             this.IsContainer ?
                                SR.AccessControl_MustSpecifyContainerAcl :
                                SR.AccessControl_MustSpecifyLeafObjectAcl,
nameof(value));
                    }

                    if (value.IsDS != this.IsDS)
                    {
                        throw new ArgumentException(
                             this.IsDS ?
                                SR.AccessControl_MustSpecifyDirectoryObjectAcl :
                                SR.AccessControl_MustSpecifyNonDirectoryObjectAcl,
nameof(value));
                    }
                }

                //
                // NULL DACLs are replaced with allow everyone full access DACLs.
                //

                if (value == null)
                {
                    _dacl = DiscretionaryAcl.CreateAllowEveryoneFullAccess(IsDS, IsContainer);
                }
                else
                {
                    _dacl = value;
                }

                _rawSd.DiscretionaryAcl = _dacl.RawAcl;
                AddControlFlags(ControlFlags.DiscretionaryAclPresent);
            }
        }

        public bool IsSystemAclCanonical
        {
            get { return (SystemAcl == null || SystemAcl.IsCanonical); }
        }

        public bool IsDiscretionaryAclCanonical
        {
            get { return (DiscretionaryAcl == null || DiscretionaryAcl.IsCanonical); }
        }

        #endregion

        #region Public Methods

        public void SetSystemAclProtection(bool isProtected, bool preserveInheritance)
        {
            if (!isProtected)
            {
                RemoveControlFlags(ControlFlags.SystemAclProtected);
            }
            else
            {
                if (!preserveInheritance && SystemAcl != null)
                {
                    SystemAcl.RemoveInheritedAces();
                }

                AddControlFlags(ControlFlags.SystemAclProtected);
            }
        }

        public void SetDiscretionaryAclProtection(bool isProtected, bool preserveInheritance)
        {
            if (!isProtected)
            {
                RemoveControlFlags(ControlFlags.DiscretionaryAclProtected);
            }
            else
            {
                if (!preserveInheritance && DiscretionaryAcl != null)
                {
                    DiscretionaryAcl.RemoveInheritedAces();
                }

                AddControlFlags(ControlFlags.DiscretionaryAclProtected);
            }
            if (DiscretionaryAcl != null && DiscretionaryAcl.EveryOneFullAccessForNullDacl)
            {
                DiscretionaryAcl.EveryOneFullAccessForNullDacl = false;
            }
        }

        public void PurgeAccessControl(SecurityIdentifier sid)
        {
            if (sid == null)
            {
                throw new ArgumentNullException(nameof(sid));
            }
            Contract.EndContractBlock();

            if (DiscretionaryAcl != null)
            {
                DiscretionaryAcl.Purge(sid);
            }
        }

        public void PurgeAudit(SecurityIdentifier sid)
        {
            if (sid == null)
            {
                throw new ArgumentNullException(nameof(sid));
            }
            Contract.EndContractBlock();

            if (SystemAcl != null)
            {
                SystemAcl.Purge(sid);
            }
        }

        public void AddDiscretionaryAcl(byte revision, int trusted)
        {
            this.DiscretionaryAcl = new DiscretionaryAcl(this.IsContainer, this.IsDS, revision, trusted);
            this.AddControlFlags(ControlFlags.DiscretionaryAclPresent);
        }

        public void AddSystemAcl(byte revision, int trusted)
        {
            this.SystemAcl = new SystemAcl(this.IsContainer, this.IsDS, revision, trusted);
            this.AddControlFlags(ControlFlags.SystemAclPresent);
        }

        #endregion

        #region internal Methods
        internal void UpdateControlFlags(ControlFlags flagsToUpdate, ControlFlags newFlags)
        {
            ControlFlags finalFlags = newFlags | (_rawSd.ControlFlags & (~flagsToUpdate));
            _rawSd.SetFlags(finalFlags);
        }

        //
        // These two add/remove method must be called with great care (and thus it is internal)
        // The caller is responsible for keeping the SaclPresent and DaclPresent bits in sync
        // with the actual SACL and DACL.
        //

        internal void AddControlFlags(ControlFlags flags)
        {
            _rawSd.SetFlags(_rawSd.ControlFlags | flags);
        }

        internal void RemoveControlFlags(ControlFlags flags)
        {
            unchecked
            {
                _rawSd.SetFlags(_rawSd.ControlFlags & ~flags);
            }
        }

        internal bool IsSystemAclPresent
        {
            get
            {
                return (_rawSd.ControlFlags & ControlFlags.SystemAclPresent) != 0;
            }
        }

        internal bool IsDiscretionaryAclPresent
        {
            get
            {
                return (_rawSd.ControlFlags & ControlFlags.DiscretionaryAclPresent) != 0;
            }
        }
        #endregion
    }
}
