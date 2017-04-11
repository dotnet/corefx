using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DesktopTestData
{
    public class InstanceCreator
    {
        static int max_array_elements = 6;
        static int max_level = 20;

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
            if (CreatorSettings.CreatorSurrogate != null)
            {
                if (CreatorSettings.CreatorSurrogate.CanCreateInstanceOf(type))
                {
                    return CreatorSettings.CreatorSurrogate.CreateInstanceOf(type, rndGen);
                }
            }

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

        static object CreateObjectOfType(Type t, ref int level, ref List<Type> requiredKnownTypes)
        {
            if (level++ > max_level)//Safety checkpoint to reduce risk of stack overflow
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
                Array resultObj = Array.CreateInstance(t.GetElementType(), max_array_elements);
                for (int i = 0; i < max_array_elements; i++)
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

        static object CreateNonDefaultCtorObj(Type resultType, ref int level, ref List<Type> requiredKnownTypes)
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
}
