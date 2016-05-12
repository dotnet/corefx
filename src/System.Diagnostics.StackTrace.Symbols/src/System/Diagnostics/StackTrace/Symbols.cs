// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace System.Diagnostics.StackTrace
{
    public static class Symbols
    {
        /// <summary>
        /// Returns the source file and line number information for the method.
        /// </summary>
        /// <param name="assemblyPath">file path of the assembly or null</param>
        /// <param name="inMemorySymbols">in memory PDB address or zero</param>
        /// <param name="inMemorySymbolsSize">in memory PDB size</param>
        /// <param name="methodToken">method token</param>
        /// <param name="ilOffset">il offset of the stack frame</param>
        /// <param name="sourceFile">source file return</param>
        /// <param name="sourceLine">line number return</param>
        /// <param name="sourceColumn">column return</param>
        public static void GetSourceLineInfo(string assemblyPath, IntPtr inMemorySymbols, int inMemorySymbolsSize, 
            int methodToken, int ilOffset, out string sourceFile, out int sourceLine, out int sourceColumn)
        {
            sourceFile = null;
            sourceLine = 0;
            sourceColumn = 0;

            MetadataReader reader;
            using (GetReader(assemblyPath, inMemorySymbols, inMemorySymbolsSize, out reader)) {
                if (reader != null) {
                    Handle handle = MetadataTokens.Handle(methodToken);

                    if (handle.Kind == HandleKind.MethodDefinition) {
                        MethodDebugInformationHandle methodDebugHandle = ((MethodDefinitionHandle)handle).ToDebugInformationHandle();
                        MethodDebugInformation methodInfo = reader.GetMethodDebugInformation(methodDebugHandle);

                        if (!methodInfo.SequencePointsBlob.IsNil) {
                            try {
                                SequencePointCollection sequencePoints = methodInfo.GetSequencePoints();

                                int sequencePointCount = 0;
                                foreach (SequencePoint sequence in sequencePoints) {
                                    sequencePointCount++;
                                }

                                if (sequencePointCount > 0) {
                                    int[] offsets = new int[sequencePointCount];
                                    int[] lines = new int[sequencePointCount];
                                    int[] columns = new int[sequencePointCount];
                                    DocumentHandle[] documents = new DocumentHandle[sequencePointCount];

                                    int i = 0;
                                    foreach (SequencePoint sequence in sequencePoints) {
                                        offsets[i] = sequence.Offset;
                                        lines[i] = sequence.StartLine;
                                        columns[i] = sequence.StartColumn;
                                        documents[i] = sequence.Document;
                                        i++;
                                    }

                                    // Search for the correct IL offset
                                    int j;
                                    for (j = 0; j < sequencePointCount; j++) {

                                        // look for the entry matching the one we're looking for
                                        if (offsets[j] >= ilOffset) {

                                            // if this offset is > what we're looking for, ajdust the index
                                            if (offsets[j] > ilOffset && j > 0) {
                                                j--;
                                            }
                                            break;
                                        }
                                    }

                                    // If we didn't find a match, default to the last sequence point
                                    if (j == sequencePointCount) {
                                        j--;
                                    }

                                    while (lines[j] == SequencePoint.HiddenLine && j > 0) {
                                        j--;
                                    }

                                    if (lines[j] != SequencePoint.HiddenLine) {
                                        sourceLine = lines[j];
                                        sourceColumn = columns[j];
                                    }
                                    sourceFile = reader.GetString(reader.GetDocument(documents[j]).Name);
                                }
                            }
                            catch {
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the portable PDB reader for the assembly path
        /// </summary>
        /// <param name="assemblyPath">file path of the assembly or null</param>
        /// <param name="inMemorySymbols">in memory PDB address or zero</param>
        /// <param name="inMemorySymbolsSize">in memory PDB size</param>
        /// <param name="reader">returns the reader</param>
        /// <returns>provider</returns>
        static IDisposable GetReader(string assemblyPath, IntPtr inMemorySymbols, int inMemorySymbolsSize, out MetadataReader reader)
        {
            reader = null;
            MetadataReaderProvider provider = null;

            if (assemblyPath != null) {
                try {
                    uint stamp;
                    int age;
                    Guid guid;

                    string pdbName = GetPdbPathFromPeStream(assemblyPath, out age, out guid, out stamp);
                    if (pdbName != null) {
                        Stream pdbStream = File.OpenRead(pdbName);

                        // Need to always return provider so that it will be disposed
                        provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
                        MetadataReader rdr = provider.GetMetadataReader();

                        // Validate that the PDB matches the assembly version
                        if (age == 1) {
                            if (IdEquals(rdr.DebugMetadataHeader.Id, guid, stamp)) {
                                reader = rdr;
                            }
                        }
                    }
                }
                catch {
                }
            }
            else if (inMemorySymbols != IntPtr.Zero && inMemorySymbolsSize > 0) {
                unsafe {
                    provider = MetadataReaderProvider.FromPortablePdbImage((byte*)inMemorySymbols.ToPointer(), inMemorySymbolsSize);
                    reader = provider.GetMetadataReader();
                }
            }

            return provider;
        }

        /// <summary>
        /// Read the pdb file name and assembly version information from the PE file.
        /// </summary>
        /// <param name="assemblyPath">PE file</param>
        /// <param name="age">age</param>
        /// <param name="guid">assembly guid</param>
        /// <param name="stamp">time stamp</param>
        /// <returns>pdb name or null</returns>
        static string GetPdbPathFromPeStream(string assemblyPath, out int age, out Guid guid, out uint stamp)
        {
            try {
                Stream peStream = File.OpenRead(assemblyPath);

                using (var peReader = new PEReader(peStream)) {
                    foreach (var entry in peReader.ReadDebugDirectory()) {
                        if (entry.Type == DebugDirectoryEntryType.CodeView) {
                            CodeViewDebugDirectoryData codeViewData = peReader.ReadCodeViewDebugDirectoryData(entry);

                            stamp = entry.Stamp;
                            age = codeViewData.Age;
                            guid = codeViewData.Guid;

                            string peDirectory = Path.GetDirectoryName(assemblyPath);
                            return Path.Combine(peDirectory, Path.GetFileName(codeViewData.Path));
                        }
                    }
                }
            }
            catch {
            }

            stamp = 0;
            age = 0;
            guid = new Guid();
            return null;
        }

        /// <summary>
        /// Returns true if the portable pdb id matches the guid and stamp.
        /// </summary>
        static bool IdEquals(ImmutableArray<byte> left, Guid rightGuid, uint rightStamp)
        {
            if (left.Length != 20)
            {
                // invalid id
                return false;
            }

            byte[] guidBytes = rightGuid.ToByteArray();
            for (int i = 0; i < guidBytes.Length; i++)
            {
                if (guidBytes[i] != left[i])
                {
                    return false;
                }
            }

            byte[] stampBytes = BitConverter.GetBytes(rightStamp);
            for (int i = 0; i < stampBytes.Length; i++)
            {
                if (stampBytes[i] != left[guidBytes.Length + i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
