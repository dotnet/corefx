using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Build.Framework;

namespace Microsoft.DotNet.Build.Tasks
{
    public partial class GenerateXUnitRunnerConfig : BuildTask
    {
        [Required]
        public string RuntimeDirectory { get; set; }

        private XElement AssemblyBindingElement { get; set; }
        private static Version s_V4 = new Version(4, 0, 0, 0);
        private XNamespace xnamespace = "urn:schemas-microsoft-com:asm.v1";

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(RuntimeDirectory))
            {
                return false;
            }

            RuntimeDirectory = RuntimeDirectory.Replace("/", "\\");

            XDocument doc = new XDocument(new XDeclaration("1.0", null, null));
            XElement configuration = new XElement("configuration");
            doc.Add(configuration);
            XElement runtimeElement = new XElement("runtime");
            configuration.Add(runtimeElement);

            AssemblyBindingElement = new XElement(xnamespace + "assemblyBinding", new XAttribute("xmlns", "urn:schemas-microsoft-com:asm.v1"));
            runtimeElement.Add(AssemblyBindingElement);

            foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(RuntimeDirectory), "*.dll"))
            {
                AssemblyName result;
                if (AssemblyNameReader.TryGetManagedAssemblyName(file, out result))
                {
                    EmitBindingElements(result);
                }
            }

            using (var stream = new FileStream(RuntimeDirectory + "xunit.console.exe.config", FileMode.Create))
            {
                doc.Save(stream);
            }

            return !Log.HasLoggedErrors;
        }

        private void EmitBindingElements(AssemblyName assemblyName)
        {
            var dependentAssembly = new XElement(xnamespace + "dependentAssembly");
            AssemblyBindingElement.Add(dependentAssembly);

            var assemblyIdentity = assemblyName.GetPublicKeyToken() != null ?
                                    new XElement(xnamespace + "assemblyIdentity",
                                        new XAttribute [] {
                                            new XAttribute("name", assemblyName.Name),
                                            new XAttribute("publicKeyToken", BitConverter.ToString(assemblyName.GetPublicKeyToken()).Replace("-", null).ToLowerInvariant()),
                                            new XAttribute("culture", string.IsNullOrEmpty(assemblyName.CultureName) ? "neutral" : assemblyName.CultureName)
                                            }
                                        )
                                    :
                                    new XElement(xnamespace + "assemblyIdentity",
                                        new XAttribute [] {
                                            new XAttribute("name", assemblyName.Name),
                                            new XAttribute("culture", string.IsNullOrEmpty(assemblyName.CultureName) ? "neutral" : assemblyName.CultureName)
                                            }
                                        );
            dependentAssembly.Add(assemblyIdentity);

            if (assemblyName.Version.Major == 4 && assemblyName.Version > s_V4)
            {
                XElement bindingRedirect = new XElement(xnamespace + "bindingRedirect",
                                                new XAttribute[] {
                                                    new XAttribute("oldVersion", s_V4),
                                                    new XAttribute("newVersion", assemblyName.Version)
                                                    }
                                                );
                dependentAssembly.Add(bindingRedirect);
            }

            XElement codeBase = new XElement(xnamespace + "codeBase",
                                            new XAttribute[] {
                                                new XAttribute("version", assemblyName.Version),
                                                new XAttribute("href", RuntimeDirectory + assemblyName.Name + ".dll")
                                                }
                                            );
            dependentAssembly.Add(codeBase);
        }
    }
}
