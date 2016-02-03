// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace System.Diagnostics
{
    public sealed partial class FileVersionInfo
    {
        private FileVersionInfo(string fileName)
        {
            _fileName = fileName;

            // For managed assemblies, read the file version information from the assembly's metadata.
            // This isn't quite what's done on Windows, which uses the Win32 GetFileVersionInfo to read
            // the Win32 resource information from the file, and the managed compiler uses these attributes
            // to fill in that resource information when compiling the assembly.  It's possible
            // that after compilation, someone could have modified the resource information such that it
            // no longer matches what was or wasn't in the assembly.  But that's a rare enough case
            // that this should match for all intents and purposes.  If this ever becomes a problem,
            // we can implement a full-fledged Win32 resource parser; that would also enable support
            // for native Win32 PE files on Unix, but that should also be an extremely rare case.
            if (!TryLoadManagedAssemblyMetadata())
            {
                // We could try to parse Executable and Linkable Format (ELF) files, but at present
                // for executables they don't store version information, which is typically just
                // available in the filename itself.  For now, we won't do anything special, but
                // we can add more cases here as we find need and opportunity.
            }
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------

        /// <summary>Attempt to load our fields from the metadata of the file, if it's a managed assembly.</summary>
        /// <returns>true if the file is a managed assembly; otherwise, false.</returns>
        private bool TryLoadManagedAssemblyMetadata()
        {
            try
            {
                // Try to load the file using the managed metadata reader
                using (FileStream assemblyStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 0x1000, useAsync: false))
                using (PEReader peReader = new PEReader(assemblyStream))
                {
                    if (peReader.HasMetadata)
                    {
                        MetadataReader metadataReader = peReader.GetMetadataReader();
                        if (metadataReader.IsAssembly)
                        {
                            LoadManagedAssemblyMetadata(metadataReader);
                            return true;
                        }
                    }
                }
            }
            catch (BadImageFormatException) { }
            return false;
        }

        /// <summary>Load our fields from the metadata of the file as represented by the provided metadata reader.</summary>
        /// <param name="metadataReader">The metadata reader for the CLI file this represents.</param>
        private void LoadManagedAssemblyMetadata(MetadataReader metadataReader)
        {
            AssemblyDefinition assemblyDefinition = metadataReader.GetAssemblyDefinition();

            // Set the internal and original names based on the file name.
            _internalName = _originalFilename = Path.GetFileName(_fileName);

            // Set the product version based on the assembly's version (this may be overwritten 
            // later in the method).
            Version productVersion = assemblyDefinition.Version;
            _productVersion = productVersion.ToString();
            _productMajor = productVersion.Major;
            _productMinor = productVersion.Minor;
            _productBuild = productVersion.Build != -1 ? productVersion.Build : 0;
            _productPrivate = productVersion.Revision != -1 ? productVersion.Revision : 0;

            // "Language Neutral" is used on Win32 for unknown language identifiers.
            _language = "Language Neutral";

            // Set other fields to default values in case they're not overwritten by attributes
            _companyName = string.Empty;
            _comments = string.Empty;
            _fileDescription = " "; // this is what the managed compiler outputs when value isn't set
            _fileVersion = string.Empty;
            _legalCopyright = " "; // this is what the managed compiler outputs when value isn't set
            _legalTrademarks = string.Empty;
            _productName = string.Empty;
            _privateBuild = string.Empty;
            _specialBuild = string.Empty;

            // Be explicit about initialization to suppress warning about fields not being set
            _isDebug = false;
            _isPatched = false;
            _isPreRelease = false;
            _isPrivateBuild = false;
            _isSpecialBuild = false;

            // Everything else is parsed from assembly attributes
            MetadataStringComparer comparer = metadataReader.StringComparer;
            foreach (CustomAttributeHandle attrHandle in assemblyDefinition.GetCustomAttributes())
            {
                CustomAttribute attr = metadataReader.GetCustomAttribute(attrHandle);
                StringHandle typeNamespaceHandle = default(StringHandle), typeNameHandle = default(StringHandle);
                if (TryGetAttributeName(metadataReader, attr, out typeNamespaceHandle, out typeNameHandle) &&
                    comparer.Equals(typeNamespaceHandle, "System.Reflection"))
                {
                    if (comparer.Equals(typeNameHandle, "AssemblyCompanyAttribute"))
                    {
                        GetStringAttributeArgumentValue(metadataReader, attr, ref _companyName);
                    }
                    else if (comparer.Equals(typeNameHandle, "AssemblyCopyrightAttribute"))
                    {
                        GetStringAttributeArgumentValue(metadataReader, attr, ref _legalCopyright);
                    }
                    else if (comparer.Equals(typeNameHandle, "AssemblyDescriptionAttribute"))
                    {
                        GetStringAttributeArgumentValue(metadataReader, attr, ref _comments);
                    }
                    else if (comparer.Equals(typeNameHandle, "AssemblyFileVersionAttribute"))
                    {
                        string versionString = string.Empty;
                        GetStringAttributeArgumentValue(metadataReader, attr, ref versionString);
                        Version v;
                        if (Version.TryParse(versionString, out v))
                        {
                            _fileVersion = v.ToString();
                            _fileMajor = v.Major;
                            _fileMinor = v.Minor;
                            _fileBuild = v.Build != -1 ? v.Build : 0;
                            _filePrivate = v.Revision != -1 ? v.Revision : 0;

                            // When the managed compiler sees an [AssemblyVersion(...)] attribute, it uses that to set 
                            // both the assembly version and the product version in the Win32 resources. If it doesn't 
                            // see an [AssemblyVersion(...)], then it sets the assembly version to 0.0.0.0, however it 
                            // sets the product version in the Win32 resources to whatever was defined in the 
                            // [AssemblyFileVersionAttribute(...)] if there was one.  Without parsing the Win32 resources,
                            // we can't differentiate these two cases, so given the rarity of explicitly setting an 
                            // assembly's version number to 0.0.0.0, we assume that if it is 0.0.0.0 then the attribute 
                            // wasn't specified and we use the file version.
                            if (_productVersion == "0.0.0.0")
                            {
                                _productVersion = _fileVersion;
                                _productMajor = _fileMajor;
                                _productMinor = _fileMinor;
                                _productBuild = _fileBuild;
                                _productPrivate = _filePrivate;
                            }
                        }
                    }
                    else if (comparer.Equals(typeNameHandle, "AssemblyProductAttribute"))
                    {
                        GetStringAttributeArgumentValue(metadataReader, attr, ref _productName);
                    }
                    else if (comparer.Equals(typeNameHandle, "AssemblyTrademarkAttribute"))
                    {
                        GetStringAttributeArgumentValue(metadataReader, attr, ref _legalTrademarks);
                    }
                    else if (comparer.Equals(typeNameHandle, "AssemblyTitleAttribute"))
                    {
                        GetStringAttributeArgumentValue(metadataReader, attr, ref _fileDescription);
                    }
                }
            }
        }

        /// <summary>Gets the name of an attribute.</summary>
        /// <param name="reader">The metadata reader.</param>
        /// <param name="attr">The attribute.</param>
        /// <param name="typeNamespaceHandle">The namespace of the attribute.</param>
        /// <param name="typeNameHandle">The name of the attribute.</param>
        /// <returns>true if the name could be retrieved; otherwise, false.</returns>
        private static bool TryGetAttributeName(MetadataReader reader, CustomAttribute attr, out StringHandle typeNamespaceHandle, out StringHandle typeNameHandle)
        {
            EntityHandle ctorHandle = attr.Constructor;
            switch (ctorHandle.Kind)
            {
                case HandleKind.MemberReference:
                    EntityHandle container = reader.GetMemberReference((MemberReferenceHandle)ctorHandle).Parent;
                    if (container.Kind == HandleKind.TypeReference)
                    {
                        TypeReference tr = reader.GetTypeReference((TypeReferenceHandle)container);
                        typeNamespaceHandle = tr.Namespace;
                        typeNameHandle = tr.Name;
                        return true;
                    }
                    break;

                case HandleKind.MethodDefinition:
                    MethodDefinition md = reader.GetMethodDefinition((MethodDefinitionHandle)ctorHandle);
                    TypeDefinition td = reader.GetTypeDefinition(md.GetDeclaringType());
                    typeNamespaceHandle = td.Namespace;
                    typeNameHandle = td.Name;
                    return true;
            }

            // Unusual case, potentially invalid IL
            typeNamespaceHandle = default(StringHandle);
            typeNameHandle = default(StringHandle);
            return false;
        }

        /// <summary>Gets the string argument value of an attribute with a single fixed string argument.</summary>
        /// <param name="reader">The metadata reader.</param>
        /// <param name="attr">The attribute.</param>
        /// <param name="value">The value parsed from the attribute, if it could be retrieved; otherwise, the value is left unmodified.</param>
        private static void GetStringAttributeArgumentValue(MetadataReader reader, CustomAttribute attr, ref string value)
        {
            EntityHandle ctorHandle = attr.Constructor;
            BlobHandle signature;
            switch (ctorHandle.Kind)
            {
                case HandleKind.MemberReference:
                    signature = reader.GetMemberReference((MemberReferenceHandle)ctorHandle).Signature;
                    break;
                case HandleKind.MethodDefinition:
                    signature = reader.GetMethodDefinition((MethodDefinitionHandle)ctorHandle).Signature;
                    break;
                default:
                    // Unusual case, potentially invalid IL
                    return;
            }

            BlobReader signatureReader = reader.GetBlobReader(signature);
            BlobReader valueReader = reader.GetBlobReader(attr.Value);

            const ushort Prolog = 1; // two-byte "prolog" defined by Ecma 335 (II.23.3) to be at the beginning of attribute value blobs
            if (valueReader.ReadUInt16() == Prolog)
            {
                SignatureHeader header = signatureReader.ReadSignatureHeader();
                int parameterCount;
                if (header.Kind == SignatureKind.Method &&                               // attr ctor must be a method
                    !header.IsGeneric &&                                                 // attr ctor must be non-generic
                    signatureReader.TryReadCompressedInteger(out parameterCount) &&      // read parameter count
                    parameterCount == 1 &&                                               // attr ctor must have 1 parameter
                    signatureReader.ReadSignatureTypeCode() == SignatureTypeCode.Void && // attr ctor return type must be void
                    signatureReader.ReadSignatureTypeCode() == SignatureTypeCode.String) // attr ctor first parameter must be string
                {
                    value = valueReader.ReadSerializedString();
                }
            }
        }

    }
}
