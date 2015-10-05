namespace System.Net.NetworkInformation
{
    public class NetworkAvailabilityEventArgs : EventArgs
    {
        private bool _isAvailable;

        internal NetworkAvailabilityEventArgs(bool isAvailable)
        {
            _isAvailable = isAvailable;
        }

        public bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }
        }
    }
}