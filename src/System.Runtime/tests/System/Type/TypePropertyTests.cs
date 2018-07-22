// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Tests.Types
{
    public abstract class TypePropertyTestBase
    {
        public abstract Type CreateType();
        
        public abstract TypeAttributes Attributes { get; }

        public virtual Type BaseType => typeof(object);

        public virtual bool ContainsGenericParameters => false;

        public virtual MethodBase DeclaringMethod => null;

        public virtual Type DeclaringType => null;

        public virtual Type ElementType => null;

        public virtual GenericParameterAttributes? GenericParameterAttributes => null;

        public virtual int? GenericParameterPosition => null;

        public virtual Type[] GenericTypeArguments => new Type[0];

        public virtual bool HasElementType => false;

        public virtual bool IsArray => false;

        public virtual bool IsByRef => false;

        public virtual bool IsCOMObject => false;

        public virtual bool IsConstructedGenericType => false;

        public virtual bool IsContextful => false;

        public virtual bool IsGenericParameter => false;

        public virtual bool IsGenericMethodParameter => IsGenericParameter && DeclaringMethod != null;

        public virtual bool IsGenericTypeDefinition => false;

        public virtual bool IsGenericTypeParameter => IsGenericParameter && DeclaringMethod == null;

        public virtual bool IsGenericType => false;

        public virtual bool IsMarshalByRef => false;

        public virtual bool IsNested => DeclaringType != null;

        public virtual bool IsPointer => false;

        public virtual bool IsPrimitive => false;

        public virtual bool IsSecurityCritical => true;

        public virtual bool IsSecuritySafeCritical => false;

        public virtual bool IsSecurityTransparent => false;

        public virtual bool IsSignatureType => false;

        public virtual bool IsSZArray => false;

        public virtual bool IsTypeDefinition => true;

        public virtual bool IsValueType => BaseType == typeof(Enum) || (BaseType == typeof(ValueType) && CreateType() != typeof(Enum));

        public virtual bool IsVariableBoundArray => false;

        public virtual MemberTypes MemberType => IsNested ? MemberTypes.TypeInfo : MemberTypes.NestedType;

        public virtual Type ReflectedType => DeclaringType;

        public virtual Type UnderlyingSystemType => CreateType();


        [Fact]
        public void DeclaringType_Get_ReturnsExpected()
        {
            Assert.Equal(DeclaringType, CreateType().DeclaringType);
        }

        [Fact]
        public void GenericTypeArguments_Get_ReturnsExpected()
        {
            Assert.Equal(GenericTypeArguments, CreateType().GenericTypeArguments);
        }

        [Fact]
        public void GenericParameterAttributes_Get_ReturnsExpected()
        {
            if (GenericParameterPosition != null)
            {
                Assert.Equal(GenericParameterAttributes, CreateType().GenericParameterAttributes);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => CreateType().GenericParameterAttributes);
            }
        }

        [Fact]
        public void GenericParameterPosition_Get_ReturnsExpected()
        {
            if (GenericParameterPosition != null)
            {
                Assert.Equal(GenericParameterPosition, CreateType().GenericParameterPosition);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => CreateType().GenericParameterPosition);
            }
        }

        [Fact]
        public void GetElementType_Invoke_ReturnsExpected()
        {
            Assert.Equal(ElementType, CreateType().GetElementType());
        }

        [Fact]
        public void HasElementType_Get_ReturnsExpected()
        {
            Assert.Equal(HasElementType, CreateType().HasElementType);
        }

        [Fact]
        public void IsArray_Get_ReturnsExpected()
        {
            Assert.Equal(IsArray, CreateType().IsArray);
        }

        [Fact]
        public void IsByRef_Get_ReturnsExpected()
        {
            Type t = CreateType();
            Assert.Equal(IsByRef, t.IsByRef);
            if (!t.IsByRef && t != typeof(TypedReference))
            {
                Assert.True(t.MakeByRefType().IsByRef);
            }
        }

        [Fact]
        public void IsConstructedGenericType_Get_ReturnsExpected()
        {
            Assert.Equal(IsConstructedGenericType, CreateType().IsConstructedGenericType);
        }

        [Fact]
        public void IsGenericParameter_Get_ReturnsExpected()
        {
            Assert.Equal(IsGenericParameter, CreateType().IsGenericParameter);
        }

        [Fact]
        public void IsGenericMethodParameter_Get_ReturnsExpected()
        {
            Assert.Equal(IsGenericMethodParameter, CreateType().IsGenericMethodParameter);
        }

        [Fact]
        public void IsGenericTypeDefinition_Get_ReturnsExpected()
        {
            Assert.Equal(IsGenericTypeDefinition, CreateType().IsGenericTypeDefinition);
        }

        [Fact]
        public void IsGenericTypeParameter_Get_ReturnsExpected()
        {
            Assert.Equal(IsGenericTypeParameter, CreateType().IsGenericTypeParameter);
        }

        [Fact]
        public void IsGenericType_Get_ReturnsExpected()
        {
            Assert.Equal(IsGenericType, CreateType().IsGenericType);
        }

        [Fact]
        public void IsPointer_Get_ReturnsExpected()
        {
            Type t = CreateType();
            Assert.Equal(IsPointer, t.IsPointer);
            if (!t.IsByRef && t != typeof(TypedReference))
            {
                Assert.True(t.MakePointerType().IsPointer);
            }
        }

        [Fact]
        public void IsNested_Get_ReturnsExpected()
        {
            Assert.Equal(DeclaringType != null, CreateType().IsNested);
        }

        [Fact]
        public void IsSecurityCritical_Get_ReturnsTrue()
        {
            Assert.True(CreateType().IsSecurityCritical);
        }

        [Fact]
        public void IsSecuritySafeCritical_Get_ReturnsFalse()
        {
            Assert.False(CreateType().IsSecuritySafeCritical);
        }

        [Fact]
        public void IsSecurityTransparent_Get_ReturnsFalse()
        {
            Assert.False(CreateType().IsSecurityTransparent);
        }
    }

    public abstract class ArrayTypeTestBase : TypePropertyTestBase
    {
        public override bool IsArray => true;

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Serializable;

        public override bool HasElementType => true;
    }

    public abstract class ClassTypeTestBase : TypePropertyTestBase
    {
        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit;
    }

    public abstract class StructTypeTestBase : TypePropertyTestBase
    {
        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.SequentialLayout | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit;
    }

    public abstract class InterfaceTypeTestBase : TypePropertyTestBase
    {
        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.ClassSemanticsMask | TypeAttributes.Abstract;
    }

    public abstract class PrimitiveTypeTestBase : StructTypeTestBase
    {
        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.SequentialLayout | TypeAttributes.Sealed | TypeAttributes.Serializable | TypeAttributes.BeforeFieldInit;
    }

    public class IntTests : PrimitiveTypeTestBase
    {
        public override Type CreateType() => typeof(int);
    }

    public class IntRefTests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(int).MakeByRefType();

        public override TypeAttributes Attributes => TypeAttributes.Class;

        public override bool IsByRef => true;

        public override bool HasElementType => true;

        public override Type ElementType => typeof(int);
    }

    public class IntPointerTests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(int).MakePointerType();

        public override TypeAttributes Attributes => TypeAttributes.Class;

        public override bool IsPointer => true;

        public override bool HasElementType => true;

        public override Type ElementType => typeof(int);
    }

    public class IntArrayTests : ArrayTypeTestBase
    {
        public override Type CreateType() => typeof(int[]);

        public override Type ElementType => typeof(int);
    }

    public class MultidimensionalIntArrayTests : ArrayTypeTestBase
    {
        public override Type CreateType() => typeof(int[,]);

        public override Type ElementType => typeof(int);
    }

    public class ArrayOfArrayTests : ArrayTypeTestBase
    {
        public override Type CreateType() => typeof(int[][]);
        
        public override Type ElementType => typeof(int[]);
    }

    public class NestedArrayTests : ArrayTypeTestBase
    {
        public override Type CreateType() => typeof(Outside.Inside[]);

        public override Type ElementType => typeof(Outside.Inside);
    }

    public class GenericNestedArrayTests : ArrayTypeTestBase
    {
        public override Type CreateType() => typeof(Outside<int>.Inside<double>[]);

        public override Type ElementType => typeof(Outside<int>.Inside<double>);
    }

    public class ArrayTypeTests : ClassTypeTestBase
    {
        public override Type CreateType() => typeof(Array);

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Serializable | TypeAttributes.BeforeFieldInit;
    }

    public class NonGenericClassTests : ClassTypeTestBase
    {
        public override Type CreateType() => typeof(NonGenericClass);
    }

    public class NonGenericSubClassOfNonGenericTests : ClassTypeTestBase
    {
        public override Type CreateType() => typeof(NonGenericSubClassOfNonGeneric);
    }

    public class TypedReferenceTypeTests : StructTypeTestBase
    {
        public override Type CreateType() => typeof(TypedReference);
    }

    public class GenericClass1Tests : ClassTypeTestBase
    {
        public override Type CreateType() => typeof(GenericClass<string>);

        public override bool IsGenericType => true;

        public override bool IsConstructedGenericType => true;

        public override Type[] GenericTypeArguments => new Type[] { typeof(string) };
    }

    public class GenericClass2Tests : ClassTypeTestBase
    {
        public override Type CreateType() => typeof(GenericClass<int, string>);

        public override bool IsGenericType => true;

        public override bool IsConstructedGenericType => true;

        public override Type[] GenericTypeArguments => new Type[] { typeof(int), typeof(string) };
    }

    public class OpenGenericClassTests : ClassTypeTestBase
    {
        public override Type CreateType() => typeof(GenericClass<>);

        public override bool IsGenericType => true;

        public override bool IsGenericTypeDefinition => true;
    }

    public class NonGenericSubClassOfGenericTests : ClassTypeTestBase
    {
        public override Type CreateType() => typeof(NonGenericSubClassOfGeneric);
    }

    public class NonGenericStructTests : StructTypeTestBase
    {
        public override Type CreateType() => typeof(NonGenericStruct);
    }

    public class RefStructTests : StructTypeTestBase
    {
        public override Type CreateType() => typeof(RefStruct);
    }

    public class GenericStruct1Tests : StructTypeTestBase
    {
        public override Type CreateType() => typeof(GenericStruct<string>);

        public override bool IsGenericType => true;

        public override bool IsConstructedGenericType => true;

        public override Type[] GenericTypeArguments => new Type[] { typeof(string) };
    }

    public class GenericStruct2Tests : StructTypeTestBase
    {
        public override Type CreateType() => typeof(GenericStruct<int, string>);

        public override bool IsGenericType => true;

        public override bool IsConstructedGenericType => true;

        public override Type[] GenericTypeArguments => new Type[] { typeof(int), typeof(string) };
    }

    public class NonGenericInterfaceTests : InterfaceTypeTestBase
    {
        public override Type CreateType() => typeof(NonGenericInterface);
    }

    public class GenericInterface1Tests : InterfaceTypeTestBase
    {
        public override Type CreateType() => typeof(GenericInterface<string>);

        public override bool IsGenericType => true;

        public override bool IsConstructedGenericType => true;

        public override Type[] GenericTypeArguments => new Type[] { typeof(string) };
    }

    public class GenericInterface2Tests : InterfaceTypeTestBase
    {
        public override Type CreateType() => typeof(GenericInterface<int, string>);

        public override bool IsGenericType => true;

        public override bool IsConstructedGenericType => true;

        public override Type[] GenericTypeArguments => new Type[] { typeof(int), typeof(string) };
    }

    public class OpenGenericInterfaceTests : InterfaceTypeTestBase
    {
        public override Type CreateType() => typeof(GenericInterface<>);

        public override bool IsGenericType => true;

        public override bool IsGenericTypeDefinition => true;
    }

    public class NonGenericNestedTests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(Outside.Inside);

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.NestedPublic | TypeAttributes.BeforeFieldInit;

        public override Type DeclaringType => typeof(Outside);
    }

    public class GenericNestedTests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(Outside<int>.Inside<double>);

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.NestedPublic | TypeAttributes.BeforeFieldInit;

        public override Type DeclaringType => typeof(Outside<>);

        public override bool IsGenericType => true;

        public override bool IsConstructedGenericType => true;

        public override Type[] GenericTypeArguments => new Type[] { typeof(int), typeof(double) };
    }

    public class OpenGenericNestedTests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(Outside<>.Inside<>);

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.NestedPublic | TypeAttributes.BeforeFieldInit;

        public override Type DeclaringType => typeof(Outside<>);

        public override bool IsGenericType => true;

        public override bool IsGenericTypeDefinition => true;
    }

    public class GenericTypeParameter1Of1Tests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(Outside<>).GetTypeInfo().GenericTypeParameters[0];

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public;

        public override Type DeclaringType => typeof(Outside<>);

        public override bool IsGenericParameter => true;

        public override bool IsGenericTypeParameter => true;

        public override int? GenericParameterPosition => 0;
    }

    public class GenericTypeParameter1Of2Tests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(GenericClass<,>).GetTypeInfo().GenericTypeParameters[0];

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public;

        public override Type DeclaringType => typeof(GenericClass<,>);

        public override bool IsGenericParameter => true;

        public override bool IsGenericTypeParameter => true;

        public override int? GenericParameterPosition => 0;
    }

    public class GenericTypeParameter2Of2Tests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(GenericClass<,>).GetTypeInfo().GenericTypeParameters[1];

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public;

        public override Type DeclaringType => typeof(GenericClass<,>);

        public override bool IsGenericParameter => true;

        public override bool IsGenericTypeParameter => true;

        public override int? GenericParameterPosition => 1;
    }

    public class NestedGenericTypeParameter1Tests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(Outside<>.Inside<>).GetTypeInfo().GenericTypeParameters[0];

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public;

        public override Type DeclaringType => typeof(Outside<>.Inside<>);

        public override bool IsGenericParameter => true;

        public override bool IsGenericTypeParameter => true;

        public override int? GenericParameterPosition => 0;
    }

    public class NestedGenericTypeParameter2Tests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(Outside<>.Inside<>).GetTypeInfo().GenericTypeParameters[1];

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public;

        public override Type DeclaringType => typeof(Outside<>.Inside<>);

        public override bool IsGenericParameter => true;

        public override bool IsGenericTypeParameter => true;

        public override int? GenericParameterPosition => 1;
    }

    public class GenericMethodParameter1Of2Tests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(Outside<>).GetMethod(nameof(Outside<string>.TwoGenericMethod)).GetGenericArguments()[0];

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public;

        public override Type DeclaringType => typeof(Outside<>);

        public override bool IsGenericParameter => true;

        public override bool IsGenericMethodParameter => true;

        public override int? GenericParameterPosition => 0;
    }

    public class GenericMethodParameter2Of2Tests : TypePropertyTestBase
    {
        public override Type CreateType() => typeof(Outside<>).GetMethod(nameof(Outside<string>.TwoGenericMethod)).GetGenericArguments()[1];

        public override TypeAttributes Attributes => TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public;

        public override Type DeclaringType => typeof(Outside<>);

        public override bool IsGenericParameter => true;

        public override bool IsGenericMethodParameter => true;

        public override int? GenericParameterPosition => 1;
    }
}
