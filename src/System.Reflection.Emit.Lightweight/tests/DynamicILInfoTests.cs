// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class DynamicILInfoTests
    {
        private const string LocalSignature = "LocalSignature";
        private const string Exceptions = "Exceptions";
        private const string Code = "Code";

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public unsafe void SetX_ArrayNull_SetsToEmptyArray(bool skipVisibility)
        {
            DynamicMethod dynamicMethod = GetDynamicMethod(skipVisibility);
            DynamicILInfo dynamicILInfo = dynamicMethod.GetDynamicILInfo();

            dynamicILInfo.SetLocalSignature(null);
            Assert.Equal(0, GetPropertyValue(dynamicILInfo, LocalSignature).Length);
            dynamicILInfo.SetExceptions(null);
            Assert.Equal(0, GetPropertyValue(dynamicILInfo, Exceptions).Length);
            dynamicILInfo.SetCode(null, 2);
            Assert.Equal(0, GetPropertyValue(dynamicILInfo, Code).Length);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public unsafe void SetX_ArrayEmpty_SetsToEmptyArray(bool skipVisibility)
        {
            DynamicMethod dynamicMethod = GetDynamicMethod(skipVisibility);
			DynamicILInfo dynamicILInfo = dynamicMethod.GetDynamicILInfo();

			dynamicILInfo.SetLocalSignature(new Byte[] { });
            Assert.Equal(0, GetPropertyValue(dynamicILInfo, LocalSignature).Length);
			dynamicILInfo.SetExceptions(new Byte[] { });
            Assert.Equal(0, GetPropertyValue(dynamicILInfo, Exceptions).Length);
            dynamicILInfo.SetCode(new Byte[] { }, 2);
            Assert.Equal(0, GetPropertyValue(dynamicILInfo, Code).Length);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public unsafe void SetX_ArrayNotNullOrEmpty_Success(bool skipVisibility)
        {
            Byte[] brCode = new Byte[] { 35, 26, 98, 0, 17, 128, 9, 23, 69, 12, 07, 84, 4, 43 };
            DynamicMethod dynamicMethod = GetDynamicMethod(skipVisibility);
			DynamicILInfo dynamicILInfo = dynamicMethod.GetDynamicILInfo();

			dynamicILInfo.SetLocalSignature(brCode);
            Assert.Equal(brCode, GetPropertyValue(dynamicILInfo, LocalSignature));
			dynamicILInfo.SetExceptions(brCode);
            Assert.Equal(brCode, GetPropertyValue(dynamicILInfo, Exceptions));
            dynamicILInfo.SetCode(brCode, 1000004);
            Assert.Equal(brCode, GetPropertyValue(dynamicILInfo, Code));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public unsafe void SetX_LengthIsZero_SetsToEmptyArray(bool skipVisibility)
        {
            DynamicMethod dynamicMethod = GetDynamicMethod(skipVisibility);
            Byte[] brCode = new Byte[] { };
			// Get the pointer to the byte array.
		    GCHandle hmem = GCHandle.Alloc((Object) brCode, GCHandleType.Pinned);
			IntPtr addr = hmem.AddrOfPinnedObject();
			DynamicILInfo dynamicILInfo = dynamicMethod.GetDynamicILInfo();

			dynamicILInfo.SetLocalSignature((byte*)addr, brCode.Length);
            Assert.Equal(0, GetPropertyValue(dynamicILInfo, LocalSignature).Length);
			dynamicILInfo.SetExceptions((byte*)addr, brCode.Length);
            Assert.Equal(0, GetPropertyValue(dynamicILInfo, Exceptions).Length);
			dynamicILInfo.SetCode((byte*)addr, brCode.Length, 4);
            Assert.Equal(0, GetPropertyValue(dynamicILInfo, Code).Length);
			hmem.Free();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public unsafe void SetX_NullInput_ThrowsArgumentNullException(bool skipVisibility)
        {
            DynamicMethod method = GetDynamicMethod(skipVisibility);
            DynamicILInfo dynamicILInfo = method.GetDynamicILInfo();

            Assert.Throws<ArgumentNullException>(() => dynamicILInfo.SetCode(null, 1, 8));
            Assert.Throws<ArgumentNullException>(() => dynamicILInfo.SetExceptions(null, 1));
            Assert.Throws<ArgumentNullException>(() => dynamicILInfo.SetLocalSignature(null, 1));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public unsafe void SetX_NegativeInputSize_ThrowsArgumentOutOfRangeException(bool skipVisibility)
        {
            DynamicMethod method = GetDynamicMethod(skipVisibility);
            DynamicILInfo dynamicILInfo = method.GetDynamicILInfo();
            var bytes = new byte[] { 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02 };

            Assert.Throws<ArgumentOutOfRangeException>(() => {fixed (byte* bytesPtr = bytes) { dynamicILInfo.SetCode(bytesPtr, -1, 8); }});
            Assert.Throws<ArgumentOutOfRangeException>(() => {fixed (byte* bytesPtr = bytes) { dynamicILInfo.SetExceptions(bytesPtr, -1); }});
            Assert.Throws<ArgumentOutOfRangeException>(() => {fixed (byte* bytesPtr = bytes) { dynamicILInfo.SetLocalSignature(bytesPtr, -1); }});
        }

        [Fact]
        public unsafe void SetX_LengthNotZero_Success()
        {
            DynamicMethod dynamicMethod = new DynamicMethod("DynamicMethod5", typeof(void), null, typeof(Int32), true);
            Byte[] brCode = new Byte[] { 0x03, 0x30, 0x0A, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x2a };
            // Get the pointer to the byte array.
            GCHandle hmem = GCHandle.Alloc((Object) brCode, GCHandleType.Pinned);
            IntPtr addr = hmem.AddrOfPinnedObject();
			DynamicILInfo dynamicILInfo = dynamicMethod.GetDynamicILInfo();

			dynamicILInfo.SetLocalSignature((byte*)addr, brCode.Length);
            Assert.Equal(brCode, GetPropertyValue(dynamicILInfo, LocalSignature));
			dynamicILInfo.SetExceptions((byte*)addr, brCode.Length);
            Assert.Equal(brCode, GetPropertyValue(dynamicILInfo, Exceptions));
			dynamicILInfo.SetCode((byte*)addr, brCode.Length, 198);
            Assert.Equal(brCode, GetPropertyValue(dynamicILInfo, Code));
            hmem.Free();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetDynamicILInfo_IsNotUniqueNotNull(bool skipVisibility)
        {
            DynamicMethod method = GetDynamicMethod(skipVisibility);
            DynamicILInfo dynamicILInfo = method.GetDynamicILInfo();
            
            Assert.NotNull(dynamicILInfo);
            Assert.Equal(dynamicILInfo, method.GetDynamicILInfo());
            Assert.Equal(method, dynamicILInfo.DynamicMethod);
        }
        
        private DynamicMethod GetDynamicMethod(bool skipVisibility)
        {
            return new DynamicMethod("DynamicMethod", typeof(void), 
                new Type[] { typeof(object), typeof(int), typeof(string) },
                typeof(Object),
                skipVisibility);
        }

        private static Byte[] GetPropertyValue(DynamicILInfo dynamicInfo, string property)
        {
            // Get property value for verification.
            PropertyInfo pinfo = typeof(DynamicILInfo).GetProperty(property, BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(pinfo);
            return (Byte[])pinfo.GetValue(dynamicInfo, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
        }
    }
}