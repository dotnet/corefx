// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        private const int MountPointFormatBufferSizeInBytes = 32;

        /// <summary>
        /// Internal FileSystem names and magic numbers taken from man(2) statfs
        /// </summary>
        /// <remarks>
        /// These value names MUST be kept in sync with those in GetDriveType below
        /// </remarks>
        internal enum UnixFileSystemTypes : long
        {
            adfs = 0XADF5,
            affs = 0XADFF,
            afs = 0X5346414F,
            anon-inode = 0X09041934,
            aufs = 0X61756673,
            autofs = 0X0187,
            befs = 0X42465331,
            bdevfs = 0X62646576,
            bfs = 0X1BADFACE,
            binfmt_misc = 0X42494E4D,
            btrfs = 0X9123683E,
            ceph = 0X00C36400,
            cgroupfs = 0X0027E0EB,
            cifs = 0XFF534D42,
            coda = 0X73757245,
            coherent = 0X012FF7B7,
            cramfs = 0X28CD3D45,
            debugfs = 0X64626720,
            devfs = 0X1373,
            devpts = 0X1CD1,
            ecryptfs = 0XF15F,
            efs = 0X00414A53,
            ext = 0X137D,
            ext2_old = 0XEF51,
            ext2 = 0XEF53,
            ext3 = 0XEF53,
            ext4 = 0XEF53,
            fat = 0X4006,
            fhgfs = 0X19830326,
            fuseblk = 0X65735546,
            fusectl = 0X65735543,
            futexfs = 0X0BAD1DEA,
            gfs/gfs2 = 0X1161970,
            gpfs = 0X47504653,
            hfs = 0X4244,
            hpfs = 0XF995E849,
            hugetlbfs = 0X958458F6,
            inodefs = 0X11307854,
            inotifyfs = 0X2BAD1DEA,
            isofs = 0X9660,
            isofs = 0X4004, // R_WIN
            isofs = 0X4000, // WIN
            jffs = 0X07C0,
            jffs2 = 0X72B6,
            jfs = 0X3153464A,
            k-afs = 0X6B414653,
            lustre = 0X0BD00BD0,
            minix_old = 0X137F, /* orig. minix */
            minix = 0X138F, /* 30 char minix */
            minix2 = 0X2468, /* minix V2 */
            minix2v2 = 0X2478, /* MINIX V2, 30 char names */
            minix3 = 0X4D5A,
            mqueue = 0X19800202,
            msdos = 0X4D44,
            nfs = 0X6969,
            nfsd = 0X6E667364,
            nilfs = 0X3434,
            novell = 0X564C,
            ntfs = 0X5346544E,
            openprom = 0X9FA1,
            ocfs2 = 0X7461636F,
            overlay = 0X794C7630,
            overlayfs = 0X794C764F,
            panfs = 0XAAD7AAEA,
            pipefs = 0X50495045,
            proc = 0X9FA0,
            pstorefs = 0X6165676C,
            qnx4 = 0X002F,
            qnx6 = 0X68191122,
            ramfs = 0X858458F6,
            reiserfs = 0X52654973,
            romfs = 0X7275,
            rpc_pipefs = 0X67596969,
            securityfs = 0X73636673,
            selinux = 0XF97CFF8C,
            smb = 0X517B,
            sockfs = 0X534F434B,
            squashfs = 0X73717368,
            sysfs = 0X62656572,
            sysv2 = 0X012FF7B6,
            sysv4 = 0X012FF7B5,
            tmpfs = 0X01021994,
            udf = 0X15013346,
            ufs = 0X00011954,
            ufs = 0X54190100, // byteswapped
            usbdevice = 0X9FA2,
            v9fs = 0X01021997,
            vmhgfs = 0XBACBACBC,
            vxfs = 0XA501FCF5,
            vzfs = 0X565A4653,
            xenfs = 0XABBA1974,
            xenix = 0X012FF7B4,
            xfs = 0X58465342,
            xia = 0X012FD16D,
            zfs = 0X2FC12FC1,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MountPointInformation
        {
            internal ulong AvailableFreeSpace;
            internal ulong TotalFreeSpace;
            internal ulong TotalSize;
        }

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetSpaceInfoForMountPoint", SetLastError = true)]
        internal static extern int GetSpaceInfoForMountPoint([MarshalAs(UnmanagedType.LPStr)]string name, out MountPointInformation mpi);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetFormatInfoForMountPoint", SetLastError = true)]
        private static extern unsafe int GetFormatInfoForMountPoint(
            [MarshalAs(UnmanagedType.LPStr)]string name,
            byte* formatNameBuffer,
            int bufferLength,
            long* formatType);

        internal static int GetFormatInfoForMountPoint(string name, out string format)
        {
            DriveType temp;
            return GetFormatInfoForMountPoint(name, out format, out temp);
        }

        internal static int GetFormatInfoForMountPoint(string name, out DriveType type)
        {
            string temp;
            return GetFormatInfoForMountPoint(name, out temp, out type);
        }

        private static int GetFormatInfoForMountPoint(string name, out string format, out DriveType type)
        {
            unsafe
            {
                byte* formatBuffer = stackalloc byte[MountPointFormatBufferSizeInBytes];    // format names should be small
                long numericFormat;
                int result = GetFormatInfoForMountPoint(name, formatBuffer, MountPointFormatBufferSizeInBytes, &numericFormat);
                if (result == 0)
                {
                    // Check if we have a numeric answer or string
                    format = numericFormat != -1 ?
                        Enum.GetName(typeof(UnixFileSystemTypes), numericFormat) :
                        Marshal.PtrToStringAnsi((IntPtr)formatBuffer);
                    type = GetDriveType(format);
                }
                else
                {
                    format = string.Empty;
                    type = DriveType.Unknown;
                }

                return result;
            }
        }

        /// <summary>Categorizes a file system name into a drive type.</summary>
        /// <param name="fileSystemName">The name to categorize.</param>
        /// <returns>The recognized drive type.</returns>
        private static DriveType GetDriveType(string fileSystemName)
        {
            // This list is based primarily on "man fs", "man mount", "mntent.h", "/proc/filesystems",
            // and "wiki.debian.org/FileSystem". It can be extended over time as we 
            // find additional file systems that should be recognized as a particular drive type.
            switch (fileSystemName)
            {
                case "iso":
                case "isofs":
                case "iso9660":
                case "fuseiso":
                case "fuseiso9660":
                case "umview-mod-umfuseiso9660":
                    return DriveType.CDRom;

                case "adfs":
                case "affs":
                case "apfs":
                case "befs":
                case "bfs":
                case "btrfs":
                case "drvfs":
                case "ecryptfs":
                case "efs":
                case "ext":
                case "ext2":
                case "ext2_old":
                case "ext3":
                case "ext4":
                case "ext4dev":
                case "fat":
                case "fuseblk":
                case "fuseext2":
                case "fusefat":
                case "hfs":
                case "hfsplus":
                case "hpfs":
                case "jbd":
                case "jbd2":
                case "jfs":
                case "jffs":
                case "jffs2":
                case "lxfs":
                case "minix":
                case "minix_old":
                case "minix2":
                case "minix2v2":
                case "msdos":
                case "ocfs2":
                case "omfs":
                case "openprom":
                case "overlay":
                case "overlayfs":
                case "ntfs":
                case "qnx4":
                case "reiserfs":
                case "squashfs":
                case "swap":
                case "sysv":
                case "ubifs":
                case "udf":
                case "ufs":
                case "umsdos":
                case "umview-mod-umfuseext2":
                case "xenix":
                case "xfs":
                case "xiafs":
                case "xmount":
                case "zfs-fuse":
                    return DriveType.Fixed;

                case "9p":
                case "autofs":
                case "autofs4":
                case "beaglefs":
                case "cifs":
                case "coda":
                case "coherent":
                case "curlftpfs":
                case "davfs2":
                case "dlm":
                case "flickrfs":
                case "fusedav":
                case "fusesmb":
                case "gfs2":
                case "glusterfs-client":
                case "gmailfs":
                case "kafs":
                case "ltspfs":
                case "ncpfs":
                case "nfs":
                case "nfs4":
                case "obexfs":
                case "s3ql":
                case "smb":
                case "smbfs":
                case "sshfs":
                case "sysfs":
                case "sysv2":
                case "sysv4":
                case "vxfs":
                case "wikipediafs":
                    return DriveType.Network;

                case "anon_inodefs":
                case "aptfs":
                case "avfs":
                case "bdev":
                case "binfmt_misc":
                case "cgroup":
                case "configfs":
                case "cramfs":
                case "cryptkeeper":
                case "cpuset":
                case "debugfs":
                case "devfs":
                case "devpts":
                case "devtmpfs":
                case "encfs":
                case "fuse":
                case "fuse.gvfsd-fuse":
                case "fusectl":
                case "hugetlbfs":
                case "libpam-encfs":
                case "ibpam-mount":
                case "mtpfs":
                case "mythtvfs":
                case "mqueue":
                case "pipefs":
                case "plptools":
                case "proc":
                case "pstore":
                case "pytagsfs":
                case "ramfs":
                case "rofs":
                case "romfs":
                case "rootfs":
                case "securityfs":
                case "sockfs":
                case "tmpfs":
                    return DriveType.Ram;

                case "gphotofs":
                case "usbfs":
                case "usbdevice":
                case "vfat":
                    return DriveType.Removable;

                    // Categorize as "Unknown" everything else not explicitly
                    // recognized as a particular drive type.
                default:
                    return DriveType.Unknown;
            }
        }
    }
}
