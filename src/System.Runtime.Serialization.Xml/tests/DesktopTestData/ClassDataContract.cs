using System;
using System.Collections.Generic;
using System.Reflection;

namespace DesktopTestData
{
    public class ClassDataContract : DataContract
    {
        ClassDataContract baseContract;
        List<DataMember> members;
        string[][] memberNames;
        bool isISerializable;
        bool hasDataContract;

        public ClassDataContract(Type type)
            : this(type, false)
        {
        }

        public ClassDataContract(Type type, bool supportCollectionDataContract)
            : base(type, supportCollectionDataContract)
        {
            Init(type);
        }


        void Init(Type type)
        {
            this.StableName = DataContract.GetStableName(type, out hasDataContract, supportCollectionDataContract);
            Type baseType = type.BaseType;
            isISerializable = (Globals.TypeOfISerializable.IsAssignableFrom(type));
            if (isISerializable)
            {
                if (hasDataContract)
                    throw new Exception("DataContractTypeCannotBeISerializable: " + type.FullName);
                if (!Globals.TypeOfISerializable.IsAssignableFrom(baseType))
                {
                    while (baseType != null)
                    {
                        if (baseType.IsDefined(Globals.TypeOfDataContractAttribute, false))
                            throw new Exception("ISerializableCannotInheritFromDataContract:" + type.FullName + "::" + baseType.FullName);
                        baseType = baseType.BaseType;
                    }
                }
            }
            if (baseType != null && baseType != Globals.TypeOfObject && baseType != Globals.TypeOfValueType)
                this.BaseContract = (ClassDataContract)DataContract.GetDataContract(baseType, supportCollectionDataContract);
            else
                this.BaseContract = null;
        }

        void ImportDataMembers()
        {
            Type type = this.UnderlyingType;
            members = new List<DataMember>();
            Dictionary<string, DataMember> memberNamesTable = new Dictionary<string, DataMember>();
            MemberInfo[] memberInfos = type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < memberInfos.Length; i++)
            {
                MemberInfo member = memberInfos[i];
                if (hasDataContract)
                {
                    object[] memberAttributes = member.GetCustomAttributes(typeof(System.Runtime.Serialization.DataMemberAttribute), false);
                    if (memberAttributes != null && memberAttributes.Length > 0)
                    {
                        if (memberAttributes.Length > 1)
                            throw new Exception("TooManyDataMembers:" + member.DeclaringType.FullName + "::" + member.Name);
                        if (member.MemberType == MemberTypes.Property)
                        {
                            PropertyInfo property = (PropertyInfo)member;

                            MethodInfo getMethod = property.GetGetMethod(true);
                            if (getMethod != null && IsMethodOverriding(getMethod))
                                continue;
                            MethodInfo setMethod = property.GetSetMethod(true);
                            if (setMethod != null && IsMethodOverriding(setMethod))
                                continue;
                            if (getMethod == null)
                                throw new Exception(" NoGetMethodForProperty : " + property.DeclaringType + " ::" + property.Name);
                            if (setMethod == null)
                                throw new Exception("NoSetMethodForProperty : " + property.DeclaringType + " :: " + property.Name);
                            if (getMethod.GetParameters().Length > 0)
                                throw new Exception("IndexedPropertyCannotBeSerialized :" + property.DeclaringType + " :: " + property.Name);
                        }
                        else if (member.MemberType != MemberTypes.Field)
                            throw new Exception("InvalidMember : " + type.FullName + " :: " + member.Name + " :: " + typeof(System.Runtime.Serialization.DataMemberAttribute).FullName);

                        DataMember memberContract = new DataMember(member);
                        System.Runtime.Serialization.DataMemberAttribute memberAttribute = (System.Runtime.Serialization.DataMemberAttribute)memberAttributes[0];
                        if (memberAttribute.Name == null)
                            memberContract.Name = member.Name;
                        else
                            memberContract.Name = memberAttribute.Name;
                        memberContract.Order = memberAttribute.Order;
                        memberContract.IsRequired = memberAttribute.IsRequired;
                        CheckAndAddMember(members, memberContract, memberNamesTable);
                    }
                }
                else
                {
                    FieldInfo field = member as FieldInfo;
                    if (field != null && !field.IsNotSerialized)
                    {
                        DataMember memberContract = new DataMember(member);
                        memberContract.Name = member.Name;
                        object[] optionalFields = field.GetCustomAttributes(Globals.TypeOfOptionalFieldAttribute, false);
                        if (optionalFields == null || optionalFields.Length == 0)
                            memberContract.Order = Globals.DefaultVersion;
                        else
                        {
                            memberContract.IsRequired = Globals.DefaultIsRequired;
                        }

                        CheckAndAddMember(members, memberContract, memberNamesTable);
                    }
                }
            }
            if (members.Count > 1)
                members.Sort(DataMemberComparer.Singleton);
        }

        public static void CheckAndAddMember(List<DataMember> members, DataMember memberContract, Dictionary<string, DataMember> memberNamesTable)
        {
            DataMember existingMemberContract;
            if (memberNamesTable.TryGetValue(memberContract.Name, out existingMemberContract))
                throw new Exception("DupMemberName :" + existingMemberContract.MemberInfo.Name + " :: " + memberContract.MemberInfo.Name + " :: " + memberContract.MemberInfo.DeclaringType.FullName + " :: " + memberContract.Name);
            memberNamesTable.Add(memberContract.Name, memberContract);
            members.Add(memberContract);
        }

        static bool IsMethodOverriding(MethodInfo method)
        {
            return method.IsVirtual && ((method.Attributes & MethodAttributes.NewSlot) == 0);
        }

        public ClassDataContract BaseContract
        {
            get { return baseContract; }
            set { baseContract = value; }
        }

        public string[][] MemberNames
        {
            get
            {
                if (memberNames == null)
                {
                    lock (this)
                    {
                        if (memberNames == null && Members != null)
                        {
                            if (baseContract == null)
                                memberNames = new string[1][];
                            else
                            {
                                int baseTypesCount = baseContract.MemberNames.Length;
                                memberNames = new string[baseTypesCount + 1][];
                                Array.Copy(baseContract.MemberNames, 0, memberNames, 0, baseTypesCount);
                            }
                            string[] declaredMemberNames = new string[Members.Count];
                            for (int i = 0; i < Members.Count; i++)
                                declaredMemberNames[i] = Members[i].Name;
                            memberNames[memberNames.Length - 1] = declaredMemberNames;
                        }
                    }
                }
                return memberNames;
            }
        }

        public List<DataMember> Members
        {
            get
            {
                if (members == null && UnderlyingType != null && !IsISerializable)
                {
                    lock (this)
                    {
                        if (members == null)
                        {
                            ImportDataMembers();
                        }
                    }
                }
                return members;
            }
            set { members = value; }
        }

        public override bool Equals(object other)
        {
            if ((object)this == other)
                return true;

            if (base.Equals(other))
            {
                ClassDataContract dataContract = other as ClassDataContract;
                if (dataContract != null)
                {
                    if (IsISerializable)
                    {
                        if (!dataContract.IsISerializable)
                            return false;
                    }
                    else
                    {
                        if (dataContract.IsISerializable)
                            return false;

                        if (Members == null)
                        {
                            if (dataContract.Members != null)
                                return false;
                        }
                        else if (dataContract.Members == null)
                            return false;
                        else
                        {
                            if (Members.Count != dataContract.Members.Count)
                                return false;

                            for (int i = 0; i < Members.Count; i++)
                            {
                                if (!Members[i].Equals(dataContract.Members[i]))
                                    return false;
                            }
                        }
                    }

                    if (BaseContract == null)
                        return (dataContract.BaseContract == null);
                    else if (dataContract.BaseContract == null)
                        return false;
                    else
                        return BaseContract.StableName.Equals(dataContract.BaseContract.StableName);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public class DataMemberComparer : IComparer<DataMember>
        {
            public int Compare(DataMember x, DataMember y)
            {
                if (x.Order < y.Order)
                    return -1;
                if (x.Order > y.Order)
                    return 1;
                return String.Compare(x.Name, y.Name, StringComparison.InvariantCulture);
            }

            public bool Equals(DataMember x, DataMember y)
            {
                return x == y;
            }

            public int GetHashCode(DataMember x)
            {
                return ((object)x).GetHashCode();
            }
            public static DataMemberComparer Singleton = new DataMemberComparer();
        }

        public class DataMemberOrderComparer : IComparer<DataMember>
        {
            public int Compare(DataMember x, DataMember y)
            {
                return x.Order - y.Order;
            }

            public bool Equals(DataMember x, DataMember y)
            {
                return x.Order == y.Order;
            }

            public int GetHashCode(DataMember x)
            {
                return ((object)x).GetHashCode();
            }
            public static DataMemberOrderComparer Singleton = new DataMemberOrderComparer();
        }

    }

}
