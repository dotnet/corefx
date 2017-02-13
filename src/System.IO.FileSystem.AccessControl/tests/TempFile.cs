using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace System.IO
{
    public class TempFile : IDisposable
    {
        private bool disposed;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        private FileStream fileStream;
        private string file;

        public TempFile(string file)
        {
            this.file = file;
            fileStream = File.Create(file);
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

                fileStream.Close();
                fileStream.Dispose();
                File.Delete(file);
            }

            disposed = true;
        }
    }
}