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

    [DataContract(Name = "EmptyDCType", Namespace = "http://www.Default.com")]
    public class EmptyDCType
    {
    }

    [KnownType(typeof(EmptyDCType))]
    public class POCOObjectContainer
    {
        public POCOObjectContainer() { Data = new EmptyDCType(); }

        public object Data;

        [IgnoreDataMember]
        public object NonSerializedData;

        public POCOObjectContainer(object input)
        {
            Data = input;
        }
    }
}
