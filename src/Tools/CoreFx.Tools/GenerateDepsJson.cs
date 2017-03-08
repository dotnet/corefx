﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.DotNet.Build.Tasks
{
    public partial class GenerateDepsJson : BuildTask
    {
        [Required]
        public string DepsJsonPath { get; set; }

        public struct FileNameAssemblyPair
        {
            public AssemblyName AssemblyName;
            public string FileName;

            public FileNameAssemblyPair(AssemblyName assemblyName, string fileName)
            {
                AssemblyName = assemblyName;
                FileName = fileName;
            } 
        }

        public string RuntimeDirectory { get; set; }

        public ITaskItem[] DepsExceptions { get; set; }

        public string OutputPath { get; set; }

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(RuntimeDirectory))
            {
                RuntimeDirectory = Path.GetDirectoryName(DepsJsonPath);
            }
            List<string> filesInDir = Directory.EnumerateFiles(Path.GetDirectoryName(RuntimeDirectory)).ToList();

            List<FileNameAssemblyPair> assemblyNames = new List<FileNameAssemblyPair>();
            foreach (string file in filesInDir)
            {
                AssemblyName result;
                if (TryGetManagedAssemblyName(file, out result))
                {
                    assemblyNames.Add(new FileNameAssemblyPair(result, Path.GetFileNameWithoutExtension(file)));
                }
            }

            JObject newDepsJson = new JObject();

            using (TextReader projectFileReader = File.OpenText(DepsJsonPath))
            {
                JsonReader projectJsonReader = new JsonTextReader(projectFileReader);
                var serializer = new JsonSerializer();
                newDepsJson = serializer.Deserialize<JObject>(projectJsonReader);
            }

            string runtimeTarget = newDepsJson["runtimeTarget"]["name"].ToString();
            string rid = runtimeTarget.Split('/')[1];
            JObject targetsSection = (JObject) newDepsJson["targets"];

            JObject targetValue = new JObject();
            JObject libraryValue = new JObject();
            
            //Allow Exceptions to be preserved
            JObject runtimeTargetSection = (JObject) targetsSection[runtimeTarget];
            foreach (ITaskItem item in DepsExceptions)
            {
                foreach (var p in runtimeTargetSection.Properties())
                {
                    if (p.Name.Contains(item.ItemSpec))
                    {
                        targetValue.Add(p);
                        libraryValue.Add(p.Name, ConstructLibraryNode());
                    }
                }
            }

            targetsSection.Remove(runtimeTarget);
            newDepsJson.Remove("libraries");
            foreach (var assembly in assemblyNames)
            {
                JObject runtimes = new JObject();
                JObject runtimeLocation = new JObject();
                string key = $"{assembly.FileName}/{assembly.AssemblyName.Version.Major}.{assembly.AssemblyName.Version.Minor}.{assembly.AssemblyName.Version.Build}";
                runtimeLocation.Add(assembly.FileName + ".dll", new JObject());
                runtimes.Add("runtime", runtimeLocation);
                try
                {
                    targetValue.Add(key, runtimes);
                }
                catch (System.ArgumentException)
                {
                    
                }

                try
                {
                    libraryValue.Add(key, ConstructLibraryNode());
                }
                catch (System.ArgumentException)
                {

                }
            }
            targetsSection.Add(runtimeTarget, targetValue);
            newDepsJson.Add("libraries", libraryValue);

            // Delete mscorlib.dll references comming from CoreCLR package. They do not exist anymore.
            var mscorlibProperties = new List<JProperty>();
            foreach (var item in newDepsJson.Descendants())
            {
                var property = item as JProperty;
                if (property == null)
                    continue;

                var name = property.Name;
                if (name.EndsWith("/lib/netstandard1.0/mscorlib.dll") || name.EndsWith("/native/mscorlib.ni.dll"))
                    mscorlibProperties.Add(property);
            }
            foreach (var item in mscorlibProperties)
            {
                item.Remove();
            }

            if (!string.IsNullOrEmpty(OutputPath))
            {
                File.WriteAllText(OutputPath, newDepsJson.ToString());
            }
            else
            {
                File.WriteAllText(DepsJsonPath, newDepsJson.ToString());
            }

            return !Log.HasLoggedErrors;
        }

        private JObject ConstructLibraryNode()
        {
            JObject libValue = new JObject();
            libValue.Add("type", "package");
            libValue.Add("serviceable", true);
            libValue.Add("sha512", "");
            return libValue;
        }
    }
}
