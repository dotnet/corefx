// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Data.OleDb;

namespace System.Data.OleDb.Tests
{
    public class ReaderTests : IntegrationTestBase
    {
        [Fact]
        public void EmptyReader()
        {
            command.CommandText =
                @"CREATE TABLE SampleTable (
                    SomeString NVARCHAR(100))";
            command.ExecuteNonQuery();

            command.CommandText =
                @"SELECT 
                    SomeString
                FROM SampleTable";
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert.False(reader.HasRows);

                var exception = Record.Exception(() => reader.GetString(1));
                Assert.NotNull(exception);
                Assert.IsType<InvalidOperationException>(exception);
                Assert.Equal(
                    "No data exists for the row/column.",
                    exception.Message);

                var values = new object[1];
                exception = Record.Exception(() => reader.GetValues(values));
                Assert.NotNull(exception);
                Assert.IsType<InvalidOperationException>(exception);
                Assert.Equal(
                    "No data exists for the row/column.",
                    exception.Message);
            }
        }

        [Fact]
        public void GetValues()
        {
            command.CommandText =
                @"CREATE TABLE SampleTable (
                    SomeInt32 INT,
                    SomeString NVARCHAR(100))";
            command.ExecuteNonQuery();

            command.CommandText =
                @"INSERT INTO SampleTable (
                    SomeInt32,
                    SomeString)
                VALUES (
                    2147483647,
                    'SomeString')";
            command.ExecuteNonQuery();

            command.CommandText =
                @"SELECT 
                    SomeInt32,
                    SomeString
                FROM SampleTable";
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                Assert.Equal(2147483647, values[0]);
                Assert.Equal("SomeString", values[1]);
            }
        }

        [Fact]
        public void InvalidRowIndex()
        {
            command.CommandText =
                @"CREATE TABLE SampleTable (
                    SomeString NVARCHAR(100))";
            command.ExecuteNonQuery();

            command.CommandText =
                @"INSERT INTO SampleTable (
                    SomeString)
                VALUES (
                    'SomeString')";
            command.ExecuteNonQuery();

            command.CommandText =
                @"SELECT 
                    SomeString
                FROM SampleTable";
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert.True(reader.HasRows);
                var exception = Record.Exception(() => reader.GetString(2));
                Assert.NotNull(exception);
                Assert.IsType<IndexOutOfRangeException>(exception);
                Assert.Equal(
                    "Index was outside the bounds of the array.",
                    exception.Message);
            }
        }

        [Fact]
        public void InvalidRowName()
        {
            command.CommandText =
                @"CREATE TABLE SampleTable (
                    SomeString NVARCHAR(100))";
            command.ExecuteNonQuery();

            command.CommandText =
                @"INSERT INTO SampleTable (
                    SomeString)
                VALUES (
                    'SomeString')";
            command.ExecuteNonQuery();

            command.CommandText =
                @"SELECT 
                    SomeString
                FROM SampleTable";
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert.True(reader.HasRows);
                var exception = Record.Exception(() => reader["SomeOtherString"]);
                Assert.NotNull(exception);
                Assert.IsType<IndexOutOfRangeException>(exception);
                Assert.Equal(
                    "SomeOtherString",
                    exception.Message);
            }
        }
    }
}