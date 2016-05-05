// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Resources
{
    /// <summary>
    /// The exception that is thrown if the main assembly does not contain the resources for the neutral
    /// culture, and an appropriate satellite assembly is missing.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public partial class MissingManifestResourceException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingManifestResourceException" />
        /// class with default properties.
        /// </summary>
        public MissingManifestResourceException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingManifestResourceException" />
        /// class with the specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MissingManifestResourceException(string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingManifestResourceException" />
        /// class with a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the <paramref name="inner" />
        /// parameter is not null, the current exception is raised in a catch block that handles the inner
        /// exception.
        /// </param>
        public MissingManifestResourceException(string message, System.Exception inner) { }
    }
    /// <summary>
    /// Informs the resource manager of an app's default culture. This class cannot be inherited.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple = false)]
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class NeutralResourcesLanguageAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NeutralResourcesLanguageAttribute" />
        /// class.
        /// </summary>
        /// <param name="cultureName">
        /// The name of the culture that the current assembly's neutral resources were written in.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="cultureName" /> parameter is null.</exception>
        public NeutralResourcesLanguageAttribute(string cultureName) { }
        /// <summary>
        /// Gets the culture name.
        /// </summary>
        /// <returns>
        /// The name of the default culture for the main assembly.
        /// </returns>
        public string CultureName { get { return default(string); } }
    }
    /// <summary>
    /// Represents a resource manager that provides convenient access to culture-specific resources
    /// at run time.Security Note: Calling methods in this class with untrusted data is a security risk. Call
    /// the methods in the class only with trusted data. For more information, see Untrusted Data Security
    /// Risks.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public partial class ResourceManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager" /> class
        /// that looks up resources contained in files with the specified root name in the given assembly.
        /// </summary>
        /// <param name="baseName">
        /// The root name of the resource file without its extension but including any fully qualified
        /// namespace name. For example, the root name for the resource file named MyApplication.MyResource.en-US.resources
        /// is MyApplication.MyResource.
        /// </param>
        /// <param name="assembly">The main assembly for the resources.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="baseName" /> or <paramref name="assembly" /> parameter is null.
        /// </exception>
        public ResourceManager(string baseName, System.Reflection.Assembly assembly) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager" /> class
        /// that looks up resources in satellite assemblies based on information from the specified type
        /// object.
        /// </summary>
        /// <param name="resourceSource">
        /// A type from which the resource manager derives all information for finding .resources files.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="resourceSource" /> parameter is null.
        /// </exception>
        public ResourceManager(System.Type resourceSource) { }
        /// <summary>
        /// Returns the value of the specified string resource.
        /// </summary>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <returns>
        /// The value of the resource localized for the caller's current UI culture, or null if <paramref name="name" />
        /// cannot be found in a resource set.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="name" /> parameter is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The value of the specified resource is not a string.
        /// </exception>
        /// <exception cref="MissingManifestResourceException">
        /// No usable set of resources has been found, and there are no resources for the default culture.
        /// For information about how to handle this exception, see the "Handling MissingManifestResourceException
        /// and MissingSatelliteAssemblyException Exceptions" section in the
        /// <see cref="ResourceManager" /> class topic.
        /// </exception>
        /// <exception cref="MissingSatelliteAssemblyException">
        /// The default culture's resources reside in a satellite assembly that could not be found. For
        /// information about how to handle this exception, see the "Handling MissingManifestResourceException
        /// and MissingSatelliteAssemblyException Exceptions" section in the <see cref="ResourceManager" />
        /// class topic.
        /// </exception>
        public string GetString(string name) { return default(string); }
        /// <summary>
        /// Returns the value of the string resource localized for the specified culture.
        /// </summary>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <returns>
        /// The value of the resource localized for the specified culture, or null if <paramref name="name" />
        /// cannot be found in a resource set.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="name" /> parameter is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The value of the specified resource is not a string.
        /// </exception>
        /// <exception cref="MissingManifestResourceException">
        /// No usable set of resources has been found, and there are no resources for a default culture.
        /// For information about how to handle this exception, see the "Handling MissingManifestResourceException
        /// and MissingSatelliteAssemblyException Exceptions" section in the <see cref="ResourceManager" />
        /// class topic.
        /// </exception>
        /// <exception cref="MissingSatelliteAssemblyException">
        /// The default culture's resources reside in a satellite assembly that could not be found. For
        /// information about how to handle this exception, see the "Handling MissingManifestResourceException
        /// and MissingSatelliteAssemblyException Exceptions" section in the <see cref="ResourceManager" />
        /// class topic.
        /// </exception>
        public virtual string GetString(string name, System.Globalization.CultureInfo culture) { return default(string); }
    }
    /// <summary>
    /// Instructs a <see cref="ResourceManager" /> object to ask for a particular
    /// version of a satellite assembly.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple = false)]
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class SatelliteContractVersionAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SatelliteContractVersionAttribute" />
        /// class.
        /// </summary>
        /// <param name="version">A string that specifies the version of the satellite assemblies to load.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="version" /> parameter is null.</exception>
        public SatelliteContractVersionAttribute(string version) { }
        /// <summary>
        /// Gets the version of the satellite assemblies with the required resources.
        /// </summary>
        /// <returns>
        /// A string that contains the version of the satellite assemblies with the required resources.
        /// </returns>
        public string Version { get { return default(string); } }
    }
}
