// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class DynamicILInfoTests
    {
        private const string FieldName = "_id";

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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetDynamicILInfo_IsUniqueNotNull(bool skipVisibility)
        {
            DynamicMethod method = GetDynamicMethod(skipVisibility);
            DynamicILInfo dynamicILInfo = method.GetDynamicILInfo();
            
            Assert.NotNull(dynamicILInfo);
            Assert.Equal(dynamicILInfo, method.GetDynamicILInfo());
            Assert.Equal(method, dynamicILInfo.DynamicMethod);
        }
        
        private DynamicMethod GetDynamicMethod(bool skipVisibility)
        {
            IDClass target = new IDClass();
            FieldInfo field = typeof(IDClass).GetField(FieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Type[] paramTypes = new Type[] { typeof(IDClass), typeof(int) };
            DynamicMethod method = new DynamicMethod("Method", typeof(int), paramTypes, typeof(IDClass), skipVisibility);
            return method;
        }
    }
}