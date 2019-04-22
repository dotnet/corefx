
using System.Collections.Generic;
using System.Resources.Binary;

namespace System.Resources.Binary
{
    partial class RuntimeResourceSet
    {
        private readonly IResourceReader Reader;

        internal RuntimeResourceSet(IResourceReader reader) :
            // explicitly do not call IResourceReader constructor since it caches all resources
            // the purpose of RuntimeResourceSet is to lazily load and cache.
            base()  
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            _defaultReader = reader as BinaryResourceReader ?? throw new ArgumentException(SR.Format(SR.NotSupported_WrongResourceReader_Type, reader.GetType()), nameof(reader));
            _resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
            Reader = reader;

            // in the CoreLib version RuntimeResourceSet creates ResourceReader and passes this in, 
            // in the custom case ManifestBasedResourceReader creates the ResourceReader and passes it in
            // so we must initialize the cache here.
            _defaultReader._resCache = _resCache;
        }
    }
}
