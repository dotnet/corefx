// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void ZeroLengthInvalidIndex()
        {
            int dummy = 0;
            Span<int> sp = new Span<int>(Array.Empty<int>());

            for (int i = -2; i <= 2; i++)
            {
                try
                {
                    dummy = sp[i];
                }
                catch (IndexOutOfRangeException ex)
                {
                    if (ex.GetType() != typeof(IndexOutOfRangeException))
                        throw;
                }

                try
                {
                    sp[i] = dummy;
                }
                catch (IndexOutOfRangeException ex)
                {
                    if (ex.GetType() != typeof(IndexOutOfRangeException))
                        throw;
                }
            }
        }
    }
}