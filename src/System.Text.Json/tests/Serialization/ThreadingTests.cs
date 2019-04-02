// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static class ThreadingTests
    {
        [Fact]
        public static void SerializeAndDeserialize()
        {
            // Ensure no exceptions are thrown due to caching or other issues.
            void SerializeAndDeserializeObject()
            {
                SimpleTestClass testObj = new SimpleTestClass();
                testObj.Initialize();
                testObj.Verify();

                string json = JsonSerializer.ToString(testObj);
                SimpleTestClass testObjDeserialized = JsonSerializer.Parse<SimpleTestClass>(json);
                testObjDeserialized.Verify();
            };

            Task[] tasks = new Task[8];
            for (int i = 0; i <tasks.Length; i++)
            {
                tasks[i] = Task.Run(() => SerializeAndDeserializeObject());
            };

            Task.WaitAll(tasks);
        }
    }
}
