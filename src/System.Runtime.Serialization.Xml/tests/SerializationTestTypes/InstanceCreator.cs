namespace SerializationTestTypes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Net;
    using System.IO;
    using System.Data;
    using System.Text;

    public class InstanceCreator
    {
        private static int s_max_array_elements = 6;
        private static int s_max_level = 20;

        public static object CreateInstanceOfArray(Type arrayType, Random rndGen)
        {
            Type type = arrayType.GetElementType();
            double rndNumber = rndGen.NextDouble();
            if (rndNumber < CreatorSettings.NullValueProbability) return null; // 1% chance of null value
            int size = (int)Math.Pow(CreatorSettings.MaxArrayLength, rndNumber); // this will create more small arrays than large ones
            size--;
            Array result = Array.CreateInstance(type, size);
            for (int i = 0; i < size; i++)
            {
                result.SetValue(CreateInstanceOf(type, rndGen), i);
            }
            return result;
        }
        public static object CreateInstanceOfArrayList(Type listType, Random rndGen)
        {
            double rndNumber = rndGen.NextDouble();
            if (rndNumber < CreatorSettings.NullValueProbability) return null; // 1% chance of null value
            int size = (int)Math.Pow(CreatorSettings.MaxListLength, rndNumber); // this will create more small lists than large ones
            size--;
            object result = Activator.CreateInstance(listType);
            MethodInfo addMethod = listType.GetMethod("Add");
            for (int i = 0; i < size; i++)
            {
                addMethod.Invoke(result, new object[] { CreateInstanceOf(typeof(String), rndGen) });
            }
            return result;
        }
        public static object CreateInstanceOfListOfT(Type listType, Random rndGen)
        {
            Type type = listType.GetGenericArguments()[0];
            double rndNumber = rndGen.NextDouble();
            if (rndNumber < CreatorSettings.NullValueProbability) return null; // 1% chance of null value
            int size = (int)Math.Pow(CreatorSettings.MaxListLength, rndNumber); // this will create more small lists than large ones
            size--;
            object result = Activator.CreateInstance(listType);
            MethodInfo addMethod = listType.GetMethod("Add");
            for (int i = 0; i < size; i++)
            {
                addMethod.Invoke(result, new object[] { CreateInstanceOf(type, rndGen) });
            }
            return result;
        }
        public static object CreateInstanceOfNullableOfT(Type nullableOfTType, Random rndGen)
        {
            if (rndGen.Next(5) == 0) return null;
            Type type = nullableOfTType.GetGenericArguments()[0];
            return CreateInstanceOf(type, rndGen);
        }
        public static object CreateInstanceOfEnum(Type enumType, Random rndGen)
        {
            bool hasFlags = enumType.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0;
            Array possibleValues = Enum.GetValues(enumType);
            if (!hasFlags)
            {
                return possibleValues.GetValue(rndGen.Next(possibleValues.Length));
            }
            else
            {
                int result = 0;
                if (rndGen.Next(10) > 0) //10% chance of value zero
                {
                    foreach (object value in possibleValues)
                    {
                        if (rndGen.Next(2) == 0)
                        {
                            result |= ((IConvertible)value).ToInt32(null);
                        }
                    }
                }
                return result;
            }
        }
        public static Array CreateInstanceOfSystemArray(Random rndGen)
        {
            Type[] memberTypes = new Type[] {
                typeof(string),
                typeof(int),
                typeof(long),
                typeof(byte),
                typeof(short),
                typeof(double),
                typeof(decimal),
                typeof(float),
                typeof(object),
                typeof(DateTime),
                typeof(TimeSpan),
                typeof(Guid),
                typeof(Uri),
                typeof(XmlQualifiedName),
            };
            double rndNumber = rndGen.NextDouble();
            if (rndNumber < CreatorSettings.NullValueProbability) return null; // 1% chance of null value
            int size = (int)Math.Pow(CreatorSettings.MaxArrayLength, rndNumber); // this will create more small arrays than large ones
            size--;
            Array result = new object[size];
            for (int i = 0; i < size; i++)
            {
                Type elementType = memberTypes[rndGen.Next(memberTypes.Length)];
                result.SetValue(CreateInstanceOf(elementType, rndGen), i);
            }
            return result;
        }
        public static object CreateInstanceOfDictionaryOfKAndV(Type dictionaryType, Random rndGen)
        {
            double nullValueProbability = CreatorSettings.NullValueProbability;
            Type[] genericArgs = dictionaryType.GetGenericArguments();
            Type typeK = genericArgs[0];
            Type typeV = genericArgs[1];
            double rndNumber = rndGen.NextDouble();
            if (rndNumber < CreatorSettings.NullValueProbability) return null; // 1% chance of null value
            int size = (int)Math.Pow(CreatorSettings.MaxListLength, rndNumber); // this will create more small dictionaries than large ones
            size--;
            object result = Activator.CreateInstance(dictionaryType);
            MethodInfo addMethod = dictionaryType.GetMethod("Add");
            MethodInfo containsKeyMethod = dictionaryType.GetMethod("ContainsKey");
            for (int i = 0; i < size; i++)
            {
                //Dictionary Keys cannot be null.Set null probability to 0
                CreatorSettings.NullValueProbability = 0;
                object newKey = CreateInstanceOf(typeK, rndGen);
                //Reset null instance probability
                CreatorSettings.NullValueProbability = nullValueProbability;
                bool containsKey = (bool)containsKeyMethod.Invoke(result, new object[] { newKey });
                if (!containsKey)
                {
                    object newValue = CreateInstanceOf(typeV, rndGen);
                    addMethod.Invoke(result, new object[] { newKey, newValue });
                }
            }
            return result;
        }
        internal static bool ContainsAttribute(MemberInfo member, Type attributeType)
        {
            object[] attributes = member.GetCustomAttributes(attributeType, false);
            return (attributes != null && attributes.Length > 0);
        }

        /// <summary>
        /// Creates an instance of the given type.
        /// </summary>
        /// <param name="type">The type to create an instance from.</param>
        /// <param name="rndGen">A random generator used to populate the instance.</param>
        /// <returns>An instance of the given type.</returns>
        public static object CreateInstanceOf(Type type, Random rndGen)
        {
            if (PrimitiveCreator.CanCreateInstanceOf(type))
            {
                return PrimitiveCreator.CreatePrimitiveInstance(type, rndGen);
            }
            if (type.Equals(typeof(System.Collections.ArrayList)))
            {
                return CreateInstanceOfArrayList(type, rndGen);
            }
            if (type.IsArray)
            {
                return CreateInstanceOfArray(type, rndGen);
            }
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return CreateInstanceOfNullableOfT(type, rndGen);
                }
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return CreateInstanceOfListOfT(type, rndGen);
                }
                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    return CreateInstanceOfDictionaryOfKAndV(type, rndGen);
                }
            }
            if (type == typeof(System.Array))
            {
                return CreateInstanceOfSystemArray(rndGen);
            }
            if (type.IsEnum)
            {
                return CreateInstanceOfEnum(type, rndGen);
            }

            if (ContainsAttribute(type, typeof(DataContractAttribute)))
            {
                return DataContractInstanceCreator.CreateInstanceOf(type, rndGen);
            }
            if (type.IsPublic)
            {
                return POCOInstanceCreator.CreateInstanceOf(type, rndGen);
            }
            return Activator.CreateInstance(type);
        }

        #region To Support VTS Tests and also can be used for future tests

        public static object CreateObjectOfType(Type t)
        {
            List<Type> kts = new List<Type>();
            return CreateObjectOfType(t, ref kts);
        }

        public static object CreateObjectOfType(Type t, ref List<Type> requiredKnownTypes)
        {
            int level = 0;
            return CreateObjectOfType(t, ref level, ref requiredKnownTypes);
        }

        private static object CreateObjectOfType(Type t, ref int level, ref List<Type> requiredKnownTypes)
        {
            if (level++ > s_max_level)//Safety checkpoint to reduce risk of stack overflow
            {
                level = 0;
                return null;
            }

            if (t.FullName == "System.String")
            {
                return "http://username:password@localhost:8080/Hello#frag1?var1=value1";
            }
            if (t.FullName == "System.Int32")
            {
                return 0;
            }
            if (t.FullName == "System.Object")
            {
                return new Object();
            }
            if (t.FullName == "System.Uri")
            {
                //                UriGen uriGen = new UriGen(UriStructure.All);
                //              return uriGen.GetNextUri(); ;
                return new Uri("http://www.microsoft.com/NonExistingPath/SomeService.svc?wsdl");
            }

            if (t.IsEnum)
            {
                Array arrayOFEnumValues = Enum.GetNames(t);
                return Enum.Parse(t, (string)arrayOFEnumValues.GetValue(0));
            }
            if (t.FullName == "System.ServiceModel.EndpointAddress")
            {
                return Activator.CreateInstance(t, "http://www.microsoft.com");
            }
            if (t.FullName == "System.ServiceModel.EndpointAddress")
            {
                return Activator.CreateInstance(t, "http://www.microsoft.com");
            }
            if (t.FullName == "System.Net.IPAddress")
            {
                return IPAddress.Any;
            }

            if (t.FullName.StartsWith("System.Nullable`1[[System.DateTimeOffset"))
            {
                requiredKnownTypes.Add(typeof(Nullable<DateTimeOffset>));
                requiredKnownTypes.Add(typeof(DateTimeOffset));
                return new Nullable<DateTimeOffset>(DateTimeOffset.Now);
            }

            if (t.IsArray && t.HasElementType)
            {
                Array resultObj = Array.CreateInstance(t.GetElementType(), s_max_array_elements);
                for (int i = 0; i < s_max_array_elements; i++)
                {
                    resultObj.SetValue(CreateObjectOfType(t.GetElementType(), ref level, ref requiredKnownTypes), i);
                }
                if (!requiredKnownTypes.Contains(t.GetElementType()))
                {
                    requiredKnownTypes.Add(t.GetElementType());
                }
                return resultObj;
            }

            if (t.FullName.StartsWith("System.Nullable`1[["))
            {
                string innerTypeName = t.FullName.Replace("System.Nullable`1[[", String.Empty).Replace("]]", String.Empty);
                Type innerType = Type.GetType(innerTypeName, false);
                if (innerType != null)
                {
                    ConstructorInfo cinfo = t.GetConstructor(new Type[] { innerType });
                    if (cinfo != null)
                    {
                        return cinfo.Invoke(new object[] { CreateObjectOfType(innerType) });
                    }
                }
            }

            if (t.IsValueType && !t.ContainsGenericParameters)
            {
                return Activator.CreateInstance(t);
            }


            Type resultType = t;
            if (t.IsGenericTypeDefinition)
            {
                Type[] genericArguments = t.GetGenericArguments();
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    genericArguments[i] = typeof(int);
                }
                resultType = t.MakeGenericType(genericArguments);
                if (!requiredKnownTypes.Contains(resultType))
                {
                    requiredKnownTypes.Add(resultType);
                }
            }

            // This is a test convention, where types with special init logic have a .ctor(bool init)
            ConstructorInfo initConstr = resultType.GetConstructor(new Type[] { typeof(bool) });
            if (initConstr != null)
            {
                return initConstr.Invoke(new object[] { true });
            }

            ConstructorInfo constr = resultType.GetConstructor(new Type[0]);
            if (constr != null)
            {
                return constr.Invoke(new object[0]);
            }
            else
            {
                return CreateNonDefaultCtorObj(resultType, ref level, ref requiredKnownTypes);
            }
        }

        private static object CreateNonDefaultCtorObj(Type resultType, ref int level, ref List<Type> requiredKnownTypes)
        {
            object result = null;

            ConstructorInfo[] constrs = resultType.GetConstructors();
            if (constrs == null || constrs.Length == 0)
            {
                return null;
            }
            ConstructorInfo resultCtor = constrs[0];

            foreach (ConstructorInfo con in constrs)
            {
                if (resultCtor.GetParameters().Length > con.GetParameters().Length
                    || (con.GetParameters().Length == 1 && con.GetParameters()[0].ParameterType == typeof(String)))
                {
                    resultCtor = con;
                }
            }

            object[] paras = new object[resultCtor.GetParameters().Length];
            int i = 0;
            foreach (ParameterInfo para in resultCtor.GetParameters())
            {
                object paraObj = null;

                if (para.ParameterType.Name.EndsWith("Ptr"))//not dealing with ptr's now
                {
                    return null;
                }
                if (para.ParameterType == typeof(String))
                {
                    if (para.Name.ToLower().Contains("file"))
                    {
                        paraObj = @"c:\temp\t.tmp";
                    }
                    else if (para.Name.ToLower().Contains("dir") || para.Name.ToLower().Contains("path"))
                    {
                        paraObj = @"c:\temp";
                    }
                    else
                    {
                        paraObj = "http://username:password@localhost:8080/Hello#frag1?var1=value1";
                    }
                }
                else
                {
                    paraObj = CreateObjectOfType(para.ParameterType, ref level, ref requiredKnownTypes);
                }

                paras[i++] = paraObj;
            }
            result = resultCtor.Invoke(paras);
            return result;
        }

        #endregion
    }

    public static class POCOInstanceCreator
    {
        private static int CompareMembers(MemberInfo member1, MemberInfo member2)
        {
            return member1.Name.CompareTo(member2.Name);
        }
        private static void FilterIgnoredDataMembers<T>(List<T> list) where T : MemberInfo
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                object[] customAttributes = ((MemberInfo)list[i]).GetCustomAttributes(false);
                foreach (object attribute in customAttributes)
                {
                    if (attribute != null && attribute.GetType().Name == "IgnoreDataMember")
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        private static void SetPublicFields(Type dcType, object obj, Random rndGen)
        {
            List<FieldInfo> fields = new List<FieldInfo>(dcType.GetFields(BindingFlags.Public | BindingFlags.Instance));
            FilterIgnoredDataMembers<FieldInfo>(fields);
            fields.Sort(new Comparison<FieldInfo>(CompareMembers));
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttributes(typeof(IgnoreMemberAttribute), false).Length == 0)
                {
                    //Set the new value only if the value was not set by default.
                    object fieldValue = InstanceCreator.CreateInstanceOf(field.FieldType, rndGen);
                    field.SetValue(obj, fieldValue);
                }
            }
        }
        private static void SetPublicProperties(Type dcType, object obj, Random rndGen, bool setInternalSetters)
        {
            try
            {
                List<PropertyInfo> properties = new List<PropertyInfo>(dcType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                FilterIgnoredDataMembers<PropertyInfo>(properties);
                properties.Sort(new Comparison<PropertyInfo>(CompareMembers));
                foreach (PropertyInfo property in properties)
                {
                    if (!setInternalSetters && !property.GetSetMethod().IsPublic)
                    {
                        continue;
                    }

                    object propertyValue = InstanceCreator.CreateInstanceOf(property.PropertyType, rndGen);
                    property.SetValue(obj, propertyValue, null);
                }
            }
            catch (Exception)
            {
            }
        }
        public static object CreateInstanceOf(Type pocoType, Random rndGen)
        {
            object result = null;
            if (rndGen.NextDouble() < CreatorSettings.NullValueProbability && !pocoType.IsValueType)
            {
                // 1% chance of null object, if it is not a struct
                return null;
            }

            //Test convention, where types with special init logic have a .ctor(bool init)
            ConstructorInfo boolConstructor = pocoType.GetConstructor(new Type[] { typeof(bool) });
            if (boolConstructor != null)
            {
                return boolConstructor.Invoke(new object[] { true });
            }
            else
            {
                ConstructorInfo randomConstructor = pocoType.GetConstructor(new Type[] { typeof(Random) });
                if (randomConstructor != null)
                {
                    result = randomConstructor.Invoke(new object[] { rndGen });
                }
                else
                {
                    ConstructorInfo defaultConstructor = pocoType.GetConstructor(new Type[0]);
                    if (defaultConstructor != null || pocoType.IsValueType)
                    {
                        if (defaultConstructor != null)
                        {
                            result = defaultConstructor.Invoke(new object[0]);
                        }
                        else
                        {
                            result = Activator.CreateInstance(pocoType);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Don't know how to create an instance of " + pocoType.FullName);
                    }

                    SetPublicFields(pocoType, result, rndGen);
                    SetPublicProperties(pocoType, result, rndGen, CreatorSettings.SetPOCONonPublicSetters);
                }
                return result;
            }
        }
    }

    public static class CreatorSettings
    {
        public static int MaxArrayLength = 10;
        public static int MaxListLength = 10;
        public static int MaxStringLength = 100;
        public static bool CreateOnlyAsciiChars = false;
        public static bool DontCreateSurrogateChars = false;
        public static bool CreateDateTimeWithSubMilliseconds = true;
        public static bool NormalizeEndOfLineOnXmlNodes = false;
        public static double NullValueProbability = 0.01;
        public static bool SetPOCONonPublicSetters = true;
        public static InstanceCreatorSurrogate CreatorSurrogate = null;
    }

    public abstract class InstanceCreatorSurrogate
    {
        /// <summary>
        /// Checks whether this surrogate can create instances of a given type.
        /// </summary>
        /// <param name="type">The type which needs to be created.</param>
        /// <returns>A true value if this surrogate can create the given type; a
        /// false value otherwise.</returns>
        public abstract bool CanCreateInstanceOf(Type type);

        /// <summary>
        /// Creates an instance of the given type.
        /// </summary>
        /// <param name="type">The type to create an instance for.</param>
        /// <param name="rndGen">A Random generator to assist in creating the instance.</param>
        /// <returns>An instance of the given type.</returns>
        public abstract object CreateInstanceOf(Type type, Random rndGen);
    }

    public static class DataContractInstanceCreator
    {
        private static string GetDataMemberName(MemberInfo member)
        {
            DataMemberAttribute[] dataMemberAttr = (DataMemberAttribute[])member.GetCustomAttributes(typeof(DataMemberAttribute), false);
            if (dataMemberAttr == null || dataMemberAttr.Length == 0 || dataMemberAttr[0].Name == null)
            {
                return member.Name;
            }
            else
            {
                return dataMemberAttr[0].Name;
            }
        }
        private static int CompareMembers(MemberInfo member1, MemberInfo member2)
        {
            return GetDataMemberName(member1).CompareTo(GetDataMemberName(member2));
        }
        private static void FilterNonDataMembers<T>(List<T> list) where T : MemberInfo
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!InstanceCreator.ContainsAttribute((MemberInfo)list[i], typeof(DataMemberAttribute)))
                {
                    list.RemoveAt(i);
                }
            }
        }
        private static void SetPublicFields(Type dcType, object obj, Random rndGen)
        {
            List<FieldInfo> fields = new List<FieldInfo>(dcType.GetFields(BindingFlags.Public | BindingFlags.Instance));
            FilterNonDataMembers<FieldInfo>(fields);
            fields.Sort(new Comparison<FieldInfo>(CompareMembers));
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttributes(typeof(IgnoreMemberAttribute), false).Length == 0)
                {
                    object fieldValue = InstanceCreator.CreateInstanceOf(field.FieldType, rndGen);
                    field.SetValue(obj, fieldValue);
                }
            }
        }
        private static void SetPublicProperties(Type dcType, object obj, Random rndGen)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>(dcType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            FilterNonDataMembers<PropertyInfo>(properties);
            properties.Sort(new Comparison<PropertyInfo>(CompareMembers));
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(IgnoreMemberAttribute), false).Length == 0)
                {
                    object propertyValue = InstanceCreator.CreateInstanceOf(property.PropertyType, rndGen);
                    property.SetValue(obj, propertyValue, null);
                }
            }
        }
        public static object CreateInstanceOf(Type dcType, Random rndGen)
        {
            object result = null;
            if (rndGen.NextDouble() < CreatorSettings.NullValueProbability && !dcType.IsValueType)
            {
                // 1% chance of null object, if it is not a struct
                return null;
            }
            //Test convention, where types with special init logic have a .ctor(bool init)
            ConstructorInfo boolConstructor = dcType.GetConstructor(new Type[] { typeof(bool) });
            if (boolConstructor != null)
            {
                return boolConstructor.Invoke(new object[] { true });
            }
            else
            {
                ConstructorInfo randomConstructor = dcType.GetConstructor(new Type[] { typeof(Random) });
                if (randomConstructor != null)
                {
                    result = randomConstructor.Invoke(new object[] { rndGen });
                }
                else
                {
                    ConstructorInfo defaultConstructor = dcType.GetConstructor(new Type[0]);
                    if (defaultConstructor != null || dcType.IsValueType)
                    {
                        if (defaultConstructor != null)
                        {
                            result = defaultConstructor.Invoke(new object[0]);
                        }
                        else
                        {
                            result = Activator.CreateInstance(dcType);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Don't know how to create an instance of " + dcType.FullName);
                    }
                    SetPublicFields(dcType, result, rndGen);
                    SetPublicProperties(dcType, result, rndGen);
                }
            }
            return result;
        }
    }

    public static class PrimitiveCreator
    {
        private static Dictionary<Type, MethodInfo> s_creators;
        static PrimitiveCreator()
        {
            Type primitiveCreatorType = typeof(PrimitiveCreator);
            s_creators = new Dictionary<Type, MethodInfo>();
            s_creators.Add(typeof(Boolean), primitiveCreatorType.GetMethod("CreateInstanceOfBoolean", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Byte), primitiveCreatorType.GetMethod("CreateInstanceOfByte", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Char), primitiveCreatorType.GetMethod("CreateInstanceOfChar", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(DateTime), primitiveCreatorType.GetMethod("CreateInstanceOfDateTime", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(DateTimeOffset), primitiveCreatorType.GetMethod("CreateInstanceOfDateTimeOffset", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(DBNull), primitiveCreatorType.GetMethod("CreateInstanceOfDBNull", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Decimal), primitiveCreatorType.GetMethod("CreateInstanceOfDecimal", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Double), primitiveCreatorType.GetMethod("CreateInstanceOfDouble", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Guid), primitiveCreatorType.GetMethod("CreateInstanceOfGuid", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Int16), primitiveCreatorType.GetMethod("CreateInstanceOfInt16", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Int32), primitiveCreatorType.GetMethod("CreateInstanceOfInt32", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Int64), primitiveCreatorType.GetMethod("CreateInstanceOfInt64", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Object), primitiveCreatorType.GetMethod("CreateInstanceOfObject", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(SByte), primitiveCreatorType.GetMethod("CreateInstanceOfSByte", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Single), primitiveCreatorType.GetMethod("CreateInstanceOfSingle", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(String), primitiveCreatorType.GetMethod("CreateInstanceOfString", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(Random) }, null));
            s_creators.Add(typeof(TimeSpan), primitiveCreatorType.GetMethod("CreateInstanceOfTimeSpan", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(UInt16), primitiveCreatorType.GetMethod("CreateInstanceOfUInt16", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(UInt32), primitiveCreatorType.GetMethod("CreateInstanceOfUInt32", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(UInt64), primitiveCreatorType.GetMethod("CreateInstanceOfUInt64", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Uri), primitiveCreatorType.GetMethod("CreateInstanceOfUri", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(XmlQualifiedName), primitiveCreatorType.GetMethod("CreateInstanceOfXmlQualifiedName", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(Stream), primitiveCreatorType.GetMethod("CreateInstanceOfStream", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(XmlElement), primitiveCreatorType.GetMethod("CreateInstanceOfXmlElement", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(XmlNode[]), primitiveCreatorType.GetMethod("CreateInstanceOfXmlNodeArray", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(DataTable), primitiveCreatorType.GetMethod("CreateInstanceOfDataTable", BindingFlags.Public | BindingFlags.Static));
            s_creators.Add(typeof(DataSet), primitiveCreatorType.GetMethod("CreateInstanceOfDataSet", BindingFlags.Public | BindingFlags.Static));
        }
        //public static System.Boolean CreateInstanceOfBoolean(Random rndGen)
        //{
        //    return (rndGen.Next(2) == 0);
        //}

        public static System.Boolean CreateInstanceOfBoolean(Random rndGen)
        {
            return true;
        }

        //public static System.Byte CreateInstanceOfByte(Random rndGen)
        //{
        //    byte[] rndValue = new byte[1];
        //    rndGen.NextBytes(rndValue);
        //    return rndValue[0];
        //}
        public static System.Byte CreateInstanceOfByte(Random rndGen)
        {
            return Byte.MaxValue;
        }


        public static System.Char CreateInstanceOfChar(Random rndGen)
        {
            return Char.MaxValue;
        }

        //public static System.Char CreateInstanceOfChar(Random rndGen)
        //{
        //    if (CreatorSettings.CreateOnlyAsciiChars)
        //    {
        //        return (Char)rndGen.Next(0x20, 0x7F);
        //    }
        //    else if (CreatorSettings.DontCreateSurrogateChars)
        //    {
        //        char c;
        //        do
        //        {
        //            c = (Char)rndGen.Next((int)Char.MinValue, (int)Char.MaxValue);
        //        } while (Char.IsSurrogate(c));
        //        return c;
        //    }
        //    else
        //    {
        //        return (Char)rndGen.Next((int)Char.MinValue, (int)Char.MaxValue + 1);
        //    }
        //}

        public static System.DateTime CreateInstanceOfDateTime(Random rndGen)
        {
            return new DateTime();
        }
        //public static System.DateTime CreateInstanceOfDateTime(Random rndGen)
        //{
        //    long temp = CreateInstanceOfInt64(rndGen);
        //    temp = Math.Abs(temp);
        //    DateTime result;
        //    try
        //    {
        //        result = new DateTime(temp % (DateTime.MaxValue.Ticks + 1));
        //    }
        //    catch (ArgumentOutOfRangeException) // jasonv - approved; specific, commented
        //    {
        //        // From http://msdn.microsoft.com/en-us/library/z2xf7zzk.aspx
        //        // ticks is less than MinValue or greater than MaxValue. 
        //        result = DateTime.Now;
        //    }

            //    int kind = rndGen.Next(3);
            //    switch (kind)
            //    {
            //        case 0:
            //            result = DateTime.SpecifyKind(result, DateTimeKind.Local);
            //            break;
            //        case 1:
            //            result = DateTime.SpecifyKind(result, DateTimeKind.Unspecified);
            //            break;
            //        default:
            //            result = DateTime.SpecifyKind(result, DateTimeKind.Utc);
            //            break;
            //    }

            //    if (!CreatorSettings.CreateDateTimeWithSubMilliseconds)
            //    {
            //        result = new DateTime(result.Year, result.Month, result.Day,
            //            result.Hour, result.Minute, result.Second, result.Millisecond, result.Kind);
            //    }

            //    return result;
            //}

        public static System.DateTimeOffset CreateInstanceOfDateTimeOffset(Random rndGen)
        {
            return new DateTimeOffset();
        }

        //public static System.DateTimeOffset CreateInstanceOfDateTimeOffset(Random rndGen)
        //{
        //    DateTime temp = CreateInstanceOfDateTime(rndGen);
        //    temp = DateTime.SpecifyKind(temp, DateTimeKind.Unspecified);
        //    int offsetMinutes = rndGen.Next(-14 * 60, 14 * 60);
        //    DateTimeOffset result = new DateTimeOffset(temp, TimeSpan.FromMinutes(offsetMinutes));
        //    return result;
        //}

        public static System.DBNull CreateInstanceOfDBNull(Random rndGen)
        {
            return DBNull.Value;
        }

        //public static System.DBNull CreateInstanceOfDBNull(Random rndGen)
        //{
        //    return (rndGen.Next(2) == 0) ? null : DBNull.Value;
        //}

        public static System.Decimal CreateInstanceOfDecimal(Random rndGen)
        {
            return new Decimal();
        }
        //public static System.Decimal CreateInstanceOfDecimal(Random rndGen)
        //{
        //    int low = CreateInstanceOfInt32(rndGen);
        //    int mid = CreateInstanceOfInt32(rndGen);
        //    int high = CreateInstanceOfInt32(rndGen);
        //    bool isNegative = (rndGen.Next(2) == 0);
        //    const int maxDecimalScale = 28;
        //    byte scale = (byte)rndGen.Next(0, maxDecimalScale + 1);
        //    return new Decimal(low, mid, high, isNegative, scale);
        //}

        public static System.Double CreateInstanceOfDouble(Random rndGen)
        {
            return Double.MaxValue;
        }

        //public static System.Double CreateInstanceOfDouble(Random rndGen)
        //{
        //    bool negative = (rndGen.Next(2) == 0);
        //    int temp = rndGen.Next(40);
        //    Double result;
        //    switch (temp)
        //    {
        //        case 0: return Double.NaN;
        //        case 1: return Double.PositiveInfinity;
        //        case 2: return Double.NegativeInfinity;
        //        case 3: return Double.MinValue;
        //        case 4: return Double.MaxValue;
        //        case 5: return Double.Epsilon;
        //        default:
        //            result = (Double)(rndGen.NextDouble() * 100000);
        //            if (negative) result = -result;
        //            return result;
        //    }
        //}

        //public static System.Guid CreateInstanceOfGuid(Random rndGen)
        //{
        //    byte[] temp = new byte[16];
        //    rndGen.NextBytes(temp);
        //    return new Guid("028150e9-f0e8-49e0-99e9-d552f7f40d37");
        //}

        public static System.Guid CreateInstanceOfGuid(Random rndGen)
        {
            return new Guid("028150e9-f0e8-49e0-99e9-d552f7f40d37");
        }

        //public static System.Int16 CreateInstanceOfInt16(Random rndGen)
        //{
        //    byte[] rndValue = new byte[2];
        //    rndGen.NextBytes(rndValue);
        //    Int16 result = 0;
        //    for (int i = 0; i < rndValue.Length; i++)
        //    {
        //        result = (Int16)(result << 8);
        //        result = (Int16)(result | (Int16)rndValue[i]);
        //    }
        //    return result;
        //}

        public static System.Int16 CreateInstanceOfInt16(Random rndGen)
        {
            return Int16.MaxValue;
        }

        public static System.Int32 CreateInstanceOfInt32(Random rndGen)
        {
            return 666;
        }

        //public static System.Int32 CreateInstanceOfInt32(Random rndGen)
        //{
        //    byte[] rndValue = new byte[4];
        //    rndGen.NextBytes(rndValue);
        //    Int32 result = 0;
        //    for (int i = 0; i < rndValue.Length; i++)
        //    {
        //        result = (Int32)(result << 8);
        //        result = (Int32)(result | (Int32)rndValue[i]);
        //    }
        //    return result;
        //}

        //public static System.Int64 CreateInstanceOfInt64(Random rndGen)
        //{
        //    byte[] rndValue = new byte[8];
        //    rndGen.NextBytes(rndValue);
        //    Int64 result = 0;
        //    for (int i = 0; i < rndValue.Length; i++)
        //    {
        //        result = (Int64)(result << 8);
        //        result = (Int64)(result | (Int64)rndValue[i]);
        //    }
        //    return result;
        //}

        public static System.Int64 CreateInstanceOfInt64(Random rndGen)
        {
            return Int64.MaxValue;
        }

        public static System.Object CreateInstanceOfObject(Random rndGen)
        {
            return new Object();
        }

        public static System.SByte CreateInstanceOfSByte(Random rndGen)
        {
            return SByte.MaxValue;
        }

        public static System.Single CreateInstanceOfSingle(Random rndGen)
        {
            return Single.MaxValue;
        }

        internal static string CreateRandomString(Random rndGen, int size, string charsToUse)
        {
            return "hello";
            //int maxSize = CreatorSettings.MaxStringLength;
            //// invalid per the XML spec (http://www.w3.org/TR/REC-xml/#charsets), cannot be sent as XML
            //const string InvalidXmlChars = "\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u000B\u000C\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F\uFFFE\uFFFF";
            //const int LowSurrogateMin = 0xDC00;
            //const int LowSurrogateMax = 0xDFFF;
            //const int HighSurrogateMin = 0xD800;
            //const int HighSurrogateMax = 0xDBFF;

            //if (size < 0)
            //{
            //    double rndNumber = Double.NaN;
            //    if (rndNumber < CreatorSettings.NullValueProbability) return null; // 1% chance of null value
            //    size = (int)Math.Pow(maxSize, rndNumber); // this will create more small strings than large ones
            //    size--;
            //}
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < size; i++)
            //{
            //    char c;
            //    if (charsToUse != null)
            //    {
            //        c = charsToUse[rndGen.Next(charsToUse.Length)];
            //        sb.Append(c);
            //    }
            //    else
            //    {
            //        if (CreatorSettings.CreateOnlyAsciiChars || rndGen.Next(2) == 0)
            //        {
            //            c = (char)rndGen.Next(0x20, 0x7F); // low-ascii chars
            //            sb.Append(c);
            //        }
            //        else
            //        {
            //            do
            //            {
            //                c = (char)rndGen.Next((int)char.MinValue, (int)char.MaxValue + 1);
            //            } while ((LowSurrogateMin <= c && c <= LowSurrogateMax) || (InvalidXmlChars.IndexOf(c) >= 0));
            //            sb.Append(c);
            //            if (HighSurrogateMin <= c && c <= HighSurrogateMax) // need to add a low surrogate
            //            {
            //                c = (char)rndGen.Next(LowSurrogateMin, LowSurrogateMax + 1);
            //                sb.Append(c);
            //            }
            //        }
            //    }
            //}
            //return sb.ToString();
        }

        public static System.String CreateInstanceOfString(Random rndGen)
        {           
            return "hello";
        }

        //public static System.String CreateInstanceOfString(Random rndGen)
        //{
        //    double rndNumber = rndGen.NextDouble();
        //    if (rndNumber < CreatorSettings.NullValueProbability)
        //    {
        //        return null;
        //    }
        //    return CreateRandomString(rndGen, -1, null);
        //}

        public static System.String CreateInstanceOfString(Random rndGen, int size, string charsToUse)
        {
            return CreateRandomString(rndGen, size, charsToUse);
        }

        public static System.TimeSpan CreateInstanceOfTimeSpan(Random rndGen)
        {
            return TimeSpan.MinValue;
        }

        public static System.UInt16 CreateInstanceOfUInt16(Random rndGen)
        {
            return UInt16.MaxValue;
        }

        public static System.UInt32 CreateInstanceOfUInt32(Random rndGen)
        {
            return UInt32.MaxValue;
        }

        public static System.UInt64 CreateInstanceOfUInt64(Random rndGen)
        { 
            return UInt64.MaxValue;
        }

        /// <summary>
        /// Creates URI instances based on RFC 2396
        /// </summary>
        internal static class UriCreator
        {
            private static readonly string s_digit;
            private static readonly string s_upalpha;
            private static readonly string s_lowalpha;
            private static readonly string s_alpha;
            private static readonly string s_alphanum;
            private static readonly string s_hex;
            private static readonly string s_mark;
            private static readonly string s_unreserved;
            private static readonly string s_reserved;

            static UriCreator()
            {
                s_digit = "0123456789";
                s_upalpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                s_lowalpha = s_upalpha.ToLower();
                s_alpha = s_upalpha + s_lowalpha;
                s_alphanum = s_alpha + s_digit;
                s_hex = s_digit + "ABCDEFabcdef";
                s_mark = "-_.!~*'()";
                s_unreserved = s_alphanum + s_mark;
                s_reserved = ";/?:@&=+$,";
            }
            private static void CreateScheme(StringBuilder sb, Random rndGen)
            {
                int size = 8;
                AddChars(sb, rndGen, s_alpha, 1);
                string schemeChars = s_alpha + s_digit + "+-.";
                AddChars(sb, rndGen, schemeChars, size);
                sb.Append(':');
            }
            private static void CreateIPv4Address(StringBuilder sb, Random rndGen)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i > 0) sb.Append('.');
                    sb.Append(1);
                }
            }
            private static void AddIPv6AddressPart(StringBuilder sb, Random rndGen)
            {
                int size = 4;
                AddChars(sb, rndGen, s_hex, size);
            }
            private static void CreateIPv6Address(StringBuilder sb, Random rndGen)
            {
                sb.Append('[');
                int temp = rndGen.Next(6);
                int i;
                switch (temp)
                {
                    case 0:
                        sb.Append("::");
                        break;
                    case 1:
                        sb.Append("::1");
                        break;
                    case 2:
                        sb.Append("FF01::101");
                        break;
                    case 3:
                        sb.Append("::1");
                        break;
                    case 4:
                        for (i = 0; i < 3; i++)
                        {
                            AddIPv6AddressPart(sb, rndGen);
                            sb.Append(':');
                        }
                        for (i = 0; i < 3; i++)
                        {
                            sb.Append(':');
                            AddIPv6AddressPart(sb, rndGen);
                        }
                        break;
                    default:
                        for (i = 0; i < 8; i++)
                        {
                            if (i > 0) sb.Append(':');
                            AddIPv6AddressPart(sb, rndGen);
                        }
                        break;
                }
                sb.Append(']');
            }
            private static void AddChars(StringBuilder sb, Random rndGen, string validChars, int size)
            {
                for (int i = 0; i < size; i++)
                {
                    sb.Append(validChars[i]);
                }
            }
            private static void CreateHostName(StringBuilder sb, Random rndGen)
            {
                int domainLabelCount = rndGen.Next(4);
                int size;
                for (int i = 0; i < domainLabelCount; i++)
                {
                    AddChars(sb, rndGen, s_alphanum, 1);
                    size = rndGen.Next(10) - 1;
                    if (size > 0)
                    {
                        AddChars(sb, rndGen, s_alphanum + "-", size);
                        AddChars(sb, rndGen, s_alphanum, 1);
                    }
                    sb.Append('.');
                }
                AddChars(sb, rndGen, s_alpha, 1);
                size = rndGen.Next(10) - 1;
                if (size > 0)
                {
                    AddChars(sb, rndGen, s_alphanum + "-", size);
                    AddChars(sb, rndGen, s_alphanum, 1);
                }
            }
            private static void CreateHost(StringBuilder sb, Random rndGen)
            {
                //int temp = rndGen.Next(3);
                //switch (temp)
                //{
                //    case 0:
                        CreateIPv4Address(sb, rndGen);
                //        break;
                //    case 1:
                //        CreateIPv6Address(sb, rndGen);
                //        break;
                //    case 2:
                //        CreateHostName(sb, rndGen);
                //        break;
                //}
            }
            private static void CreateUserInfo(StringBuilder sb, Random rndGen)
            {
                AddChars(sb, rndGen, s_alpha, 7);
                //if (rndGen.Next(3) > 0)
                {
                    sb.Append(':');
                    AddChars(sb, rndGen, s_alpha, 7);
                }
                sb.Append('@');
            }
            private static void AddEscapedChar(StringBuilder sb, Random rndGen)
            {
                sb.Append('%');
                AddChars(sb, rndGen, s_hex, 2);
            }
            private static void AddPathSegment(StringBuilder sb, Random rndGen)
            {
                string pchar = s_unreserved + ":@&=+$,";
                int size = rndGen.Next(1, 10);
                for (int i = 0; i < size; i++)
                {
                    if (rndGen.Next(pchar.Length + 1) > 0)
                    {
                        AddChars(sb, rndGen, pchar, 1);
                    }
                    else
                    {
                        AddEscapedChar(sb, rndGen);
                    }
                }
            }
            private static void AddUriC(StringBuilder sb, Random rndGen)
            {
                int size = 15;
                string reservedPlusUnreserved = s_reserved + s_unreserved;
                for (int i = 0; i < size; i++)
                {
                    //if (rndGen.Next(5) > 0)
                    {
                        AddChars(sb, rndGen, reservedPlusUnreserved, 1);
                    }
                    //else
                    {
                        //AddEscapedChar(sb, rndGen);
                    }
                }
            }
            internal static string CreateUri(Random rndGen, out UriKind kind)
            {
                StringBuilder sb = new StringBuilder();
                //kind = UriKind.Relative;
                //Devdiv bug 187103
                kind = UriKind.Absolute;
                //if (rndGen.Next(3) > 0)
                {
                    // Add URI scheme
                    CreateScheme(sb, rndGen);
                    kind = UriKind.Absolute;
                }
                //if (rndGen.Next(3) > 0)
                //{
                //    // Add URI host
                //    sb.Append("//");
                //    if (rndGen.Next(10) == 0)
                //    {
                //        CreateUserInfo(sb, rndGen);
                //    }
                //    CreateHost(sb, rndGen);
                //    if (rndGen.Next(2) > 0)
                //    {
                //        sb.Append(':');
                //        sb.Append(rndGen.Next(65536));
                //    }
                //}
                //if (rndGen.Next(4) > 0)
                //{
                //    // Add URI path
                //    for (int i = 0; i < rndGen.Next(1, 4); i++)
                //    {
                //        sb.Append('/');
                //        AddPathSegment(sb, rndGen);
                //    }
                //}
                //if (rndGen.Next(3) == 0)
                //{
                //    // Add URI query string
                //    sb.Append('?');
                //    AddUriC(sb, rndGen);
                //}
                return sb.ToString();
            }
        }

        public static System.Uri CreateInstanceOfUri(Random rndGen)
        {
            return new Uri("my.schema://userName:password@my.domain/path1/path2?query1=123&query2=%22hello%22");
        }

        public static string CreateInstanceOfUriString(Random rndGen)
        {
            UriKind kind;
            return UriCreator.CreateUri(rndGen, out kind);
        }

        public static XmlQualifiedName CreateInstanceOfXmlQualifiedName(Random rndGen)
        {
            return new XmlQualifiedName();
            //if (rndGen.Next(20) == 0) return new XmlQualifiedName();
            //StringBuilder sb = new StringBuilder();
            //int localNameLength = rndGen.Next(1, 30);
            ////TODO: Expand to include int'l chars
            //const string LocalNameStartChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
            //string LocalNameChars = LocalNameStartChars + "0123456789.-";
            //string NamespaceChars = LocalNameChars + ":";
            //for (int i = 0; i < localNameLength; i++)
            //{
            //    if (i == 0)
            //    {
            //        sb.Append(LocalNameStartChars[rndGen.Next(LocalNameStartChars.Length)]);
            //    }
            //    else
            //    {
            //        sb.Append(LocalNameChars[rndGen.Next(LocalNameChars.Length)]);
            //    }
            //}
            //string localName = sb.ToString();
            //if (rndGen.Next(3) == 0) return new XmlQualifiedName(localName);
            //sb.Length = 0;
            //int namespaceUriLength = rndGen.Next(1, 40);
            //for (int i = 0; i < namespaceUriLength; i++)
            //{
            //    sb.Append(NamespaceChars[rndGen.Next(NamespaceChars.Length)]);
            //}
            //string namespaceUri = sb.ToString();
            //return new XmlQualifiedName(localName, namespaceUri);
        }

        public static Stream CreateInstanceOfStream(Random rndGen)
        {
            string data = (string)InstanceCreator.CreateInstanceOf(typeof(String), rndGen);
            Stream inputStream = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(data.ToCharArray());
            inputStream.Write(bytes, 0, bytes.Length);
            inputStream.Position = 0;
            return inputStream;
        }

#if !SILVERLIGHT
        private const string TemplateXmlDocument =
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
            "<Bookstore name=\"Contoso\">\r\n" +
            "\t<cts:Inventory xmlns:cts=\"http://www.contoso.org/Bookstore\" xmlns=\"http://www.contoso.org/Bookstore/BookInfo\">\r\n" +
            "\t\t<cts:Book isbn=\"0735621519\">\r\n" +
            "\t\t\t<Author>David Pallman</Author>\r\n" +
            "\t\t\t<Title>Programming \"Indigo\"</Title>\r\n" +
            "\t\t\t<Subtitle><![CDATA[The code name for the Unified Framework for building service-oriented applications on the Microsoft Windows Platform]]></Subtitle>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t\t<cts:Book isbn=\"0672328771\">\r\n" +
            "\t\t\t<Author>Craig McMurtry</Author>\r\n" +
            "\t\t\t<Author>Marc Mercuri</Author>\r\n" +
            "\t\t\t<Author>Nigel Watling</Author>\r\n" +
            "\t\t\t<Title>Microsoft Windows Communication Foundation: Hands-on</Title>\r\n" +
            "\t\t\t<Subtitle><![CDATA[Beta Edition]]></Subtitle>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t\t<cts:Book isbn=\"0735623066\">\r\n" +
            "\t\t\t<Author>Justin Smith</Author>\r\n" +
            "\t\t\t<Title>Inside Microsoft Windows Communication Foundation</Title>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t\t<cts:Book isbn=\"1590597028\">\r\n" +
            "\t\t\t<Author>Chris Peiris</Author>\r\n" +
            "\t\t\t<Author>Dennis Mulder</Author>\r\n" +
            "\t\t\t<Author>Amit Bahree</Author>\r\n" +
            "\t\t\t<Author>Aftab Chopra</Author>\r\n" +
            "\t\t\t<Author>Shawn Cicoria</Author>\r\n" +
            "\t\t\t<Author>Nishith Pathak</Author>\r\n" +
            "\t\t\t<Title>Pro WCF: Practical Microsoft SOA Implementation</Title>\r\n" +
            "\t\t\t<Publisher>Apress</Publisher>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t\t<cts:Book isbn=\"0321399838\">\r\n" +
            "\t\t\t<Author>Dharma Shukla</Author>\r\n" +
            "\t\t\t<Author>Bob Schmidt</Author>\r\n" +
            "\t\t\t<Title>Essential Windows Workflow Foundation</Title>\r\n" +
            "\t\t\t<Subtitle><![CDATA[(Microsoft .NET Development Series)]]></Subtitle>\r\n" +
            "\t\t\t<Reviews>\r\n" +
            "\t\t\t\t<Review date=\"November 9, 2006\">\r\n" +
            "\t\t\t\t\t<Reviewer>BERNARDO H. N. SILVA \"Bernardo Heynemann\"</Reviewer>\r\n" +
            "\t\t\t\t\t<Caption>Absolutely Must Read</Caption>\r\n" +
            "\t\t\t\t\t<Detail>\r\n" +
            "\t\t\t\t\t\t<![CDATA[By the end of the first chapter I found myself with a \"W00000000T?!?\" face! This book is insanely GOOD! \r\n" +
            "I'm learning a lot about the Foundation behind the Workflow. \r\n" +
            "The authors take WF and break it down by chapters. Each chapter dissects a piece of the Foundation. \r\n" +
            "An absolutely must-read for anyone working now with Windows Workflow Foundation (WF).]]>\r\n" +
            "\t\t\t\t\t</Detail>\r\n" +
            "\t\t\t\t</Review>\r\n" +
            "\t\t\t\t<Review date=\"November 2, 2006\">\r\n" +
            "\t\t\t\t\t<Reviewer>W Boudville (US)</Reviewer>\r\n" +
            "\t\t\t\t\t<Caption>takes scheduling of programs to another level</Caption>\r\n" +
            "\t\t\t\t\t<Detail>\r\n" +
            "\t\t\t\t\t\t<![CDATA[WWF encapsulates some intriguing abilities that were hitherto not available in C#/.NET, or in the competing Java environment. Or at least not easily available. In both areas, there has already been the concept of serialisation. Where you can write code from memory to disk in a manner such that the code's classes can be read back as functioning binaries, at some later time. Both also have transactions and threads. \r\n" +
            "WWF takes those ideas and merges them. The authors show how this results in the concept of a resumable program. The core idea in WWF. So a runtime program can be passivated (the equivalent of the earlier serialisation idea), and given a globally unique id. Then, a special Runtime program can de-passivate the program and run it, at some future time. In essence, it gets around the conundrum that when a conventional program, in any language, ends, then it ends. You needed to write custom code in another program, that could invoke the first, in some fashion. Very clumsy and error prone. WWF provides a declarative and robust way to transcend the ending of a program. Takes scheduling to the next level. \r\n" +
            "Plus, the book shows that the de-passivating of a resumable program can be done on another machine, that has access to the medium in which the program was passivated. (This was the point of using a globally unique id for the passivated program.) Obvious implications for load balancing and robustness design.]]></Detail>\r\n" +
            "\t\t\t\t</Review>\r\n" +
            "\t\t\t</Reviews>\r\n" +
            "\t\t</cts:Book>\r\n" +
            "\t</cts:Inventory>\r\n" +
            "</Bookstore>\r\n";

        public static XmlElement CreateInstanceOfXmlElement(Random rndGen)
        {
            XmlDocument doc = new XmlDocument();
            string docString = TemplateXmlDocument;
            if (CreatorSettings.NormalizeEndOfLineOnXmlNodes)
            {
                docString = docString.Replace("\r\n", "\n").Replace("\r", "\n");
            }
            doc.LoadXml(docString);
            return doc.DocumentElement;
        }

        public static XmlNode[] CreateInstanceOfXmlNodeArray(Random rndGen)
        {
            XmlDocument doc = new XmlDocument();
            string docString = TemplateXmlDocument;
            if (CreatorSettings.NormalizeEndOfLineOnXmlNodes)
            {
                docString = docString.Replace("\r\n", "\n").Replace("\r", "\n");
            }
            doc.LoadXml(docString);
            List<XmlNode> result = new List<XmlNode>();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("book", "http://www.contoso.org/Bookstore");
            XmlNodeList nodes = doc.SelectNodes("//book:Book", nsManager);
            //for (int i = 0; i < nodes.Count; i++)
            //{
            //    if (rndGen.Next(2) == 0) result.Add(nodes[i]);
            //}
            //if (rndGen.Next(5) == 0) // we'll repeat some nodes
            //{
            //    for (int i = 0; i < nodes.Count; i++)
            //    {
            //        if (rndGen.Next(2) == 0) result.Add(nodes[i]);
            //    }
            //}
            return result.ToArray();
        }

        //public static DataTable CreateInstanceOfDataTable(Random rndGen)
        //{
        //    string tableName = CreateInstanceOfString(rndGen);

        //    while (String.IsNullOrEmpty(tableName))
        //    {
        //        tableName = CreateInstanceOfString(rndGen);
        //    }
        //    string tableNamespace = null;
        //    if (rndGen.Next(4) == 0)
        //    {
        //        do
        //        {
        //            tableNamespace = CreateInstanceOfString(rndGen);
        //            if (null != tableNamespace)
        //            {
        //                tableNamespace = tableNamespace.Trim();
        //            }
        //            //Blank namespace resultsin: System.Xml.Schema.XmlSchemaException:"The Namespace ' ' is an invalid URI."}
        //        }
        //        while (String.IsNullOrEmpty(tableNamespace));
        //    }
        //    DataTable result = new DataTable(tableName, tableNamespace);
        //    Type[] types = new Type[] {
        //        typeof(string),
        //        typeof(DateTime),
        //        typeof(int),
        //        typeof(decimal),
        //        typeof(double),
        //        typeof(Uri),
        //    };
        //    int columnCount = rndGen.Next(6);
        //    Type[] columnTypes = new Type[columnCount];
        //    string columnNameChars = "abcdefghijklmnopqrstuvwxwz1234567890";
        //    for (int i = 0; i < columnCount; i++)
        //    {
        //        int columnNameLen = rndGen.Next(5, 10);
        //        string columnName = CreateInstanceOfString(rndGen, columnNameLen, columnNameChars);
        //        columnTypes[i] = types[rndGen.Next(types.Length)];
        //        result.Columns.Add(columnName, columnTypes[i]);
        //    }
        //    if (columnCount > 0)
        //    {
        //        int rowCount = rndGen.Next(CreatorSettings.MaxListLength);
        //        for (int i = 0; i < rowCount; i++)
        //        {
        //            object[] rowElements = new object[columnCount];
        //            for (int j = 0; j < columnCount; j++)
        //            {
        //                rowElements[j] = CreatePrimitiveInstance(columnTypes[j], rndGen);
        //            }
        //            result.Rows.Add(rowElements);
        //        }
        //    }
        //    return result;
        //}

        //public static DataSet CreateInstanceOfDataSet(Random rndGen)
        //{
        //    string datasetName = CreateInstanceOfString(rndGen);
        //    while (String.IsNullOrEmpty(datasetName))
        //    {
        //        datasetName = CreateInstanceOfString(rndGen);
        //    }
        //    DataSet set = new DataSet(datasetName);
        //    int tableCount = rndGen.Next(6);
        //    if (tableCount == 0) tableCount = 1;
        //    for (int i = 0; i < tableCount; i++)
        //    {
        //        set.Tables.Add(PrimitiveCreator.CreateInstanceOfDataTable(rndGen));
        //    }
        //    return set;
        //}

#endif

        public static bool CanCreateInstanceOf(Type type)
        {
            return s_creators.ContainsKey(type);
        }
        public static object CreatePrimitiveInstance(Type type, Random rndGen)
        {
            if (s_creators.ContainsKey(type))
            {
                return s_creators[type].Invoke(null, new object[] { rndGen });
            }
            else
            {
                throw new ArgumentException("Type " + type.FullName + " not supported");
            }
        }
    }
}