// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Xunit;
using System;

namespace System.Threading.Tasks.Tests.CancelWait
{
    public static class TaskCancelWaitTests
    {
        #region Test Methods

        private static TaskInfo Nest(TaskInfo root, params Func<TaskInfo, TaskInfo>[] children)
        {
            root.AddChildren(children.Select(f => f(root)).ToArray());
            return root;
        }

        public static IEnumerable<object[]> Task_Data()
        {
            foreach (WorkloadType load in Enum.GetValues(typeof(WorkloadType)))
            {
                foreach (TaskCreationOptions option in new[] { TaskCreationOptions.None, TaskCreationOptions.LongRunning })
                {
                    // 14, 5, 19, 16
                    yield return new object[] { new TaskInfo(null, "node", load, option) };

                    foreach (WorkloadType work in Enum.GetValues(typeof(WorkloadType)))
                    {
                        // 22, 4, 1, 9, 23
                        yield return new object[] {
                            Nest(new TaskInfo(null, "node", load, option),
                                node => new TaskInfo(node, "node_1", work, option))
                            };
                        yield return new object[] {
                            Nest(new TaskInfo(null, "node", load, option),
                                node => new TaskInfo(node, "node_1", work, option, true))
                            };
                        yield return new object[] {
                            Nest(new TaskInfo(null, "node", load, option, true, false),
                                node => new TaskInfo(node, "node_1", work, option))
                            };
                        yield return new object[] {
                            Nest(new TaskInfo(null, "node", load, option),
                                node => new TaskInfo(node, "node_1", work, option | TaskCreationOptions.AttachedToParent))
                            };
                        yield return new object[] {
                            Nest(new TaskInfo(null, "node", load, option),
                                node => new TaskInfo(node, "node_1", work, option| TaskCreationOptions.AttachedToParent, true))
                            };
                    }

                    // 18, 21, 8, 13, 2, 24, 12, 10
                    yield return new object[] {
                        Nest(new TaskInfo(null, "node", load, option),
                            node => new TaskInfo(node, "node_1", WorkloadType.Medium, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent),
                            node => new TaskInfo(node, "node_2", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true),
                            node => new TaskInfo(node, "node_3", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true),
                            node => new TaskInfo(node, "node_4", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true),
                            node => new TaskInfo(node, "node_5", WorkloadType.VeryLight, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent),
                            node => new TaskInfo(node, "node_6", WorkloadType.VeryLight, TaskCreationOptions.LongRunning, true),
                            node => new TaskInfo(node, "node_7", WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true))
                    };

                    // 20, 6, 15, 3, 17, 11, 25, 7
                    yield return new object[] {
                        Nest(new TaskInfo(null, "node", load, option),
                            node => Nest(new TaskInfo(node, "node_1", WorkloadType.VeryLight, TaskCreationOptions.None),
                                node_1 => new TaskInfo(node_1, "node_1_1", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent, true),
                                node_1 => new TaskInfo(node_1, "node_1_2", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true),
                                node_1 => new TaskInfo(node_1, "node_1_3", WorkloadType.Medium, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent)),
                            node => Nest(new TaskInfo(node, "node_2", WorkloadType.Heavy, TaskCreationOptions.LongRunning),
                                node_2 => new TaskInfo(node_2, "node_2_1", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning),
                                node_2 => new TaskInfo(node_2, "node_2_2", WorkloadType.Heavy, TaskCreationOptions.None),
                                node_2 => new TaskInfo(node_2, "node_2_3", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent)),
                            node => Nest(new TaskInfo(node, "node_3", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent),
                                node_3 => new TaskInfo(node_3, "node_3_1", WorkloadType.Heavy, TaskCreationOptions.None, true),
                                node_3 => new TaskInfo(node_3, "node_3_2", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent)),
                            node => new TaskInfo(node, "node_4", WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true))
                    };
                }
            }
        }

        /// <summary>
        /// Gets a row of data for task tests.
        /// </summary>
        /// The format is:
        ///  1. A list of task workloads
        ///  2. The maximum duration to wait (-1 means no timeout)
        /// <returns>A row of test data.</returns>
        public static IEnumerable<object[]> Task_Wait_Data()
        {
            foreach (int wait in new[] { Waits.Short.Milliseconds, Waits.Instant.Milliseconds })
            {
                foreach (object[] data in Task_Data())
                {
                    yield return new object[] { data[0], wait };
                }
            }
        }

        /// <summary>
        /// Gets a row of data for task tests.
        /// </summary>
        /// The format is:
        ///  1. A list of task workloads
        ///  2. The maximum duration to wait (-1 means no timeout)
        /// <returns>A row of test data.</returns>
        public static IEnumerable<object[]> Task_Wait_Data_Longrunning()
        {
            foreach (int wait in new[] { Waits.Infinite.Milliseconds, Waits.Long.Milliseconds })
            {
                foreach (object[] data in Task_Data())
                {
                    yield return new object[] { data[0], wait };
                }
            }
        }

        [Theory]
        [MemberData("Task_Data")]
        public static void Task_Cancel_Test(TaskInfo node)
        {
            TestParameters parameters = new TestParameters(node, API.Cancel, WaitBy.None, -1);

            TaskCancelWaitTest test = new TaskCancelWaitTest(parameters);
            test.RealRun();
        }

        [Theory]
        [MemberData("Task_Wait_Data")]
        public static void Task_Wait_Millisecond_Test(TaskInfo node, int duration)
        {
            TestParameters parameters = new TestParameters(node, API.Wait, WaitBy.Millisecond, duration);

            TaskCancelWaitTest test = new TaskCancelWaitTest(parameters);
            test.RealRun();
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Wait_Data_Longrunning")]
        public static void Task_Wait_Millisecond_Test_Longrunning(TaskInfo node, int duration)
        {
            Task_Wait_Millisecond_Test(node, duration);
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Data")]
        public static void Task_Wait_Test(TaskInfo node)
        {
            TestParameters parameters = new TestParameters(node, API.Wait, WaitBy.None, -1);

            TaskCancelWaitTest test = new TaskCancelWaitTest(parameters);
            test.RealRun();
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait38()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Medium, TaskCreationOptions.AttachedToParent);

            Task_Wait_TimeSpan_Test(node, -1);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait39()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Light, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.VeryLight, TaskCreationOptions.LongRunning);

            node.AddChildren(new[] { node_1, node_2, });

            Task_Wait_TimeSpan_Test(node, -1);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait40()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true);
            TaskInfo node_1_1 = new TaskInfo(node_1, "node_1_1", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true);
            node_1.AddChildren(new[] { node_1_1, });

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true);

            TaskInfo node_3 = new TaskInfo(node, "node_3", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent, true);
            TaskInfo node_3_1 = new TaskInfo(node_3, "node_3_1", WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true);
            TaskInfo node_3_2 = new TaskInfo(node_3, "node_3_2", WorkloadType.Light, TaskCreationOptions.AttachedToParent);
            node_3.AddChildren(new[] { node_3_1, node_3_2, });

            node.AddChildren(new[] { node_1, node_2, node_3, });

            Task_Wait_TimeSpan_Test(node, -1);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait41()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Medium, TaskCreationOptions.AttachedToParent);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Heavy, TaskCreationOptions.None);

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Heavy, TaskCreationOptions.LongRunning);

            TaskInfo node_3 = new TaskInfo(node, "node_3", WorkloadType.Medium, TaskCreationOptions.LongRunning, true);

            TaskInfo node_4 = new TaskInfo(node, "node_4", WorkloadType.VeryLight, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_5 = new TaskInfo(node, "node_5", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent);

            TaskInfo node_6 = new TaskInfo(node, "node_6", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_7 = new TaskInfo(node, "node_7", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true);

            node.AddChildren(new[] { node_1, node_2, node_3, node_4, node_5, node_6, node_7, });

            Task_Wait_TimeSpan_Test(node, -1);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait42()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Light, TaskCreationOptions.LongRunning, true);

            node.AddChildren(new[] { node_1, });

            Task_Wait_TimeSpan_Test(node, -1);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait43()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryLight, TaskCreationOptions.LongRunning, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);
            TaskInfo node_1_1 = new TaskInfo(node_1, "node_1_1", WorkloadType.Medium, TaskCreationOptions.LongRunning, true);
            TaskInfo node_1_2 = new TaskInfo(node_1, "node_1_2", WorkloadType.VeryHeavy, TaskCreationOptions.None);
            node_1.AddChildren(new[] { node_1_1, node_1_2, });

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.VeryLight, TaskCreationOptions.AttachedToParent, true);
            TaskInfo node_2_1 = new TaskInfo(node_2, "node_2_1", WorkloadType.Light, TaskCreationOptions.AttachedToParent);
            TaskInfo node_2_2 = new TaskInfo(node_2, "node_2_2", WorkloadType.Heavy, TaskCreationOptions.LongRunning, true);
            node_2.AddChildren(new[] { node_2_1, node_2_2, });

            node.AddChildren(new[] { node_1, node_2, });

            Task_Wait_TimeSpan_Test(node, -1);
        }

        private static void Task_Wait_TimeSpan_Test(TaskInfo node, int duration)
        {
            TestParameters parameters = new TestParameters(node, API.Wait, WaitBy.TimeSpan, duration);

            TaskCancelWaitTest test = new TaskCancelWaitTest(parameters);
            test.RealRun();
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait80()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Light, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait81()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Medium, TaskCreationOptions.LongRunning, true);
            TaskInfo node_1_1 = new TaskInfo(node_1, "node_1_1", WorkloadType.Heavy, TaskCreationOptions.LongRunning, true);
            node_1.AddChildren(new[] { node_1_1, });

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Heavy, TaskCreationOptions.LongRunning);

            TaskInfo node_3 = new TaskInfo(node, "node_3", WorkloadType.VeryLight, TaskCreationOptions.LongRunning, true);
            TaskInfo node_3_1 = new TaskInfo(node_3, "node_3_1", WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true);
            TaskInfo node_3_2 = new TaskInfo(node_3, "node_3_2", WorkloadType.VeryLight, TaskCreationOptions.LongRunning);
            node_3.AddChildren(new[] { node_3_1, node_3_2, });

            node.AddChildren(new[] { node_1, node_2, node_3, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait82()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Heavy, TaskCreationOptions.None);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Heavy, TaskCreationOptions.None);
            TaskInfo node_1_1 = new TaskInfo(node_1, "node_1_1", WorkloadType.Light, TaskCreationOptions.None);
            TaskInfo node_1_2 = new TaskInfo(node_1, "node_1_2", WorkloadType.Light, TaskCreationOptions.LongRunning);
            node_1.AddChildren(new[] { node_1_1, node_1_2, });

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.VeryLight, TaskCreationOptions.AttachedToParent, true);
            TaskInfo node_2_1 = new TaskInfo(node_2, "node_2_1", WorkloadType.VeryLight, TaskCreationOptions.AttachedToParent);
            TaskInfo node_2_2 = new TaskInfo(node_2, "node_2_2", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);
            node_2.AddChildren(new[] { node_2_1, node_2_2, });

            node.AddChildren(new[] { node_1, node_2, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait83()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Light, TaskCreationOptions.None);

            node.AddChildren(new[] { node_1, node_2, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait84()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning);

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait85()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Medium, TaskCreationOptions.LongRunning, true);

            node.AddChildren(new[] { node_1, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait86()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Heavy, TaskCreationOptions.LongRunning, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Light, TaskCreationOptions.LongRunning, true);

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_3 = new TaskInfo(node, "node_3", WorkloadType.Heavy, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

            TaskInfo node_4 = new TaskInfo(node, "node_4", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true);

            TaskInfo node_5 = new TaskInfo(node, "node_5", WorkloadType.VeryLight, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

            TaskInfo node_6 = new TaskInfo(node, "node_6", WorkloadType.Medium, TaskCreationOptions.AttachedToParent);

            TaskInfo node_7 = new TaskInfo(node, "node_7", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);

            node.AddChildren(new[] { node_1, node_2, node_3, node_4, node_5, node_6, node_7, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait87()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Medium, TaskCreationOptions.AttachedToParent);

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait88()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Heavy, TaskCreationOptions.LongRunning);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Light, TaskCreationOptions.LongRunning, true);

            node.AddChildren(new[] { node_1, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait89()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Light, TaskCreationOptions.LongRunning, true);

            node.AddChildren(new[] { node_1, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait90()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Light, TaskCreationOptions.LongRunning, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.VeryLight, TaskCreationOptions.AttachedToParent);

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Medium, TaskCreationOptions.LongRunning, true);

            TaskInfo node_3 = new TaskInfo(node, "node_3", WorkloadType.Heavy, TaskCreationOptions.None);

            TaskInfo node_4 = new TaskInfo(node, "node_4", WorkloadType.Light, TaskCreationOptions.LongRunning, true);

            TaskInfo node_5 = new TaskInfo(node, "node_5", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_6 = new TaskInfo(node, "node_6", WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_7 = new TaskInfo(node, "node_7", WorkloadType.Light, TaskCreationOptions.LongRunning, true);

            node.AddChildren(new[] { node_1, node_2, node_3, node_4, node_5, node_6, node_7, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait91()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Heavy, TaskCreationOptions.LongRunning);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.VeryLight, TaskCreationOptions.LongRunning, true);

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Medium, TaskCreationOptions.LongRunning, true);

            node.AddChildren(new[] { node_1, node_2, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait92()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Heavy, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);

            node.AddChildren(new[] { node_1, node_2, });

            Task_Wait_TimeSpan_Test(node, 197);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait131()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Heavy, TaskCreationOptions.LongRunning, true);
            TaskInfo node_1_1 = new TaskInfo(node_1, "node_1_1", WorkloadType.VeryLight, TaskCreationOptions.LongRunning, true);
            TaskInfo node_1_2 = new TaskInfo(node_1, "node_1_2", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true);
            node_1.AddChildren(new[] { node_1_1, node_1_2, });

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Medium, TaskCreationOptions.LongRunning, true);
            TaskInfo node_2_1 = new TaskInfo(node_2, "node_2_1", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true);
            TaskInfo node_2_2 = new TaskInfo(node_2, "node_2_2", WorkloadType.Heavy, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            node_2.AddChildren(new[] { node_2_1, node_2_2, });

            node.AddChildren(new[] { node_1, node_2, });

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait132()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Medium, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Medium, TaskCreationOptions.AttachedToParent);

            node.AddChildren(new[] { node_1, });

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait133()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Light, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_3 = new TaskInfo(node, "node_3", WorkloadType.VeryLight, TaskCreationOptions.LongRunning, true);

            TaskInfo node_4 = new TaskInfo(node, "node_4", WorkloadType.VeryLight, TaskCreationOptions.LongRunning);

            TaskInfo node_5 = new TaskInfo(node, "node_5", WorkloadType.Light, TaskCreationOptions.LongRunning);

            TaskInfo node_6 = new TaskInfo(node, "node_6", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_7 = new TaskInfo(node, "node_7", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);

            node.AddChildren(new[] { node_1, node_2, node_3, node_4, node_5, node_6, node_7, });

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait134()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent, true);

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait135()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Medium, TaskCreationOptions.LongRunning);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.VeryHeavy, TaskCreationOptions.None);
            TaskInfo node_1_1 = new TaskInfo(node_1, "node_1_1", WorkloadType.VeryLight, TaskCreationOptions.AttachedToParent, true);
            node_1.AddChildren(new[] { node_1_1, });

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Heavy, TaskCreationOptions.None);

            TaskInfo node_3 = new TaskInfo(node, "node_3", WorkloadType.Heavy, TaskCreationOptions.LongRunning);
            TaskInfo node_3_1 = new TaskInfo(node_3, "node_3_1", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true);
            TaskInfo node_3_2 = new TaskInfo(node_3, "node_3_2", WorkloadType.Light, TaskCreationOptions.LongRunning);
            node_3.AddChildren(new[] { node_3_1, node_3_2, });

            node.AddChildren(new[] { node_1, node_2, node_3, });

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait136()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);
            TaskInfo node_1_1 = new TaskInfo(node_1, "node_1_1", WorkloadType.Medium, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            node_1.AddChildren(new[] { node_1_1, });

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_3 = new TaskInfo(node, "node_3", WorkloadType.Light, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            TaskInfo node_3_1 = new TaskInfo(node_3, "node_3_1", WorkloadType.Light, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            TaskInfo node_3_2 = new TaskInfo(node_3, "node_3_2", WorkloadType.VeryLight, TaskCreationOptions.AttachedToParent, true);
            node_3.AddChildren(new[] { node_3_1, node_3_2, });

            node.AddChildren(new[] { node_1, node_2, node_3, });

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait137()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.Heavy, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Light, TaskCreationOptions.AttachedToParent, true);

            node.AddChildren(new[] { node_1, node_2, });

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait138()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Light, TaskCreationOptions.None);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent);
            TaskInfo node_1_1 = new TaskInfo(node_1, "node_1_1", WorkloadType.VeryLight, TaskCreationOptions.AttachedToParent);
            TaskInfo node_1_2 = new TaskInfo(node_1, "node_1_2", WorkloadType.Light, TaskCreationOptions.None);
            node_1.AddChildren(new[] { node_1_1, node_1_2, });

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Light, TaskCreationOptions.LongRunning, true);
            TaskInfo node_2_1 = new TaskInfo(node_2, "node_2_1", WorkloadType.Light, TaskCreationOptions.LongRunning, true);
            TaskInfo node_2_2 = new TaskInfo(node_2, "node_2_2", WorkloadType.Medium, TaskCreationOptions.AttachedToParent);
            node_2.AddChildren(new[] { node_2_1, node_2_2, });

            node.AddChildren(new[] { node_1, node_2, });

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait139()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.VeryLight, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            TaskInfo node_1_1 = new TaskInfo(node_1, "node_1_1", WorkloadType.VeryHeavy, TaskCreationOptions.None);
            node_1.AddChildren(new[] { node_1_1, });

            TaskInfo node_2 = new TaskInfo(node, "node_2", WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_3 = new TaskInfo(node, "node_3", WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning);
            TaskInfo node_3_1 = new TaskInfo(node_3, "node_3_1", WorkloadType.Heavy, TaskCreationOptions.LongRunning, true);
            TaskInfo node_3_2 = new TaskInfo(node_3, "node_3_2", WorkloadType.Light, TaskCreationOptions.LongRunning, true);
            node_3.AddChildren(new[] { node_3_1, node_3_2, });

            node.AddChildren(new[] { node_1, node_2, node_3, });

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait140()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true);

            TaskInfo node_1 = new TaskInfo(node, "node_1", WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true);

            node.AddChildren(new[] { node_1, });

            Task_Wait_TimeSpan_Test(node, 27);
        }

        [Fact]
        [OuterLoop]
        public static void TaskCancelWait141()
        {
            TaskInfo node = new TaskInfo(null, "node", WorkloadType.Heavy, TaskCreationOptions.LongRunning, true);

            Task_Wait_TimeSpan_Test(node, 27);
        }

        #endregion
    }
}
