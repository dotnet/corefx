// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// TaskPropertiesTest.cs
//
// Test class using UnitTestDriver that test the ID and CreationOptions property of a Task
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    public static class TaskPropertiesTests
    {
        /// <summary>
        /// Test to ensure that the task and task scheduler IDs are unique when tasks are started
        /// </summary>
        [Fact]
        public static void TaskIDTest()
        {
            // Read Parameters
            int taskCount = 100;
            ISet<int> taskIDs = new HashSet<int>();
            // Get a list of task ids
            foreach (int id in Enumerable.Range(0, taskCount).Select(ignored => Task.Factory.StartNew(() => { }).Id))
            {
                // Task ids must be unique
                Assert.True(taskIDs.Add(id));
            }
            Assert.Equal(taskCount, taskIDs.Count);
        }

        /// <summary>
        /// An enumeration of combined TaskCreationOption s
        /// </summary>
        /// The format of the array is a follows:
        ///  1. The combined TaskCreationOption
        /// <returns>An enumeration of combined TaskCreationOption s</returns>
        public static IEnumerable<object[]> TaskOptions_Data()
        {
            int largest = Enum.GetValues(typeof(TaskCreationOptions)).Cast<int>().Max();
            foreach (int option in Enumerable.Range(0, largest * 2 - 1))
            {
                // There is no option corresponding to bit 6, 0x20 (32),
                // because it's already used in TaskContinuationOptions
                if ((option & 0x20) == 0x20)
                {
                    continue;
                }

                // Because the enum is defined as a flag set, we can cast from int
                yield return new object[] { (TaskCreationOptions)option };
            }
        }

        //Use the Start method to start the task
        [Theory]
        [MemberData("TaskOptions_Data")]
        public static void TaskOptionsTestAsync(TaskCreationOptions options)
        {
            Task task = new Task(() => { }, options);
            task.Start();
            Assert.Equal(options, task.CreationOptions);
        }

        //Use the RunSynchronously method to start the task
        [Theory]
        [MemberData("TaskOptions_Data")]
        public static void TaskOptionsTestSync(TaskCreationOptions options)
        {
            Task task = new Task(() => { }, options);
            task.RunSynchronously();
            Assert.Equal(options, task.CreationOptions);
        }
    }
}
