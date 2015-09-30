namespace System.Reflection.Metadata.Decoding
{
    [Flags]
    public enum SignatureDecoderOptions
    {
        /// <summary>
        /// Disable all options (default when no options are passed).
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Causes the decoder to pass non-null for <see cref="Nullable{Bool}"/> isValueType arguments.
        /// </summary>
        /// <remarks>
        /// There is additional overhead for this case when dealing with .winmd files to handle projection.
        /// </remarks>
        DifferentiateValueTypes = 0x1
    }
}