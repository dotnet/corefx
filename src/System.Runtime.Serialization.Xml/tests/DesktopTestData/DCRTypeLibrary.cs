using System.Runtime.Serialization;

namespace DesktopTestData
{
    [DataContract]
    public class ObjectContainer
    {
        [DataMember]
        private object data;

        [DataMember]
        private object data2;

        public ObjectContainer(object input)
        {
            data = input;
            data2 = data;
        }

        public object Data
        {
            get { return data; }
        }

        public object Data2
        {
            get { return data2; }
        }
    }
}
