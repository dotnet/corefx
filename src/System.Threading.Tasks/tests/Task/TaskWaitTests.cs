// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

using CW = System.Threading.Tasks.Tests.CancelWait;

namespace System.Threading.Tasks.Tests
{
    public static class TaskWaitTests
    {
        #region Test Methods

        private static CW.TaskInfo Nest(CW.TaskInfo root, params Func<CW.TaskInfo, CW.TaskInfo>[] children)
        {
            root.AddChildren(children.Select(f => f(root)).ToArray());
            return root;
        }

        public static IEnumerable<object[]> Task_Data()
        {
            foreach (CW.WorkloadType load in Enum.GetValues(typeof(CW.WorkloadType)))
            {
                foreach (TaskCreationOptions option in new[] { TaskCreationOptions.None, TaskCreationOptions.LongRunning })
                {
                    // 14, 5, 19, 16
                    yield return new object[] { new CW.TaskInfo(null, "node", load, option) };

                    foreach (CW.WorkloadType work in Enum.GetValues(typeof(CW.WorkloadType)))
                    {
                        // 22, 4, 1, 9, 23
                        yield return new object[] {
                            Nest(new CW.TaskInfo(null, "node", load, option),
                                node => new CW.TaskInfo(node, "node_1", work, option))
                            };
                        yield return new object[] {
                            Nest(new CW.TaskInfo(null, "node", load, option),
                                node => new CW.TaskInfo(node, "node_1", work, option, true))
                            };
                        yield return new object[] {
                            Nest(new CW.TaskInfo(null, "node", load, option, true, false),
                                node => new CW.TaskInfo(node, "node_1", work, option))
                            };
                        yield return new object[] {
                            Nest(new CW.TaskInfo(null, "node", load, option),
                                node => new CW.TaskInfo(node, "node_1", work, option | TaskCreationOptions.AttachedToParent))
                            };
                        yield return new object[] {
                            Nest(new CW.TaskInfo(null, "node", load, option),
                                node => new CW.TaskInfo(node, "node_1", work, option| TaskCreationOptions.AttachedToParent, true))
                            };
                    }

                    // 18, 21, 8, 13, 2, 24, 12, 10
                    yield return new object[] {
                        Nest(new CW.TaskInfo(null, "node", load, option),
                            node => new CW.TaskInfo(node, "node_1", CW.WorkloadType.Medium, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent),
                            node => new CW.TaskInfo(node, "node_2", CW.WorkloadType.Light, TaskCreationOptions.AttachedToParent, true),
                            node => new CW.TaskInfo(node, "node_3", CW.WorkloadType.VeryHeavy, TaskCreationOptions.AttachedToParent, true),
                            node => new CW.TaskInfo(node, "node_4", CW.WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true),
                            node => new CW.TaskInfo(node, "node_5", CW.WorkloadType.VeryLight, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent),
                            node => new CW.TaskInfo(node, "node_6", CW.WorkloadType.VeryLight, TaskCreationOptions.LongRunning, true),
                            node => new CW.TaskInfo(node, "node_7", CW.WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true))
                    };

                    // 20, 6, 15, 3, 17, 11, 25, 7
                    yield return new object[] {
                        Nest(new CW.TaskInfo(null, "node", load, option),
                            node => Nest(new CW.TaskInfo(node, "node_1", CW.WorkloadType.VeryLight, TaskCreationOptions.None),
                                node_1 => new CW.TaskInfo(node_1, "node_1_1", CW.WorkloadType.Heavy, TaskCreationOptions.AttachedToParent, true),
                                node_1 => new CW.TaskInfo(node_1, "node_1_2", CW.WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning, true),
                                node_1 => new CW.TaskInfo(node_1, "node_1_3", CW.WorkloadType.Medium, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent)),
                            node => Nest(new CW.TaskInfo(node, "node_2", CW.WorkloadType.Heavy, TaskCreationOptions.LongRunning),
                                node_2 => new CW.TaskInfo(node_2, "node_2_1", CW.WorkloadType.VeryHeavy, TaskCreationOptions.LongRunning),
                                node_2 => new CW.TaskInfo(node_2, "node_2_2", CW.WorkloadType.Heavy, TaskCreationOptions.None),
                                node_2 => new CW.TaskInfo(node_2, "node_2_3", CW.WorkloadType.Heavy, TaskCreationOptions.AttachedToParent)),
                            node => Nest(new CW.TaskInfo(node, "node_3", CW.WorkloadType.Heavy, TaskCreationOptions.AttachedToParent),
                                node_3 => new CW.TaskInfo(node_3, "node_3_1", CW.WorkloadType.Heavy, TaskCreationOptions.None, true),
                                node_3 => new CW.TaskInfo(node_3, "node_3_2", CW.WorkloadType.Heavy, TaskCreationOptions.AttachedToParent)),
                            node => new CW.TaskInfo(node, "node_4", CW.WorkloadType.Medium, TaskCreationOptions.AttachedToParent, true))
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
        [MemberData("Task_Wait_Data")]
        public static void Task_Wait_Millisecond_Test(CW.TaskInfo node, int duration)
        {
            CW.TestParameters parameters = new CW.TestParameters(node, CW.API.Wait, CW.WaitBy.Millisecond, duration);

            CW.TaskCancelWaitTest test = new CW.TaskCancelWaitTest(parameters);
            test.RealRun();
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Wait_Data_Longrunning")]
        public static void Task_Wait_Millisecond_Test_Longrunning(CW.TaskInfo node, int duration)
        {
            Task_Wait_Millisecond_Test(node, duration);
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Data")]
        public static void Task_Wait_Test(CW.TaskInfo node)
        {
            CW.TestParameters parameters = new CW.TestParameters(node, CW.API.Wait, CW.WaitBy.None, -1);

            CW.TaskCancelWaitTest test = new CW.TaskCancelWaitTest(parameters);
            test.RealRun();
        }

        [Theory]
        [MemberData("Task_Wait_Data")]
        public static void Task_Wait_TimeSpan_Test(CW.TaskInfo node, int duration)
        {
            CW.TestParameters parameters = new CW.TestParameters(node, CW.API.Wait, CW.WaitBy.TimeSpan, duration);

            CW.TaskCancelWaitTest test = new CW.TaskCancelWaitTest(parameters);
            test.RealRun();
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Wait_Data_Longrunning")]
        public static void Task_Wait_TimeSpan_Test_Longrunning(CW.TaskInfo node, int duration)
        {
            Task_Wait_TimeSpan_Test(node, duration);
        }

        #endregion
    }
}
