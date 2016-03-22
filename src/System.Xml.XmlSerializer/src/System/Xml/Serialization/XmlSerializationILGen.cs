// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization
{
    using Emit;
    using Linq;
    internal class XmlSerializationILGen
    {
        private int _nextMethodNumber = 0;
        private InternalHashtable _methodNames = new InternalHashtable();
        // Lookup name->created Method
        private Dictionary<string, MethodBuilderInfo> _methodBuilders = new Dictionary<string, MethodBuilderInfo>();
        // Lookup name->class Member
        internal Dictionary<string, MemberInfo> memberInfos = new Dictionary<string, MemberInfo>();
        private ReflectionAwareILGen _raCodeGen;
        private TypeScope[] _scopes;
        private TypeDesc _stringTypeDesc = null;
        private TypeDesc _qnameTypeDesc = null;
        private string _className;
        private TypeMapping[] _referencedMethods;
        private int _references = 0;
        private InternalHashtable _generatedMethods = new InternalHashtable();
        private ModuleBuilder _moduleBuilder;
        private TypeAttributes _typeAttributes;
        protected TypeBuilder typeBuilder;
        protected CodeGenerator ilg;

        internal XmlSerializationILGen(TypeScope[] scopes, string access, string className)
        {
            _scopes = scopes;
            if (scopes.Length > 0)
            {
                _stringTypeDesc = scopes[0].GetTypeDesc(typeof(String));
                _qnameTypeDesc = scopes[0].GetTypeDesc(typeof(XmlQualifiedName));
            }
            _raCodeGen = new ReflectionAwareILGen();
            _className = className;
            System.Diagnostics.Debug.Assert(access == "public");
            _typeAttributes = TypeAttributes.Public;
        }

        internal int NextMethodNumber { get { return _nextMethodNumber; } set { _nextMethodNumber = value; } }
        internal ReflectionAwareILGen RaCodeGen { get { return _raCodeGen; } }
        internal TypeDesc StringTypeDesc { get { return _stringTypeDesc; } }
        internal TypeDesc QnameTypeDesc { get { return _qnameTypeDesc; } }
        internal string ClassName { get { return _className; } }
        internal TypeScope[] Scopes { get { return _scopes; } }
        internal InternalHashtable MethodNames { get { return _methodNames; } }
        internal InternalHashtable GeneratedMethods { get { return _generatedMethods; } }

        internal ModuleBuilder ModuleBuilder
        {
            get { System.Diagnostics.Debug.Assert(_moduleBuilder != null); return _moduleBuilder; }
            set { System.Diagnostics.Debug.Assert(_moduleBuilder == null && value != null); _moduleBuilder = value; }
        }
        internal TypeAttributes TypeAttributes { get { return _typeAttributes; } }

        private static Dictionary<string, Regex> s_regexs = new Dictionary<string, Regex>();
        static internal Regex NewRegex(string pattern)
        {
            Regex regex;
            lock (s_regexs)
            {
                if (!s_regexs.TryGetValue(pattern, out regex))
                {
                    regex = new Regex(pattern);
                    s_regexs.Add(pattern, regex);
                }
            }
            return regex;
        }

        internal MethodBuilder EnsureMethodBuilder(TypeBuilder typeBuilder, string methodName,
            MethodAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            MethodBuilderInfo methodBuilderInfo;
            if (!_methodBuilders.TryGetValue(methodName, out methodBuilderInfo))
            {
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                    methodName,
                    attributes,
                    returnType,
                    parameterTypes);
                methodBuilderInfo = new MethodBuilderInfo(methodBuilder, parameterTypes);
                _methodBuilders.Add(methodName, methodBuilderInfo);
            }
#if DEBUG
            else
            {
                methodBuilderInfo.Validate(returnType, parameterTypes, attributes);
            }
#endif
            return methodBuilderInfo.MethodBuilder;
        }

        internal MethodBuilderInfo GetMethodBuilder(string methodName)
        {
            System.Diagnostics.Debug.Assert(_methodBuilders.ContainsKey(methodName));
            return _methodBuilders[methodName];
        }
        internal virtual void GenerateMethod(TypeMapping mapping) { }

        internal void GenerateReferencedMethods()
        {
            while (_references > 0)
            {
                TypeMapping mapping = _referencedMethods[--_references];
                GenerateMethod(mapping);
            }
        }

        internal string ReferenceMapping(TypeMapping mapping)
        {
            if (_generatedMethods[mapping] == null)
            {
                _referencedMethods = EnsureArrayIndex(_referencedMethods, _references);
                _referencedMethods[_references++] = mapping;
            }
            return (string)_methodNames[mapping];
        }

        private TypeMapping[] EnsureArrayIndex(TypeMapping[] a, int index)
        {
            if (a == null) return new TypeMapping[32];
            if (index < a.Length) return a;
            TypeMapping[] b = new TypeMapping[a.Length + 32];
            Array.Copy(a, 0, b, 0, index);
            return b;
        }

        internal string GetCSharpString(string value)
        {
            return ReflectionAwareILGen.GetCSharpString(value);
        }

        internal FieldBuilder GenerateHashtableGetBegin(string privateName, string publicName, TypeBuilder serializerContractTypeBuilder)
        {
            FieldBuilder fieldBuilder = serializerContractTypeBuilder.DefineField(
                privateName,
                WellKnownTypes.DictionaryObjectObject,
                FieldAttributes.Private
                );
            ilg = new CodeGenerator(serializerContractTypeBuilder);
            PropertyBuilder propertyBuilder = serializerContractTypeBuilder.DefineProperty(
                publicName,
                PropertyAttributes.None,
                WellKnownTypes.IDictionary,
                Array.Empty<Type>());

            ilg.BeginMethod(
                WellKnownTypes.IDictionary,
                "get_" + publicName,
                Array.Empty<Type>(),
                Array.Empty<string>(),
                CodeGenerator.PublicOverrideMethodAttributes | MethodAttributes.SpecialName);
            propertyBuilder.SetGetMethod(ilg.MethodBuilder);

            ilg.Ldarg(0);
            ilg.LoadMember(fieldBuilder);
            ilg.Load(null);
            // this 'if' ends in GenerateHashtableGetEnd
            ilg.If(Cmp.EqualTo);

            ConstructorInfo Hashtable_ctor = WellKnownTypes.DictionaryObjectObject.GetConstructor(
                CodeGenerator.InstanceBindingFlags,
                Array.Empty<Type>());
            LocalBuilder _tmpLoc = ilg.DeclareLocal(WellKnownTypes.DictionaryObjectObject, "_tmp");
            ilg.New(Hashtable_ctor);
            ilg.Stloc(_tmpLoc);

            return fieldBuilder;
        }

        internal void GenerateHashtableGetEnd(FieldBuilder fieldBuilder)
        {
            ilg.Ldarg(0);
            ilg.LoadMember(fieldBuilder);
            ilg.Load(null);
            ilg.If(Cmp.EqualTo);
            {
                ilg.Ldarg(0);
                ilg.Ldloc(WellKnownTypes.DictionaryObjectObject, "_tmp");
                ilg.StoreMember(fieldBuilder);
            }
            ilg.EndIf();
            // 'endif' from GenerateHashtableGetBegin
            ilg.EndIf();

            ilg.Ldarg(0);
            ilg.LoadMember(fieldBuilder);
            ilg.GotoMethodEnd();

            ilg.EndMethod();
        }
        internal FieldBuilder GeneratePublicMethods(string privateName, string publicName, MethodBuilder[] methods, XmlMapping[] xmlMappings, TypeBuilder serializerContractTypeBuilder)
        {
            FieldBuilder fieldBuilder = GenerateHashtableGetBegin(privateName, publicName, serializerContractTypeBuilder);
            if (methods != null && methods.Length != 0 && xmlMappings != null && xmlMappings.Length == methods.Length)
            {
                MethodInfo Hashtable_set_Item = WellKnownTypes.DictionaryObjectObject.GetMethod(
                    "set_Item",
                    CodeGenerator.InstanceBindingFlags,
                    new[] { WellKnownTypes.Object, WellKnownTypes.Object }
                    );
                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i] == null)
                        continue;
                    ilg.Ldloc(WellKnownTypes.DictionaryObjectObject, "_tmp");
                    ilg.Ldstr(GetCSharpString(xmlMappings[i].Key));
                    ilg.Ldstr(GetCSharpString(methods[i].Name));
                    ilg.Call(Hashtable_set_Item);
                }
            }
            GenerateHashtableGetEnd(fieldBuilder);
            return fieldBuilder;
        }

        internal void GenerateSupportedTypes(System.Type[] types, TypeBuilder serializerContractTypeBuilder)
        {
            ilg = new CodeGenerator(serializerContractTypeBuilder);
            ilg.BeginMethod(
                WellKnownTypes.Boolean,
                "CanSerialize",
                new Type[] { WellKnownTypes.Type },
                new string[] { "type" },
                CodeGenerator.PublicOverrideMethodAttributes);
            InternalHashtable uniqueTypes = new InternalHashtable();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i].ToReference();

                if (type == null)
                    continue;
                if (!type.GetTypeInfo().IsPublic && !type.GetTypeInfo().IsNestedPublic)
                    continue;
                if (uniqueTypes[type] != null)
                    continue;
                // DDB172141: Wrong generated CS for serializer of List<string> type
                if (type.GetTypeInfo().IsGenericType || type.GetTypeInfo().ContainsGenericParameters)
                    continue;
                uniqueTypes[type] = type;
                ilg.Ldarg("type");
                ilg.Ldc(type);
                ilg.If(Cmp.EqualTo);
                {
                    ilg.Ldc(true);
                    ilg.GotoMethodEnd();
                }
                ilg.EndIf();
            }
            ilg.Ldc(false);
            ilg.GotoMethodEnd();
            ilg.EndMethod();
        }

        internal TypeBuilder GenerateBaseSerializer(string baseSerializer, ConstructorBuilder readerCtor, ConstructorBuilder writerCtor, CodeIdentifiers classes)
        {
            baseSerializer = CodeIdentifier.MakeValid(baseSerializer);
            baseSerializer = classes.AddUnique(baseSerializer, baseSerializer);

            TypeBuilder baseSerializerTypeBuilder = CodeGenerator.CreateTypeBuilder(
                _moduleBuilder,
                CodeIdentifier.GetCSharpName(baseSerializer),
                TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.BeforeFieldInit,
                WellKnownTypes.XmlSerializer,
                Array.Empty<Type>());

            ilg = new CodeGenerator(baseSerializerTypeBuilder);
            ilg.BeginMethod(WellKnownTypes.XmlSerializationReader,
                "CreateReader",
                Array.Empty<Type>(),
                Array.Empty<string>(),
                CodeGenerator.ProtectedOverrideMethodAttributes);
            ilg.New(readerCtor);
            ilg.EndMethod();

            ilg.BeginMethod(WellKnownTypes.XmlSerializationWriter,
                "CreateWriter",
                Array.Empty<Type>(),
                Array.Empty<string>(),
                CodeGenerator.ProtectedOverrideMethodAttributes);
            ilg.New(writerCtor);
            ilg.EndMethod();

            baseSerializerTypeBuilder.DefineDefaultConstructor(
                MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            return baseSerializerTypeBuilder;
        }

        internal TypeBuilder GenerateTypedSerializer(MethodBuilder readMethod, MethodBuilder writeMethod, XmlMapping mapping, CodeIdentifiers classes, TypeBuilder baseSerializer, TypeBuilder readerClass, TypeBuilder writerClass, out ConstructorBuilder defaultCtor)
        {
            string serializerName = CodeIdentifier.MakeValid(Accessor.UnescapeName(mapping.Accessor.Mapping.TypeDesc.Name));
            serializerName = classes.AddUnique(serializerName + "Serializer", mapping);

            TypeBuilder typedSerializerTypeBuilder = CodeGenerator.CreateTypeBuilder(
                _moduleBuilder,
                CodeIdentifier.GetCSharpName(serializerName),
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                baseSerializer,
                Array.Empty<Type>());

            ilg = new CodeGenerator(typedSerializerTypeBuilder);
            ilg.BeginMethod(
                WellKnownTypes.Boolean,
                "CanDeserialize",
                new Type[] { WellKnownTypes.XmlReader },
                new string[] { "xmlReader" },
                CodeGenerator.PublicOverrideMethodAttributes
            );

            if (mapping.Accessor.Any)
            {
                ilg.Ldc(true);
                ilg.Stloc(ilg.ReturnLocal);
                ilg.Br(ilg.ReturnLabel);
            }
            else
            {
                MethodInfo XmlReader_IsStartElement = WellKnownTypes.XmlReader.GetMethod(
                     "IsStartElement",
                     CodeGenerator.InstanceBindingFlags,
                     new Type[] { WellKnownTypes.String, WellKnownTypes.String }
                     );
                ilg.Ldarg(ilg.GetArg("xmlReader"));
                ilg.Ldstr(GetCSharpString(mapping.Accessor.Name));
                ilg.Ldstr(GetCSharpString(mapping.Accessor.Namespace));
                ilg.Call(XmlReader_IsStartElement);
                ilg.Stloc(ilg.ReturnLocal);
                ilg.Br(ilg.ReturnLabel);
            }
            ilg.MarkLabel(ilg.ReturnLabel);
            ilg.Ldloc(ilg.ReturnLocal);
            ilg.EndMethod();

            if (writeMethod != null)
            {
                ilg = new CodeGenerator(typedSerializerTypeBuilder);
                ilg.BeginMethod(
                    WellKnownTypes.Void,
                    "Serialize",
                    new Type[] { WellKnownTypes.Object, WellKnownTypes.XmlSerializationWriter },
                    new string[] { "objectToSerialize", "writer" },
                    CodeGenerator.ProtectedOverrideMethodAttributes);

                ilg.Ldarg("writer");
                ilg.Castclass(writerClass);
                ilg.Ldarg("objectToSerialize");
                if (mapping is XmlMembersMapping)
                {
                    ilg.ConvertValue(WellKnownTypes.Object, WellKnownTypes.ObjectArray);
                }
                ilg.Call(writeMethod);
                ilg.EndMethod();
            }
            if (readMethod != null)
            {
                ilg = new CodeGenerator(typedSerializerTypeBuilder);
                ilg.BeginMethod(
                    WellKnownTypes.Object,
                    "Deserialize",
                    new Type[] { WellKnownTypes.XmlSerializationReader },
                    new string[] { "reader" },
                    CodeGenerator.ProtectedOverrideMethodAttributes);

                ilg.Ldarg("reader");
                ilg.Castclass(readerClass);
                ilg.Call(readMethod);
                ilg.EndMethod();
            }

            defaultCtor = typedSerializerTypeBuilder.DefineDefaultConstructor(CodeGenerator.PublicMethodAttributes);
            return typedSerializerTypeBuilder;
        }

        private FieldBuilder GenerateTypedSerializers(Dictionary<string, ConstructorBuilder> serializerCtors, TypeBuilder serializerContractTypeBuilder)
        {
            string privateName = "typedSerializers";
            FieldBuilder fieldBuilder = GenerateHashtableGetBegin(privateName, "TypedSerializers", serializerContractTypeBuilder);
            MethodInfo Hashtable_Add = WellKnownTypes.DictionaryObjectObject.GetMethod(
                "Add",
                CodeGenerator.InstanceBindingFlags,
                new Type[] { WellKnownTypes.Object, WellKnownTypes.Object }
                );

            // order for determinism:
            foreach (var nameAndCtor in serializerCtors.OrderBy(e => e.Key))
            {
                ilg.Ldloc(WellKnownTypes.DictionaryObjectObject, "_tmp");
                ilg.Ldstr(GetCSharpString(nameAndCtor.Key));
                ilg.New(nameAndCtor.Value);
                ilg.Call(Hashtable_Add);
            }
            GenerateHashtableGetEnd(fieldBuilder);
            return fieldBuilder;
        }

        //GenerateGetSerializer(serializers, xmlMappings);
        private void GenerateGetSerializer(Dictionary<string, ConstructorBuilder> serializerCtors, XmlMapping[] xmlMappings, TypeBuilder serializerContractTypeBuilder)
        {
            ilg = new CodeGenerator(serializerContractTypeBuilder);
            ilg.BeginMethod(
                WellKnownTypes.XmlSerializer,
                "GetSerializer",
                new Type[] { WellKnownTypes.Type },
                new string[] { "type" },
                CodeGenerator.PublicOverrideMethodAttributes);

            for (int i = 0; i < xmlMappings.Length; i++)
            {
                if (xmlMappings[i] is XmlTypeMapping)
                {
                    Type type = xmlMappings[i].Accessor.Mapping.TypeDesc.Type;
                    if (type == null)
                        continue;
                    if (!type.GetTypeInfo().IsPublic && !type.GetTypeInfo().IsNestedPublic)
                        continue;
                    // DDB172141: Wrong generated CS for serializer of List<string> type
                    if (type.GetTypeInfo().IsGenericType || type.GetTypeInfo().ContainsGenericParameters)
                        continue;
                    ilg.Ldarg("type");
                    ilg.Ldc(type);
                    ilg.If(Cmp.EqualTo);
                    {
                        ilg.New(serializerCtors[xmlMappings[i].Key]);
                        ilg.Stloc(ilg.ReturnLocal);
                        ilg.Br(ilg.ReturnLabel);
                    }
                    ilg.EndIf();
                }
            }
            ilg.Load(null);
            ilg.Stloc(ilg.ReturnLocal);
            ilg.Br(ilg.ReturnLabel);
            ilg.MarkLabel(ilg.ReturnLabel);
            ilg.Ldloc(ilg.ReturnLocal);
            ilg.EndMethod();
        }

        internal void GenerateSerializerContract(XmlMapping[] xmlMappings, System.Type[] types, ConstructorBuilder readerCtor, MethodBuilder[] readMethods, ConstructorBuilder writerCtor, MethodBuilder[] writerMethods, Dictionary<string, ConstructorBuilder> serializerCtors)
        {
            TypeBuilder serializerContractTypeBuilder = CodeGenerator.CreateTypeBuilder(
                _moduleBuilder,
                "XmlSerializerContract",
                TypeAttributes.Public | TypeAttributes.BeforeFieldInit,
                WellKnownTypes.XmlSerializerImplementation,
                Array.Empty<Type>()
                );

            ilg = new CodeGenerator(serializerContractTypeBuilder);
            PropertyBuilder propertyBuilder = serializerContractTypeBuilder.DefineProperty(
                "Reader",
                PropertyAttributes.None,
                WellKnownTypes.XmlSerializationReader,
                Array.Empty<Type>());
            ilg.BeginMethod(
                WellKnownTypes.XmlSerializationReader,
                "get_Reader",
                Array.Empty<Type>(),
                Array.Empty<string>(),
                CodeGenerator.PublicOverrideMethodAttributes | MethodAttributes.SpecialName);
            propertyBuilder.SetGetMethod(ilg.MethodBuilder);
            ilg.New(readerCtor);
            ilg.EndMethod();

            ilg = new CodeGenerator(serializerContractTypeBuilder);
            propertyBuilder = serializerContractTypeBuilder.DefineProperty(
                "Writer",
                PropertyAttributes.None,
                WellKnownTypes.XmlSerializationWriter,
                Array.Empty<Type>());
            ilg.BeginMethod(
                WellKnownTypes.XmlSerializationWriter,
                "get_Writer",
                Array.Empty<Type>(),
                Array.Empty<string>(),
                CodeGenerator.PublicOverrideMethodAttributes | MethodAttributes.SpecialName);
            propertyBuilder.SetGetMethod(ilg.MethodBuilder);
            ilg.New(writerCtor);
            ilg.EndMethod();

            FieldBuilder readMethodsField = GeneratePublicMethods("readMethods", "ReadMethods", readMethods, xmlMappings, serializerContractTypeBuilder);
            FieldBuilder writeMethodsField = GeneratePublicMethods("writeMethods", "WriteMethods", writerMethods, xmlMappings, serializerContractTypeBuilder);
            FieldBuilder typedSerializersField = GenerateTypedSerializers(serializerCtors, serializerContractTypeBuilder);
            GenerateSupportedTypes(types, serializerContractTypeBuilder);
            GenerateGetSerializer(serializerCtors, xmlMappings, serializerContractTypeBuilder);

            // Default ctor
            ConstructorInfo baseCtor = WellKnownTypes.XmlSerializerImplementation.GetConstructor(
                CodeGenerator.InstanceBindingFlags,
                Array.Empty<Type>()
                );
            ilg = new CodeGenerator(serializerContractTypeBuilder);
            ilg.BeginMethod(
                WellKnownTypes.Void,
                ".ctor",
                Array.Empty<Type>(),
                Array.Empty<string>(),
                CodeGenerator.PublicMethodAttributes | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName
                );
            ilg.Ldarg(0);
            ilg.Load(null);
            ilg.StoreMember(readMethodsField);
            ilg.Ldarg(0);
            ilg.Load(null);
            ilg.StoreMember(writeMethodsField);
            ilg.Ldarg(0);
            ilg.Load(null);
            ilg.StoreMember(typedSerializersField);
            ilg.Ldarg(0);
            ilg.Call(baseCtor);
            ilg.EndMethod();
        }

        internal static bool IsWildcard(SpecialMapping mapping)
        {
            if (mapping is SerializableMapping)
                return ((SerializableMapping)mapping).IsAny;
            return mapping.TypeDesc.CanBeElementValue;
        }
        internal void ILGenLoad(string source)
        {
            ILGenLoad(source, null);
        }
        internal void ILGenLoad(string source, Type type)
        {
            if (source.StartsWith("o.@", StringComparison.Ordinal))
            {
                System.Diagnostics.Debug.Assert(memberInfos.ContainsKey(source.Substring(3)));
                MemberInfo memInfo = memberInfos[source.Substring(3)];
                ilg.LoadMember(ilg.GetVariable("o"), memInfo);
                if (type != null)
                {
                    Type memType = (memInfo is FieldInfo) ? ((FieldInfo)memInfo).FieldType : ((PropertyInfo)memInfo).PropertyType;
                    ilg.ConvertValue(memType, type);
                }
            }
            else
            {
                SourceInfo info = new SourceInfo(source, null, null, null, ilg);
                info.Load(type);
            }
        }
    }
}
#endif
