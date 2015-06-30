namespace NCLTest.Common 
{
    using CoreFXTestLibrary;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Threading.Tasks;
    
    public static class AsyncUtilitiesExtensions
    {
        public static string ReadToEndAndClose(this Stream stream)
        {
            using (stream)
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static void WriteAndClose(this Stream stream, string data)
        {
            using (stream)
            {
                Write(stream, data);
            }
        }

        public static void Write(this Stream stream, string data)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(data);
            }
        }

        public static void AssertEquals<T>(this T[] array1, T[] array2)
        {
            Assert.AreEqual(array1.Length, array2.Length);
            for (int i = 0; i < array1.Length; i++)
            {
                Assert.AreEqual(array1[i], array2[i]);
            }
        }

        public static Task Finally(this Task task, Action finallyFunc)
        {
            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(t =>
            {
                finallyFunc();
                if (t.IsFaulted) 
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled) 
                {
                    tcs.TrySetCanceled();
                }
                else 
                {
                    tcs.TrySetResult(null);
                }
            });
            return tcs.Task;
        }
    }
}

