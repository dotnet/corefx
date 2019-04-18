
using System.Collections.Generic;
using System.Resources.Binary;

namespace System.Resources.Binary
{
    partial class RuntimeResourceSet
    {
        private readonly IResourceReader Reader;

        public RuntimeResourceSet(IResourceReader reader) : base(reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            BinaryResourceReader binaryResourceReader = reader as BinaryResourceReader;

            if (binaryResourceReader == null)
                throw new ArgumentException(SR.Format(SR.NotSupported_WrongResourceReader_Type, reader.GetType()), nameof(reader));

            _resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
            _defaultReader = binaryResourceReader;
            Reader = reader;

            // in the inbox version this type creates the reader and passes this in, in the custom case the reader is passed in,
            // so we must initialize the cache here.
            binaryResourceReader._resCache = _resCache;
        }
    }
}
