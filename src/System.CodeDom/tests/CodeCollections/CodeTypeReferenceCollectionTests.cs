// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.CodeDom.Tests
{
	public class CodeTypeReferenceCollectionTests
	{
		[Fact]
		public void Ctor_IsEmpty()
		{
			var collection = new CodeTypeReferenceCollection();
			Assert.Equal(0, collection.Count);
		}

		public static IEnumerable<object[]> AddRange_TestData()
		{
			yield return new object[] { new CodeTypeReference[0] };
			yield return new object[] { new CodeTypeReference[] { new CodeTypeReference() } };
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor__CodeTypeReferenceArray_Works(CodeTypeReference[] value)
		{
			var collection = new CodeTypeReferenceCollection(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor_CodeTypeReferenceCollection_Works(CodeTypeReference[] value)
		{
			var collection = new CodeTypeReferenceCollection(new CodeTypeReferenceCollection(value));
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeTypeReferenceArray_Works(CodeTypeReference[] value)
		{
			var collection = new CodeTypeReferenceCollection();
			collection.AddRange(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeTypeReferenceCollection_Works(CodeTypeReference[] value)
		{
			var collection = new CodeTypeReferenceCollection();
			collection.AddRange(new CodeTypeReferenceCollection(value));
			VerifyCollection(collection, value);
		}

		[Fact]
		public void AddRange_Null_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeTypeReferenceCollection((CodeTypeReference[])null));
			Assert.Throws<ArgumentNullException>("value", () => new CodeTypeReferenceCollection((CodeTypeReferenceCollection)null));

			var collection = new CodeTypeReferenceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeTypeReference[])null));
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeTypeReferenceCollection)null));
		}

		[Fact]
		public void AddRange_NullObjectInValue_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeTypeReferenceCollection(new CodeTypeReference[] { null }));

			var collection = new CodeTypeReferenceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(new CodeTypeReference[] { null }));
		}

		[Fact]
		public void Add_CodeTypeReference_Insert_Remove()
		{
			var collection = new CodeTypeReferenceCollection();

			var value1 = new CodeTypeReference();
			Assert.Equal(0, collection.Add(value1));
			Assert.Equal(1, collection.Count);
			Assert.Equal(value1, collection[0]);

			var value2 = new CodeTypeReference();
			collection.Insert(0, value2);
			Assert.Equal(2, collection.Count);
			Assert.Same(value2, collection[0]);

			collection.Remove(value1);
			Assert.Equal(1, collection.Count);

			collection.Remove(value2);
			Assert.Equal(0, collection.Count);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("System.Int32")]
		public void Add_String(string type)
		{
			var collection = new CodeTypeReferenceCollection();
			collection.Add(type);
			Assert.Equal(new CodeTypeReference(type).BaseType, collection[0].BaseType);
		}

		[Theory]
		[InlineData(typeof(int))]
		public void Add_Type(Type type)
		{
			var collection = new CodeTypeReferenceCollection();
			collection.Add(type);
			Assert.Equal(new CodeTypeReference(type).BaseType, collection[0].BaseType);
		}

		[Fact]
		public void Add_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeTypeReferenceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Add((CodeTypeReference)null));
			Assert.Throws<ArgumentNullException>("type", () => collection.Add((Type)null));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(1)]
		public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeTypeReferenceCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new CodeTypeReference()));
		}

		[Fact]
		public void Insert_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeTypeReferenceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Insert(0, null));
		}

		[Fact]
		public void Remove_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeTypeReferenceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Remove(null));
		}

		[Fact]
		public void Remove_NoSuchObject_ThrowsArgumentException()
		{
			var collection = new CodeTypeReferenceCollection();
			Assert.Throws<ArgumentException>(null, () => collection.Remove(new CodeTypeReference()));
		}

		[Fact]
		public void Contains_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeTypeReferenceCollection();
			Assert.False(collection.Contains(null));
			Assert.False(collection.Contains(new CodeTypeReference()));
		}

		[Fact]
		public void IndexOf_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeTypeReferenceCollection();
			Assert.Equal(-1, collection.IndexOf(null));
			Assert.Equal(-1, collection.IndexOf(new CodeTypeReference()));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Item_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeTypeReferenceCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new CodeTypeReference());
		}

		[Fact]
		public void ItemSet_Get_ReturnsExpected()
		{
			var value1 = new CodeTypeReference();
			var value2 = new CodeTypeReference();
			var collection = new CodeTypeReferenceCollection();
			collection.Add(value1);

			collection[0] = value2;
			Assert.Equal(1, collection.Count);
			Assert.Same(value2, collection[0]);
		}

		private static void VerifyCollection(CodeTypeReferenceCollection collection, CodeTypeReference[] contents)
		{
			Assert.Equal(contents.Length, collection.Count);
			for (int i = 0; i < contents.Length; i++)
			{
				CodeTypeReference content = contents[i];
				Assert.Equal(i, collection.IndexOf(content));
				Assert.True(collection.Contains(content));
				Assert.Same(content, collection[i]);
			}

			const int Index = 1;
			var copy = new CodeTypeReference[collection.Count + Index];
			collection.CopyTo(copy, Index);
			Assert.Null(copy[0]);
			for (int i = Index; i < copy.Length; i++)
			{
				Assert.Same(contents[i - Index], copy[i]);
			}
		}
	}
}
