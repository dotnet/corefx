// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.CodeDom.Tests
{
	public class CodeAttributeArgumentCollectionTests
	{
		[Fact]
		public void Ctor_IsEmpty()
		{
			var collection = new CodeAttributeArgumentCollection();
			Assert.Equal(0, collection.Count);
		}

		public static IEnumerable<object[]> AddRange_TestData()
		{
			yield return new object[] { new CodeAttributeArgument[0] };
			yield return new object[] { new CodeAttributeArgument[] { new CodeAttributeArgument() } };
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor__CodeAttributeArgumentArray_Works(CodeAttributeArgument[] value)
		{
			var collection = new CodeAttributeArgumentCollection(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor_CodeAttributeArgumentCollection_Works(CodeAttributeArgument[] value)
		{
			var collection = new CodeAttributeArgumentCollection(new CodeAttributeArgumentCollection(value));
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeAttributeArgumentArray_Works(CodeAttributeArgument[] value)
		{
			var collection = new CodeAttributeArgumentCollection();
			collection.AddRange(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeAttributeArgumentCollection_Works(CodeAttributeArgument[] value)
		{
			var collection = new CodeAttributeArgumentCollection();
			collection.AddRange(new CodeAttributeArgumentCollection(value));
			VerifyCollection(collection, value);
		}

		[Fact]
		public void AddRange_Null_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeAttributeArgumentCollection((CodeAttributeArgument[])null));
			Assert.Throws<ArgumentNullException>("value", () => new CodeAttributeArgumentCollection((CodeAttributeArgumentCollection)null));

			var collection = new CodeAttributeArgumentCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeAttributeArgument[])null));
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeAttributeArgumentCollection)null));
		}

		[Fact]
		public void AddRange_NullObjectInValue_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeAttributeArgumentCollection(new CodeAttributeArgument[] { null }));

			var collection = new CodeAttributeArgumentCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(new CodeAttributeArgument[] { null }));
		}

		[Fact]
		public void Add_Insert_Remove()
		{
			var collection = new CodeAttributeArgumentCollection();

			var value1 = new CodeAttributeArgument();
			Assert.Equal(0, collection.Add(value1));
			Assert.Equal(1, collection.Count);
			Assert.Equal(value1, collection[0]);

			var value2 = new CodeAttributeArgument();
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
			var collection = new CodeAttributeArgumentCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Add(null));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(1)]
		public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeAttributeArgumentCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new CodeAttributeArgument()));
		}

		[Fact]
		public void Insert_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeAttributeArgumentCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Insert(0, null));
		}

		[Fact]
		public void Remove_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeAttributeArgumentCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Remove(null));
		}

		[Fact]
		public void Remove_NoSuchObject_ThrowsArgumentException()
		{
			var collection = new CodeAttributeArgumentCollection();
			Assert.Throws<ArgumentException>(null, () => collection.Remove(new CodeAttributeArgument()));
		}

		[Fact]
		public void Contains_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeAttributeArgumentCollection();
			Assert.False(collection.Contains(null));
			Assert.False(collection.Contains(new CodeAttributeArgument()));
		}

		[Fact]
		public void IndexOf_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeAttributeArgumentCollection();
			Assert.Equal(-1, collection.IndexOf(null));
			Assert.Equal(-1, collection.IndexOf(new CodeAttributeArgument()));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Item_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeAttributeArgumentCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new CodeAttributeArgument());
		}

		[Fact]
		public void ItemSet_Get_ReturnsExpected()
		{
			var value1 = new CodeAttributeArgument();
			var value2 = new CodeAttributeArgument();
			var collection = new CodeAttributeArgumentCollection();
			collection.Add(value1);

			collection[0] = value2;
			Assert.Equal(1, collection.Count);
			Assert.Same(value2, collection[0]);
		}

		private static void VerifyCollection(CodeAttributeArgumentCollection collection, CodeAttributeArgument[] contents)
		{
			Assert.Equal(contents.Length, collection.Count);
			for (int i = 0; i < contents.Length; i++)
			{
				CodeAttributeArgument content = contents[i];
				Assert.Equal(i, collection.IndexOf(content));
				Assert.True(collection.Contains(content));
				Assert.Same(content, collection[i]);
			}

			const int Index = 1;
			var copy = new CodeAttributeArgument[collection.Count + Index];
			collection.CopyTo(copy, Index);
			Assert.Null(copy[0]);
			for (int i = Index; i < copy.Length; i++)
			{
				Assert.Same(contents[i - Index], copy[i]);
			}
		}
	}
}
