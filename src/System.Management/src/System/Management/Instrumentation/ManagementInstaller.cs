namespace System.Management.Instrumentation
{
    using System;
    using System.Reflection;
    using System.Collections;
    using System.Configuration.Install;
    using System.Text;
    using System.IO;
    using System.Runtime.Versioning;
    using System.Globalization;
    using System.Security.Permissions;

    /// <summary>
    ///    <para>Installs instrumented assemblies. Include an instance of this installer class in the project installer for
    ///       an assembly that includes instrumentation.</para>
    /// </summary>
    /// <remarks>
    /// <para> If this is the only installer for your application, you may use the helper class <see cref='System.Management.Instrumentation.DefaultManagementProjectInstaller'/>
    /// provided in this namespace. </para>
    /// </remarks>
    /// <example>
    ///    <para>If you have a master project installer for your 
    ///       project, add the following code to your project installers constructor:</para>
    ///    <code lang='C#'>// Instantiate installer for assembly.
    /// ManagementInstaller managementInstaller = new ManagementInstaller();
    /// 
    /// // Add installer to collection.
    /// Installers.Add(managementInstaller);
    ///    </code>
    ///    <code lang='VB'>'Instantiate installer for assembly.
    /// Dim managementInstaller As New ManagementInstaller()
    /// 
    /// 'Add installer to collection.
    /// Installers.Add(managementInstaller)
    ///    </code>
    /// </example>
    public class ManagementInstaller : Installer
	{
        // TODO: Is this correct
        private static bool helpPrinted = false;
        /// <summary>
        ///    <para>Gets or sets installer options for this class.</para>
        /// </summary>
        /// <value>
        ///    <para>The help text for all the installers in the installer collection, including
        ///       the description of what each installer does and the command-line options (for
        ///       the installation program) that can be passed to and understood by each
        ///       installer.</para>
        /// </value>
        public override string HelpText {
            get {
                if (helpPrinted)
                    return base.HelpText;
                else {
                    helpPrinted = true;
//                    return Res.GetString(Res.HelpText) + "\r\n" + base.HelpText;
                    // TODO: Localize
                    StringBuilder help = new StringBuilder();
                    help.Append("/MOF=[filename]\r\n");
					help.Append(" " + RC.GetString("FILETOWRITE_MOF")+"\r\n\r\n");

					//
					// [RAID: 123895]
					// If the force parameter is present, we update registration information independent if it already
					// exists.
					//
					help.Append ( "/Force or /F\r\n" ) ;
					help.Append(" " + RC.GetString("FORCE_UPDATE"));
                    return help.ToString() + base.HelpText;
                }
            }
        }

    
		/// <summary>
		///    <para>Installs the assembly.</para>
		/// </summary>
		/// <param name='savedState'>The state of the assembly.</param>
             [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
		public override void Install(IDictionary savedState)
		{
			// Demand IO permission required for LoadFrom FXCop 191096 
			FileIOPermission ioPermission = new FileIOPermission(FileIOPermissionAccess.Read, (string)Context.Parameters["assemblypath"]);
			ioPermission.Demand();

			base.Install(savedState);
            // TODO: Localize
            Context.LogMessage(RC.GetString("WMISCHEMA_INSTALLATIONSTART"));

            string assemblyPath = Context.Parameters["assemblypath"];
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            SchemaNaming naming = SchemaNaming.GetSchemaNaming(assembly);
			
			//
			// We always use the full version number for Whidbey.
			//
			naming.DecoupledProviderInstanceName = AssemblyNameUtility.UniqueToAssemblyFullVersion(assembly);
            // See if this assembly provides instrumentation
            if(null == naming)
                return;

			//
			// [RAID: 123895]
			// If the force parameter is present, we update registration information independent if it already
			// exists.
			//
            if( ( naming.IsAssemblyRegistered() == false ) || ( Context.Parameters.ContainsKey ( "force" ) ) || ( Context.Parameters.ContainsKey ( "f" ) ) )
            {
                Context.LogMessage(RC.GetString("REGESTRING_ASSEMBLY") + " " + naming.DecoupledProviderInstanceName);

                naming.RegisterNonAssemblySpecificSchema(Context);
                naming.RegisterAssemblySpecificSchema();
            }
            mof = naming.Mof;

            Context.LogMessage(RC.GetString("WMISCHEMA_INSTALLATIONEND"));
		}

        string mof;

		/// <summary>
		///    <para>Commits the assembly to the operation.</para>
		/// </summary>
		/// <param name='savedState'>The state of the assembly.</param>
              [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
		public override void Commit(IDictionary savedState) {
			base.Commit(savedState);

            // See if we were asked to generate a MOF file
            if(Context.Parameters.ContainsKey("mof"))
            {
                string mofFile = Context.Parameters["mof"];

                // bug#62252 - Pick a default MOF file name
                if(mofFile == null || mofFile.Length == 0)
                {
                    mofFile = Context.Parameters["assemblypath"];
                    if(mofFile == null || mofFile.Length == 0)
                        mofFile = "defaultmoffile";
                    else
                        mofFile = Path.GetFileName(mofFile);
                }

                // Append '.mof' in necessary
                if(mofFile.Length<4)
                    mofFile += ".mof";
                else
                {
                    if(String.Compare(mofFile.Substring(mofFile.Length-4,4),".mof",StringComparison.OrdinalIgnoreCase)!=0)
                        mofFile += ".mof";
                }
                Context.LogMessage(RC.GetString("MOFFILE_GENERATING") + " " + mofFile);
                using(StreamWriter log = new StreamWriter(mofFile, false, Encoding.Unicode))
                {
                    log.WriteLine("//**************************************************************************");
                    log.WriteLine("//* {0}", mofFile);
                    log.WriteLine("//**************************************************************************");
                    log.WriteLine(mof);
                }
            }
		}
		/// <summary>
		///    <para>Rolls back the state of the assembly.</para>
		/// </summary>
		/// <param name='savedState'>The state of the assembly.</param>
		public override void Rollback(IDictionary savedState) {
			base.Rollback(savedState);
		}

		/// <summary>
		///    <para>Uninstalls the assembly.</para>
		/// </summary>
		/// <param name='savedState'>The state of the assembly.</param>
		public override void Uninstall(IDictionary savedState) {
			base.Uninstall(savedState);
		}
	}

    /// <summary>
    ///    <para> Installs an instrumented assembly. This class is a default project installer for assemblies that contain
    ///       management instrumentation and do not use other installers (such as services, or message
    ///       queues). To use this default project installer, simply derive a class from
    ///    <see cref='System.Management.Instrumentation.DefaultManagementProjectInstaller'/> inside the assembly. No methods need
    ///       to be overridden.</para>
    /// </summary>
    /// <remarks>
    ///    <para>If your project has a master project
    ///       installer, use the <see cref='System.Management.Instrumentation.ManagementInstaller'/> class instead.</para>
    /// </remarks>
    /// <example>
    ///    <para>Add the following code to your instrumented assembly to enable the installation step:</para>
    ///    <code lang='C#'>[System.ComponentModel.RunInstaller(true)]
    /// public class MyInstaller : DefaultManagementProjectInstaller {}
    ///    </code>
    ///    <code lang='VB'>&lt;System.ComponentModel.RunInstaller(true)&gt;
    /// public class MyInstaller
    ///     Inherits DefaultManagementProjectInstaller
    ///    </code>
    /// </example>
    public class DefaultManagementProjectInstaller : Installer
    {
        /// <summary>
        ///    <para>Initializes a new instance of the 
        ///    <see cref='System.Management.Instrumentation.DefaultManagementProjectInstaller'/> class. This is the default constructor.</para>
        /// </summary>
        public DefaultManagementProjectInstaller()
        {
            // Instantiate installer for assembly.
            ManagementInstaller managementInstaller = new ManagementInstaller();

            // Add installers to collection. Order is not important.
            Installers.Add(managementInstaller);
        }        
    }
}
