using System;
using System.Runtime.Serialization;

namespace DesktopTestData
{
    [Serializable]
    [KnownType(typeof(PublicDC))]
    public class SerExplicitInterfaceIObjRef : IObjectReference
    {
        [NonSerialized]
        static PublicDC containedData = new PublicDC();

        #region IObjectReference Members

        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return containedData;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return DCRUtils.CompareIObjectRefTypes(containedData, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    [KnownType(typeof(PublicDC))]
    public class SerIObjRef : IObjectReference
    {
        [NonSerialized]
        static PublicDC containedData = new PublicDC();

        #region IObjectReference Members

        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return containedData;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return obj.Equals(containedData);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract(IsReference = false)]
    [KnownType(typeof(PublicDC))]
    public class DCExplicitInterfaceIObjRef : IObjectReference
    {
        [DataMember]
        public SelfRef1 data;

        [NonSerialized]
        static public SelfRef1 containedData = new SelfRef1();

        public DCExplicitInterfaceIObjRef() { }
        public DCExplicitInterfaceIObjRef(bool init)
        {
            data = new SelfRef1(true);
        }

        #region IObjectReference Members

        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return containedData;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return DCRUtils.CompareIObjectRefTypes(containedData, obj);
        }

        public override int GetHashCode()
        {
            return containedData.GetHashCode();
        }
    }

    [DataContract(IsReference = false)]
    [KnownType(typeof(PublicDC))]
    public class DCIObjRef : IObjectReference
    {
        [DataMember]
        public SimpleDCWithRef data;

        [NonSerialized]
        static SimpleDCWithRef containedData = new SimpleDCWithRef(true);

        public DCIObjRef() { }
        public DCIObjRef(bool init) { }

        #region IObjectReference Members

        public object GetRealObject(StreamingContext context)
        {
            return containedData;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return DCRUtils.CompareIObjectRefTypes(containedData, obj);
        }

        public override int GetHashCode()
        {
            return containedData.GetHashCode();
        }
    }

    [Serializable]
    [KnownType(typeof(PrivateDC))]
    public class SerExplicitInterfaceIObjRefReturnsPrivate : IObjectReference
    {
        [NonSerialized]
        static PrivateDC containedData = new PrivateDC();

        #region IObjectReference Members

        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return containedData;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return DCRUtils.CompareIObjectRefTypes(containedData, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    [KnownType(typeof(PrivateDC))]
    public class SerIObjRefReturnsPrivate : IObjectReference
    {
        [NonSerialized]
        static PrivateDC containedData = new PrivateDC();

        #region IObjectReference Members

        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return containedData;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return DCRUtils.CompareIObjectRefTypes(containedData, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract(IsReference = false)]
    [KnownType(typeof(PrivateDC))]
    public class DCExplicitInterfaceIObjRefReturnsPrivate : IObjectReference
    {
        [DataMember]
        PrivateDC data = new PrivateDC();

        [NonSerialized]
        static PrivateDC containedData = new PrivateDC();

        #region IObjectReference Members

        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return containedData;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return DCRUtils.CompareIObjectRefTypes(containedData, obj);
        }

        public override int GetHashCode()
        {
            return containedData.GetHashCode();
        }
    }

    [DataContract(IsReference = false)]
    [KnownType(typeof(PrivateDC))]
    public class DCIObjRefReturnsPrivate : IObjectReference
    {
        [DataMember]
        PrivateDC data = new PrivateDC();

        [NonSerialized]
        static PrivateDC containedData = new PrivateDC();

        #region IObjectReference Members

        public object GetRealObject(StreamingContext context)
        {
            return containedData;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return DCRUtils.CompareIObjectRefTypes(containedData, obj);
        }

        public override int GetHashCode()
        {
            return containedData.GetHashCode();
        }
    }
}
