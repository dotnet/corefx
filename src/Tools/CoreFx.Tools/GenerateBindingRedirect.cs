using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Microsoft.DotNet.Build.Tasks
{
    public class GenerateBindingRedirect : Task
    {
        [Required]
        public ITaskItem[] Assemblies { get; set; }

        [Required]
        public ITaskItem[] Executables { get; set; }

        [Required]
        public string OutputPath { get; set; }

        [Required]
        public bool OutputCodeBase { get; set; }

        private static XNamespace ns { get; set; }

        public override bool Execute()
        {
            XElement developmentMode = new XElement("developmentMode", new XAttribute("developerInstallation", true));
            XDocument doc = new XDocument(new XElement("configuration", new XElement("runtime", developmentMode)));
            foreach (ITaskItem executable in Executables)
            {
                string executableName = Path.GetFileName(executable.ItemSpec);
                using (FileStream fs = new FileStream(Path.Combine(OutputPath, executableName + ".config"), FileMode.Create))
                {
                    doc.Save(fs);
                }
            }
            
            return !Log.HasLoggedErrors;
        }
    }
}
