using System;
using System.Collections.Generic;
using System.Text;

namespace System.Text.Json
{
    /// <summary>
    /// This enum defines the various ways the <see cref="Utf8JsonReader"/> can deal with comments.
    /// </summary>
    public enum ReferenceHandlingOnDeserialize
    {
        /// <summary>
        /// Indicates to the deserialier to ignore metadata properties in the payload.
        /// </summary>
        IgnoreMetadata,
        /// <summary>
        /// Indicates to the deserializer to use metadata properties in the payload in order to handle duplicate references.
        /// </summary>
        PreserveDuplicates,
    }
}
