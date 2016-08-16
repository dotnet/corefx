// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Diagnostics
{
    /// <summary>
    /// Provides version information for a physical file on disk.
    /// </summary>
    public sealed partial class FileVersionInfo
    {
        internal FileVersionInfo() { }
        /// <summary>
        /// Gets the comments associated with the file.
        /// </summary>
        /// <returns>
        /// The comments associated with the file or null if the file did not contain version information.
        /// </returns>
        public string Comments { get { return default(string); } }
        /// <summary>
        /// Gets the name of the company that produced the file.
        /// </summary>
        /// <returns>
        /// The name of the company that produced the file or null if the file did not contain version
        /// information.
        /// </returns>
        public string CompanyName { get { return default(string); } }
        /// <summary>
        /// Gets the build number of the file.
        /// </summary>
        /// <returns>
        /// A value representing the build number of the file or null if the file did not contain version
        /// information.
        /// </returns>
        public int FileBuildPart { get { return default(int); } }
        /// <summary>
        /// Gets the description of the file.
        /// </summary>
        /// <returns>
        /// The description of the file or null if the file did not contain version information.
        /// </returns>
        public string FileDescription { get { return default(string); } }
        /// <summary>
        /// Gets the major part of the version number.
        /// </summary>
        /// <returns>
        /// A value representing the major part of the version number or 0 (zero) if the file did not contain
        /// version information.
        /// </returns>
        public int FileMajorPart { get { return default(int); } }
        /// <summary>
        /// Gets the minor part of the version number of the file.
        /// </summary>
        /// <returns>
        /// A value representing the minor part of the version number of the file or 0 (zero) if the file
        /// did not contain version information.
        /// </returns>
        public int FileMinorPart { get { return default(int); } }
        /// <summary>
        /// Gets the name of the file that this instance of <see cref="FileVersionInfo" />
        /// describes.
        /// </summary>
        /// <returns>
        /// The name of the file described by this instance of <see cref="FileVersionInfo" />.
        /// </returns>
        public string FileName { get { return default(string); } }
        /// <summary>
        /// Gets the file private part number.
        /// </summary>
        /// <returns>
        /// A value representing the file private part number or null if the file did not contain version
        /// information.
        /// </returns>
        public int FilePrivatePart { get { return default(int); } }
        /// <summary>
        /// Gets the file version number.
        /// </summary>
        /// <returns>
        /// The version number of the file or null if the file did not contain version information.
        /// </returns>
        public string FileVersion { get { return default(string); } }
        /// <summary>
        /// Gets the internal name of the file, if one exists.
        /// </summary>
        /// <returns>
        /// The internal name of the file. If none exists, this property will contain the original name
        /// of the file without the extension.
        /// </returns>
        public string InternalName { get { return default(string); } }
        /// <summary>
        /// Gets a value that specifies whether the file contains debugging information or is compiled
        /// with debugging features enabled.
        /// </summary>
        /// <returns>
        /// true if the file contains debugging information or is compiled with debugging features enabled;
        /// otherwise, false.
        /// </returns>
        public bool IsDebug { get { return default(bool); } }
        /// <summary>
        /// Gets a value that specifies whether the file has been modified and is not identical to the
        /// original shipping file of the same version number.
        /// </summary>
        /// <returns>
        /// true if the file is patched; otherwise, false.
        /// </returns>
        public bool IsPatched { get { return default(bool); } }
        /// <summary>
        /// Gets a value that specifies whether the file is a development version, rather than a commercially
        /// released product.
        /// </summary>
        /// <returns>
        /// true if the file is prerelease; otherwise, false.
        /// </returns>
        public bool IsPreRelease { get { return default(bool); } }
        /// <summary>
        /// Gets a value that specifies whether the file was built using standard release procedures.
        /// </summary>
        /// <returns>
        /// true if the file is a private build; false if the file was built using standard release procedures
        /// or if the file did not contain version information.
        /// </returns>
        public bool IsPrivateBuild { get { return default(bool); } }
        /// <summary>
        /// Gets a value that specifies whether the file is a special build.
        /// </summary>
        /// <returns>
        /// true if the file is a special build; otherwise, false.
        /// </returns>
        public bool IsSpecialBuild { get { return default(bool); } }
        /// <summary>
        /// Gets the default language string for the version info block.
        /// </summary>
        /// <returns>
        /// The description string for the Microsoft Language Identifier in the version resource or null
        /// if the file did not contain version information.
        /// </returns>
        public string Language { get { return default(string); } }
        /// <summary>
        /// Gets all copyright notices that apply to the specified file.
        /// </summary>
        /// <returns>
        /// The copyright notices that apply to the specified file.
        /// </returns>
        public string LegalCopyright { get { return default(string); } }
        /// <summary>
        /// Gets the trademarks and registered trademarks that apply to the file.
        /// </summary>
        /// <returns>
        /// The trademarks and registered trademarks that apply to the file or null if the file did not
        /// contain version information.
        /// </returns>
        public string LegalTrademarks { get { return default(string); } }
        /// <summary>
        /// Gets the name the file was created with.
        /// </summary>
        /// <returns>
        /// The name the file was created with or null if the file did not contain version information.
        /// </returns>
        public string OriginalFilename { get { return default(string); } }
        /// <summary>
        /// Gets information about a private version of the file.
        /// </summary>
        /// <returns>
        /// Information about a private version of the file or null if the file did not contain version
        /// information.
        /// </returns>
        public string PrivateBuild { get { return default(string); } }
        /// <summary>
        /// Gets the build number of the product this file is associated with.
        /// </summary>
        /// <returns>
        /// A value representing the build number of the product this file is associated with or null if
        /// the file did not contain version information.
        /// </returns>
        public int ProductBuildPart { get { return default(int); } }
        /// <summary>
        /// Gets the major part of the version number for the product this file is associated with.
        /// </summary>
        /// <returns>
        /// A value representing the major part of the product version number or null if the file did not
        /// contain version information.
        /// </returns>
        public int ProductMajorPart { get { return default(int); } }
        /// <summary>
        /// Gets the minor part of the version number for the product the file is associated with.
        /// </summary>
        /// <returns>
        /// A value representing the minor part of the product version number or null if the file did not
        /// contain version information.
        /// </returns>
        public int ProductMinorPart { get { return default(int); } }
        /// <summary>
        /// Gets the name of the product this file is distributed with.
        /// </summary>
        /// <returns>
        /// The name of the product this file is distributed with or null if the file did not contain version
        /// information.
        /// </returns>
        public string ProductName { get { return default(string); } }
        /// <summary>
        /// Gets the private part number of the product this file is associated with.
        /// </summary>
        /// <returns>
        /// A value representing the private part number of the product this file is associated with or
        /// null if the file did not contain version information.
        /// </returns>
        public int ProductPrivatePart { get { return default(int); } }
        /// <summary>
        /// Gets the version of the product this file is distributed with.
        /// </summary>
        /// <returns>
        /// The version of the product this file is distributed with or null if the file did not contain
        /// version information.
        /// </returns>
        public string ProductVersion { get { return default(string); } }
        /// <summary>
        /// Gets the special build information for the file.
        /// </summary>
        /// <returns>
        /// The special build information for the file or null if the file did not contain version information.
        /// </returns>
        public string SpecialBuild { get { return default(string); } }
        /// <summary>
        /// Returns a <see cref="FileVersionInfo" /> representing the version information
        /// associated with the specified file.
        /// </summary>
        /// <param name="fileName">
        /// The fully qualified path and name of the file to retrieve the version information for.
        /// </param>
        /// <returns>
        /// A <see cref="FileVersionInfo" /> containing information about the file.
        /// If the file did not contain version information, the <see cref="FileVersionInfo" />
        /// contains only the name of the file requested.
        /// </returns>
        /// <exception cref="IO.FileNotFoundException">The file specified cannot be found.</exception>
        public static System.Diagnostics.FileVersionInfo GetVersionInfo(string fileName) { return default(System.Diagnostics.FileVersionInfo); }
        /// <summary>
        /// Returns a partial list of properties in the <see cref="FileVersionInfo" />
        /// and their values.
        /// </summary>
        /// <returns>
        /// A list of the following properties in this class and their values:
        /// <see cref="FileName" />, <see cref="InternalName" />,
        /// <see cref="OriginalFilename" />, <see cref="FileVersion" />,
        /// <see cref="FileDescription" />, <see cref="ProductName" />,
        /// <see cref="ProductVersion" />, <see cref="IsDebug" />,
        /// <see cref="IsPatched" />, <see cref="IsPreRelease" />,
        /// <see cref="IsPrivateBuild" />, <see cref="IsSpecialBuild" />,
        /// <see cref="Language" />.If the file did not contain version information, this list will contain only the name of
        /// the requested file. Boolean values will be false, and all other entries will be null.
        /// </returns>
        public override string ToString() { return default(string); }
    }
}
