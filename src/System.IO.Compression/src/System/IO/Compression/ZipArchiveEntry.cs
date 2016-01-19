// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;

namespace System.IO.Compression
{
    //The disposable fields that this class owns get disposed when the ZipArchive it belongs to gets disposed
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public partial class ZipArchiveEntry
    {
        #region Fields

        private const UInt16 DefaultVersionToExtract = 10;

        //The maximum index of our buffers, from the maximum index of a byte array
        private const int MaxSingleBufferSize = 0x7FFFFFC7;

        private ZipArchive _archive;
        private readonly Boolean _originallyInArchive;
        private readonly Int32 _diskNumberStart;
        private readonly ZipVersionMadeByPlatform _versionMadeByPlatform;
        private readonly byte _versionMadeBySpecification;
        private ZipVersionNeededValues _versionToExtract;
        private BitFlagValues _generalPurposeBitFlag;
        private CompressionMethodValues _storedCompressionMethod;
        private DateTimeOffset _lastModified;
        private Int64 _compressedSize;
        private Int64 _uncompressedSize;
        private Int64 _offsetOfLocalHeader;
        private Int64? _storedOffsetOfCompressedData;
        private UInt32 _crc32;
        //An array of buffers, each a maximum of MaxSingleBufferSize in size
        private Byte[][] _compressedBytes;
        private MemoryStream _storedUncompressedData;
        private Boolean _currentlyOpenForWrite;
        private Boolean _everOpenedForWrite;
        private Stream _outstandingWriteStream;
        private String _storedEntryName;
        private Byte[] _storedEntryNameBytes;
        //only apply to update mode
        private List<ZipGenericExtraField> _cdUnknownExtraFields;
        private List<ZipGenericExtraField> _lhUnknownExtraFields;
        private Byte[] _fileComment;
        private CompressionLevel? _compressionLevel;

        #endregion Fields

        //Initializes, attaches it to archive
        internal ZipArchiveEntry(ZipArchive archive, ZipCentralDirectoryFileHeader cd, CompressionLevel compressionLevel)

            : this(archive, cd)
        {
            _compressionLevel = compressionLevel;
        }

        //Initializes, attaches it to archive
        internal ZipArchiveEntry(ZipArchive archive, ZipCentralDirectoryFileHeader cd)
        {
            _archive = archive;

            _originallyInArchive = true;

            _diskNumberStart = cd.DiskNumberStart;
            _versionMadeByPlatform = (ZipVersionMadeByPlatform)cd.VersionMadeByCompatibility;
            _versionMadeBySpecification = cd.VersionMadeBySpecification;
            _versionToExtract = (ZipVersionNeededValues)cd.VersionNeededToExtract;
            _generalPurposeBitFlag = (BitFlagValues)cd.GeneralPurposeBitFlag;
            CompressionMethod = (CompressionMethodValues)cd.CompressionMethod;
            _lastModified = new DateTimeOffset(ZipHelper.DosTimeToDateTime(cd.LastModified));
            _compressedSize = cd.CompressedSize;
            _uncompressedSize = cd.UncompressedSize;
            _offsetOfLocalHeader = cd.RelativeOffsetOfLocalHeader;
            /* we don't know this yet: should be _offsetOfLocalHeader + 30 + _storedEntryNameBytes.Length + extrafieldlength
                * but entryname/extra length could be different in LH
                */
            _storedOffsetOfCompressedData = null;
            _crc32 = cd.Crc32;

            _compressedBytes = null;
            _storedUncompressedData = null;
            _currentlyOpenForWrite = false;
            _everOpenedForWrite = false;
            _outstandingWriteStream = null;

            FullName = DecodeEntryName(cd.Filename);

            _lhUnknownExtraFields = null;
            //the cd should have these as null if we aren't in Update mode
            _cdUnknownExtraFields = cd.ExtraFields;
            _fileComment = cd.FileComment;

            _compressionLevel = null;
        }

        //Initializes new entry
        internal ZipArchiveEntry(ZipArchive archive, String entryName, CompressionLevel compressionLevel)

            : this(archive, entryName)
        {
            _compressionLevel = compressionLevel;
        }

        //Initializes new entry
        internal ZipArchiveEntry(ZipArchive archive, String entryName)
        {
            _archive = archive;

            _originallyInArchive = false;

            _diskNumberStart = 0;
            _versionMadeByPlatform = CurrentZipPlatform;
            _versionMadeBySpecification = 0;
            _versionToExtract = ZipVersionNeededValues.Default; //this must happen before following two assignment
            _generalPurposeBitFlag = 0;
            CompressionMethod = CompressionMethodValues.Deflate;
            _lastModified = DateTimeOffset.Now;

            _compressedSize = 0; //we don't know these yet
            _uncompressedSize = 0;
            _offsetOfLocalHeader = 0;
            _storedOffsetOfCompressedData = null;
            _crc32 = 0;

            _compressedBytes = null;
            _storedUncompressedData = null;
            _currentlyOpenForWrite = false;
            _everOpenedForWrite = false;
            _outstandingWriteStream = null;

            FullName = entryName;

            _cdUnknownExtraFields = null;
            _lhUnknownExtraFields = null;
            _fileComment = null;

            _compressionLevel = null;

            if (_storedEntryNameBytes.Length > UInt16.MaxValue)
                throw new ArgumentException(SR.EntryNamesTooLong);

            //grab the stream if we're in create mode
            if (_archive.Mode == ZipArchiveMode.Create)
            {
                _archive.AcquireArchiveStream(this);
            }
        }

        /// <summary>
        /// The ZipArchive that this entry belongs to. If this entry has been deleted, this will return null.
        /// </summary>
        public ZipArchive Archive { get { return _archive; } }

        /// <summary>
        /// The compressed size of the entry. If the archive that the entry belongs to is in Create mode, attempts to get this property will always throw an exception. If the archive that the entry belongs to is in update mode, this property will only be valid if the entry has not been opened. 
        /// </summary>
        /// <exception cref="InvalidOperationException">This property is not available because the entry has been written to or modified.</exception>
        public Int64 CompressedLength
        {
            get
            {
                Contract.Ensures(Contract.Result<Int64>() >= 0);

                if (_everOpenedForWrite)
                    throw new InvalidOperationException(SR.LengthAfterWrite);
                return _compressedSize;
            }
        }

        /// <summary>
        /// The relative path of the entry as stored in the Zip archive. Note that Zip archives allow any string to be the path of the entry, including invalid and absolute paths.
        /// </summary>
        public String FullName
        {
            get
            {
                Contract.Ensures(Contract.Result<String>() != null);
                return _storedEntryName;
            }

            private set
            {
                if (value == null)
                    throw new ArgumentNullException("FullName");

                bool isUTF8;
                _storedEntryNameBytes = EncodeEntryName(value, out isUTF8);
                _storedEntryName = value;

                if (isUTF8)
                    _generalPurposeBitFlag |= BitFlagValues.UnicodeFileName;
                else
                    _generalPurposeBitFlag &= ~BitFlagValues.UnicodeFileName;

                if (ParseFileName(value, _versionMadeByPlatform) == "")
                    VersionToExtractAtLeast(ZipVersionNeededValues.ExplicitDirectory);
            }
        }

        /// <summary>
        /// The last write time of the entry as stored in the Zip archive. When setting this property, the DateTime will be converted to the
        /// Zip timestamp format, which supports a resolution of two seconds. If the data in the last write time field is not a valid Zip timestamp,
        /// an indicator value of 1980 January 1 at midnight will be returned.
        /// </summary>
        /// <exception cref="NotSupportedException">An attempt to set this property was made, but the ZipArchive that this entry belongs to was
        /// opened in read-only mode.</exception>
        /// <exception cref="ArgumentOutOfRangeException">An attempt was made to set this property to a value that cannot be represented in the
        /// Zip timestamp format. The earliest date/time that can be represented is 1980 January 1 0:00:00 (midnight), and the last date/time
        /// that can be represented is 2107 December 31 23:59:58 (one second before midnight).</exception>
        public DateTimeOffset LastWriteTime
        {
            get
            {
                return _lastModified;
            }
            set
            {
                ThrowIfInvalidArchive();
                if (_archive.Mode == ZipArchiveMode.Read)
                    throw new NotSupportedException(SR.ReadOnlyArchive);
                if (_archive.Mode == ZipArchiveMode.Create && _everOpenedForWrite)
                    throw new IOException(SR.FrozenAfterWrite);
                if (value.DateTime.Year < ZipHelper.ValidZipDate_YearMin || value.DateTime.Year > ZipHelper.ValidZipDate_YearMax)
                    throw new ArgumentOutOfRangeException("value", SR.DateTimeOutOfRange);

                _lastModified = value;
            }
        }

        /// <summary>
        /// The uncompressed size of the entry. This property is not valid in Create mode, and it is only valid in Update mode if the entry has not been opened.
        /// </summary>
        /// <exception cref="InvalidOperationException">This property is not available because the entry has been written to or modified.</exception>
        public Int64 Length
        {
            get
            {
                Contract.Ensures(Contract.Result<Int64>() >= 0);

                if (_everOpenedForWrite)
                    throw new InvalidOperationException(SR.LengthAfterWrite);
                return _uncompressedSize;
            }
        }

        /// <summary>
        /// The filename of the entry. This is equivalent to the substring of Fullname that follows the final directory separator character.
        /// </summary>
        public String Name
        {
            get
            {
                return ParseFileName(FullName, _versionMadeByPlatform);
            }
        }

        /// <summary>
        /// Deletes the entry from the archive.
        /// </summary>
        /// <exception cref="IOException">The entry is already open for reading or writing.</exception>
        /// <exception cref="NotSupportedException">The ZipArchive that this entry belongs to was opened in a mode other than ZipArchiveMode.Update. </exception>
        /// <exception cref="ObjectDisposedException">The ZipArchive that this entry belongs to has been disposed.</exception>
        public void Delete()
        {
            if (_archive == null)
                return;

            if (_currentlyOpenForWrite)
                throw new IOException(SR.DeleteOpenEntry);

            if (_archive.Mode != ZipArchiveMode.Update)
                throw new NotSupportedException(SR.DeleteOnlyInUpdate);

            _archive.ThrowIfDisposed();

            _archive.RemoveEntry(this);
            _archive = null;
            UnloadStreams();
        }

        /// <summary>
        /// Opens the entry. If the archive that the entry belongs to was opened in Read mode, the returned stream will be readable, and it may or may not be seekable. If Create mode, the returned stream will be writeable and not seekable. If Update mode, the returned stream will be readable, writeable, seekable, and support SetLength.
        /// </summary>
        /// <returns>A Stream that represents the contents of the entry.</returns>
        /// <exception cref="IOException">The entry is already currently open for writing. -or- The entry has been deleted from the archive. -or- The archive that this entry belongs to was opened in ZipArchiveMode.Create, and this entry has already been written to once.</exception>
        /// <exception cref="InvalidDataException">The entry is missing from the archive or is corrupt and cannot be read. -or- The entry has been compressed using a compression method that is not supported.</exception>
        /// <exception cref="ObjectDisposedException">The ZipArchive that this entry belongs to has been disposed.</exception>
        public Stream Open()
        {
            Contract.Ensures(Contract.Result<Stream>() != null);

            ThrowIfInvalidArchive();

            switch (_archive.Mode)
            {
                case ZipArchiveMode.Read:
                    return OpenInReadMode(true);
                case ZipArchiveMode.Create:
                    return OpenInWriteMode();
                case ZipArchiveMode.Update:
                default:
                    Debug.Assert(_archive.Mode == ZipArchiveMode.Update);
                    return OpenInUpdateMode();
            }
        }

        /// <summary>
        /// Returns the FullName of the entry.
        /// </summary>
        /// <returns>FullName of the entry</returns>
        public override String ToString()
        {
            Contract.Ensures(Contract.Result<String>() != null);

            return FullName;
        }

        /*
        public void MoveTo(String destinationEntryName)
        {
            if (destinationEntryName == null)
                throw new ArgumentNullException("destinationEntryName");

            if (String.IsNullOrEmpty(destinationEntryName))
                throw new ArgumentException("destinationEntryName cannot be empty", "destinationEntryName");

            if (_archive == null)
                throw new InvalidOperationException("Attempt to move a deleted entry");

            if (_archive._isDisposed)
                throw new ObjectDisposedException(_archive.ToString());

            if (_archive.Mode != ZipArchiveMode.Update)
                throw new NotSupportedException("MoveTo can only be used when the archive is in Update mode");

            String oldFilename = _filename;
            _filename = destinationEntryName;
            if (_filenameLength > UInt16.MaxValue)
            {
                _filename = oldFilename;
                throw new ArgumentException("Archive entry names must be smaller than 2^16 bytes");
            }
        }
        */

        #region Privates

        // Only allow opening ZipArchives with large ZipArchiveEntries in update mode when running in a 64-bit process.
        // This is for compatibility with old behavior that threw an exception for all process bitnesses, because this
        // will not work in a 32-bit process.
        private static readonly bool s_allowLargeZipArchiveEntriesInUpdateMode = IntPtr.Size > 4;

        internal Boolean EverOpenedForWrite { get { return _everOpenedForWrite; } }

        private Int64 OffsetOfCompressedData
        {
            get
            {
                if (_storedOffsetOfCompressedData == null)
                {
                    _archive.ArchiveStream.Seek(_offsetOfLocalHeader, SeekOrigin.Begin);
                    //by calling this, we are using local header _storedEntryNameBytes.Length and extraFieldLength
                    //to find start of data, but still using central directory size information
                    if (!ZipLocalFileHeader.TrySkipBlock(_archive.ArchiveReader))
                        throw new InvalidDataException(SR.LocalFileHeaderCorrupt);
                    _storedOffsetOfCompressedData = _archive.ArchiveStream.Position;
                }
                return _storedOffsetOfCompressedData.Value;
            }
        }

        private MemoryStream UncompressedData
        {
            get
            {
                if (_storedUncompressedData == null)
                {
                    //this means we have never opened it before

                    //if _uncompressedSize > Int32.MaxValue, it's still okay, because MemoryStream will just
                    //grow as data is copied into it
                    _storedUncompressedData = new MemoryStream((Int32)_uncompressedSize);

                    if (_originallyInArchive)
                    {
                        using (Stream decompressor = OpenInReadMode(false))
                        {
                            try
                            {
                                decompressor.CopyTo(_storedUncompressedData);
                            }
                            catch (InvalidDataException)
                            {
                                /* this is the case where the archive say the entry is deflate, but deflateStream
                                 * throws an InvalidDataException. This property should only be getting accessed in
                                 * Update mode, so we want to make sure _storedUncompressedData stays null so
                                 * that later when we dispose the archive, this entry loads the compressedBytes, and
                                 * copies them straight over
                                 */
                                _storedUncompressedData.Dispose();
                                _storedUncompressedData = null;
                                _currentlyOpenForWrite = false;
                                _everOpenedForWrite = false;
                                throw;
                            }
                        }
                    }

                    //if they start modifying it, we should make sure it will get deflated
                    CompressionMethod = CompressionMethodValues.Deflate;
                }

                return _storedUncompressedData;
            }
        }

        private CompressionMethodValues CompressionMethod
        {
            get { return _storedCompressionMethod; }
            set
            {
                if (value == CompressionMethodValues.Deflate)
                    VersionToExtractAtLeast(ZipVersionNeededValues.Deflate);
                _storedCompressionMethod = value;
            }
        }

        private Encoding DefaultSystemEncoding
        {
            // On the desktop, this was Encoding.GetEncoding(0), which gives you the encoding object
            // that corresponds too the default system codepage.
            // However, in ProjectN, not only Encoding.GetEncoding(Int32) is not exposed, but there is also
            // no guarantee that a notion of a default system code page exists on the OS.
            // In fact, we can really only rely on UTF8 and UTF16 being present on all platforms.
            // We fall back to UTF8 as this is what is used by ZIP when as the "unicode encoding".
            get
            {
                return Encoding.UTF8;
                // return Encoding.GetEncoding(0);
            }
        }

        private String DecodeEntryName(Byte[] entryNameBytes)
        {
            Debug.Assert(entryNameBytes != null);

            Encoding readEntryNameEncoding;
            if ((_generalPurposeBitFlag & BitFlagValues.UnicodeFileName) == 0)
            {
                readEntryNameEncoding = (_archive == null)
                                            ? DefaultSystemEncoding
                                            : _archive.EntryNameEncoding ?? DefaultSystemEncoding;
            }
            else
            {
                readEntryNameEncoding = Encoding.UTF8;
            }

            return readEntryNameEncoding.GetString(entryNameBytes);
        }

        private Byte[] EncodeEntryName(String entryName, out bool isUTF8)
        {
            Debug.Assert(entryName != null);

            Encoding writeEntryNameEncoding;
            if (_archive != null && _archive.EntryNameEncoding != null)
                writeEntryNameEncoding = _archive.EntryNameEncoding;
            else
                writeEntryNameEncoding = ZipHelper.RequiresUnicode(entryName)
                                            ? Encoding.UTF8
                                            : DefaultSystemEncoding;

            isUTF8 = writeEntryNameEncoding.Equals(Encoding.UTF8);
            return writeEntryNameEncoding.GetBytes(entryName);
        }

        /* does almost everything you need to do to forget about this entry
         * writes the local header/data, gets rid of all the data,
         * closes all of the streams except for the very outermost one that
         * the user holds on to and is responsible for closing
         * 
         * after calling this, and only after calling this can we be guaranteed
         * that we are reading to write the central directory
         * 
         * should only throw an exception in extremely exceptional cases because it is called from dispose
         */
        internal void WriteAndFinishLocalEntry()
        {
            CloseStreams();
            WriteLocalFileHeaderAndDataIfNeeded();
            UnloadStreams();
        }

        //should only throw an exception in extremely exceptional cases because it is called from dispose
        internal void WriteCentralDirectoryFileHeader()
        {
            //This part is simple, because we should definitely know the sizes by this time
            BinaryWriter writer = new BinaryWriter(_archive.ArchiveStream);

            //_entryname only gets set when we read in or call moveTo. MoveTo does a check, and
            //reading in should not be able to produce a entryname longer than UInt16.MaxValue
            Debug.Assert(_storedEntryNameBytes.Length <= UInt16.MaxValue);

            //decide if we need the Zip64 extra field:
            Zip64ExtraField zip64ExtraField = new Zip64ExtraField();
            UInt32 compressedSizeTruncated, uncompressedSizeTruncated, offsetOfLocalHeaderTruncated;

            Boolean zip64Needed = false;

            if (SizesTooLarge()
#if DEBUG_FORCE_ZIP64
 || _archive._forceZip64
#endif
)
            {
                zip64Needed = true;
                compressedSizeTruncated = ZipHelper.Mask32Bit;
                uncompressedSizeTruncated = ZipHelper.Mask32Bit;

                //If we have one of the sizes, the other must go in there as speced for LH, but not necessarily for CH, but we do it anyways
                zip64ExtraField.CompressedSize = _compressedSize;
                zip64ExtraField.UncompressedSize = _uncompressedSize;
            }
            else
            {
                compressedSizeTruncated = (UInt32)_compressedSize;
                uncompressedSizeTruncated = (UInt32)_uncompressedSize;
            }


            if (_offsetOfLocalHeader > UInt32.MaxValue
#if DEBUG_FORCE_ZIP64
 || _archive._forceZip64
#endif
)
            {
                zip64Needed = true;
                offsetOfLocalHeaderTruncated = ZipHelper.Mask32Bit;

                //If we have one of the sizes, the other must go in there as speced for LH, but not necessarily for CH, but we do it anyways
                zip64ExtraField.LocalHeaderOffset = _offsetOfLocalHeader;
            }
            else
            {
                offsetOfLocalHeaderTruncated = (UInt32)_offsetOfLocalHeader;
            }

            if (zip64Needed)
                VersionToExtractAtLeast(ZipVersionNeededValues.Zip64);

            //determine if we can fit zip64 extra field and original extra fields all in
            Int32 bigExtraFieldLength = (zip64Needed ? zip64ExtraField.TotalSize : 0)
                                      + (_cdUnknownExtraFields != null ? ZipGenericExtraField.TotalSize(_cdUnknownExtraFields) : 0);
            UInt16 extraFieldLength;
            if (bigExtraFieldLength > UInt16.MaxValue)
            {
                extraFieldLength = (UInt16)(zip64Needed ? zip64ExtraField.TotalSize : 0);
                _cdUnknownExtraFields = null;
            }
            else
            {
                extraFieldLength = (UInt16)bigExtraFieldLength;
            }

            writer.Write(ZipCentralDirectoryFileHeader.SignatureConstant);      // Central directory file header signature  (4 bytes)
            writer.Write(_versionMadeBySpecification);                          // Version made by Specification (version)  (1 byte)
            writer.Write((byte)CurrentZipPlatform);                             // Version made by Compatibility (type)     (1 byte)
            writer.Write((UInt16)_versionToExtract);                            // Minimum version needed to extract        (2 bytes)
            writer.Write((UInt16)_generalPurposeBitFlag);                       // General Purpose bit flag                 (2 bytes)
            writer.Write((UInt16)CompressionMethod);                            // The Compression method                   (2 bytes)
            writer.Write(ZipHelper.DateTimeToDosTime(_lastModified.DateTime));  // File last modification time and date     (4 bytes)
            writer.Write(_crc32);                                               // CRC-32                                   (4 bytes)
            writer.Write(compressedSizeTruncated);                              // Compressed Size                          (4 bytes)
            writer.Write(uncompressedSizeTruncated);                            // Uncompressed Size                        (4 bytes)
            writer.Write((UInt16)_storedEntryNameBytes.Length);                 // File Name Length                         (2 bytes)
            writer.Write(extraFieldLength);                                     // Extra Field Length                       (2 bytes)

            // This should hold because of how we read it originally in ZipCentralDirectoryFileHeader:
            Debug.Assert((_fileComment == null) || (_fileComment.Length <= UInt16.MaxValue));

            writer.Write(_fileComment != null ? (UInt16)_fileComment.Length : (UInt16)0);    //file comment length
            writer.Write((UInt16)0);    //disk number start
            writer.Write((UInt16)0);    //internal file attributes
            writer.Write((UInt32)0);    //external file attributes
            writer.Write(offsetOfLocalHeaderTruncated); //offset of local header

            writer.Write(_storedEntryNameBytes);

            //write extra fields
            if (zip64Needed)
                zip64ExtraField.WriteBlock(_archive.ArchiveStream);
            if (_cdUnknownExtraFields != null)
                ZipGenericExtraField.WriteAllBlocks(_cdUnknownExtraFields, _archive.ArchiveStream);

            if (_fileComment != null)
                writer.Write(_fileComment);
        }

        //returns false if fails, will get called on every entry before closing in update mode
        //can throw InvalidDataException
        internal Boolean LoadLocalHeaderExtraFieldAndCompressedBytesIfNeeded()
        {
            String message;
            //we should have made this exact call in _archive.Init through ThrowIfOpenable
            Debug.Assert(IsOpenable(false, true, out message));

            //load local header's extra fields. it will be null if we couldn't read for some reason
            if (_originallyInArchive)
            {
                _archive.ArchiveStream.Seek(_offsetOfLocalHeader, SeekOrigin.Begin);

                _lhUnknownExtraFields = ZipLocalFileHeader.GetExtraFields(_archive.ArchiveReader);
            }

            if (!_everOpenedForWrite && _originallyInArchive)
            {
                //we know that it is openable at this point

                _compressedBytes = new Byte[(_compressedSize / MaxSingleBufferSize) + 1][];
                for (int i = 0; i < _compressedBytes.Length - 1; i++)
                {
                    _compressedBytes[i] = new Byte[MaxSingleBufferSize];
                }
                _compressedBytes[_compressedBytes.Length - 1] = new Byte[_compressedSize % MaxSingleBufferSize];

                _archive.ArchiveStream.Seek(OffsetOfCompressedData, SeekOrigin.Begin);

                for (int i = 0; i < _compressedBytes.Length - 1; i++)
                {
                    ZipHelper.ReadBytes(_archive.ArchiveStream, _compressedBytes[i], MaxSingleBufferSize);
                }
                ZipHelper.ReadBytes(_archive.ArchiveStream, _compressedBytes[_compressedBytes.Length - 1], (Int32)(_compressedSize % MaxSingleBufferSize));
            }

            return true;
        }

        internal void ThrowIfNotOpenable(Boolean needToUncompress, Boolean needToLoadIntoMemory)
        {
            String message;
            if (!IsOpenable(needToUncompress, needToLoadIntoMemory, out message))
                throw new InvalidDataException(message);
        }

        private CheckSumAndSizeWriteStream GetDataCompressor(Stream backingStream, Boolean leaveBackingStreamOpen, EventHandler onClose)
        {
            //stream stack: backingStream -> DeflateStream -> CheckSumWriteStream

            //we should always be compressing with deflate. Stored is used for empty files, but we don't actually
            //call through this function for that - we just write the stored value in the header
            Debug.Assert(CompressionMethod == CompressionMethodValues.Deflate);

            Stream compressorStream = _compressionLevel.HasValue
                                            ? new DeflateStream(backingStream, _compressionLevel.Value, leaveBackingStreamOpen)
                                            : new DeflateStream(backingStream, CompressionMode.Compress, leaveBackingStreamOpen);
            Boolean isIntermediateStream = true;

            Boolean leaveCompressorStreamOpenOnClose = leaveBackingStreamOpen && !isIntermediateStream;
            var checkSumStream = new CheckSumAndSizeWriteStream(
                compressorStream, 
                backingStream, 
                leaveCompressorStreamOpenOnClose, 
                this, 
                onClose,
                (Int64 initialPosition, Int64 currentPosition, UInt32 checkSum, Stream backing, ZipArchiveEntry thisRef, EventHandler closeHandler) =>
                {
                    thisRef._crc32 = checkSum;
                    thisRef._uncompressedSize = currentPosition;
                    thisRef._compressedSize = backing.Position - initialPosition;

                    if (closeHandler != null)
                        closeHandler(thisRef, EventArgs.Empty);
                });

            return checkSumStream;
        }

        private Stream GetDataDecompressor(Stream compressedStreamToRead)
        {
            Stream uncompressedStream = null;
            switch (CompressionMethod)
            {
                case CompressionMethodValues.Deflate:
                    uncompressedStream = new DeflateStream(compressedStreamToRead, CompressionMode.Decompress);
                    break;
                case CompressionMethodValues.Stored:
                default:
                    //we can assume that only deflate/stored are allowed because we assume that
                    //IsOpenable is checked before this function is called
                    Debug.Assert(CompressionMethod == CompressionMethodValues.Stored);

                    uncompressedStream = compressedStreamToRead;
                    break;
            }

            return uncompressedStream;
        }

        private Stream OpenInReadMode(Boolean checkOpenable)
        {
            if (checkOpenable)
                ThrowIfNotOpenable(true, false);

            Stream compressedStream = new SubReadStream(_archive.ArchiveStream, OffsetOfCompressedData, _compressedSize);
            return GetDataDecompressor(compressedStream);
        }

        private Stream OpenInWriteMode()
        {
            if (_everOpenedForWrite)
                throw new IOException(SR.CreateModeWriteOnceAndOneEntryAtATime);

            //we assume that if another entry grabbed the archive stream, that it set this entry's _everOpenedForWrite property to true by calling WriteLocalFileHeaderIfNeeed
            Debug.Assert(_archive.IsStillArchiveStreamOwner(this));

            _everOpenedForWrite = true;
            CheckSumAndSizeWriteStream crcSizeStream = GetDataCompressor(_archive.ArchiveStream, true,
                                                            (object o, EventArgs e) =>
                                                            {
                                                                //release the archive stream
                                                                var entry = (ZipArchiveEntry)o;
                                                                entry._archive.ReleaseArchiveStream(entry);
                                                                entry._outstandingWriteStream = null;
                                                            });
            _outstandingWriteStream = new DirectToArchiveWriterStream(crcSizeStream, this);

            return new WrappedStream(baseStream: _outstandingWriteStream, closeBaseStream: true);
        }

        private Stream OpenInUpdateMode()
        {
            if (_currentlyOpenForWrite)
                throw new IOException(SR.UpdateModeOneStream);

            ThrowIfNotOpenable(true, true);

            _everOpenedForWrite = true;
            _currentlyOpenForWrite = true;
            //always put it at the beginning for them
            UncompressedData.Seek(0, SeekOrigin.Begin);
            return new WrappedStream(UncompressedData, this, thisRef => 
                                     {
                                         //once they close, we know uncompressed length, but still not compressed length
                                         //so we don't fill in any size information
                                         //those fields get figured out when we call GetCompressor as we write it to
                                         //the actual archive
                                         thisRef._currentlyOpenForWrite = false;
                                     });
        }

        private Boolean IsOpenable(Boolean needToUncompress, Boolean needToLoadIntoMemory, out String message)
        {
            message = null;

            if (_originallyInArchive)
            {
                if (needToUncompress)
                {
                    if (CompressionMethod != CompressionMethodValues.Stored &&
                        CompressionMethod != CompressionMethodValues.Deflate)
                    {
                        message = SR.UnsupportedCompression;
                        return false;
                    }
                }
                if (_diskNumberStart != _archive.NumberOfThisDisk)
                {
                    message = SR.SplitSpanned;
                    return false;
                }
                if (_offsetOfLocalHeader > _archive.ArchiveStream.Length)
                {
                    message = SR.LocalFileHeaderCorrupt;
                    return false;
                }
                _archive.ArchiveStream.Seek(_offsetOfLocalHeader, SeekOrigin.Begin);
                if (!ZipLocalFileHeader.TrySkipBlock(_archive.ArchiveReader))
                {
                    message = SR.LocalFileHeaderCorrupt;
                    return false;
                }
                //when this property gets called, some duplicated work
                if (OffsetOfCompressedData + _compressedSize > _archive.ArchiveStream.Length)
                {
                    message = SR.LocalFileHeaderCorrupt;
                    return false;
                }
                //This limitation originally existed because a) it is unreasonable to load > 4GB into memory
                //but also because the stream reading functions make it hard.  This has been updated to handle
                //this scenario in a 64-bit process using multiple buffers, delivered first as an OOB for
                //compatibility.
                if (needToLoadIntoMemory)
                {
                    if (_compressedSize > Int32.MaxValue)
                    {
                        if (!s_allowLargeZipArchiveEntriesInUpdateMode)
                        {
                            message = SR.EntryTooLarge;
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private Boolean SizesTooLarge()
        {
            return _compressedSize > UInt32.MaxValue || _uncompressedSize > UInt32.MaxValue;
        }

        //return value is true if we allocated an extra field for 64 bit headers, un/compressed size
        private Boolean WriteLocalFileHeader(Boolean isEmptyFile)
        {
            BinaryWriter writer = new BinaryWriter(_archive.ArchiveStream);

            //_entryname only gets set when we read in or call moveTo. MoveTo does a check, and
            //reading in should not be able to produce a entryname longer than UInt16.MaxValue
            Debug.Assert(_storedEntryNameBytes.Length <= UInt16.MaxValue);

            //decide if we need the Zip64 extra field:
            Zip64ExtraField zip64ExtraField = new Zip64ExtraField();
            Boolean zip64Used = false;
            UInt32 compressedSizeTruncated, uncompressedSizeTruncated;

            //if we already know that we have an empty file don't worry about anything, just do a straight shot of the header
            if (isEmptyFile)
            {
                CompressionMethod = CompressionMethodValues.Stored;
                compressedSizeTruncated = 0;
                uncompressedSizeTruncated = 0;
                Debug.Assert(_compressedSize == 0);
                Debug.Assert(_uncompressedSize == 0);
                Debug.Assert(_crc32 == 0);
            }
            else
            {
                //if we have a non-seekable stream, don't worry about sizes at all, and just set the right bit
                //if we are using the data descriptor, then sizes and crc should be set to 0 in the header
                if (_archive.Mode == ZipArchiveMode.Create && _archive.ArchiveStream.CanSeek == false && !isEmptyFile)
                {
                    _generalPurposeBitFlag |= BitFlagValues.DataDescriptor;
                    zip64Used = false;
                    compressedSizeTruncated = 0;
                    uncompressedSizeTruncated = 0;
                    //the crc should not have been set if we are in create mode, but clear it just to be sure
                    Debug.Assert(_crc32 == 0);
                }
                else //if we are not in streaming mode, we have to decide if we want to write zip64 headers
                {
                    if (SizesTooLarge()
#if DEBUG_FORCE_ZIP64
 || (_archive._forceZip64 && _archive.Mode == ZipArchiveMode.Update)
#endif
)
                    {
                        zip64Used = true;
                        compressedSizeTruncated = ZipHelper.Mask32Bit;
                        uncompressedSizeTruncated = ZipHelper.Mask32Bit;

                        //prepare Zip64 extra field object. If we have one of the sizes, the other must go in there
                        zip64ExtraField.CompressedSize = _compressedSize;
                        zip64ExtraField.UncompressedSize = _uncompressedSize;

                        VersionToExtractAtLeast(ZipVersionNeededValues.Zip64);
                    }
                    else
                    {
                        zip64Used = false;
                        compressedSizeTruncated = (UInt32)_compressedSize;
                        uncompressedSizeTruncated = (UInt32)_uncompressedSize;
                    }
                }
            }

            //save offset
            _offsetOfLocalHeader = writer.BaseStream.Position;

            //calculate extra field. if zip64 stuff + original extraField aren't going to fit, dump the original extraField, because this is more important
            Int32 bigExtraFieldLength = (zip64Used ? zip64ExtraField.TotalSize : 0)
                                      + (_lhUnknownExtraFields != null ? ZipGenericExtraField.TotalSize(_lhUnknownExtraFields) : 0);
            UInt16 extraFieldLength;
            if (bigExtraFieldLength > UInt16.MaxValue)
            {
                extraFieldLength = (UInt16)(zip64Used ? zip64ExtraField.TotalSize : 0);
                _lhUnknownExtraFields = null;
            }
            else
            {
                extraFieldLength = (UInt16)bigExtraFieldLength;
            }

            //write header
            writer.Write(ZipLocalFileHeader.SignatureConstant);
            writer.Write((UInt16)_versionToExtract);
            writer.Write((UInt16)_generalPurposeBitFlag);
            writer.Write((UInt16)CompressionMethod);
            writer.Write(ZipHelper.DateTimeToDosTime(_lastModified.DateTime)); //UInt32
            writer.Write(_crc32);               //UInt32
            writer.Write(compressedSizeTruncated);  //UInt32
            writer.Write(uncompressedSizeTruncated); //UInt32
            writer.Write((UInt16)_storedEntryNameBytes.Length);
            writer.Write(extraFieldLength);    //UInt16

            writer.Write(_storedEntryNameBytes);

            if (zip64Used)
                zip64ExtraField.WriteBlock(_archive.ArchiveStream);
            if (_lhUnknownExtraFields != null)
                ZipGenericExtraField.WriteAllBlocks(_lhUnknownExtraFields, _archive.ArchiveStream);

            return zip64Used;
        }

        private void WriteLocalFileHeaderAndDataIfNeeded()
        {
            //_storedUncompressedData gets frozen here, and is what gets written to the file
            if (_storedUncompressedData != null || _compressedBytes != null)
            {
                if (_storedUncompressedData != null)
                {
                    _uncompressedSize = _storedUncompressedData.Length;

                    //The compressor fills in CRC and sizes
                    //The DirectToArchiveWriterStream writes headers and such
                    using (Stream entryWriter = new DirectToArchiveWriterStream(
                                                    GetDataCompressor(_archive.ArchiveStream, true, null),
                                                    this))
                    {
                        _storedUncompressedData.Seek(0, SeekOrigin.Begin);
                        _storedUncompressedData.CopyTo(entryWriter);
                        _storedUncompressedData.Dispose();
                        _storedUncompressedData = null;
                    }
                }
                else
                {
                    // we know the sizes at this point, so just go ahead and write the headers
                    if (_uncompressedSize == 0)
                        CompressionMethod = CompressionMethodValues.Stored;
                    WriteLocalFileHeader(false);
                    foreach (Byte[] compressedBytes in _compressedBytes)
                    {
                        _archive.ArchiveStream.Write(compressedBytes, 0, compressedBytes.Length);
                    }
                }
            }
            else //there is no data in the file, but if we are in update mode, we still need to write a header
            {
                if (_archive.Mode == ZipArchiveMode.Update || !_everOpenedForWrite)
                {
                    _everOpenedForWrite = true;
                    WriteLocalFileHeader(true);
                }
            }
        }

        /* Using _offsetOfLocalHeader, seeks back to where CRC and sizes should be in the header,
         * writes them, then seeks back to where you started
         * Assumes that the stream is currently at the end of the data
        */
        private void WriteCrcAndSizesInLocalHeader(Boolean zip64HeaderUsed)
        {
            Int64 finalPosition = _archive.ArchiveStream.Position;
            BinaryWriter writer = new BinaryWriter(_archive.ArchiveStream);

            Boolean zip64Needed = SizesTooLarge()
#if DEBUG_FORCE_ZIP64
 || _archive._forceZip64
#endif
;
            Boolean pretendStreaming = zip64Needed && !zip64HeaderUsed;

            UInt32 compressedSizeTruncated = zip64Needed ? ZipHelper.Mask32Bit : (UInt32)_compressedSize;
            UInt32 uncompressedSizeTruncated = zip64Needed ? ZipHelper.Mask32Bit : (UInt32)_uncompressedSize;

            /* first step is, if we need zip64, but didn't allocate it, pretend we did a stream write, because
             * we can't go back and give ourselves the space that the extra field needs.
             * we do this by setting the correct property in the bit flag */
            if (pretendStreaming)
            {
                _generalPurposeBitFlag |= BitFlagValues.DataDescriptor;

                _archive.ArchiveStream.Seek(_offsetOfLocalHeader + ZipLocalFileHeader.OffsetToBitFlagFromHeaderStart,
                                            SeekOrigin.Begin);
                writer.Write((UInt16)_generalPurposeBitFlag);
            }

            /* next step is fill out the 32-bit size values in the normal header. we can't assume that
             * they are correct. we also write the CRC */
            _archive.ArchiveStream.Seek(_offsetOfLocalHeader + ZipLocalFileHeader.OffsetToCrcFromHeaderStart,
                                            SeekOrigin.Begin);
            if (!pretendStreaming)
            {
                writer.Write(_crc32);
                writer.Write(compressedSizeTruncated);
                writer.Write(uncompressedSizeTruncated);
            }
            else //but if we are pretending to stream, we want to fill in with zeroes
            {
                writer.Write((UInt32)0);
                writer.Write((UInt32)0);
                writer.Write((UInt32)0);
            }

            /* next step: if we wrote the 64 bit header initially, a different implementation might
             * try to read it, even if the 32-bit size values aren't masked. thus, we should always put the
             * correct size information in there. note that order of uncomp/comp is switched, and these are
             * 64-bit values
             * also, note that in order for this to be correct, we have to insure that the zip64 extra field
             * is alwasy the first extra field that is written */
            if (zip64HeaderUsed)
            {
                _archive.ArchiveStream.Seek(_offsetOfLocalHeader + ZipLocalFileHeader.SizeOfLocalHeader
                                            + _storedEntryNameBytes.Length + Zip64ExtraField.OffsetToFirstField,
                                            SeekOrigin.Begin);
                writer.Write(_uncompressedSize);
                writer.Write(_compressedSize);

                _archive.ArchiveStream.Seek(finalPosition, SeekOrigin.Begin);
            }

            // now go to the where we were. assume that this is the end of the data
            _archive.ArchiveStream.Seek(finalPosition, SeekOrigin.Begin);

            /* if we are pretending we did a stream write, we want to write the data descriptor out
             * the data descriptor can have 32-bit sizes or 64-bit sizes. In this case, we always use
             * 64-bit sizes */
            if (pretendStreaming)
            {
                writer.Write(_crc32);
                writer.Write(_compressedSize);
                writer.Write(_uncompressedSize);
            }
        }

        private void WriteDataDescriptor()
        {
            // data descriptor can be 32-bit or 64-bit sizes. 32-bit is more compatible, so use that if possible
            // signature is optional but recommended by the spec

            BinaryWriter writer = new BinaryWriter(_archive.ArchiveStream);

            writer.Write(ZipLocalFileHeader.DataDescriptorSignature);
            writer.Write(_crc32);
            if (SizesTooLarge())
            {
                writer.Write(_compressedSize);
                writer.Write(_uncompressedSize);
            }
            else
            {
                writer.Write((UInt32)_compressedSize);
                writer.Write((UInt32)_uncompressedSize);
            }
        }

        private void UnloadStreams()
        {
            if (_storedUncompressedData != null)
                _storedUncompressedData.Dispose();
            _compressedBytes = null;
            _outstandingWriteStream = null;
        }

        private void CloseStreams()
        {
            //if the user left the stream open, close the underlying stream for them
            if (_outstandingWriteStream != null)
            {
                _outstandingWriteStream.Dispose();
            }
        }

        private void VersionToExtractAtLeast(ZipVersionNeededValues value)
        {
            if (_versionToExtract < value)
            {
                _versionToExtract = value;
            }
        }

        private void ThrowIfInvalidArchive()
        {
            if (_archive == null)
                throw new InvalidOperationException(SR.DeletedEntry);
            _archive.ThrowIfDisposed();
        }

        /// <summary>
        /// Gets the file name of the path based on Windows path separator characters
        /// </summary>
        private static string GetFileName_Windows(string path)
        {
            int length = path.Length;
            for (int i = length; --i >= 0;)
            {
                char ch = path[i];
                if (ch == '\\' || ch == '/' || ch == ':')
                    return path.Substring(i + 1);
            }
            return path;
        }

        /// <summary>
        /// Gets the file name of the path based on Unix path separator characters
        /// </summary>
        private static string GetFileName_Unix(string path)
        {
            int length = path.Length;
            for (int i = length; --i >= 0;)
                if (path[i] == '/')
                    return path.Substring(i + 1);
            return path;
        }

        #endregion Privates

        #region Nested Types

        private class DirectToArchiveWriterStream : Stream
        {
            #region fields

            private Int64 _position;
            private CheckSumAndSizeWriteStream _crcSizeStream;
            private Boolean _everWritten;
            private Boolean _isDisposed;
            private ZipArchiveEntry _entry;
            private Boolean _usedZip64inLH;
            private Boolean _canWrite;

            #endregion

            #region constructors

            //makes the assumption that somewhere down the line, crcSizeStream is eventually writing directly to the archive
            //this class calls other functions on ZipArchiveEntry that write directly to the archive
            public DirectToArchiveWriterStream(CheckSumAndSizeWriteStream crcSizeStream, ZipArchiveEntry entry)
            {
                _position = 0;
                _crcSizeStream = crcSizeStream;
                _everWritten = false;
                _isDisposed = false;
                _entry = entry;
                _usedZip64inLH = false;
                _canWrite = true;
            }

            #endregion

            #region properties

            public override Int64 Length
            {
                get
                {
                    ThrowIfDisposed();
                    throw new NotSupportedException(SR.SeekingNotSupported);
                }
            }
            public override Int64 Position
            {
                get
                {
                    Contract.Ensures(Contract.Result<Int64>() >= 0);

                    ThrowIfDisposed();
                    return _position;
                }
                set
                {
                    ThrowIfDisposed();
                    throw new NotSupportedException(SR.SeekingNotSupported);
                }
            }

            public override Boolean CanRead { get { return false; } }
            public override Boolean CanSeek { get { return false; } }
            public override Boolean CanWrite { get { return _canWrite; } }

            #endregion

            #region methods

            private void ThrowIfDisposed()
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(this.GetType().ToString(), SR.HiddenStreamName);
            }

            public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
            {
                ThrowIfDisposed();
                throw new NotSupportedException(SR.ReadingNotSupported);
            }

            public override Int64 Seek(Int64 offset, SeekOrigin origin)
            {
                ThrowIfDisposed();
                throw new NotSupportedException(SR.SeekingNotSupported);
            }

            public override void SetLength(Int64 value)
            {
                ThrowIfDisposed();
                throw new NotSupportedException(SR.SetLengthRequiresSeekingAndWriting);
            }

            //careful: assumes that write is the only way to write to the stream, if writebyte/beginwrite are implemented
            //they must set _everWritten, etc.
            public override void Write(Byte[] buffer, Int32 offset, Int32 count)
            {
                //we can't pass the argument checking down a level
                if (buffer == null)
                    throw new ArgumentNullException("buffer");
                if (offset < 0)
                    throw new ArgumentOutOfRangeException("offset", SR.ArgumentNeedNonNegative);
                if (count < 0)
                    throw new ArgumentOutOfRangeException("count", SR.ArgumentNeedNonNegative);
                if ((buffer.Length - offset) < count)
                    throw new ArgumentException(SR.OffsetLengthInvalid);
                Contract.EndContractBlock();

                ThrowIfDisposed();
                Debug.Assert(CanWrite);

                //if we're not actually writing anything, we don't want to trigger the header
                if (count == 0)
                    return;

                if (!_everWritten)
                {
                    _everWritten = true;
                    //write local header, we are good to go
                    _usedZip64inLH = _entry.WriteLocalFileHeader(false);
                }

                _crcSizeStream.Write(buffer, offset, count);
                _position += count;
            }

            public override void Flush()
            {
                ThrowIfDisposed();
                Debug.Assert(CanWrite);

                _crcSizeStream.Flush();
            }

            protected override void Dispose(Boolean disposing)
            {
                if (disposing && !_isDisposed)
                {
                    _crcSizeStream.Dispose(); //now we have size/crc info

                    if (!_everWritten)
                    {
                        //write local header, no data, so we use stored
                        _entry.WriteLocalFileHeader(true);
                    }
                    else
                    {
                        //go back and finish writing
                        if (_entry._archive.ArchiveStream.CanSeek)
                            //finish writing local header if we have seek capabilities

                            _entry.WriteCrcAndSizesInLocalHeader(_usedZip64inLH);
                        else
                            //write out data descriptor if we don't have seek capabilities
                            _entry.WriteDataDescriptor();
                    }
                    _canWrite = false;
                    _isDisposed = true;
                }

                base.Dispose(disposing);
            }
            #endregion
        }  // DirectToArchiveWriterStream

        [Flags]
        private enum BitFlagValues : ushort { DataDescriptor = 0x8, UnicodeFileName = 0x800 }

        private enum CompressionMethodValues : ushort { Stored = 0x0, Deflate = 0x8 }

        private enum OpenableValues { Openable, FileNonExistent, FileTooLarge }
        #endregion Nested Types
    }
}
