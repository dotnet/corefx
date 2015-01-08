namespace System
{
    public delegate void ConsoleCancelEventHandler(Object sender, ConsoleCancelEventArgs e);

    public sealed class ConsoleCancelEventArgs : EventArgs
    {
        private ConsoleSpecialKey _type;
        private bool _cancel;  // Whether to cancel the CancelKeyPress event

        internal ConsoleCancelEventArgs(ConsoleSpecialKey type)
        {
            _type = type;
            _cancel = false;
        }

        // Whether to cancel the break event.  By setting this to true, the
        // Control-C will not kill the process.
        public bool Cancel
        {
            get { return _cancel; }
            set
            {
                _cancel = value;
            }
        }

        public ConsoleSpecialKey SpecialKey
        {
            get { return _type; }
        }
    }
}
