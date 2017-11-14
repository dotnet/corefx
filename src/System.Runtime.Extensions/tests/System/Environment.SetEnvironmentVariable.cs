// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.InteropServices;
using System.Security;
using Xunit;

namespace System.Tests
{
    public class SetEnvironmentVariable
    {
        private const string NullString = "\u0000";

        internal static bool IsSupportedTarget(EnvironmentVariableTarget target)
        {
            return target == EnvironmentVariableTarget.Process || (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !PlatformDetection.IsUap);
        }

        [Fact]
        public void NullVariableThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => Environment.SetEnvironmentVariable(null, "test"));
        }

        [Fact]
        public void IncorrectVariableThrowsArgument()
        {
            AssertExtensions.Throws<ArgumentException>("variable", () => Environment.SetEnvironmentVariable(string.Empty, "test"));
            AssertExtensions.Throws<ArgumentException>("variable", () => Environment.SetEnvironmentVariable(NullString, "test"));
            AssertExtensions.Throws<ArgumentException>("variable", null, () => Environment.SetEnvironmentVariable("Variable=Something", "test"));
        }

        private static void ExecuteAgainstTarget(
            EnvironmentVariableTarget target,
            Action action,
            Action cleanUp = null)
        {
            bool shouldCleanUp = cleanUp != null;
            try
            {
                action();
            }
            catch (SecurityException)
            {
                shouldCleanUp = false;
                Assert.True(target == EnvironmentVariableTarget.Machine || (target == EnvironmentVariableTarget.User && PlatformDetection.IsUap),
                            "only machine target, or user when in uap, should have access issues");
                Assert.True(PlatformDetection.IsWindows, "and it should be Windows");
                Assert.False(PlatformDetection.IsWindowsAndElevated, "and we shouldn't be elevated");
            }
            finally
            {
                if (shouldCleanUp)
                    cleanUp();
            }
        }

        [Theory]
        [MemberData(nameof(EnvironmentTests.EnvironmentVariableTargets), MemberType = typeof(EnvironmentTests))]
        public void Default(EnvironmentVariableTarget target)
        {
            string varName = $"Test_SetEnvironmentVariable_Default ({target})";
            const string value = "true";

            ExecuteAgainstTarget(target,
            () =>
            {
                Environment.SetEnvironmentVariable(varName, value, target);
                Assert.Equal(IsSupportedTarget(target) ? value : null,
                    Environment.GetEnvironmentVariable(varName, target));
            },
            () =>
            {
                // Clear the test variable
                Environment.SetEnvironmentVariable(varName, null, target);
            });
        }


        [Theory]
        [MemberData(nameof(EnvironmentTests.EnvironmentVariableTargets), MemberType = typeof(EnvironmentTests))]
        public void ModifyEnvironmentVariable(EnvironmentVariableTarget target)
        {
            string varName = $"Test_ModifyEnvironmentVariable ({target})";
            const string value = "false";

            ExecuteAgainstTarget(target,
            () =>
            {
                // First set the value to something and then change it and ensure that it gets modified.
                Environment.SetEnvironmentVariable(varName, "true", target);

                Environment.SetEnvironmentVariable(varName, value, target);

                // Check whether the variable exists.
                Assert.Equal(IsSupportedTarget(target) ? value : null, Environment.GetEnvironmentVariable(varName, target));
            },
            () =>
            {
                // Clear the test variable
                Environment.SetEnvironmentVariable(varName, null, target);
            });
        }

        [Theory]
        [MemberData(nameof(EnvironmentTests.EnvironmentVariableTargets), MemberType = typeof(EnvironmentTests))]
        public void ModifyEnvironmentVariable_AndEnumerate(EnvironmentVariableTarget target)
        {
            string varName = $"Test_ModifyEnvironmentVariable_AndEnumerate ({target})";
            const string value = "false";

            ExecuteAgainstTarget(target,
            () =>
            {
                // First set the value to something and then change it and ensure that it gets modified.
                Environment.SetEnvironmentVariable(varName, "true", target);

                // Enumerate to validate our first value to ensure we can still set after enumerating
                IDictionary variables = Environment.GetEnvironmentVariables(target);
                if (IsSupportedTarget(target))
                {
                    Assert.True(variables.Contains(varName), "has the key we entered");
                    Assert.Equal("true", variables[varName]);
                }

                Environment.SetEnvironmentVariable(varName, value, target);

                // Check whether the variable exists.
                Assert.Equal(IsSupportedTarget(target) ? value : null, Environment.GetEnvironmentVariable(varName, target));
            },
            () =>
            {
                // Clear the test variable
                Environment.SetEnvironmentVariable(varName, null, target);
            });
        }

        [Theory]
        [MemberData(nameof(EnvironmentTests.EnvironmentVariableTargets), MemberType = typeof(EnvironmentTests))]
        public void DeleteEnvironmentVariable(EnvironmentVariableTarget target)
        {
            string varName = $"Test_DeleteEnvironmentVariable ({target})";
            const string value = "false";

            ExecuteAgainstTarget(target,
            () =>
            {
                // First set the value to something and then ensure that it can be deleted.
                Environment.SetEnvironmentVariable(varName, value);
                Environment.SetEnvironmentVariable(varName, string.Empty);
                Assert.Equal(null, Environment.GetEnvironmentVariable(varName));

                Environment.SetEnvironmentVariable(varName, value);
                Environment.SetEnvironmentVariable(varName, null);
                Assert.Equal(null, Environment.GetEnvironmentVariable(varName));

                Environment.SetEnvironmentVariable(varName, value);
                Environment.SetEnvironmentVariable(varName, NullString);
                Assert.Equal(null, Environment.GetEnvironmentVariable(varName));
            });
        }

        [Fact]
        public void DeleteEnvironmentVariableNonInitialNullInName()
        {
            const string varNamePrefix = "Begin_DeleteEnvironmentVariableNonInitialNullInName";
            const string varNameSuffix = "End_DeleteEnvironmentVariableNonInitialNullInName";
            const string varName = varNamePrefix + NullString + varNameSuffix;
            const string value = "false";

            try
            {
                Environment.SetEnvironmentVariable(varName, value);
                Environment.SetEnvironmentVariable(varName, null);
                Assert.Equal(Environment.GetEnvironmentVariable(varName), null);
                Assert.Equal(Environment.GetEnvironmentVariable(varNamePrefix), null);
            }
            finally
            {
                // Clear the test variable
                Environment.SetEnvironmentVariable(varName, null);
            }
        }

        [Fact]
        public void DeleteEnvironmentVariableInitialNullInValue()
        {
            const string value = NullString + "test";
            const string varName = "DeleteEnvironmentVariableInitialNullInValue";

            try
            {
                Environment.SetEnvironmentVariable(varName, value);
                Assert.Equal(null, Environment.GetEnvironmentVariable(varName));
            }
            finally
            {
                Environment.SetEnvironmentVariable(varName, String.Empty);
            }
        }

        [Fact]
        public void NonInitialNullCharacterInVariableName()
        {
            const string varNamePrefix = "NonInitialNullCharacterInVariableName_Begin";
            const string varNameSuffix = "NonInitialNullCharacterInVariableName_End";
            const string varName = varNamePrefix + NullString + varNameSuffix;
            const string value = "true";

            try
            {
                Environment.SetEnvironmentVariable(varName, value);
                Assert.Equal(value, Environment.GetEnvironmentVariable(varName));
                Assert.Equal(value, Environment.GetEnvironmentVariable(varNamePrefix));
            }
            finally
            {
                Environment.SetEnvironmentVariable(varName, String.Empty);
                Environment.SetEnvironmentVariable(varNamePrefix, String.Empty);
            }
        }

        [Fact]
        public void NonInitialNullCharacterInValue()
        {
            const string varName = "Test_TestNonInitialZeroCharacterInValue";
            const string valuePrefix = "Begin";
            const string valueSuffix = "End";
            const string value = valuePrefix + NullString + valueSuffix;

            try
            {
                Environment.SetEnvironmentVariable(varName, value);
                Assert.Equal(valuePrefix, Environment.GetEnvironmentVariable(varName));
            }
            finally
            {
                Environment.SetEnvironmentVariable(varName, String.Empty);
            }
        }

        [Fact]
        public void DeleteNonExistentEnvironmentVariable()
        {
            const string varName = "Test_TestDeletingNonExistingEnvironmentVariable";

            if (Environment.GetEnvironmentVariable(varName) != null)
            {
                Environment.SetEnvironmentVariable(varName, null);
            }

            Environment.SetEnvironmentVariable("TestDeletingNonExistingEnvironmentVariable", String.Empty);
        }
    }
}
