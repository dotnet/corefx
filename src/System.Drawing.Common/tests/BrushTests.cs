// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Xunit;

namespace System.Drawing.Tests
{
    public class BrushTests : RemoteExecutorTestBase
    {
        [Fact]
        public void SetNativeBrush_Brush_Success()
        {
            using (var brush = new SubBrush())
            {
                brush.PublicSetNativeBrush((IntPtr)10);
                brush.PublicSetNativeBrush(IntPtr.Zero);

                brush.PublicSetNativeBrush((IntPtr)10);
                brush.PublicSetNativeBrush(IntPtr.Zero);
            }
        }

        private class SubBrush : Brush
        {
            public override object Clone() => this;
            public void PublicSetNativeBrush(IntPtr brush) => SetNativeBrush(brush);
        }
    }
}
