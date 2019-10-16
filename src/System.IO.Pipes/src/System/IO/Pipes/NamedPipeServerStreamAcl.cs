namespace System.IO.Pipes
{
    public static class NamedPipeServerStreamAcl
    {
        public static NamedPipeServerStream Create(
            string pipeName,
            PipeDirection direction,
            int maxNumberOfServerInstances,
            PipeTransmissionMode transmissionMode,
            PipeOptions options,
            int inBufferSize,
            int outBufferSize,
            PipeSecurity pipeSecurity,
            HandleInheritability inheritability = HandleInheritability.None,
            PipeAccessRights additionalAccessRights = default)
        {
            return null;
        }
    }
}
