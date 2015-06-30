/// <summary>
/// Extensions that add the legacy APM Pattern (Begin/End) for generic Streams
/// </summary>
namespace System.IO
{
    using System.Threading.Tasks;
        
    public static class StreamAPMExtensions
    {
        public static IAsyncResult BeginRead(this Stream s, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return s.ReadAsync(buffer, offset, count).ToApm<int>(callback, state);
        }

        public static IAsyncResult BeginWrite(this Stream s, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return s.WriteAsync(buffer, offset, count).ToApm(callback, state);
        }

        public static int EndRead(this Stream s, IAsyncResult asyncResult)
        {
            var t = (Task<int>)asyncResult;
            return t.GetAwaiter().GetResult();
        }

        public static void EndWrite(this Stream s, IAsyncResult asyncResult)
        {
            Task t = (Task)asyncResult;
            t.GetAwaiter().GetResult();
        }
    }
}
