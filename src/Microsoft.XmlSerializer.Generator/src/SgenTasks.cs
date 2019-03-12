using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Microsoft.XmlSerializer.Generator
{
    public class SgenParameters : Task
    {
        public string Types { get; set; }
        public string References { get; set; }
        public bool UseProxyTypes { get; set; }
        public bool Verbose { get; set; }
        public string KeyFile { get; set; }
        public string KeyContainer { get; set; }
        public bool DelaySign { get; set; }

        public override bool Execute()
        {
            var sgenCommands = new List<string>();
            if(Types != null)
                sgenCommands.Add("--type " + Types);
            if (UseProxyTypes)
                sgenCommands.Add("--proxytypes");
            if (Verbose)
                sgenCommands.Add("--verbose");
            if (References != null)
            {
                sgenCommands.Add("--reference " + References);
            }

            var cscCommands = new List<string>();
            if (KeyFile != null)
                cscCommands.Add("-keyfile:" + KeyFile);
            if (KeyContainer != null)
                cscCommands.Add("-keycontainer:" + KeyContainer);
            if (DelaySign)
                cscCommands.Add("-delaysign");

            try
            {
                if(sgenCommands.Count > 0)
                {
                    string sgenRspPath = Environment.GetEnvironmentVariable("SgenRspFilePath");
                    File.WriteAllLines(sgenRspPath, sgenCommands.ToArray());
                }

                if(cscCommands.Count > 0)
                {
                    string cscRspPath = Environment.GetEnvironmentVariable("SgenCscRspFilePath");
                    File.WriteAllLines(cscRspPath, cscCommands.ToArray());
                }
            }
            catch (Exception e)
            {
                //sgenparameter task won't break the build even when exceptions are thrown, it will continue create serializer using default settings
                Log.LogError(string.Format("Unable to generate temporary response file, error: {0}", e.Message));
                Log.LogError("SgenParameters task failed. Serializer is generating using default parameters.");
                return true;
            }
           
            return true;
        }
        
    }

    public class SgenSetEnv : Task
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }

        public override bool Execute()
        {
            try
            {
                Environment.SetEnvironmentVariable(Name, Value);
            }
            catch(Exception e)
            {
                Log.LogError(string.Format("SgenSetEnv task failed, error: {0}", e.Message));
                return false;
            }
            return true;
        }
    }
}
