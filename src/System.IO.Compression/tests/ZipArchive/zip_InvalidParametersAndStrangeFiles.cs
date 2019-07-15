// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace System.IO.Compression.Tests
{
    public class zip_InvalidParametersAndStrangeFiles : ZipFileTestBase
    {
        private static readonly int s_bufferSize = 10240;
        private static readonly string s_tamperedFileName = "binary.wmv";
        private static void ConstructorThrows<TException>(Func<ZipArchive> constructor, string Message) where TException : Exception
        {
            try
            {
                Assert.Throws<TException>(() =>
                {
                    using (ZipArchive archive = constructor()) { }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("{0}: {1}", Message, e.ToString()));
                throw;
            }
        }

        [Fact]
        public static async Task InvalidInstanceMethods()
        {
            Stream zipFile = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));
            using (ZipArchive archive = new ZipArchive(zipFile, ZipArchiveMode.Update))
            {
                //non-existent entry
                Assert.True(null == archive.GetEntry("nonExistentEntry")); //"Should return null on non-existent entry name"
                //null/empty string
                Assert.Throws<ArgumentNullException>(() => archive.GetEntry(null)); //"Should throw on null entry name"

                ZipArchiveEntry entry = archive.GetEntry("first.txt");

                //null/empty string
                AssertExtensions.Throws<ArgumentException>("entryName", () => archive.CreateEntry("")); //"Should throw on empty entry name"
                Assert.Throws<ArgumentNullException>(() => archive.CreateEntry(null)); //"should throw on null entry name"
            }
        }

        [Fact]
        public static void InvalidConstructors()
        {
            //out of range enum values
            ConstructorThrows<ArgumentOutOfRangeException>(() =>
                new ZipArchive(new MemoryStream(), (ZipArchiveMode)(-1)), "Out of range enum");
            ConstructorThrows<ArgumentOutOfRangeException>(() =>
                new ZipArchive(new MemoryStream(), (ZipArchiveMode)(4)), "out of range enum");
            ConstructorThrows<ArgumentOutOfRangeException>(() =>
                new ZipArchive(new MemoryStream(), (ZipArchiveMode)(10)), "Out of range enum");

            //null/closed stream
            ConstructorThrows<ArgumentNullException>(() =>
                new ZipArchive((Stream)null, ZipArchiveMode.Read), "Null/closed stream");
            ConstructorThrows<ArgumentNullException>(() =>
                new ZipArchive((Stream)null, ZipArchiveMode.Create), "Null/closed stream");
            ConstructorThrows<ArgumentNullException>(() =>
                new ZipArchive((Stream)null, ZipArchiveMode.Update), "Null/closed stream");

            MemoryStream ms = new MemoryStream();
            ms.Dispose();

            ConstructorThrows<ArgumentException>(() =>
                new ZipArchive(ms, ZipArchiveMode.Read), "Disposed Base Stream");
            ConstructorThrows<ArgumentException>(() =>
                new ZipArchive(ms, ZipArchiveMode.Create), "Disposed Base Stream");
            ConstructorThrows<ArgumentException>(() =>
                new ZipArchive(ms, ZipArchiveMode.Update), "Disposed Base Stream");

            //non-seekable to update
            using (LocalMemoryStream nonReadable = new LocalMemoryStream(),
                nonWriteable = new LocalMemoryStream(),
                nonSeekable = new LocalMemoryStream())
            {
                nonReadable.SetCanRead(false);
                nonWriteable.SetCanWrite(false);
                nonSeekable.SetCanSeek(false);

                ConstructorThrows<ArgumentException>(() => new ZipArchive(nonReadable, ZipArchiveMode.Read), "Non readable stream");

                ConstructorThrows<ArgumentException>(() => new ZipArchive(nonWriteable, ZipArchiveMode.Create), "Non-writable stream");

                ConstructorThrows<ArgumentException>(() => new ZipArchive(nonReadable, ZipArchiveMode.Update), "Non-readable stream");
                ConstructorThrows<ArgumentException>(() => new ZipArchive(nonWriteable, ZipArchiveMode.Update), "Non-writable stream");
                ConstructorThrows<ArgumentException>(() => new ZipArchive(nonSeekable, ZipArchiveMode.Update), "Non-seekable stream");
            }
        }

        [Theory]
        [InlineData("LZMA.zip")]
        [InlineData("invalidDeflate.zip")]
        public static async Task ZipArchiveEntry_InvalidUpdate(string zipname)
        {
            string filename = bad(zipname);
            Stream updatedCopy = await StreamHelpers.CreateTempCopyStream(filename);
            string name;
            long length, compressedLength;
            DateTimeOffset lastWriteTime;
            using (ZipArchive archive = new ZipArchive(updatedCopy, ZipArchiveMode.Update, true))
            {
                ZipArchiveEntry e = archive.Entries[0];
                name = e.FullName;
                lastWriteTime = e.LastWriteTime;
                length = e.Length;
                compressedLength = e.CompressedLength;
                Assert.Throws<InvalidDataException>(() => e.Open()); //"Should throw on open"
            }

            //make sure that update mode preserves that unreadable file
            using (ZipArchive archive = new ZipArchive(updatedCopy, ZipArchiveMode.Update))
            {
                ZipArchiveEntry e = archive.Entries[0];
                Assert.Equal(name, e.FullName); //"Name isn't the same"
                Assert.Equal(lastWriteTime, e.LastWriteTime); //"LastWriteTime not the same"
                Assert.Equal(length, e.Length); //"Length isn't the same"
                Assert.Equal(compressedLength, e.CompressedLength); //"CompressedLength isn't the same"
                Assert.Throws<InvalidDataException>(() => e.Open()); //"Should throw on open"
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Fix not shipped for full framework.")]
        public static async Task ZipArchiveEntry_CorruptedStream_ReadMode_CopyTo_UpToUncompressedSize()
        {
            MemoryStream stream = await LocalMemoryStream.readAppFileAsync(zfile("normal.zip"));

            int nameOffset = PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header
            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 22, nameOffset + s_tamperedFileName.Length); // patch in central directory too

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                using (MemoryStream ms = new MemoryStream())
                using (Stream source = e.Open())
                {
                    source.CopyTo(ms);
                    Assert.Equal(e.Length, ms.Length);     // Only allow to decompress up to uncompressed size
                    byte[] buffer = new byte[s_bufferSize];
                    Assert.Equal(0, source.Read(buffer, 0, buffer.Length)); // shouldn't be able read more                        
                    ms.Seek(0, SeekOrigin.Begin);
                    int read;
                    while ((read = ms.Read(buffer, 0, buffer.Length)) != 0)
                    { // No need to do anything, just making sure all bytes readable
                    }
                    Assert.Equal(ms.Position, ms.Length); // all bytes must be read
                }
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Fix not shipped for full framework.")]
        public static async Task ZipArchiveEntry_CorruptedStream_ReadMode_Read_UpToUncompressedSize()
        {
            MemoryStream stream = await LocalMemoryStream.readAppFileAsync(zfile("normal.zip"));

            int nameOffset = PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header
            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 22, nameOffset + s_tamperedFileName.Length); // patch in central directory too

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                using (MemoryStream ms = new MemoryStream())
                using (Stream source = e.Open())
                {
                    byte[] buffer = new byte[s_bufferSize];
                    int read;
                    while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    Assert.Equal(e.Length, ms.Length);     // Only allow to decompress up to uncompressed size
                    Assert.Equal(0, source.Read(buffer, 0, s_bufferSize)); // shouldn't be able read more 
                    ms.Seek(0, SeekOrigin.Begin);
                    while ((read = ms.Read(buffer, 0, buffer.Length)) != 0)
                    { // No need to do anything, just making sure all bytes readable from output stream
                    }
                    Assert.Equal(ms.Position, ms.Length); // all bytes must be read
                }
            }
        }

        [Fact]

        public static void ZipArchiveEntry_CorruptedStream_EnsureNoExtraBytesReadOrOverWritten()
        {
            MemoryStream stream = populateStream().Result;

            int nameOffset = PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header
            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 22, nameOffset + s_tamperedFileName.Length); // patch in central directory too

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                using (Stream source = e.Open())
                {
                    byte[] buffer = new byte[e.Length + 20];
                    Array.Fill<byte>(buffer, 0xDE);
                    int read;
                    int offset = 0;
                    int length = buffer.Length;

                    while ((read = source.Read(buffer, offset, length)) != 0)
                    {
                        offset += read;
                        length -= read;
                    }
                    for (int i = offset; i < buffer.Length; i++)
                    {
                        Assert.Equal(0xDE, buffer[i]);
                    }
                }
            }
        }

        private static async Task<MemoryStream> populateStream()
        {
            return await LocalMemoryStream.readAppFileAsync(zfile("normal.zip"));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Deflate64 zip support is a netcore feature not available on full framework.")]
        public static async Task Zip64ArchiveEntry_CorruptedStream_CopyTo_UpToUncompressedSize()
        {
            MemoryStream stream = await LocalMemoryStream.readAppFileAsync(compat("deflate64.zip"));

            int nameOffset = PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header
            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 22, nameOffset + s_tamperedFileName.Length); // patch in central directory too

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                using (var ms = new MemoryStream())
                using (Stream source = e.Open())
                {
                    source.CopyTo(ms);
                    Assert.Equal(e.Length, ms.Length);     // Only allow to decompress up to uncompressed size
                    ms.Seek(0, SeekOrigin.Begin);
                    int read;
                    byte[] buffer = new byte[s_bufferSize];
                    while ((read = ms.Read(buffer, 0, buffer.Length)) != 0)
                    { // No need to do anything, just making sure all bytes readable
                    }
                    Assert.Equal(ms.Position, ms.Length); // all bytes must be read
                }
            }
        }

        [Fact]
        public static async Task ZipArchiveEntry_CorruptedStream_UnCompressedSizeBiggerThanExpected_NothingShouldBreak()
        {
            MemoryStream stream = await LocalMemoryStream.readAppFileAsync(zfile("normal.zip"));

            int nameOffset = PatchDataRelativeToFileNameFillBytes(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header
            PatchDataRelativeToFileNameFillBytes(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 22, nameOffset + s_tamperedFileName.Length); // patch in central directory too

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                using (MemoryStream ms = new MemoryStream())
                using (Stream source = e.Open())
                {
                    source.CopyTo(ms);
                    Assert.True(e.Length > ms.Length);           // Even uncompressed size is bigger than decompressed size there should be no error
                    Assert.True(e.CompressedLength < ms.Length);
                }
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Deflate64 zip support is a netcore feature not available on full framework.")]
        public static async Task Zip64ArchiveEntry_CorruptedFile_Read_UpToUncompressedSize()
        {
            MemoryStream stream = await LocalMemoryStream.readAppFileAsync(compat("deflate64.zip"));

            int nameOffset = PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header
            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 22, nameOffset + s_tamperedFileName.Length); // patch in central directory too

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                using (var ms = new MemoryStream())
                using (Stream source = e.Open())
                {
                    byte[] buffer = new byte[s_bufferSize];
                    int read;
                    while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    Assert.Equal(e.Length, ms.Length);     // Only allow to decompress up to uncompressed size
                    Assert.Equal(0, source.Read(buffer, 0, buffer.Length)); // Shouldn't be readable more
                }
            }
        }


        [Fact]
        public static async Task UnseekableVeryLargeArchive_DataDescriptor_Read_Zip64()
        {
            MemoryStream stream = await LocalMemoryStream.readAppFileAsync(strange("veryLarge.zip"));

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.GetEntry("bigFile.bin");
                
                Assert.Equal(6_442_450_944, e.Length);
                Assert.Equal(6_261_752, e.CompressedLength);

                using (Stream source = e.Open())
                {
                    byte[] buffer = new byte[s_bufferSize];
                    int read = source.Read(buffer, 0, buffer.Length);   // We don't want to inflate this large archive entirely
                                                                        // just making sure it read successfully
                    Assert.Equal(s_bufferSize, read);                   
                }
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Fix not shipped for full framework.")]
        public static async Task ZipArchive_CorruptedLocalHeader_UncompressedSize_NotMatchWithCentralDirectory()
        {
            MemoryStream stream = await LocalMemoryStream.readAppFileAsync(zfile("normal.zip"));

            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                Assert.Throws<InvalidDataException>(() => e.Open());
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Fix not shipped for full framework.")]
        public static async Task ZipArchive_CorruptedLocalHeader_CompressedSize_NotMatchWithCentralDirectory()
        {
            MemoryStream stream = await LocalMemoryStream.readAppFileAsync(zfile("normal.zip"));

            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 12);  // patch compressed size in file header

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                Assert.Throws<InvalidDataException>(() => e.Open());
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework does not allow unseekable streams.")]
        public static async Task ZipArchive_Unseekable_Corrupted_FileDescriptor_NotMatchWithCentralDirectory()
        {
            using (var s = new MemoryStream())
            {
                var testStream = new WrappedStream(s, false, true, false, null);
                await CreateFromDir(zfolder("normal"), testStream, ZipArchiveMode.Create);
              
                PatchDataDescriptorRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), s, 8);  // patch uncompressed size in file descriptor

                using (ZipArchive archive = new ZipArchive(s, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                    Assert.Throws<InvalidDataException>(() => e.Open());
                }
            }
        }

        [Fact]
        public static async Task UpdateZipArchive_AppendTo_CorruptedFileEntry()
        {
            MemoryStream stream = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));
            int updatedUncompressedLength = 1310976;
            string append = "\r\n\r\nThe answer my friend, is blowin' in the wind.";
            byte[] data = Encoding.ASCII.GetBytes(append);
            long oldCompressedSize = 0;

            int nameOffset = PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header
            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 22, nameOffset + s_tamperedFileName.Length); // patch in central directory too

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update, true))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                oldCompressedSize = e.CompressedLength;
                using (Stream s = e.Open())
                {
                    Assert.Equal(updatedUncompressedLength, s.Length);
                    s.Seek(0, SeekOrigin.End);                  
                    s.Write(data, 0, data.Length);
                    Assert.Equal(updatedUncompressedLength + data.Length, s.Length);
                }
            }

            using (ZipArchive modifiedArchive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = modifiedArchive.GetEntry(s_tamperedFileName);
                using (Stream s = e.Open())
                using (var ms = new MemoryStream())
                {
                    await s.CopyToAsync(ms, s_bufferSize);
                    Assert.Equal(updatedUncompressedLength + data.Length, ms.Length);
                    ms.Seek(updatedUncompressedLength, SeekOrigin.Begin);
                    byte[] read = new byte[data.Length];
                    ms.Read(read, 0, data.Length);
                    Assert.Equal(append, Encoding.ASCII.GetString(read));
                }
                Assert.True(oldCompressedSize > e.CompressedLength); // old compressed size must be reduced by Uncomressed size limit
            }
        }

        [Fact]
        public static async Task UpdateZipArchive_OverwriteCorruptedEntry()
        {
            MemoryStream stream = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));
            int updatedUncompressedLength = 1310976;
            string overwrite = "\r\n\r\nThe answer my friend, is blowin' in the wind.";
            byte[] data = Encoding.ASCII.GetBytes(overwrite);

            int nameOffset = PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header
            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 22, nameOffset + s_tamperedFileName.Length); // patch in central directory too

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update, true))
            {
                ZipArchiveEntry e = archive.GetEntry(s_tamperedFileName);
                string fileName = zmodified(Path.Combine("overwrite", "first.txt"));
                var file = FileData.GetFile(fileName);

                using (var s = new MemoryStream(data))
                using (Stream es = e.Open())
                {
                    Assert.Equal(updatedUncompressedLength, es.Length);
                    es.SetLength(0);
                    await s.CopyToAsync(es, s_bufferSize);
                    Assert.Equal(data.Length, es.Length);
                }
            }

            using (ZipArchive modifiedArchive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = modifiedArchive.GetEntry(s_tamperedFileName);
                using (Stream s = e.Open())
                using (var ms = new MemoryStream())
                {
                    await s.CopyToAsync(ms, s_bufferSize);
                    Assert.Equal(data.Length, ms.Length);
                    Assert.Equal(overwrite, Encoding.ASCII.GetString(ms.GetBuffer(), 0, data.Length));
                }
            }
        }

        [Fact]
        public static async Task UpdateZipArchive_AddFileTo_ZipWithCorruptedFile()
        {
            string addingFile = "added.txt";
            MemoryStream stream = await StreamHelpers.CreateTempCopyStream(zfile("normal.zip"));
            MemoryStream file = await StreamHelpers.CreateTempCopyStream(zmodified(Path.Combine("addFile", addingFile)));

            int nameOffset = PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 8);  // patch uncompressed size in file header
            PatchDataRelativeToFileName(Encoding.ASCII.GetBytes(s_tamperedFileName), stream, 22, nameOffset + s_tamperedFileName.Length); // patch in central directory too          

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update, true))
            {
                ZipArchiveEntry e = archive.CreateEntry(addingFile);
                using (Stream es = e.Open())
                {
                    file.CopyTo(es);
                }
            }

            using (ZipArchive modifiedArchive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = modifiedArchive.GetEntry(s_tamperedFileName);
                using (Stream s = e.Open())
                using (var ms = new MemoryStream())
                {
                    await s.CopyToAsync(ms, s_bufferSize);
                    Assert.Equal(e.Length, ms.Length);  // tampered file should read up to uncompressed size
                }

                ZipArchiveEntry addedEntry = modifiedArchive.GetEntry(addingFile);
                Assert.NotNull(addedEntry);
                Assert.Equal(addedEntry.Length, file.Length);

                using (Stream s = addedEntry.Open())
                { // Make sure file content added correctly
                    int read = 0;
                    byte[] buffer1 = new byte[1024];
                    byte[] buffer2 = new byte[1024];
                    file.Seek(0, SeekOrigin.Begin);

                    while ((read = s.Read(buffer1, 0, buffer1.Length)) != 0 )
                    {
                        file.Read(buffer2, 0, buffer2.Length);
                        Assert.Equal(buffer1, buffer2);
                    }
                }     
            }
        }

        private static int PatchDataDescriptorRelativeToFileName(byte[] fileNameInBytes, MemoryStream packageStream, int distance, int start = 0)
        {
            byte[] dataDescriptorSignature = BitConverter.GetBytes(0x08074B50);
            byte[] buffer = packageStream.GetBuffer();
            int startOfName = FindSequenceIndex(fileNameInBytes, buffer, start);
            int startOfDataDescriptor = FindSequenceIndex(dataDescriptorSignature, buffer, startOfName);
            var startOfUpdatingData = startOfDataDescriptor + distance;

            // updating 4 byte data
            buffer[startOfUpdatingData] = 0;
            buffer[startOfUpdatingData + 1] = 1;
            buffer[startOfUpdatingData + 2] = 20;
            buffer[startOfUpdatingData + 3] = 0;

            return startOfName;
        }

        private static int PatchDataRelativeToFileName(byte[] fileNameInBytes, MemoryStream packageStream, int distance, int start = 0)
        {
            var buffer = packageStream.GetBuffer();
            var startOfName = FindSequenceIndex(fileNameInBytes, buffer, start);
            var startOfUpdatingData = startOfName - distance;

            // updating 4 byte data
            buffer[startOfUpdatingData] = 0;
            buffer[startOfUpdatingData + 1] = 1;
            buffer[startOfUpdatingData + 2] = 20;
            buffer[startOfUpdatingData + 3] = 0;

            return startOfName;
        }

        private static int PatchDataRelativeToFileNameFillBytes(byte[] fileNameInBytes, MemoryStream packageStream, int distance, int start = 0)
        {
            var buffer = packageStream.GetBuffer();
            var startOfName = FindSequenceIndex(fileNameInBytes, buffer, start);
            var startOfUpdatingData = startOfName - distance;

            // updating 4 byte data
            buffer[startOfUpdatingData] = 255;
            buffer[startOfUpdatingData + 1] = 255;
            buffer[startOfUpdatingData + 2] = 255;
            buffer[startOfUpdatingData + 3] = 0;

            return startOfName;
        }

        private static int FindSequenceIndex(byte[] searchItem, byte[] whereToSearch, int startIndex = 0)
        {
            for (int start = startIndex; start < whereToSearch.Length - searchItem.Length; ++start)
            {
                int searchIndex = 0;
                while (searchIndex < searchItem.Length && searchItem[searchIndex] == whereToSearch[start + searchIndex])
                {
                    ++searchIndex;
                }
                if (searchIndex == searchItem.Length)
                {
                    return start;
                }
            }
            return -1;
        }

        [Theory]
        [InlineData("CDoffsetOutOfBounds.zip")]
        [InlineData("EOCDmissing.zip")]
        public static async Task ZipArchive_InvalidStream(string zipname)
        {
            string filename = bad(zipname);
            using (var stream = await StreamHelpers.CreateTempCopyStream(filename))
                Assert.Throws<InvalidDataException>(() => new ZipArchive(stream, ZipArchiveMode.Read));
        }

        [Theory]
        [InlineData("CDoffsetInBoundsWrong.zip")]
        [InlineData("numberOfEntriesDifferent.zip")]
        public static async Task ZipArchive_InvalidEntryTable(string zipname)
        {
            string filename = bad(zipname);
            using (ZipArchive archive = new ZipArchive(await StreamHelpers.CreateTempCopyStream(filename), ZipArchiveMode.Read))
                Assert.Throws<InvalidDataException>(() => archive.Entries[0]);
        }

        [Theory]
        [InlineData("compressedSizeOutOfBounds.zip", true)]
        [InlineData("localFileHeaderSignatureWrong.zip", true)]
        [InlineData("localFileOffsetOutOfBounds.zip", true)]
        [InlineData("LZMA.zip", true)]
        [InlineData("invalidDeflate.zip", false)]
        public static async Task ZipArchive_InvalidEntry(string zipname, bool throwsOnOpen)
        {
            string filename = bad(zipname);
            using (ZipArchive archive = new ZipArchive(await StreamHelpers.CreateTempCopyStream(filename), ZipArchiveMode.Read))
            {
                ZipArchiveEntry e = archive.Entries[0];
                if (throwsOnOpen)
                {
                    Assert.Throws<InvalidDataException>(() => e.Open()); //"should throw on open"
                }
                else
                {
                    using (Stream s = e.Open())
                    {
                        Assert.Throws<InvalidDataException>(() => s.ReadByte()); //"Unreadable stream"
                    }
                }
            }
        }

        [Fact]
        public static async Task ZipArchiveEntry_InvalidLastWriteTime_Read()
        {
            using (ZipArchive archive = new ZipArchive(await StreamHelpers.CreateTempCopyStream(
                 bad("invaliddate.zip")), ZipArchiveMode.Read))
            {
                Assert.Equal(new DateTime(1980, 1, 1, 0, 0, 0), archive.Entries[0].LastWriteTime.DateTime); //"Date isn't correct on invalid date"
            }
        }

        [Fact]
        public static void ZipArchiveEntry_InvalidLastWriteTime_Write()
        {
            using (ZipArchive archive = new ZipArchive(new MemoryStream(), ZipArchiveMode.Create))
            {
                ZipArchiveEntry entry = archive.CreateEntry("test");
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    //"should throw on bad date"
                    entry.LastWriteTime = new DateTimeOffset(1979, 12, 3, 5, 6, 2, new TimeSpan());
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    //"Should throw on bad date"
                    entry.LastWriteTime = new DateTimeOffset(2980, 12, 3, 5, 6, 2, new TimeSpan());
                });
            }
        }

        [Theory]
        [InlineData("extradata/extraDataLHandCDentryAndArchiveComments.zip", "verysmall", true)]
        [InlineData("extradata/extraDataThenZip64.zip", "verysmall", true)]
        [InlineData("extradata/zip64ThenExtraData.zip", "verysmall", true)]
        [InlineData("dataDescriptor.zip", "normalWithoutBinary", false)]
        [InlineData("filenameTimeAndSizesDifferentInLH.zip", "verysmall", false)]
        public static async Task StrangeFiles(string zipFile, string zipFolder, bool requireExplicit)
        {
            IsZipSameAsDir(await StreamHelpers.CreateTempCopyStream(strange(zipFile)), zfolder(zipFolder), ZipArchiveMode.Update, requireExplicit, checkTimes: true);
        }

        /// <summary>
        /// This test tiptoes the buffer boundaries to ensure that the size of a read buffer doesn't
        /// cause any bytes to be left in ZLib's buffer.
        /// </summary>
        [Fact]
        public static void ZipWithLargeSparseFile()
        {
            string zipname = strange("largetrailingwhitespacedeflation.zip");
            string entryname = "A/B/C/D";
            using (FileStream stream = File.Open(zipname, FileMode.Open, FileAccess.Read))
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = archive.GetEntry(entryname);
                long size = entry.Length;

                for (int bufferSize = 1; bufferSize <= size; bufferSize++)
                {
                    using (Stream entryStream = entry.Open())
                    {
                        byte[] b = new byte[bufferSize];
                        int read = 0, count = 0;
                        while ((read = entryStream.Read(b, 0, bufferSize)) > 0)
                        {
                            count += read;
                        }
                        Assert.Equal(size, count);
                    }
                }
            }
        }

        private static readonly byte[] s_emptyFileCompressedWithEtx =
        {
            0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00, 0x08, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x78, 0x6C,
            0x2F, 0x63, 0x75, 0x73, 0x74, 0x6F, 0x6D, 0x50, 0x72, 0x6F, 0x70, 0x65, 0x72, 0x74, 0x79, 0x32,
            0x2E, 0x62, 0x69, 0x6E, 0x03, 0x00, 0x50, 0x4B, 0x01, 0x02, 0x14, 0x00, 0x14, 0x00, 0x06, 0x00,
            0x08, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x78, 0x6C, 0x2F, 0x63, 0x75, 0x73, 0x74, 0x6F, 0x6D, 0x50, 0x72, 0x6F,
            0x70, 0x65, 0x72, 0x74, 0x79, 0x32, 0x2E, 0x62, 0x69, 0x6E, 0x50, 0x4B, 0x05, 0x06, 0x00, 0x00,
            0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x44, 0x00, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x00, 0x00
        };
        private static readonly byte[] s_emptyFileCompressedWrongSize =
        {
            0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00, 0x08, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x78, 0x6C,
            0x2F, 0x63, 0x75, 0x73, 0x74, 0x6F, 0x6D, 0x50, 0x72, 0x6F, 0x70, 0x65, 0x72, 0x74, 0x79, 0x32,
            0x2E, 0x62, 0x69, 0x6E, 0xBA, 0xAD, 0x03, 0x00, 0x50, 0x4B, 0x01, 0x02, 0x14, 0x00, 0x14, 0x00,
            0x06, 0x00, 0x08, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x78, 0x6C, 0x2F, 0x63, 0x75, 0x73, 0x74, 0x6F, 0x6D, 0x50,
            0x72, 0x6F, 0x70, 0x65, 0x72, 0x74, 0x79, 0x32, 0x2E, 0x62, 0x69, 0x6E, 0x50, 0x4B, 0x05, 0x06,
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x44, 0x00, 0x00, 0x00, 0x38, 0x00, 0x00, 0x00,
            0x00, 0x00
        };
        public static IEnumerable<object[]> EmptyFiles = new List<object[]>()
        {
            new object[] { s_emptyFileCompressedWithEtx },
            new object[] { s_emptyFileCompressedWrongSize }
        };

        /// <summary>
        /// This test checks behavior of ZipArchive with unexpected zip files:
        /// 1. EmptyFileCompressedWithEOT has 
        /// Deflate 0x08, _uncompressedSize 0, _compressedSize 2, compressed data: 0x0300 (\u0003 ETX)
        /// 2. EmptyFileCompressedWrongSize has 
        /// Deflate 0x08, _uncompressedSize 0, _compressedSize 4, compressed data: 0xBAAD0300 (just bad data)
        /// ZipArchive is expected to change compression method to Stored (0x00) and ignore "bad" compressed size
        /// </summary>
        [Theory]
        [MemberData(nameof(EmptyFiles))]
        public void ReadArchive_WithEmptyDeflatedFile(byte[] fileBytes)
        {
            using (var testStream = new MemoryStream(fileBytes))
            {
                const string ExpectedFileName = "xl/customProperty2.bin";
                // open archive with zero-length file that is compressed (Deflate = 0x8)
                using (var zip = new ZipArchive(testStream, ZipArchiveMode.Update, leaveOpen: true))
                {
                    // dispose without making any changes will rewrite the archive
                }

                byte[] fileContent = testStream.ToArray();

                // compression method should change to "uncompressed" (Stored = 0x0)
                Assert.Equal(0, fileContent[8]);

                // extract and check the file. should stay empty.
                using (var zip = new ZipArchive(testStream, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry entry = zip.GetEntry(ExpectedFileName);
                    Assert.Equal(0, entry.Length);
                    Assert.Equal(0, entry.CompressedLength);
                    using (Stream entryStream = entry.Open())
                    {
                        Assert.Equal(0, entryStream.Length);
                    }
                }
            }
        }
    }
}

