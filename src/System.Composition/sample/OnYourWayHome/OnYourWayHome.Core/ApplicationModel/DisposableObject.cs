using System;

namespace OnYourWayHome.ApplicationModel
{
    // Idealy this would be a mixin or a private base class 
    // if C# or the CLI supported such a concept.
    public abstract class DisposableObject : IDisposable
    {
        private bool _isDisposed;

        protected DisposableObject()
        {
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            finally
            {
                _isDisposed = true;
            }
        }

        protected abstract void Dispose(bool disposing);
    }
}
