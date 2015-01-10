using System;

namespace OnYourWayHome.ApplicationModel.Composition
{
    public interface IResolver
    {
        object Resolve(Type type);
    }
}
