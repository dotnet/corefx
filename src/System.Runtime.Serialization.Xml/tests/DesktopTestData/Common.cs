using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DesktopTestData
{

    [Flags]
    public enum SerializerEnum
    {
        None = 0,
        NetDataContractSerializer = 1,
        BinaryFormatter = 2,
        DataContractSerializer = 4,
        DataContractJsonSerializer = 8
    }

    [Serializable]
    public class TestParameters
    {
        public TestParameters()
        {
        }

        public TestParameters(SerializerEnum serEnum, Type t, string ResolverName, bool wrapInObjectContainer, Type expectedException, string expectedErrorMessage)
        {
            serializerEnum = serEnum;
            typeToRoundtrip = t;
            DCResolverName = ResolverName;
            useObjectContainer = wrapInObjectContainer;
            expectedExp = expectedException;
            expectedErrorMsg = expectedErrorMessage;
        }

        public TestParameters(SerializerEnum serEnum, Type t, string ResolverName, bool wrapInObjectContainer) : this(serEnum, t, ResolverName, wrapInObjectContainer, null, null) { }

        public SerializerEnum serializerEnum;
        public Type typeToRoundtrip;
        public List<Type> typeToRoundtripList;
        public string DCResolverName;
        public bool useObjectContainer;
        public Type expectedExp;
        public string expectedErrorMsg;
    }

    public static class DCRUtils
    {
        public static string NoValue = "<NoValue>";

        public static string DefaultNS = "http://www.default.com";

        /// <summary>
        /// Reads the name and namespace for a type from a text file
        /// </summary>
        /// <param name="dcType"></param>
        /// <param name="resolvedTypeName"></param>
        /// <param name="resolvedNamespace"></param>
        public static void ResolveType(Type dcType, out string resolvedTypeName, out string resolvedNamespace)
        {
            resolvedTypeName = NoValue;
            resolvedNamespace = NoValue;

            if (dcType.Equals(typeof(EmptyDCType)))
            {
                resolvedTypeName = "EmptyDCType";
                resolvedNamespace = DefaultNS;
            }
        }


        /// <summary>
        /// Reads the name and namespace for a type from a text file
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="typeNamespace"></param>
        /// <returns></returns>
        public static Type ResolveName(string typeName, string typeNamespace)
        {
            switch (typeNamespace)
            {
                case "http://www.default.com":
                    {
                        switch (typeName)
                        {
                            case "EmptyDCType":
                                {
                                    return typeof(EmptyDCType);
                                }
                            default:
                                {
                                    return typeof(TypeNotFound);
                                }
                        }
                    }
                default:
                    {
                        return typeof(TypeNotFound);
                    }
            }
        }

        /// <summary>
        /// Returns datacontractresolver specified in the input parameter
        /// </summary>
        /// <returns></returns>
        public static DataContractResolver GetDCResolver(string resolverName, Type[] resolverTypes = null)
        {
            switch (resolverName)
            {
                case "PublicDerivedDCR_1":
                    {
                        return new PublicDerivedDCR_1();
                    }
                case "SimpleResolver":
                    {
                        return new SimpleResolver();
                    }
                case "PrimitiveTypeResolver":
                    {
                        return new PrimitiveTypeResolver();
                    }
                case "PrivateDerivedDCR":
                    {
                        return new PrivateDerivedDCR();
                    }
                case "PublicDerivedDCR_2":
                    {
                        return new PublicDerivedDCR_2();
                    }
                case "DCR_ReturnNull_1":
                    {
                        return new DCR_ReturnNull_1();
                    }
                case "DCR_ResolveTypeToEmpty":
                    {
                        return new DCR_ResolveTypeToEmpty();
                    }
                case "DCR_ReturnNull_2":
                    {
                        return new DCR_ReturnNull_2();
                    }
                case "DCRThrowArgumentException":
                    {
                        return new DCRThrowArgumentException();
                    }
                case "DCRThrowInvalidOpExp":
                    {
                        return new DCRThrowInvalidOpExp();
                    }
                case "DCR_NegativeScenarios":
                    {
                        return new DCR_NegativeScenarios();
                    }
                case "DCR_ReturnNull_3":
                    {
                        return new DCR_ReturnNull_3();
                    }
                case "GenericNullableResolver":
                    {
                        return new GenericNullableResolver();
                    }
                case "KTandDCRResolver":
                    {
                        if (resolverTypes != null)
                        {
                            List<Type> resolverList = new List<Type>();
                            foreach (Type t in resolverTypes)
                            {
                                resolverList.Add(t);
                            }
                            return new KTandDCRResolver(resolverList);
                        }
                        return new KTandDCRResolver();
                    }
                case "<NULL>":
                    {
                        return null;
                    }
                default:
                    {
                        throw new NotSupportedException("Test issue: GetDCResolver does not support resolver " + resolverName);
                    }
            }
        }



        public static object SingleRoundtripPerEpisode(SerializerEnum serializerToUse, Type t, Object instance, DataContractResolver dcr)
        {
            if (serializerToUse.Equals(SerializerEnum.DataContractSerializer))
            {
                System.Diagnostics.Debug.WriteLine("SingleRoundtripPerEpisode type {0} using resolver {1}", t.FullName, dcr.GetType().Name);
                DataContractSerializer dcs = (DataContractSerializer)SerializerFactory.GetSerializer(t, serializerToUse);
                MemoryStream ms = new MemoryStream();
                XmlDictionaryWriter xmlWriter = XmlDictionaryWriter.CreateTextWriter(ms);
                dcs.WriteObject(xmlWriter, instance, dcr);
                xmlWriter.Flush();
                ms.Position = 0;
                XmlDictionaryReader xmlReader = XmlDictionaryReader.CreateTextReader(ms, XmlDictionaryReaderQuotas.Max);
                return dcs.ReadObject(xmlReader, false, dcr);
            }
            else
            {
                throw new NotSupportedException("Test Issue: serializer.ToString() not supported in DCR testing");
            }
        }

        public static void RoundtripPerEpisodeAndCompare(TestParameters param)
        {
            bool exceptionReported = false;
            StringBuilder error = new StringBuilder();
            if (null == param.typeToRoundtrip)
            {
                foreach (Type t in param.typeToRoundtripList)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("RoundtripPerEpisodeAndCompare type {0} using Resolver {1}", t.FullName, param.DCResolverName);
                        param.typeToRoundtrip = t;
                        SingleRoundtripPerEpisodeAndCompare(param);
                        DoubleRoundtripPerEpisodeAndCompare(param);
                    }
                    catch (TargetInvocationException TInvExp)
                    {
                        exceptionReported = true;
                        Exception Texp = TInvExp;
                        if (null != TInvExp.InnerException)
                        {
                            Texp = TInvExp.InnerException;
                        }
                        error.AppendLine(String.Format("Roundtrip Type = {2}. Exception: Type = {0}. Error Message = {1} StackTrace = {2} ", Texp.GetType().FullName, Texp.Message, t.FullName, Texp.StackTrace));
                    }
                    catch (Exception exp)
                    {
                        exceptionReported = true;
                        error.AppendLine(String.Format("Roundtrip Type = {2}. Exception: Type = {0}. Error Message = {1} StackTrace = {2} ", exp.GetType().FullName, exp.Message, t.FullName, exp.StackTrace));
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("RoundtripPerEpisodeAndCompare type {0} using Resolver {1}", param.typeToRoundtrip.FullName, param.DCResolverName);
                SingleRoundtripPerEpisodeAndCompare(param);
                DoubleRoundtripPerEpisodeAndCompare(param);
            }

            if (exceptionReported)
            {
                throw new Exception(error.ToString());
            }
        }

        public static object CreateInstance(bool useObjectContainer, Type typeToRoundTrip, out Type actualTypeToRoundtrip)
        {
            object originalInstance = null;
            object customInstance = null;
            //CSDMain:114750
            MethodInfo method = typeToRoundTrip.GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static);
            if (null != method)
            {
                customInstance = method.Invoke(null, new object[] { });
                originalInstance = customInstance;
            }
            else if (typeToRoundTrip.ContainsGenericParameters)
            {
                customInstance = CreateInstanceOfGenericType(typeToRoundTrip);
                originalInstance = customInstance;
            }
            if (useObjectContainer)
            {
                if (null != customInstance)
                {
                    originalInstance = new ObjectContainer(customInstance);
                }
                else
                {
                    originalInstance = new ObjectContainer(InstanceCreator.CreateInstanceOf(typeToRoundTrip, new Random()));
                }
            }
            else
            {
                //Create instance if generictypeinstance is not created already
                if (null == originalInstance)
                {
                    originalInstance = InstanceCreator.CreateInstanceOf(typeToRoundTrip, new Random());
                }
            }
            actualTypeToRoundtrip = originalInstance.GetType();
            return originalInstance;
        }

        public static void SingleRoundtripPerEpisodeAndCompare(TestParameters param)
        {
            Type typeToRoundTrip = null;
            object originalInstance = CreateInstance(param.useObjectContainer, param.typeToRoundtrip, out typeToRoundTrip);
            object roundtrippedInstance = null;
            if (null != param.expectedExp)
            {
                ExceptionHelpers.CheckForException(param.expectedExp, param.expectedErrorMsg, delegate
                {
                    roundtrippedInstance = SingleRoundtripPerEpisode(param.serializerEnum, typeToRoundTrip, originalInstance, GetDCResolver(param.DCResolverName));

                });
            }
            else
            {
                roundtrippedInstance = SingleRoundtripPerEpisode(param.serializerEnum, typeToRoundTrip, originalInstance, GetDCResolver(param.DCResolverName));
                System.Diagnostics.Debug.WriteLine("Singleroundtrip per episode successful for type " + originalInstance.GetType().Name);
                ComparisonHelper.CompareRecursively(originalInstance, roundtrippedInstance);
                System.Diagnostics.Debug.WriteLine("Compared object successfully in method 'SingleRoundtripPerEpisodeAndCompare'");
            }

        }

        public static object DoubleRoundtripPerEpisode(SerializerEnum serializerToUse, Type t, Object instance, DataContractResolver dcr)
        {
            object result = SingleRoundtripPerEpisode(serializerToUse, t, instance, dcr);
            return SingleRoundtripPerEpisode(serializerToUse, t, result, dcr);
        }

        public static void DoubleRoundtripPerEpisodeAndCompare(TestParameters param)
        {
            Type typeToRoundTrip = null;
            object originalInstance = CreateInstance(param.useObjectContainer, param.typeToRoundtrip, out typeToRoundTrip);
            object roundtrippedInstance = null;
            if (null != param.expectedExp)
            {
                ExceptionHelpers.CheckForException(param.expectedExp, param.expectedErrorMsg, delegate
                {
                    roundtrippedInstance = DoubleRoundtripPerEpisode(param.serializerEnum, typeToRoundTrip, originalInstance, GetDCResolver(param.DCResolverName));
                });
            }
            else
            {
                roundtrippedInstance = DoubleRoundtripPerEpisode(param.serializerEnum, typeToRoundTrip, originalInstance, GetDCResolver(param.DCResolverName));
                System.Diagnostics.Debug.WriteLine("Double roundtrip per episode successful for type " + originalInstance.GetType().Name);
                ComparisonHelper.CompareRecursively(originalInstance, roundtrippedInstance);
                System.Diagnostics.Debug.WriteLine("Compared object successfully in method 'DoubleRoundtripPerEpisodeAndCompare'");
            }

        }

        /// <summary>
        /// Serialize and Deserialize the object with specified serializer
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="testInstance"></param>
        /// <returns></returns>
        public static object SingleRoundTripTest(XmlObjectSerializer serializer, object testInstance)
        {
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, testInstance);
            ms.Position = 0;
            return serializer.ReadObject(ms);
        }


        public static void RoundTripAndCompare(TestParameters param)
        {
            bool exceptionReported = false;
            StringBuilder error = new StringBuilder();
            if (null == param.typeToRoundtrip)
            {
                foreach (Type t in param.typeToRoundtripList)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("RoundtripAndCompare type {0} using Resolver {1}", t.FullName, param.DCResolverName);
                        param.typeToRoundtrip = t;
                        SingleRoundTripAndCompare(param);
                        DoubleRoundTripAndCompare(param);
                    }
                    catch (TargetInvocationException TInvExp)
                    {
                        exceptionReported = true;
                        Exception Texp = TInvExp;
                        if (null != TInvExp.InnerException)
                        {
                            Texp = TInvExp.InnerException;
                        }
                        error.AppendLine(String.Format("Roundtrip Type = {2}. Exception: Type = {0}. Error Message = {1} StackTrace = {2} ", Texp.GetType().FullName, Texp.Message, t.FullName, Texp.StackTrace));
                    }
                    catch (Exception exp)
                    {
                        exceptionReported = true;
                        error.AppendLine(String.Format("Roundtrip Type = {2}. Exception: Type = {0}. Error Message = {1} StackTrace = {2} ", exp.GetType().FullName, exp.Message, t.FullName, exp.StackTrace));
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("RoundtripAndCompare type {0} using Resolver {1}", param.typeToRoundtrip.FullName, param.DCResolverName);
                SingleRoundTripAndCompare(param);
                DoubleRoundTripAndCompare(param);
            }
            if (exceptionReported)
            {
                throw new Exception(error.ToString());
            }
        }

        /// <summary>
        /// Roundtrips and compares the original and roundtripped object.
        /// Throws an exception if the objects are not same
        /// </summary>
        /// <param name="serializerEnum"></param>
        /// <param name="t"></param>
        /// <param name="DatacontractResolver"></param>
        /// <param name="useObjectContainer">Roundtrips instance of ObjectContainer class and assigns the instance of Type t to its member</param>
        public static void SingleRoundTripAndCompare(TestParameters testParams)
        {
            Type typeToRoundTrip = null;
            object originalInstance = CreateInstance(testParams.useObjectContainer, testParams.typeToRoundtrip, out typeToRoundTrip);

            XmlObjectSerializer serializer = SerializerFactory.GetSerializer(typeToRoundTrip, testParams.serializerEnum, GetDCResolver(testParams.DCResolverName));
            object roundtrippedInstance = null;
            if (null != testParams.expectedExp)
            {
                ExceptionHelpers.CheckForException(testParams.expectedExp, testParams.expectedErrorMsg,
                    delegate
                    {
                        roundtrippedInstance = DCRUtils.SingleRoundTripTest(serializer, originalInstance);

                    });
            }
            else
            {
                roundtrippedInstance = DCRUtils.SingleRoundTripTest(serializer, originalInstance);
                System.Diagnostics.Debug.WriteLine("SingleRoundtriped instance " + originalInstance.GetType().Name);
                ComparisonHelper.CompareRecursively(originalInstance, roundtrippedInstance);
                System.Diagnostics.Debug.WriteLine("Compared object successfully in method 'SingleRoundTripAndCompare'");
            }

        }

        /// <summary>
        /// Serialize and Deserialize the object with specified serializer
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="testInstance"></param>
        /// <returns></returns>
        public static object DoubleRoundTripTest(XmlObjectSerializer serializer, object testInstance)
        {
            object result = SingleRoundTripTest(serializer, testInstance);
            return SingleRoundTripTest(serializer, result);
        }

        /// <summary>
        /// Roundtrips and compares the original and roundtripped object.
        /// Throws an exception if the objects are not same
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="testInstance"></param>
        /// <param name="useObjectContainer">Roundtrips instance of ObjectContainer class and assigns the instance of Type t to its member</param>
        public static void DoubleRoundTripAndCompare(TestParameters param)
        {
            Type typeToRoundTrip = null;
            object originalInstance = CreateInstance(param.useObjectContainer, param.typeToRoundtrip, out typeToRoundTrip);
            XmlObjectSerializer serializer = SerializerFactory.GetSerializer(typeToRoundTrip, param.serializerEnum, GetDCResolver(param.DCResolverName));
            object roundtrippedInstance = null;
            if (null != param.expectedExp)
            {
                ExceptionHelpers.CheckForException(param.expectedExp, param.expectedErrorMsg, delegate
                {
                    roundtrippedInstance = DCRUtils.DoubleRoundTripTest(serializer, originalInstance);

                });
            }
            else
            {
                roundtrippedInstance = DCRUtils.DoubleRoundTripTest(serializer, originalInstance);
                System.Diagnostics.Debug.WriteLine("DoubleRoundtriped instance " + originalInstance.GetType().Name);
                ComparisonHelper.CompareRecursively(originalInstance, roundtrippedInstance);
                System.Diagnostics.Debug.WriteLine("Compared object successfully in method 'DoubleRoundTripAndCompare'");
            }
        }

        public static object CreateInstanceOfGenericType(Type t)
        {
            object instance = null;
            switch (t.Name)
            {
                case "CustomGeneric1`1":
                    {
                        instance = new CustomGeneric1<KT1Base>();
                    }
                    break;
                case "CustomGeneric2`2":
                    {
                        instance = new CustomGeneric2<KT1Base, NonDCPerson>();
                    }
                    break;
                case "GenericBase`1":
                    {
                        instance = new GenericBase<NonDCPerson>();
                    }
                    break;
                case "GenericBase2`2":
                    {
                        instance = new GenericBase2<KT1Base, NonDCPerson>();
                    }
                    break;
                case "CustomGeneric2`1":
                    {
                        instance = new CustomGeneric2<NonDCPerson>();
                    }
                    break;
            }
            return instance;
        }

        // This method will generate a Class Object both in memory and in a csharp file.
        // Designed to be used for KnownType testing it will either apply the [KnownTypeAttribute(typeof(type))] for each Type passed in, or
        // it will apply the [KnownTypeAttribute("GetKnownTypes")] to the Class and use the method call for resolving KnownTypes,or
        // it will generate the class with neither of those two attributes.
        public static CompilerResults GenerateClass(List<Type> listOfKnownTypes, bool useKTMethod, bool useKTConstructor, string namespaceName, string className, string fileName)
        {
            //Create Class Namespaces
            CodeNamespace cnsCodeDom = new CodeNamespace(namespaceName);

            // Add usings
            cnsCodeDom.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
            cnsCodeDom.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            //Create Class Declaration
            CodeTypeDeclaration ctd = new CodeTypeDeclaration(className);
            ctd.IsClass = true;
            ctd.TypeAttributes = TypeAttributes.Public;
            cnsCodeDom.Types.Add(ctd);

            // Add DataContract Attribute to class
            CodeAttributeDeclaration dataContractAttribute = new CodeAttributeDeclaration(
            new CodeTypeReference("DataContractAttribute"));
            ctd.CustomAttributes.Add(dataContractAttribute);
            // If using the constructor then no attributes applied at all.
            if (useKTConstructor == false)
            {
                // Apply either [KnownTypeAttribute(typeof(type))] or [KnownTypeAttribute("GetKnownTypes")]
                if (useKTMethod == true)
                {
                    CodeAttributeArgument ktMethod = new CodeAttributeArgument(
                        new CodePrimitiveExpression("GetKnownTypes"));
                    CodeAttributeDeclaration knownTypeMethodAttribute = new CodeAttributeDeclaration(
                        new CodeTypeReference("KnownTypeAttribute"), ktMethod);
                    ctd.CustomAttributes.Add(knownTypeMethodAttribute);
                }
                else
                {
                    foreach (Type t in listOfKnownTypes)
                    {
                        CodeAttributeArgument ktArgument = new CodeAttributeArgument(
                            new CodeTypeOfExpression(t));
                        CodeAttributeDeclaration knownTypeAttribute = new CodeAttributeDeclaration(
                            new CodeTypeReference("KnownTypeAttribute"), ktArgument);
                        ctd.CustomAttributes.Add(knownTypeAttribute);
                    }
                }
            }

            // Create Class Fields
            CodeMemberField ktField = new CodeMemberField(typeof(object[]), "knownTypes");
            ktField.Attributes = MemberAttributes.Private;
            ktField.Attributes = MemberAttributes.Static;
            CodeMemberField dcrField = new CodeMemberField(typeof(object[]), "dataContractResolverTypes");
            dcrField.Attributes = MemberAttributes.Private;
            ctd.Members.Add(ktField);
            ctd.Members.Add(dcrField);

            // Add DataMember attribute to fields
            CodeAttributeDeclaration dmAttribute = new CodeAttributeDeclaration(
                new CodeTypeReference("DataMemberAttribute"));
            ktField.CustomAttributes.Add(dmAttribute);
            dcrField.CustomAttributes.Add(dmAttribute);

            // Create Property 1
            CodeMemberProperty mpKnownTypes = new CodeMemberProperty();
            mpKnownTypes.Attributes = MemberAttributes.Public;
            mpKnownTypes.Type = new CodeTypeReference(typeof(object[]));
            mpKnownTypes.Name = "KnownTypes";
            mpKnownTypes.HasGet = true;
            mpKnownTypes.GetStatements.Add(new CodeSnippetExpression("return knownTypes"));
            mpKnownTypes.HasSet = true;
            mpKnownTypes.SetStatements.Add(new CodeSnippetExpression("knownTypes = value"));

            // Create Property 2
            CodeMemberProperty mpDCRTypes = new CodeMemberProperty();
            mpDCRTypes.Attributes = MemberAttributes.Public;
            mpDCRTypes.Type = new CodeTypeReference(typeof(object[]));
            mpDCRTypes.Name = "DataContractResolverTypes";
            mpDCRTypes.HasGet = true;
            mpDCRTypes.GetStatements.Add(new CodeSnippetExpression("return dataContractResolverTypes"));
            mpDCRTypes.HasSet = true;
            mpDCRTypes.SetStatements.Add(new CodeSnippetExpression("dataContractResolverTypes = value"));

            // Add DataMember Attribute to properties
            mpKnownTypes.CustomAttributes.Add(dmAttribute);
            mpDCRTypes.CustomAttributes.Add(dmAttribute);
            ctd.Members.Add(mpKnownTypes);
            ctd.Members.Add(mpDCRTypes);

            // Create GetKnownTypes Method
            CodeMemberMethod getKnownTypes = new CodeMemberMethod();
            getKnownTypes.Attributes = MemberAttributes.Static;
            getKnownTypes.ReturnType = new CodeTypeReference(typeof(Type[]));
            getKnownTypes.Name = "GetKnownTypes";
            getKnownTypes.Statements.Add(new CodeSnippetExpression("List<System.Type> t = new List<System.Type>();\n" +
                                                                    "\t\t\tforeach (object i in knownTypes)\n" +
                                                                    "\t\t\t{\n" +
                                                                    "\t\t\t\tt.Add(i.GetType());\n" +
                                                                    "\t\t\t}\n" +
                                                                    "\t\t\treturn t.ToArray()"));
            ctd.Members.Add(getKnownTypes);

            //Generate Source Code File, for later debugging
            CSharpCodeProvider cscProvider = new CSharpCodeProvider();
            using (StreamWriter sw = new StreamWriter(File.Open(
                Path.Combine(@".\", fileName + ".cs"),
                FileMode.Create)))
            {
                ICodeGenerator codeGenerator = cscProvider.CreateGenerator(sw);
                CodeGeneratorOptions cop = new CodeGeneratorOptions();
                codeGenerator.GenerateCodeFromNamespace(cnsCodeDom, sw, cop);
            }

            // Generate in-memory assembly
            CompilerParameters parameters = new CompilerParameters();

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");
            parameters.ReferencedAssemblies.Add(Path.Combine(@".\", "Test.WCF.DCS.dll"));
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = false;

            CodeCompileUnit compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(cnsCodeDom);

            return cscProvider.CompileAssemblyFromDom(parameters, compileUnit);
        }

        // Create instances of types
        public static object[] InstantiateTypes(List<Type> list)
        {
            object[] instances = new object[list.Count];
            for (int i = 0; i < instances.Length; i++)
            {   // Not all Types have default constructors.
                ConstructorInfo initConstr = list[i].GetConstructor(new Type[] { typeof(bool) });
                if (initConstr != null)
                {
                    instances[i] = initConstr.Invoke(new object[] { true });
                }
                else
                {
                    instances[i] = Activator.CreateInstance(list[i]);
                }
            }
            return instances;
        }

        // Filter for non-public types and duplicates, return one List.
        public static List<Type> FilterAndMergeLists(List<List<Type>> unfilteredLists)
        {
            List<Type> filteredList = new List<Type>();

            foreach (List<Type> list in unfilteredLists)
            {
                foreach (Type t in list)
                {
                    if (t.IsPublic && !filteredList.Contains(t))
                    {
                        filteredList.Add(t);
                    }
                }
            }
            return filteredList;
        }

        // This is specifically used for IObjectRef types only for comparison purposes.
        public static bool CompareIObjectRefTypes(object serialized, object deSerialized)
        {
            Dictionary<DataContract, List<RefData>> alreadyRefdValues = ObjectRefUtil.GetReferenceCounts(serialized);
            Dictionary<DataContract, List<RefData>> alreadyRefdValues2 = ObjectRefUtil.GetReferenceCounts(deSerialized);
            if (!ObjectRefUtil.IsEqual(alreadyRefdValues, alreadyRefdValues2))
            {
                return false;
            }
            return true;
        }
    }

    public static class SerializerFactory
    {
        public static XmlObjectSerializer GetSerializer(Type testType, SerializerEnum serializer)
        {
            return SerializerFactory.GetSerializer(testType, serializer, null);
        }

        public static XmlObjectSerializer GetSerializer(Type testType, SerializerEnum serializer, DataContractResolver dcr)
        {
            return SerializerFactory.GetSerializer(testType, serializer, null, dcr);
        }

        public static XmlObjectSerializer GetSerializer(Type testType, SerializerEnum serializer, IEnumerable<Type> knownTypesCollection, DataContractResolver dcr)
        {
            return SerializerFactory.GetSerializer(testType, serializer, knownTypesCollection, null, dcr);
        }

        public static XmlObjectSerializer GetSerializer(Type testType, SerializerEnum serializer, IEnumerable<Type> knownTypesCollection, string surrogate, DataContractResolver dcr)
        {
            switch (serializer)
            {
                case SerializerEnum.DataContractSerializer:
                    var dataContractSerializerSettings = new DataContractSerializerSettings()
                    {
                        DataContractResolver = dcr
                        , IgnoreExtensionDataObject = false
                        , KnownTypes = knownTypesCollection
                        , MaxItemsInObjectGraph = int.MaxValue
                        , PreserveObjectReferences = false
                    };
                    return new DataContractSerializer(testType, dataContractSerializerSettings);
                default:
                    {
                        throw new NotSupportedException("Test Issue: serializer.ToString() not supported for DCR tests");
                    }
            }
        }

    }
}
