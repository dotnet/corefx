using System.Collections.Generic;

namespace System.Linq.ChainLinq.Optimizations
{
    interface IPipelineArray<T>
    {
        void Pipeline(T[] array);
    }

    interface IPipelineEnumerable<T>
    {
        void Pipeline(IEnumerable<T> array);
    }

    interface IPipelineList<T>
    {
        void Pipeline(List<T> list);
    }

    interface IPipelineIList<T>
    {
        void Pipeline(IList<T> list, int start, int count);
    }
}
