// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace System.IO.FileSystem.Tests
{
    public abstract class FileSystemTest : IDisposable
    {
        public static readonly byte[] TestBuffer = { 0xBA, 0x5E, 0xBA, 0x11, 0xF0, 0x07, 0xBA, 0x11 };

        private const bool CleanUp = true;

        public readonly string TestDirectory;

        public FileSystemTest()
        {
            // Use a unique test directory per test class
            TestDirectory = Path.Combine(Directory.GetCurrentDirectory(), GetType().Name);

            try
            {
                Directory.CreateDirectory(TestDirectory);
            }
            catch 
            {
                // Don't want this to crash the test, we'll fail appropriately in other test 
                // cases if Directory.Create is broken.
            }
        }

        // Generates a test file path to use that is unique name per test case / call
        public string GetTestFilePath([CallerMemberName]string fileName = null, [CallerLineNumber]int lineNumber = 0)
        {
            return Path.Combine(TestDirectory, String.Format("{0}_{1}", fileName ?? "testFile", lineNumber));
        }

        // Perform a test action on a newly created directory
        protected void TestOnValidDirectory(Action<string> testAction, [CallerMemberName]string fileName = null, [CallerLineNumber]int lineNumber = 0)
        {
            string testDirPath = GetTestFilePath(fileName, lineNumber);
            Directory.CreateDirectory(testDirPath);
            try
            {
                testAction(testDirPath);
            }
            finally
            {
                if (CleanUp)
                    Directory.Delete(testDirPath);
            }
        }

        // Perform a test action on a newly created file
        protected void TestOnValidFile(Action<string> testAction, [CallerMemberName]string fileName = null, [CallerLineNumber]int lineNumber = 0)
        {
            string testFilePath = GetTestFilePath(fileName, lineNumber);
            FileStream fs = File.Create(testFilePath);
            fs.Dispose();
            try
            {
                testAction(testFilePath);
            }
            finally
            {
                if (CleanUp)
                    File.Delete(testFilePath);
            }
        }

        protected void TestOnValidFileAndDirectory(Action<string> testAction, [CallerMemberName]string fileName = null, [CallerLineNumber]int lineNumber = 0)
        {
            TestOnValidFile(testAction, fileName + "_File", lineNumber);
            TestOnValidDirectory(testAction, fileName + "_Directory", lineNumber);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            // if (disposing)  no managed resources

            // clean up non-managed resources
            try
            {
                if (CleanUp)
                    Directory.Delete(TestDirectory, true);
            }
            catch
            {
                // Don't throw during dispose
            }
        }
    }
}
