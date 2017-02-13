using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
    public class TempDirectory : IDisposable
    {
        private bool disposed;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        private string directory;

        public TempDirectory(string directory)
        {
            this.directory = directory;
            Directory.CreateDirectory(directory);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                handle.Dispose();
                Directory.Delete(directory);
            }

            disposed = true;
        }
    }
}