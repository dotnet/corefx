using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace DesktopTestData
{
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
}
