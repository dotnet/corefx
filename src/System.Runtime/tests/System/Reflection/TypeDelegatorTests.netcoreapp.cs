using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Reflection.Tests
{
    public class TypeDelegatorNetcoreTests
    {
        private static readonly IEnumerable<Type> NonArrayBaseTypes = new Type[]
         {
            typeof(int),
            typeof(void),
            typeof(int*),
            typeof(Outside),
            typeof(Outside<>),
            typeof(Outside<>).GetTypeInfo().GenericTypeParameters[0],
            Type.GetTypeFromCLSID(default(Guid)),
            new object().GetType().GetType()
         };

        [Fact]
        public void IsSZArray_FalseForNonArrayTypes()
        {
            foreach (Type type in NonArrayBaseTypes)
            {
                Assert.False(new TypeDelegator(type).IsSZArray);
            }
        }

        [Fact]
        public void IsSZArray_TrueForSZArrayTypes()
        {
            foreach (Type type in NonArrayBaseTypes.Select(nonArrayBaseType => nonArrayBaseType.MakeArrayType()))
            {
                Assert.True(new TypeDelegator(type).IsSZArray);
            }
        }

        [Fact]
        public void IsSZArray_FalseForVariableBoundArrayTypes()
        {
            foreach (Type type in NonArrayBaseTypes.Select(nonArrayBaseType => nonArrayBaseType.MakeArrayType(1)))
            {
                Assert.False(new TypeDelegator(type).IsSZArray);
            }
        }

        [Fact]
        public void IsSZArray_FalseForNonArrayByRefType()
        {
            Assert.False(new TypeDelegator(typeof(int).MakeByRefType()).IsSZArray);
        }

        [Fact]
        public void IsSZArray_FalseForByRefSZArrayType()
        {
            Assert.False(new TypeDelegator(typeof(int[]).MakeByRefType()).IsSZArray);
        }


        [Fact]
        public void IsSZArray_FalseForByRefVariableArrayType()
        {
            Assert.False(new TypeDelegator(typeof(int[,]).MakeByRefType()).IsSZArray);
        }

        [Fact]
        public void IsVariableBoundArray_FalseForNonArrayTypes()
        {
            foreach (Type type in NonArrayBaseTypes)
            {
                Assert.False(new TypeDelegator(type).IsVariableBoundArray);
            }
        }

        [Fact]
        public void IsVariableBoundArray_FalseForSZArrayTypes()
        {
            foreach (Type type in NonArrayBaseTypes.Select(nonArrayBaseType => nonArrayBaseType.MakeArrayType()))
            {
                Assert.False(new TypeDelegator(type).IsVariableBoundArray);
            }
        }

        [Fact]
        public void IsVariableBoundArray_TrueForVariableBoundArrayTypes()
        {
            foreach (Type type in NonArrayBaseTypes.Select(nonArrayBaseType => nonArrayBaseType.MakeArrayType(1)))
            {
                Assert.True(new TypeDelegator(type).IsVariableBoundArray);
            }
        }

        [Fact]
        public void IsVariableBoundArray_FalseForNonArrayByRefType()
        {
            Assert.False(new TypeDelegator(typeof(int).MakeByRefType()).IsVariableBoundArray);
        }

        [Fact]
        public void IsVariableBoundArray_FalseForByRefSZArrayType()
        {
            Assert.False(new TypeDelegator(typeof(int[]).MakeByRefType()).IsVariableBoundArray);
        }


        [Fact]
        public void IsVariableBoundArray_FalseForByRefVariableArrayType()
        {
            Assert.False(new TypeDelegator(typeof(int[,]).MakeByRefType()).IsVariableBoundArray);
        }
    }
}