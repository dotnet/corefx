using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    sealed class CreateSet<T> : Consumer<T, Set<T>>
    {
        public CreateSet(IEqualityComparer<T> comparer) : base(new Set<T>(comparer)) { }

        public override ChainStatus ProcessNext(T input)
        {
            Result.Add(input);
            return ChainStatus.Flow;
        }
    }

    sealed class CreateSetDefaultComparer<T> : Consumer<T, SetDefaultComparer<T>>
    {
        public CreateSetDefaultComparer() : base(new SetDefaultComparer<T>()) { }

        public override ChainStatus ProcessNext(T input)
        {
            Result.Add(input);
            return ChainStatus.Flow;
        }
    }

}
