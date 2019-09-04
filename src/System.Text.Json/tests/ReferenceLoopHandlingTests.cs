// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Text.Json.Serialization;
using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class ReferenceLoopHandlingTests
    {
        private class Employee
        {
            [Serialization.JsonPropertyName("todo")]//todo: add a referenceloophandling attribute.
            public Employee Manager { get; set; }
            public string Name { get; set; }

        }

        [Fact]
        public static void SerializeReferenceLoop()
        {
            var joe = new Employee { Name = "Joe User" };
            var mike = new Employee { Name = "Mike Manager" };
            joe.Manager = mike;
            mike.Manager = mike;
            //mike.Manager.Manager.Manager.Manager = null;

            var options = new JsonSerializerOptions { 
                WriteIndented = true, 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
            };

            string json = JsonSerializer.Serialize(joe, options);

            Console.WriteLine(json);
        }

        [Fact]
        public static void WriteReferenceLoop()
        {
            var joe = new Employee { Name = "Joe User" };
            var mike = new Employee { Name = "Mike Manager" };
            joe.Manager = mike;
            mike.Manager = mike;
            //mike.Manager.Manager.Manager.Manager = null;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            Utf8JsonWriter writer = new Utf8JsonWriter();

            var json = JsonSerializer.Serialize(joe, options);

            Console.WriteLine(json);
        }
    }
}
