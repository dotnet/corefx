// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.IO
{
    public abstract partial class FileSystemInfo : MarshalByRefObject, ISerializable
    {
        // FullPath and OriginalPath are documented fields
        protected string FullPath;          // fully qualified path of the file or directory
        protected string OriginalPath;      // path passed in by the user

        internal string _name;

        protected FileSystemInfo(SerializationInfo info, StreamingContext context)
        {
            throw new PlatformNotSupportedException();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new PlatformNotSupportedException();
        }

        // Full path of the directory/file
        public virtual string FullName => FullPath;

        public string Extension
        {
            get
            {
                // GetFullPathInternal would have already stripped out the terminating "." if present.
                int length = FullPath.Length;
                for (int i = length; --i >= 0;)
                {
                    char ch = FullPath[i];
                    if (ch == '.')
                        return FullPath.Substring(i, length - i);
                    if (PathInternal.IsDirectorySeparator(ch) || ch == Path.VolumeSeparatorChar)
                        break;
                }
                return string.Empty;
            }
        }

        public virtual string Name => _name;

        // Whether a file/directory exists
        public virtual bool Exists
        {
            get
            {
                try
                {
                    return ExistsCore;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Delete a file/directory
        public abstract void Delete();

        public DateTime CreationTime
        {
            get
            {
                // depends on the security check in get_CreationTimeUtc
                return CreationTimeUtc.ToLocalTime();
            }
            set
            {
                CreationTimeUtc = value.ToUniversalTime();
            }
        }

        public DateTime CreationTimeUtc
        {
            get
            {
                return CreationTimeCore.UtcDateTime;
            }
            set
            {
                CreationTimeCore = File.GetUtcDateTimeOffset(value);
            }
        }


        public DateTime LastAccessTime
        {
            get
            {
                return LastAccessTimeUtc.ToLocalTime();
            }
            set
            {
                LastAccessTimeUtc = value.ToUniversalTime();
            }
        }

        public DateTime LastAccessTimeUtc
        {
            get
            {
                return LastAccessTimeCore.UtcDateTime;
            }
            set
            {
                LastAccessTimeCore = File.GetUtcDateTimeOffset(value);
            }
        }

        public DateTime LastWriteTime
        {
            get
            {
                return LastWriteTimeUtc.ToLocalTime();
            }
            set
            {
                LastWriteTimeUtc = value.ToUniversalTime();
            }
        }

        public DateTime LastWriteTimeUtc
        {
            get
            {
                return LastWriteTimeCore.UtcDateTime;
            }
            set
            {
                LastWriteTimeCore = File.GetUtcDateTimeOffset(value);
            }
        }

        /// <summary>
        /// Returns the original path. Use FullName or Name properties for the full path or file/directory name.
        /// </summary>
        public override string ToString() => OriginalPath ?? string.Empty;
    }
}
