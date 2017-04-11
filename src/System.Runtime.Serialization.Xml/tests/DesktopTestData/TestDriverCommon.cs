using System;

namespace DesktopTestData
{
    [Flags]
    public enum ResultSuccessType
    {
        None = 0,
        XmlCompare = 2,
        ObjectCompare = 4,
        ReferenceCountCompare = 8,
        Serialization = 16,
        DeSerialization = 32
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Interface | AttributeTargets.Struct)]
    public abstract class ExpectedResultAttribute : Attribute
    {
        public bool IgnoreFromTest;
        public bool IsNegative;
        public string AssociatedProductBugs;

        public override bool Equals(object obj)
        {
            ExpectedResultAttribute other = obj as ExpectedResultAttribute;
            return (this.IgnoreFromTest == other.IgnoreFromTest && this.IsNegative == other.IsNegative && this.AssociatedProductBugs == other.AssociatedProductBugs);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash += this.IgnoreFromTest.GetHashCode() + this.IsNegative.GetHashCode();
            if (!String.IsNullOrEmpty(this.AssociatedProductBugs)) { hash += this.AssociatedProductBugs.GetHashCode(); }
            return hash;
        }
    }

    public class TrustedSerializationHelper
    {
        public static Type TypeOfBytePtr = typeof(byte*);
    }
}
