using System;

namespace DesktopTestData
{
    public class RefData
    {
        public object Data;
        public int RefCount = 0;

        public RefData(object data)
        {
            Data = data;
        }

        public override bool Equals(object obj)
        {
            RefData other = obj as RefData;
            if (other == null) return false;
            return (Object.ReferenceEquals(this.Data, other.Data));
        }

        public override int GetHashCode()
        {
            if (Data != null)
            {
                return Data.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RoundTripResultAttribute : ExpectedResultAttribute
    {
        public ResultSuccessType Result;
        public string ExceptionMessage;

        //TODO - for now setting it to true, latter should be changed to false
        public bool ValidateOnlyExceptionName = true;
        public string InstanceCreatorMethodName = "CreateInstance";

        public override bool Equals(object obj)
        {
            RoundTripResultAttribute other = obj as RoundTripResultAttribute;
            if (obj == null) { return false; }
            return (this.Result == other.Result && this.ExceptionMessage == other.ExceptionMessage
                    && this.ValidateOnlyExceptionName == other.ValidateOnlyExceptionName
                    && this.InstanceCreatorMethodName == other.InstanceCreatorMethodName
                    && base.Equals(obj));
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash += Result.GetHashCode() + ValidateOnlyExceptionName.GetHashCode();
            if (!String.IsNullOrEmpty(ExceptionMessage)) { hash += ExceptionMessage.GetHashCode(); }
            if (!String.IsNullOrEmpty(InstanceCreatorMethodName)) { hash += InstanceCreatorMethodName.GetHashCode(); }
            return hash;
        }
    }
}
