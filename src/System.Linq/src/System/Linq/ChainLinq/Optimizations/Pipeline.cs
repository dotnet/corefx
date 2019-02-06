using System.Collections.Generic;

namespace System.Linq.ChainLinq.Optimizations
{
    interface IPipeline<T>
    {
        void Pipeline(T source);
    }
}
