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
        /// <param name="inMemoryAddress">in memory PE (assemblyPath != null) or PDB (assemblyPath == null) address or zero</param>
        /// <param name="inMemorySize">in memory PE or PDB size</param>
        /// <param name="methodToken">method token</param>
        /// <param name="ilOffset">il offset of the stack frame</param>
        /// <param name="sourceFile">source file return</param>
        /// <param name="sourceLine">line number return</param>
        /// <param name="sourceColumn">column return</param>
        public static void GetSourceLineInfo(string assemblyPath, IntPtr inMemoryAddress, int inMemorySize, 
            int methodToken, int ilOffset, out string sourceFile, out int sourceLine, out int sourceColumn)
        {
            sourceFile = null;
            sourceLine = 0;
            sourceColumn = 0;

            MetadataReader reader;
            using (GetReader(assemblyPath, inMemoryAddress, inMemorySize, out reader))
            {
                if (reader != null)
                {
                    Handle handle = MetadataTokens.Handle(methodToken);

                    if (handle.Kind == HandleKind.MethodDefinition)
                    {
                        MethodDebugInformationHandle methodDebugHandle = ((MethodDefinitionHandle)handle).ToDebugInformationHandle();
                        MethodDebugInformation methodInfo = reader.GetMethodDebugInformation(methodDebugHandle);

                        if (!methodInfo.SequencePointsBlob.IsNil)
                        {
                            SequencePointCollection sequencePoints = methodInfo.GetSequencePoints();

                            SequencePoint? bestPointSoFar = null;
                            foreach (SequencePoint point in sequencePoints)
                            {
                                if (point.Offset > ilOffset)
                                    break;

                                if (point.StartLine != SequencePoint.HiddenLine)
                                    bestPointSoFar = point;
                            }

                            if (bestPointSoFar.HasValue)
                            {
                                sourceLine = bestPointSoFar.Value.StartLine;
                                sourceColumn = bestPointSoFar.Value.StartColumn;
                                sourceFile = reader.GetString(reader.GetDocument(bestPointSoFar.Value.Document).Name);
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
        /// <param name="inMemoryAddress">in memory PE (assemblyPath != null) or PDB (assemblyPath == null) address or zero</param>
        /// <param name="inMemorySize">in memory PE or PDB size</param>
        /// <param name="reader">returns the reader</param>
        /// <returns>provider</returns>
        static IDisposable GetReader(string assemblyPath, IntPtr inMemoryAddress, int inMemorySize, out MetadataReader reader)
        {
            reader = null;
            MetadataReaderProvider provider = null;

            if (assemblyPath != null)
            {
                uint stamp;
                int age;
                Guid guid;

                string pdbName = GetPdbPathFromPeStream(assemblyPath, inMemoryAddress, inMemorySize, out age, out guid, out stamp);
                if (pdbName != null)
                {
                    try
                    {
                        Stream pdbStream = File.OpenRead(pdbName);

                        // Need to always return provider so that it will be disposed
                        provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
                        MetadataReader rdr = provider.GetMetadataReader();

                        // Validate that the PDB matches the assembly version
                        if (age == 1)
                        {
                            if (IdEquals(rdr.DebugMetadataHeader.Id, guid, stamp))
                            {
                                reader = rdr;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else if (inMemoryAddress != IntPtr.Zero && inMemorySize > 0)
            {
                unsafe
                {
                    provider = MetadataReaderProvider.FromPortablePdbImage((byte*)inMemoryAddress.ToPointer(), inMemorySize);
                    reader = provider.GetMetadataReader();
                }
            }

            return provider;
        }

        /// <summary>
        /// Read the pdb file name and assembly version information from the PE file.
        /// </summary>
        /// <param name="assemblyPath">PE file path</param>
        /// <param name="inMemoryAddress">in memory PE address</param>
        /// <param name="inMemorySize">in memory PE size</param>
        /// <param name="age">age</param>
        /// <param name="guid">assembly guid</param>
        /// <param name="stamp">time stamp</param>
        /// <returns>pdb name or null</returns>
        static unsafe string GetPdbPathFromPeStream(string assemblyPath, IntPtr inMemoryAddress, int inMemorySize, out int age, out Guid guid, out uint stamp)
        {
            try
            {
                if (inMemoryAddress != IntPtr.Zero && inMemorySize > 0)
                {
                    using (var peReader = new PEReader((byte*)inMemoryAddress.ToPointer(), inMemorySize))
                    {
                        foreach (var entry in peReader.ReadDebugDirectory())
                        {
                            if (entry.Type == DebugDirectoryEntryType.CodeView)
                            {
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
            }
            catch
            {
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
