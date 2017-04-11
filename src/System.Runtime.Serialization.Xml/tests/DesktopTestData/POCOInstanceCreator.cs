using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTestData
{
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
            catch
            {
                throw;
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

    /// <summary>
    /// Apply this attribute to skip a field while creating instance
    /// </summary>
    public class IgnoreMemberAttribute : Attribute
    {
    }
}
