// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Security;

namespace System.IO
{
    public sealed partial class DriveInfo
    {
        public static DriveInfo[] GetDrives()
        {
            List<string> mountPoints = Interop.Sys.GetAllMountPoints();
            DriveInfo[] info = new DriveInfo[mountPoints.Count];
            for (int i = 0; i < info.Length; i++)
            {
                info[i] = new DriveInfo(mountPoints[i]);
            }

            return info;
        }

        private static string NormalizeDriveName(string driveName)
        {
            if (driveName.Contains("\0"))
            {
                throw new ArgumentException(SR.Format(SR.Arg_InvalidDriveChars, driveName), "driveName");
            }
            if (driveName.Length == 0)
            {
                throw new ArgumentException(SR.Arg_MustBeNonEmptyDriveName, "driveName");
            }
            return driveName;
        }

        public DriveType DriveType
        {
            [SecuritySafeCritical]
            get
            {
                DriveType type;
                int result = Interop.Sys.GetFormatInfoForMountPoint(Name, out type);
                if (result == 0)
                {
                    return type;
                }
                else
                {
                    Interop.ErrorInfo errorInfo = Interop.Sys.GetLastErrorInfo();

                    // This is one of the few properties that doesn't throw on failure,
                    // instead returning a value from the enum.
                    switch (errorInfo.Error)
                    {
                        case Interop.Error.ELOOP:
                        case Interop.Error.ENAMETOOLONG:
                        case Interop.Error.ENOENT:
                        case Interop.Error.ENOTDIR:
                            return DriveType.NoRootDirectory;
                        default:
                            return DriveType.Unknown;
                    }
                }
            }
        }

        public string DriveFormat
        {
            [SecuritySafeCritical]
            get
            {
                string format = string.Empty;
                CheckStatfsResultAndThrowIfNecessary(Interop.Sys.GetFormatInfoForMountPoint(Name, out format));
                return format;
            }
        }

        public long AvailableFreeSpace
        {
            [SecuritySafeCritical]
            get
            {
                Interop.Sys.MountPointInformation mpi = default(Interop.Sys.MountPointInformation);
                CheckStatfsResultAndThrowIfNecessary(Interop.Sys.GetSpaceInfoForMountPoint(Name, out mpi));
                return mpi.AvailableFreeSpace;
            }
        }

        public long TotalFreeSpace
        {
            [SecuritySafeCritical]
            get
            {
                Interop.Sys.MountPointInformation mpi = default(Interop.Sys.MountPointInformation);
                CheckStatfsResultAndThrowIfNecessary(Interop.Sys.GetSpaceInfoForMountPoint(Name, out mpi));
                return mpi.TotalFreeSpace;
            } 
        }

        public long TotalSize
        {
            [SecuritySafeCritical]
            get
            {
                Interop.Sys.MountPointInformation mpi = default(Interop.Sys.MountPointInformation);
                CheckStatfsResultAndThrowIfNecessary(Interop.Sys.GetSpaceInfoForMountPoint(Name, out mpi));
                return mpi.TotalSize;
            }
        }

        public String VolumeLabel
        {
            [SecuritySafeCritical]
            get
            {
                return Name;
            }
            [SecuritySafeCritical]
            set
            {
                throw new PlatformNotSupportedException();
            }
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------

        private void CheckStatfsResultAndThrowIfNecessary(int result)
        {
            if (result != 0)
            {
                var errorInfo = Interop.Sys.GetLastErrorInfo();
                if (errorInfo.Error == Interop.Error.ENOENT)
                {
                    throw new DriveNotFoundException(SR.Format(SR.IO_DriveNotFound_Drive, Name)); // match Win32
                }
                else
                {
                    throw Interop.GetExceptionForIoErrno(errorInfo, isDirectory: true);
                }
            }
        }
    }
}
