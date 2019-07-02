﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ValueTests
    {
        [Fact]
        public static void WritePrimitives()
        {
            {
                string json = JsonSerializer.Serialize(1);
                Assert.Equal("1", json);
            }

            {
                int? value = 1;
                string json = JsonSerializer.Serialize(value);
                Assert.Equal("1", json);
            }

            {
                int? value = null;
                string json = JsonSerializer.Serialize(value);
                Assert.Equal("null", json);
            }

            {
                Span<byte> json = JsonSerializer.SerializeToUtf8Bytes(1);
                Assert.Equal(Encoding.UTF8.GetBytes("1"), json.ToArray());
            }

            {
                string json = JsonSerializer.Serialize(long.MaxValue);
                Assert.Equal(long.MaxValue.ToString(), json);
            }

            {
                Span<byte> json = JsonSerializer.SerializeToUtf8Bytes(long.MaxValue);
                Assert.Equal(Encoding.UTF8.GetBytes(long.MaxValue.ToString()), json.ToArray());
            }

            {
                string json = JsonSerializer.Serialize("Hello");
                Assert.Equal(@"""Hello""", json);
            }

            {
                Span<byte> json = JsonSerializer.SerializeToUtf8Bytes("Hello");
                Assert.Equal(Encoding.UTF8.GetBytes(@"""Hello"""), json.ToArray());
            }

            {
                Uri uri = new Uri("https://domain/path");
                Assert.Equal(@"""https:\u002f\u002fdomain\u002fpath""", JsonSerializer.Serialize(uri));
            }

            {
                Uri.TryCreate("~/path", UriKind.RelativeOrAbsolute, out Uri uri);
                Assert.Equal(@"""~\u002fpath""", JsonSerializer.Serialize(uri));
            }
        }
    }
}
