using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace DesktopTestData
{
    public class EnumDataContract : DataContract
    {
        PrimitiveDataContract baseContract;
        List<DataMember> members;
        List<long> values;
        bool isULong;
        bool isFlags;
        bool hasDataContract;

        public EnumDataContract()
        {
            IsValueType = true;
        }

        public EnumDataContract(Type type) : base(type)
        {
            StableName = DataContract.GetStableName(type, out hasDataContract);

            Type baseType = Enum.GetUnderlyingType(type);
            baseContract = PrimitiveDataContract.GetPrimitiveDataContract(baseType);
            isULong = (baseType == Globals.TypeOfULong);
            IsFlags = type.IsDefined(Globals.TypeOfFlagsAttribute, false);
        }

        public PrimitiveDataContract BaseContract
        {
            get
            {
                return baseContract;
            }
            set
            {
                baseContract = value;
                isULong = (baseContract.UnderlyingType == Globals.TypeOfULong);
            }
        }

        void ImportDataMembers()
        {
            Type type = this.UnderlyingType;
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            members = new List<DataMember>(fields.Length);
            Dictionary<string, DataMember> memberNamesTable = new Dictionary<string, DataMember>();
            values = new List<long>(fields.Length);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                bool dataMemberValid = false;
                object[] memberAttributes = field.GetCustomAttributes(Globals.TypeOfDataMemberAttribute, false);
                if (hasDataContract)
                {
                    if (memberAttributes != null && memberAttributes.Length > 0)
                    {
                        if (memberAttributes.Length > 1)
                            throw new Exception("TooManyDataMembers :" + field.DeclaringType.FullName + " :: " + field.Name);
                        DataMemberAttribute memberAttribute = (DataMemberAttribute)memberAttributes[0];
                        DataMember memberContract = new DataMember(field);
                        if (memberAttribute.Name == null || memberAttribute.Name.Length == 0)
                            memberContract.Name = field.Name;
                        else
                            memberContract.Name = memberAttribute.Name;
                        ClassDataContract.CheckAndAddMember(members, memberContract, memberNamesTable);
                        dataMemberValid = true;
                    }
                }
                else
                {
                    if (!field.IsNotSerialized)
                    {
                        DataMember memberContract = new DataMember(field);
                        memberContract.Name = field.Name;
                        ClassDataContract.CheckAndAddMember(members, memberContract, memberNamesTable);
                        dataMemberValid = true;
                    }
                }

                if (dataMemberValid)
                {
                    object enumValue = field.GetValue(null);
                    if (isULong)
                        values.Add((long)((IConvertible)enumValue).ToUInt64(null));
                    else
                        values.Add(((IConvertible)enumValue).ToInt64(null));
                }
            }
        }

        public List<DataMember> Members
        {
            get
            {
                if (members == null && UnderlyingType != null)
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

        public List<long> Values
        {
            get
            {
                if (values == null && UnderlyingType != null)
                {
                    lock (this)
                    {
                        if (values == null)
                        {
                            ImportDataMembers();
                        }
                    }
                }
                return values;
            }
            set { values = value; }
        }

        public bool IsFlags
        {
            get { return isFlags; }
            set { isFlags = value; }
        }

        public override bool Equals(object other)
        {
            if ((object)this == other)
                return true;

            if (base.Equals(other))
            {
                EnumDataContract dataContract = other as EnumDataContract;
                if (dataContract != null)
                {
                    if (IsFlags != dataContract.IsFlags)
                        return false;

                    if (Members.Count != dataContract.Members.Count || Values.Count != dataContract.Values.Count)
                        return false;

                    for (int i = 0; i < Members.Count; i++)
                    {
                        if (Values[i] != dataContract.Values[i] || Members[i].Name != dataContract.Members[i].Name)
                            return false;
                    }

                    return BaseContract.StableName.Equals(dataContract.BaseContract.StableName);
                }
            }
            return false;
        }
        public override int GetHashCode()
        {
            if (members != null)
            {
                int hash = 0;
                foreach (DataMember member in members)
                    hash += member.Name.GetHashCode();
                hash += base.StableName.GetHashCode();
                return hash;
            }
            else
            {
                return base.GetHashCode();
            }
        }

    }
}
