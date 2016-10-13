// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.CodeDom.Tests
{
	public class CodeNamespaceCollectionTests
	{
		[Fact]
		public void Ctor_IsEmpty()
		{
			var collection = new CodeNamespaceCollection();
			Assert.Equal(0, collection.Count);
		}

		public static IEnumerable<object[]> AddRange_TestData()
		{
			yield return new object[] { new CodeNamespace[0] };
			yield return new object[] { new CodeNamespace[] { new CodeNamespace() } };
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor__CodeNamespaceArray_Works(CodeNamespace[] value)
		{
			var collection = new CodeNamespaceCollection(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor_CodeNamespaceCollection_Works(CodeNamespace[] value)
		{
			var collection = new CodeNamespaceCollection(new CodeNamespaceCollection(value));
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeNamespaceArray_Works(CodeNamespace[] value)
		{
			var collection = new CodeNamespaceCollection();
			collection.AddRange(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeNamespaceCollection_Works(CodeNamespace[] value)
		{
			var collection = new CodeNamespaceCollection();
			collection.AddRange(new CodeNamespaceCollection(value));
			VerifyCollection(collection, value);
		}

		[Fact]
		public void AddRange_Null_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeNamespaceCollection((CodeNamespace[])null));
			Assert.Throws<ArgumentNullException>("value", () => new CodeNamespaceCollection((CodeNamespaceCollection)null));

			var collection = new CodeNamespaceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeNamespace[])null));
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeNamespaceCollection)null));
		}

		[Fact]
		public void AddRange_NullObjectInValue_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeNamespaceCollection(new CodeNamespace[] { null }));

			var collection = new CodeNamespaceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(new CodeNamespace[] { null }));
		}

		[Fact]
		public void Add_Insert_Remove()
		{
			var collection = new CodeNamespaceCollection();

			var value1 = new CodeNamespace();
			Assert.Equal(0, collection.Add(value1));
			Assert.Equal(1, collection.Count);
			Assert.Equal(value1, collection[0]);

			var value2 = new CodeNamespace();
			collection.Insert(0, value2);
			Assert.Equal(2, collection.Count);
			Assert.Same(value2, collection[0]);

			collection.Remove(value1);
			Assert.Equal(1, collection.Count);

			collection.Remove(value2);
			Assert.Equal(0, collection.Count);
		}

		[Fact]
		public void Add_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeNamespaceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Add(null));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(1)]
		public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeNamespaceCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new CodeNamespace()));
		}

		[Fact]
		public void Insert_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeNamespaceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Insert(0, null));
		}

		[Fact]
		public void Remove_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeNamespaceCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Remove(null));
		}

		[Fact]
		public void Remove_NoSuchObject_ThrowsArgumentException()
		{
			var collection = new CodeNamespaceCollection();
			Assert.Throws<ArgumentException>(null, () => collection.Remove(new CodeNamespace()));
		}

		[Fact]
		public void Contains_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeNamespaceCollection();
			Assert.False(collection.Contains(null));
			Assert.False(collection.Contains(new CodeNamespace()));
		}

		[Fact]
		public void IndexOf_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeNamespaceCollection();
			Assert.Equal(-1, collection.IndexOf(null));
			Assert.Equal(-1, collection.IndexOf(new CodeNamespace()));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Item_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeNamespaceCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new CodeNamespace());
		}

		[Fact]
		public void ItemSet_Get_ReturnsExpected()
		{
			var value1 = new CodeNamespace();
			var value2 = new CodeNamespace();
			var collection = new CodeNamespaceCollection();
			collection.Add(value1);

			collection[0] = value2;
			Assert.Equal(1, collection.Count);
			Assert.Same(value2, collection[0]);
		}

		private static void VerifyCollection(CodeNamespaceCollection collection, CodeNamespace[] contents)
		{
			Assert.Equal(contents.Length, collection.Count);
			for (int i = 0; i < contents.Length; i++)
			{
				CodeNamespace content = contents[i];
				Assert.Equal(i, collection.IndexOf(content));
				Assert.True(collection.Contains(content));
				Assert.Same(content, collection[i]);
			}

			const int Index = 1;
			var copy = new CodeNamespace[collection.Count + Index];
			collection.CopyTo(copy, Index);
			Assert.Null(copy[0]);
			for (int i = Index; i < copy.Length; i++)
			{
				Assert.Same(contents[i - Index], copy[i]);
			}
		}
	}
}
