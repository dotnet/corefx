// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Runtime.Serialization.Formatters.Tests
{
    public class SerializationGuardTests
    {
        [Fact]
        void BlockAssemblyLoads()
        {
            TryPayload(new AssemblyLoader());
        }

        [Fact]
        void BlockProcessStarts()
        {
            TryPayload(new ProcessStarter());
        }

        [Fact]
        void BlockFileWrites()
        {
            TryPayload(new FileWriter());
        }

        [Fact]
        void BlockReflectionDodging()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter writer = new BinaryFormatter();
            writer.Serialize(ms, new ReflectionDodger());
            ms.Position = 0;

            BinaryFormatter reader = new BinaryFormatter();
            Assert.Throws<TargetInvocationException>(() => reader.Deserialize(ms));
        }

        [Fact]
        void BlockAsyncDodging()
        {
            TryPayload(new AsyncDodger());
        }

        static void TryPayload(object payload)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter writer = new BinaryFormatter();
            writer.Serialize(ms, payload);
            ms.Position = 0;

            BinaryFormatter reader = new BinaryFormatter();
            Assert.Throws<DeserializationBlockedException>(() => reader.Deserialize(ms)); // This should really look for DeserializationBlockedException, but it's not in all of the frameworks this test builds for
        }
    }

    [Serializable]
    class AssemblyLoader : ISerializable
    {
        public AssemblyLoader() { }

        public AssemblyLoader(SerializationInfo info, StreamingContext context)
        {
            Assembly.Load(new byte[1000]);
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }

    [Serializable]
    class ProcessStarter : ISerializable
    {
        public ProcessStarter() { }

        private ProcessStarter(SerializationInfo info, StreamingContext context)
        {
            Process.Start("calc.exe");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }

    [Serializable]
    class FileWriter : ISerializable
    {
        public FileWriter() { }

        private FileWriter(SerializationInfo info, StreamingContext context)
        {
            string tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, "foo");
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }

    [Serializable]
    class ReflectionDodger : ISerializable
    {
        public ReflectionDodger() { }

        private ReflectionDodger(SerializationInfo info, StreamingContext context)
        {
            object tracker = null;
            Type threadType = typeof(object).Assembly.GetType("System.Threading.Thread");
            if (threadType != null)
            {
                MethodInfo trackerMethod = threadType.GetMethod("GetThreadDeserializationTracker", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (trackerMethod != null)
                {
                    object[] args = new object[trackerMethod.GetParameters().Length]; // The number of arguments differs by runtime
                    try
                    {
                        tracker = trackerMethod.Invoke(null, args);
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw ex.GetBaseException();
                    }
                }
            }
            Assert.Null(tracker);
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }

    [Serializable]
    class AsyncDodger : ISerializable
    {
        public AsyncDodger() { }

        private AsyncDodger(SerializationInfo info, StreamingContext context)
        {
            try
            {
                Task t = Task.Factory.StartNew(LoadAssemblyOnBackgroundThread, TaskCreationOptions.LongRunning);
                t.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        private void LoadAssemblyOnBackgroundThread()
        {
            Assembly.Load(new byte[1000]);
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
