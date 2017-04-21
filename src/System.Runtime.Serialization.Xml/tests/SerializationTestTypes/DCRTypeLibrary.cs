using System.Runtime.Serialization;

namespace SerializationTestTypes
{
    [DataContract]
    public class ObjectContainer
    {
        [DataMember]
        private object _data;

        [DataMember]
        private object _data2;

        public ObjectContainer(object input)
        {
            _data = input;
            _data2 = _data;
        }

        public object Data
        {
            get { return _data; }
        }

        public object Data2
        {
            get { return _data2; }
        }
    }

    [DataContract]
    public class DCRVariations
    {
        [DataMember]
        public object unknownType1;

        [DataMember]
        public object unknownType2;
    }

    [DataContract]
    public class CustomClass
    {
        [DataMember()]
        private object[] knownTypes;

        [DataMember()]
        private object[] dataContractResolverTypes;

        [DataMember()]
        public virtual object[] KnownTypes
        {
            get { return knownTypes; }
            set { knownTypes = value; }
        }

        [DataMember()]
        public virtual object[] DataContractResolverTypes
        {
            get { return dataContractResolverTypes; }
            set { dataContractResolverTypes = value; }
        }
    }
}
