// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.CodeDom.Tests
{
	public class CodeCatchClauseCollectionTests
	{
		[Fact]
		public void Ctor_IsEmpty()
		{
			var collection = new CodeCatchClauseCollection();
			Assert.Equal(0, collection.Count);
		}

		public static IEnumerable<object[]> AddRange_TestData()
		{
			yield return new object[] { new CodeCatchClause[0] };
			yield return new object[] { new CodeCatchClause[] { new CodeCatchClause() } };
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor__CodeCatchClauseArray_Works(CodeCatchClause[] value)
		{
			var collection = new CodeCatchClauseCollection(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor_CodeCatchClauseCollection_Works(CodeCatchClause[] value)
		{
			var collection = new CodeCatchClauseCollection(new CodeCatchClauseCollection(value));
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeCatchClauseArray_Works(CodeCatchClause[] value)
		{
			var collection = new CodeCatchClauseCollection();
			collection.AddRange(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeCatchClauseCollection_Works(CodeCatchClause[] value)
		{
			var collection = new CodeCatchClauseCollection();
			collection.AddRange(new CodeCatchClauseCollection(value));
			VerifyCollection(collection, value);
		}

		[Fact]
		public void AddRange_Null_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeCatchClauseCollection((CodeCatchClause[])null));
			Assert.Throws<ArgumentNullException>("value", () => new CodeCatchClauseCollection((CodeCatchClauseCollection)null));

			var collection = new CodeCatchClauseCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeCatchClause[])null));
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeCatchClauseCollection)null));
		}

		[Fact]
		public void AddRange_NullObjectInValue_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeCatchClauseCollection(new CodeCatchClause[] { null }));

			var collection = new CodeCatchClauseCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(new CodeCatchClause[] { null }));
		}

		[Fact]
		public void Add_Insert_Remove()
		{
			var collection = new CodeCatchClauseCollection();

			var value1 = new CodeCatchClause();
			Assert.Equal(0, collection.Add(value1));
			Assert.Equal(1, collection.Count);
			Assert.Equal(value1, collection[0]);

			var value2 = new CodeCatchClause();
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
			var collection = new CodeCatchClauseCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Add(null));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(1)]
		public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeCatchClauseCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new CodeCatchClause()));
		}

		[Fact]
		public void Insert_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeCatchClauseCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Insert(0, null));
		}

		[Fact]
		public void Remove_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeCatchClauseCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Remove(null));
		}

		[Fact]
		public void Remove_NoSuchObject_ThrowsArgumentException()
		{
			var collection = new CodeCatchClauseCollection();
			Assert.Throws<ArgumentException>(null, () => collection.Remove(new CodeCatchClause()));
		}

		[Fact]
		public void Contains_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeCatchClauseCollection();
			Assert.False(collection.Contains(null));
			Assert.False(collection.Contains(new CodeCatchClause()));
		}

		[Fact]
		public void IndexOf_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeCatchClauseCollection();
			Assert.Equal(-1, collection.IndexOf(null));
			Assert.Equal(-1, collection.IndexOf(new CodeCatchClause()));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Item_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeCatchClauseCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new CodeCatchClause());
		}

		[Fact]
		public void ItemSet_Get_ReturnsExpected()
		{
			var value1 = new CodeCatchClause();
			var value2 = new CodeCatchClause();
			var collection = new CodeCatchClauseCollection();
			collection.Add(value1);

			collection[0] = value2;
			Assert.Equal(1, collection.Count);
			Assert.Same(value2, collection[0]);
		}

		private static void VerifyCollection(CodeCatchClauseCollection collection, CodeCatchClause[] contents)
		{
			Assert.Equal(contents.Length, collection.Count);
			for (int i = 0; i < contents.Length; i++)
			{
				CodeCatchClause content = contents[i];
				Assert.Equal(i, collection.IndexOf(content));
				Assert.True(collection.Contains(content));
				Assert.Same(content, collection[i]);
			}

			const int Index = 1;
			var copy = new CodeCatchClause[collection.Count + Index];
			collection.CopyTo(copy, Index);
			Assert.Null(copy[0]);
			for (int i = Index; i < copy.Length; i++)
			{
				Assert.Same(contents[i - Index], copy[i]);
			}
		}
	}
}
