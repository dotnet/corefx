// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Text.Json.Linq.Tests
{
    /// <summary>
    /// Helper class simulating external library
    /// </summary>
    internal static class EmployeesDatabase
    {
        private static int s_id = 0;
        public static KeyValuePair<string, JTreeNode> GetNextEmployee()
        {
            var employee = new JTreeObject()
            {
                { "name", "John" } ,
                { "surname", "Smith"},
                { "age", 45 }
            };

            return new KeyValuePair<string, JTreeNode>("employee" + s_id++, employee);
        }

        public static IEnumerable<KeyValuePair<string, JTreeNode>> GetTenBestEmployees()
        {
            for (int i = 0; i < 10; i++)
                yield return GetNextEmployee();
        }

        /// <summary>
        /// Returns following JTreeObject:
        /// {
        ///     { "name" : "John" }
        ///     { "phone numbers" : { "work" :  "425-555-0123", "home": "425-555-0134"  } }
        ///     { 
        ///         "reporting employees" : 
        ///         {
        ///             "software developers" :
        ///             {
        ///                 "full time employees" : /JTreeObject of 3 employees from database/ 
        ///                 "intern employees" : /JTreeObject of 2 employees from database/ 
        ///             },
        ///             "HR" : /JTreeObject of 10 employees fromk database/ 
        ///         }
        /// </summary>
        /// <returns></returns>
        public static JTreeObject GetManager()
        {
            var manager = GetNextEmployee().Value as JTreeObject;

            manager.Add
            (
                "phone numbers",
                new JTreeObject()
                {
                    { "work", "425-555-0123" }, { "home", "425-555-0134" }
                }
            );

            manager.Add
            (
                "reporting employees", new JTreeObject()
                {
                    {
                        "software developers", new JTreeObject()
                        {
                            {
                                "full time employees", new JTreeObject()
                                {
                                    EmployeesDatabase.GetNextEmployee(),
                                    EmployeesDatabase.GetNextEmployee(),
                                    EmployeesDatabase.GetNextEmployee(),
                                }
                            },
                            {
                                "intern employees", new JTreeObject()
                                {
                                    EmployeesDatabase.GetNextEmployee(),
                                    EmployeesDatabase.GetNextEmployee(),
                                }
                            }
                        }
                    },
                    {
                        "HR", new JTreeObject()
                        {
                            {
                                "full time employees", new JTreeObject(EmployeesDatabase.GetTenBestEmployees())
                            }
                        }
                    }
                }
            );

            return manager;
        }
    }

    /// <summary>
    /// Helper class simulating enum
    /// </summary>
    internal enum AvailableStateCodes
    {
        WA,
        CA,
        NY,
    }
}
