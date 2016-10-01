// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml.Serialization
{
    using System.Configuration;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections;
    using System.IO;
    using System;
    using System.Text;
    using System.Xml;
    using System.Threading;
    using System.Security;
    using System.Security.Policy;
    using System.Xml.Serialization.Configuration;
    using System.Diagnostics;
    using System.CodeDom.Compiler;
    using System.Globalization;
    using System.Runtime.Versioning;
    using System.Diagnostics.CodeAnalysis;
    using System.Collections.Generic;
	using System.Xml.Extensions;
    using System.Linq;

    internal class TempAssembly
    {
        internal const string GeneratedAssemblyNamespace = "Microsoft.Xml.Serialization.GeneratedAssembly";
        private Assembly _assembly;
        private XmlSerializerImplementation _contract = null;
        private IDictionary _writerMethods;
        private IDictionary _readerMethods;
        private TempMethodDictionary _methods;
        private Hashtable _assemblies = new Hashtable();

        internal class TempMethod
        {
            internal MethodInfo writeMethod;
            internal MethodInfo readMethod;
            internal string name;
            internal string ns;
            internal bool isSoap;
            internal string methodKey;
        }

        private TempAssembly()
        {
        }

        internal TempAssembly(XmlMapping[] xmlMappings, Type[] types, string defaultNamespace, string location, Evidence evidence)
        {
#if !NET_NATIVE
            bool containsSoapMapping = false;
            for (int i = 0; i < xmlMappings.Length; i++)
            {
                xmlMappings[i].CheckShallow();
                if (xmlMappings[i].IsSoap)
                {
                    containsSoapMapping = true;
                }
            }

            // We will make best effort to use RefEmit for assembly generation
            bool fallbackToCSharpAssemblyGeneration = false;

            if (!containsSoapMapping && !TempAssembly.UseLegacySerializerGeneration)
            {
                try
                {
                    _assembly = GenerateRefEmitAssembly(xmlMappings, types, defaultNamespace, evidence);
                }
                // Only catch and handle known failures with RefEmit
                catch (CodeGeneratorConversionException)
                {
                    fallbackToCSharpAssemblyGeneration = true;
                }
                // Add other known exceptions here...
                //
            }
            else
            {
                fallbackToCSharpAssemblyGeneration = true;
            }

            if (fallbackToCSharpAssemblyGeneration)
            {
                _assembly = GenerateAssembly(xmlMappings, types, defaultNamespace, evidence, XmlSerializerCompilerParameters.Create(location), null, _assemblies);
            }
#endif

#if DEBUG
            // use exception in the place of Debug.Assert to avoid throwing asserts from a server process such as aspnet_ewp.exe
            if (_assembly == null) throw new InvalidOperationException(SR.Format(SR.XmlInternalErrorDetails, "Failed to generate XmlSerializer assembly, but did not throw"));
#endif
            InitAssemblyMethods(xmlMappings);
        }

        internal TempAssembly(XmlMapping[] xmlMappings, Assembly assembly, XmlSerializerImplementation contract)
        {
            _assembly = assembly;
            InitAssemblyMethods(xmlMappings);
            _contract = contract;
        }

        internal static bool UseLegacySerializerGeneration
        {
            get
            {
                return false;
            }
        }

        internal TempAssembly(XmlSerializerImplementation contract)
        {
            _contract = contract;
        }

        internal XmlSerializerImplementation Contract
        {
            get
            {
                if (_contract == null)
                {
                    _contract = (XmlSerializerImplementation)Activator.CreateInstance(GetTypeFromAssembly(_assembly, "XmlSerializerContract"));
                }
                return _contract;
            }
        }

        internal void InitAssemblyMethods(XmlMapping[] xmlMappings)
        {
            _methods = new TempMethodDictionary();
            for (int i = 0; i < xmlMappings.Length; i++)
            {
                TempMethod method = new TempMethod();
                method.isSoap = xmlMappings[i].IsSoap;
                method.methodKey = xmlMappings[i].Key;
                XmlTypeMapping xmlTypeMapping = xmlMappings[i] as XmlTypeMapping;
                if (xmlTypeMapping != null)
                {
                    method.name = xmlTypeMapping.ElementName;
                    method.ns = xmlTypeMapping.Namespace;
                }
                _methods.Add(xmlMappings[i].Key, method);
            }
        }

        /// <devdoc>
        ///    <para>
        ///    Attempts to load pre-generated serialization assembly.
        ///    First check for the [XmlSerializerAssembly] attribute
        ///    </para>
        /// </devdoc>
        // SxS: This method does not take any resource name and does not expose any resources to the caller.
        // It's OK to suppress the SxS warning.
        internal static Assembly LoadGeneratedAssembly(Type type, string defaultNamespace, out XmlSerializerImplementation contract)
        {
            Assembly serializer = null;
            contract = null;
            string serializerName = null;

            //BinCompat TODO: Check if this needs to come back
            // Packaged apps do not support loading generated serializers.
            /*if (Microsoft.Win32.UnsafeNativeMethods.IsPackagedProcess.Value)
            {
                return null;
            }*/

            // check to see if we loading explicit pre-generated assembly
            object[] attrs = type.GetCustomAttributes(typeof(XmlSerializerAssemblyAttribute), false);
            if (attrs.Length == 0)
            {
                // Guess serializer name: if parent assembly signed use strong name 
                AssemblyName name = type.GetTypeInfo().Assembly.GetName();
                serializerName = Compiler.GetTempAssemblyName(name, defaultNamespace);
                // use strong name 
                name.Name = serializerName;
                //BinCompat TODO: Check if this was actually needed
                //name.CodeBase = null;
                //name.CultureInfo = CultureInfo.InvariantCulture;
                try
                {
                    serializer = Assembly.Load(name);
                }
                catch (Exception e)
                {
                    if (e is OutOfMemoryException)
                    {
                        throw;
                    }
                    byte[] token = name.GetPublicKeyToken();
                    if (token != null && token.Length > 0)
                    {
                        // the parent assembly was signed, so do not try to LoadWithPartialName
                        return null;
                    }
                    //BinCompat TODO: Test this
                    throw new NotImplementedException(string.Format("Could not load assembly ", name));
                    /*#pragma warning disable 618
                                        serializer = Assembly.LoadWithPartialName(serializerName, null);
#pragma warning restore 618*/
                }
                if (serializer == null)
                {
                    return null;
                }
            }
            else
            {
                XmlSerializerAssemblyAttribute assemblyAttribute = (XmlSerializerAssemblyAttribute)attrs[0];
                if (assemblyAttribute.AssemblyName != null && assemblyAttribute.CodeBase != null)
                    throw new InvalidOperationException(SR.Format(SR.XmlPregenInvalidXmlSerializerAssemblyAttribute, "AssemblyName", "CodeBase"));

                // found XmlSerializerAssemblyAttribute attribute, it should have all needed information to load the pre-generated serializer
                if (assemblyAttribute.AssemblyName != null)
                {
                    serializerName = assemblyAttribute.AssemblyName;
                    //BinCompat TODO: Test this
                    throw new NotImplementedException(string.Format("Could not load assembly ", serializerName));
                    /*#pragma warning disable 618
                                        serializer = Assembly.LoadWithPartialName(serializerName, null);
#pragma warning restore 618*/
                }
                else if (assemblyAttribute.CodeBase != null && assemblyAttribute.CodeBase.Length > 0)
                {
                    serializerName = assemblyAttribute.CodeBase;
                    //BinCompat TODO: Test this
                    throw new NotImplementedException(string.Format("Could not load assembly ", serializerName));
                    //serializer = Assembly.LoadFrom(serializerName);
                }
                else
                {
                    serializerName = type.GetTypeInfo().Assembly.FullName;
                    serializer = type.GetTypeInfo().Assembly;
                }
                if (serializer == null)
                {
                    throw new FileNotFoundException(null, serializerName);
                }
            }
            Type contractType = GetTypeFromAssembly(serializer, "XmlSerializerContract");
            contract = (XmlSerializerImplementation)Activator.CreateInstance(contractType);
            if (contract.CanSerialize(type))
                return serializer;

            return null;
        }

        // SxS: This method does not take any resource name and does not expose any resources to the caller.
        // It's OK to suppress the SxS warning.
        private static string GenerateAssemblyId(Type type)
        {
            //BinCompat TODO
            throw new NotImplementedException();

            /*Module[] modules = type.GetTypeInfo().Assembly.GetModules();
            ArrayList list = new ArrayList();
            for (int i = 0; i < modules.Length; i++)
            {
                list.Add(modules[i].ModuleVersionId.ToString());
            }
            list.Sort();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i].ToString());
                sb.Append(",");
            }
            return sb.ToString();*/
        }

        internal static Assembly GenerateAssembly(XmlMapping[] xmlMappings, Type[] types, string defaultNamespace, Evidence evidence, XmlSerializerCompilerParameters parameters, Assembly assembly, Hashtable assemblies)
        {
            Compiler compiler = new Compiler();
            try
            {
                Hashtable scopeTable = new Hashtable();
                foreach (XmlMapping mapping in xmlMappings)
                    scopeTable[mapping.Scope] = mapping;
                TypeScope[] scopes = new TypeScope[scopeTable.Keys.Count];
                scopeTable.Keys.CopyTo(scopes, 0);

                assemblies.Clear();
                Hashtable importedTypes = new Hashtable();
                foreach (TypeScope scope in scopes)
                {
                    foreach (Type t in scope.Types)
                    {
                        compiler.AddImport(t, importedTypes);
                        Assembly a = t.GetTypeInfo().Assembly;
                        string name = a.FullName;
                        if (assemblies[name] != null)
                            continue;
                    }
                }
                for (int i = 0; i < types.Length; i++)
                {
                    compiler.AddImport(types[i], importedTypes);
                }
                compiler.AddImport(typeof(object).GetTypeInfo().Assembly);
                compiler.AddImport(typeof(XmlSerializer).GetTypeInfo().Assembly);

                IndentedWriter writer = new IndentedWriter(compiler.Source, false);

                // Add AssemblyVersion attribute to match parent accembly version
                if (types != null && types.Length > 0 && types[0] != null)
                {
                    writer.WriteLine("[assembly:System.Reflection.AssemblyVersionAttribute(\"" + types[0].GetTypeInfo().Assembly.GetName().Version.ToString() + "\")]");
                }
                if (assembly != null && types.Length > 0)
                {
                    for (int i = 0; i < types.Length; i++)
                    {
                        Type type = types[i];
                        if (type == null)
                            continue;
                        if (DynamicAssemblies.IsTypeDynamic(type))
                        {
                            throw new InvalidOperationException(SR.Format(SR.XmlPregenTypeDynamic, types[i].FullName));
                        }
                    }
                    writer.Write("[assembly:");
                    writer.Write(typeof(XmlSerializerVersionAttribute).FullName);
                    writer.Write("(");
                    writer.Write("ParentAssemblyId=");
                    ReflectionAwareCodeGen.WriteQuotedCSharpString(writer, GenerateAssemblyId(types[0]));
                    writer.Write(", Version=");
                    ReflectionAwareCodeGen.WriteQuotedCSharpString(writer, ThisAssembly.Version);
                    if (defaultNamespace != null)
                    {
                        writer.Write(", Namespace=");
                        ReflectionAwareCodeGen.WriteQuotedCSharpString(writer, defaultNamespace);
                    }
                    writer.WriteLine(")]");
                }
                CodeIdentifiers classes = new CodeIdentifiers();
                classes.AddUnique("XmlSerializationWriter", "XmlSerializationWriter");
                classes.AddUnique("XmlSerializationReader", "XmlSerializationReader");
                string suffix = null;
                if (types != null && types.Length == 1 && types[0] != null)
                {
                    suffix = CodeIdentifier.MakeValid(types[0].Name);
                    if (types[0].IsArray)
                    {
                        suffix += "Array";
                    }
                }

                writer.WriteLine("namespace " + GeneratedAssemblyNamespace + " {");
                writer.Indent++;

                writer.WriteLine();

                string writerClass = "XmlSerializationWriter" + suffix;
                writerClass = classes.AddUnique(writerClass, writerClass);
                XmlSerializationWriterCodeGen writerCodeGen = new XmlSerializationWriterCodeGen(writer, scopes, "public", writerClass);

                writerCodeGen.GenerateBegin();
                string[] writeMethodNames = new string[xmlMappings.Length];

                for (int i = 0; i < xmlMappings.Length; i++)
                {
                    writeMethodNames[i] = writerCodeGen.GenerateElement(xmlMappings[i]);
                }
                writerCodeGen.GenerateEnd();

                writer.WriteLine();

                string readerClass = "XmlSerializationReader" + suffix;
                readerClass = classes.AddUnique(readerClass, readerClass);
                XmlSerializationReaderCodeGen readerCodeGen = new XmlSerializationReaderCodeGen(writer, scopes, "public", readerClass);

                readerCodeGen.GenerateBegin();
                string[] readMethodNames = new string[xmlMappings.Length];
                for (int i = 0; i < xmlMappings.Length; i++)
                {
                    readMethodNames[i] = readerCodeGen.GenerateElement(xmlMappings[i]);
                }
                readerCodeGen.GenerateEnd(readMethodNames, xmlMappings, types);

                string baseSerializer = readerCodeGen.GenerateBaseSerializer("XmlSerializer1", readerClass, writerClass, classes);
                Hashtable serializers = new Hashtable();
                for (int i = 0; i < xmlMappings.Length; i++)
                {
                    if (serializers[xmlMappings[i].Key] == null)
                    {
                        serializers[xmlMappings[i].Key] = readerCodeGen.GenerateTypedSerializer(readMethodNames[i], writeMethodNames[i], xmlMappings[i], classes, baseSerializer, readerClass, writerClass);
                    }
                }
                readerCodeGen.GenerateSerializerContract("XmlSerializerContract", xmlMappings, types, readerClass, readMethodNames, writerClass, writeMethodNames, serializers);
                writer.Indent--;
                writer.WriteLine("}");

                return compiler.Compile(assembly, defaultNamespace, parameters, evidence);
            }
            finally
            {
                compiler.Close();
            }
        }

#if !NET_NATIVE
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "It is safe because the serialization assembly is generated by the framework code, not by the user.")]
        internal static Assembly GenerateRefEmitAssembly(XmlMapping[] xmlMappings, Type[] types, string defaultNamespace, Evidence evidence)
        {
            var scopeTable = new Dictionary<TypeScope, XmlMapping>();
            foreach (XmlMapping mapping in xmlMappings)
                scopeTable[mapping.Scope] = mapping;
            TypeScope[] scopes = new TypeScope[scopeTable.Keys.Count];
            scopeTable.Keys.CopyTo(scopes, 0);

            string assemblyName = "Microsoft.GeneratedCode";
            AssemblyBuilder assemblyBuilder = CodeGenerator.CreateAssemblyBuilder(assemblyName);
            // Add AssemblyVersion attribute to match parent accembly version
            if (types != null && types.Length > 0 && types[0] != null)
            {
                ConstructorInfo AssemblyVersionAttribute_ctor = typeof(AssemblyVersionAttribute).GetConstructor(
                    new Type[] { typeof(String) }
                    );
                string assemblyVersion = types[0].GetTypeInfo().Assembly.GetName().Version.ToString();
                assemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(AssemblyVersionAttribute_ctor, new Object[] { assemblyVersion }));
            }
            CodeIdentifiers classes = new CodeIdentifiers();
            classes.AddUnique("XmlSerializationWriter", "XmlSerializationWriter");
            classes.AddUnique("XmlSerializationReader", "XmlSerializationReader");
            string suffix = null;
            if (types != null && types.Length == 1 && types[0] != null)
            {
                suffix = CodeIdentifier.MakeValid(types[0].Name);
                if (types[0].IsArray)
                {
                    suffix += "Array";
                }
            }

            ModuleBuilder moduleBuilder = CodeGenerator.CreateModuleBuilder(assemblyBuilder, assemblyName);

            string writerClass = "XmlSerializationWriter" + suffix;
            writerClass = classes.AddUnique(writerClass, writerClass);
            XmlSerializationWriterILGen writerCodeGen = new XmlSerializationWriterILGen(scopes, "public", writerClass);
            writerCodeGen.ModuleBuilder = moduleBuilder;

            writerCodeGen.GenerateBegin();
            string[] writeMethodNames = new string[xmlMappings.Length];

            for (int i = 0; i < xmlMappings.Length; i++)
            {
                writeMethodNames[i] = writerCodeGen.GenerateElement(xmlMappings[i]);
            }
            Type writerType = writerCodeGen.GenerateEnd();

            string readerClass = "XmlSerializationReader" + suffix;
            readerClass = classes.AddUnique(readerClass, readerClass);
            XmlSerializationReaderILGen readerCodeGen = new XmlSerializationReaderILGen(scopes, "public", readerClass);

            readerCodeGen.ModuleBuilder = moduleBuilder;
            readerCodeGen.CreatedTypes.Add(writerType.Name, writerType);

            readerCodeGen.GenerateBegin();
            string[] readMethodNames = new string[xmlMappings.Length];
            for (int i = 0; i < xmlMappings.Length; i++)
            {
                readMethodNames[i] = readerCodeGen.GenerateElement(xmlMappings[i]);
            }
            readerCodeGen.GenerateEnd(readMethodNames, xmlMappings, types);

            string baseSerializer = readerCodeGen.GenerateBaseSerializer("XmlSerializer1", readerClass, writerClass, classes);
            var serializers = new Dictionary<string, string>();
            for (int i = 0; i < xmlMappings.Length; i++)
            {
                if (!serializers.ContainsKey(xmlMappings[i].Key))
                {
                    serializers[xmlMappings[i].Key] = readerCodeGen.GenerateTypedSerializer(readMethodNames[i], writeMethodNames[i], xmlMappings[i], classes, baseSerializer, readerClass, writerClass);
                }
            }
            readerCodeGen.GenerateSerializerContract("XmlSerializerContract", xmlMappings, types, readerClass, readMethodNames, writerClass, writeMethodNames, serializers);

            return writerType.GetTypeInfo().Assembly;
        }
#endif

        private static MethodInfo GetMethodFromType(Type type, string methodName)
        {
            MethodInfo method = type.GetMethod(methodName);
            if (method != null)
                return method;

            // Not support pregen.  Workaround SecurityCritical required for assembly.CodeBase api.
            MissingMethodException missingMethod = new MissingMethodException(type.FullName + "::" + methodName);
            throw missingMethod;
        }

        internal static Type GetTypeFromAssembly(Assembly assembly, string typeName)
        {
            typeName = GeneratedAssemblyNamespace + "." + typeName;
            Type type = assembly.GetType(typeName);
            if (type == null) throw new InvalidOperationException(SR.Format(SR.XmlMissingType, typeName, assembly.FullName));
            return type;
        }

        internal bool CanRead(XmlMapping mapping, XmlReader xmlReader)
        {
            if (mapping == null)
                return false;

            if (mapping.Accessor.Any)
            {
                return true;
            }
            TempMethod method = _methods[mapping.Key];
            return xmlReader.IsStartElement(method.name, method.ns);
        }

        private string ValidateEncodingStyle(string encodingStyle, string methodKey)
        {
            if (encodingStyle != null && encodingStyle.Length > 0)
            {
                if (_methods[methodKey].isSoap)
                {
                    if (encodingStyle != Soap.Encoding && encodingStyle != Soap12.Encoding)
                    {
                        throw new InvalidOperationException(SR.Format(SR.XmlInvalidEncoding3, encodingStyle, Soap.Encoding, Soap12.Encoding));
                    }
                }
                else
                {
                    throw new InvalidOperationException(SR.Format(SR.XmlInvalidEncodingNotEncoded1, encodingStyle));
                }
            }
            else
            {
                if (_methods[methodKey].isSoap)
                {
                    encodingStyle = Soap.Encoding;
                }
            }
            return encodingStyle;
        }

        internal object InvokeReader(XmlMapping mapping, XmlReader xmlReader, XmlDeserializationEvents events, string encodingStyle)
        {
            XmlSerializationReader reader = null;
            try
            {
                encodingStyle = ValidateEncodingStyle(encodingStyle, mapping.Key);
                reader = Contract.Reader;
                reader.Init(xmlReader, events, encodingStyle, this);
                if (_methods[mapping.Key].readMethod == null)
                {
                    if (_readerMethods == null)
                    {
                        _readerMethods = Contract.ReadMethods;
                    }
                    string methodName = (string)_readerMethods[mapping.Key];
                    if (methodName == null)
                    {
                        throw new InvalidOperationException(SR.Format(SR.XmlNotSerializable, mapping.Accessor.Name));
                    }
                    _methods[mapping.Key].readMethod = GetMethodFromType(reader.GetType(), methodName);
                }
                return _methods[mapping.Key].readMethod.Invoke(reader, Array.Empty<object>());
            }
            catch (SecurityException e)
            {
                throw new InvalidOperationException(SR.XmlNoPartialTrust, e);
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
        }

        internal void InvokeWriter(XmlMapping mapping, XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
        {
            XmlSerializationWriter writer = null;
            try
            {
                encodingStyle = ValidateEncodingStyle(encodingStyle, mapping.Key);
                writer = Contract.Writer;
                writer.Init(xmlWriter, namespaces, encodingStyle, id, this);
                if (_methods[mapping.Key].writeMethod == null)
                {
                    if (_writerMethods == null)
                    {
                        _writerMethods = Contract.WriteMethods;
                    }
                    string methodName = (string)_writerMethods[mapping.Key];
                    if (methodName == null)
                    {
                        throw new InvalidOperationException(SR.Format(SR.XmlNotSerializable, mapping.Accessor.Name));
                    }
                    _methods[mapping.Key].writeMethod = GetMethodFromType(writer.GetType(), methodName);
                }
                _methods[mapping.Key].writeMethod.Invoke(writer, new object[] { o });
            }
            catch (SecurityException e)
            {
                throw new InvalidOperationException(SR.XmlNoPartialTrust, e);
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }
        }

        internal sealed class TempMethodDictionary : Dictionary<string, TempMethod>
        {
        }
    }

    internal sealed class XmlSerializerCompilerParameters
    {
        private bool _needTempDirAccess;
        private CompilerParameters _parameters;
        private XmlSerializerCompilerParameters(CompilerParameters parameters, bool needTempDirAccess)
        {
            _needTempDirAccess = needTempDirAccess;
            _parameters = parameters;
        }

        internal bool IsNeedTempDirAccess { get { return _needTempDirAccess; } }
        internal CompilerParameters CodeDomParameters { get { return _parameters; } }

        internal static XmlSerializerCompilerParameters Create(string location)
        {
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            if (string.IsNullOrEmpty(location))
            {
                // Trim leading and trailing white spaces (VSWhidbey 229873)
                if (!string.IsNullOrEmpty(location))
                {
                    location = location.Trim();
                }
            }
            parameters.TempFiles = new TempFileCollection(location);
            return new XmlSerializerCompilerParameters(parameters, string.IsNullOrEmpty(location));
        }

        internal static XmlSerializerCompilerParameters Create(CompilerParameters parameters, bool needTempDirAccess)
        {
            return new XmlSerializerCompilerParameters(parameters, needTempDirAccess);
        }
    }


    internal class TempAssemblyCacheKey
    {
        private string _ns;
        private object _type;

        internal TempAssemblyCacheKey(string ns, object type)
        {
            _type = type;
            _ns = ns;
        }

        public override bool Equals(object o)
        {
            TempAssemblyCacheKey key = o as TempAssemblyCacheKey;
            if (key == null) return false;
            return (key._type == _type && key._ns == _ns);
        }

        public override int GetHashCode()
        {
            return ((_ns != null ? _ns.GetHashCode() : 0) ^ (_type != null ? _type.GetHashCode() : 0));
        }
    }

    internal class TempAssemblyCache
    {
        private Dictionary<TempAssemblyCacheKey, TempAssembly> _cache = new Dictionary<TempAssemblyCacheKey, TempAssembly>();

        internal TempAssembly this[string ns, object o]
        {
            get
            {
                TempAssembly tempAssembly;
                _cache.TryGetValue(new TempAssemblyCacheKey(ns, o), out tempAssembly);
                return  tempAssembly;
            }
        }

        internal void Add(string ns, object o, TempAssembly assembly)
        {
            TempAssemblyCacheKey key = new TempAssemblyCacheKey(ns, o);
            lock (this)
            {
                TempAssembly tempAssembly;
                if (_cache.TryGetValue(key, out tempAssembly) && tempAssembly == assembly) return;
                _cache = new Dictionary<TempAssemblyCacheKey, TempAssembly>(_cache); // clone
                _cache[key] = assembly;
            }
        }
    }
}

