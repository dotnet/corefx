using System;
using System.Reflection;

namespace DesktopTestData
{
    public class DataMember
    {
        DataContract memberTypeContract;
        string name;
        int order;
        bool isRequired;
        bool isNullable;
        MemberInfo memberInfo;
        protected internal bool supportCollectionDataContract;

        public DataMember()
        {
        }

        public DataMember(MemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        public MemberInfo MemberInfo
        {
            get { return memberInfo; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        internal bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }

        public bool IsRequired
        {
            get { return isRequired; }
            set { isRequired = value; }
        }

        public object GetMemberValue(object obj)
        {
            FieldInfo field = MemberInfo as FieldInfo;

            if (field != null)
                return ((FieldInfo)MemberInfo).GetValue(obj);

            return ((PropertyInfo)MemberInfo).GetValue(obj, null);
        }

        public Type MemberType
        {
            get
            {
                FieldInfo field = MemberInfo as FieldInfo;
                if (field != null)
                    return field.FieldType;
                return ((PropertyInfo)MemberInfo).PropertyType;
            }
        }

        public DataContract MemberTypeContract
        {
            get
            {
                if (memberTypeContract == null)
                {
                    if (MemberInfo != null)
                    {
                        lock (this)
                        {
                            if (memberTypeContract == null)
                            {
                                memberTypeContract = DataContract.GetDataContract(MemberType, supportCollectionDataContract);
                            }
                        }
                    }
                }
                return memberTypeContract;
            }
            set
            {
                memberTypeContract = value;
            }
        }

        public override bool Equals(object other)
        {
            if ((object)this == other)
                return true;

            DataMember dataMember = other as DataMember;
            if (dataMember != null)
            {
                return (Name == dataMember.Name
                        //&& Order == dataMember.Order //order value is not part of the data contract, order is
                        && IsNullable == dataMember.IsNullable
                        && IsRequired == dataMember.IsRequired
                        && MemberTypeContract.StableName.Equals(dataMember.MemberTypeContract.StableName));
            }
            return false;
        }

        public static bool operator <(DataMember dm1, DataMember dm2)
        {
            if (dm1.Order != dm2.Order)
                return dm1.Order < dm2.Order;
            else
                return ((string.Compare(dm1.Name, dm2.Name) < 0) ? true : false);
        }

        public static bool operator >(DataMember dm1, DataMember dm2)
        {
            if (dm1.Order != dm2.Order)
                return dm1.Order > dm2.Order;
            else
                return ((string.Compare(dm1.Name, dm2.Name) > 0) ? true : false);
        }

        public bool NameOrderEquals(object other)
        {
            if ((object)this == other)
                return true;
            DataMember dataMember = other as DataMember;
            if (dataMember != null)
            {
                return (Name == dataMember.Name
                        && Order == dataMember.Order
                        && MemberTypeContract.StableName.Equals(dataMember.MemberTypeContract.StableName));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
