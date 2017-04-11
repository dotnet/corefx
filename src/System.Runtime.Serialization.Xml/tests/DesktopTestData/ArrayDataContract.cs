using System;

namespace DesktopTestData
{
    public class ArrayDataContract : DataContract
    {
        DataContract itemContract;
        int rank;

        public ArrayDataContract(Type type, bool supportCollectionDataContract)
            : base(type, supportCollectionDataContract)
        {
            rank = type.GetArrayRank();
            StableName = DataContract.GetStableName(type, supportCollectionDataContract);
        }

        public ArrayDataContract(Type type)
            : this(type, false)
        {
        }

        public DataContract ItemContract
        {
            get
            {
                if (itemContract == null && UnderlyingType != null)
                {
                    itemContract = DataContract.GetDataContract(UnderlyingType.GetElementType(), supportCollectionDataContract);
                }
                return itemContract;
            }
            set
            {
                itemContract = value;
            }
        }

        public override bool Equals(object other)
        {
            if ((object)this == other)
                return true;

            if (base.Equals(other))
            {
                ArrayDataContract dataContract = other as ArrayDataContract;
                if (dataContract != null)
                {
                    return ItemContract.Equals(dataContract.itemContract);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
