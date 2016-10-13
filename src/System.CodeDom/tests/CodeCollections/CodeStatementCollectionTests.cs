// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.CodeDom.Tests
{
	public class CodeStatementCollectionTests
	{
		[Fact]
		public void Ctor_IsEmpty()
		{
			var collection = new CodeStatementCollection();
			Assert.Equal(0, collection.Count);
		}

		public static IEnumerable<object[]> AddRange_TestData()
		{
			yield return new object[] { new CodeStatement[0] };
			yield return new object[] { new CodeStatement[] { new CodeStatement() } };
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor__CodeStatementArray_Works(CodeStatement[] value)
		{
			var collection = new CodeStatementCollection(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor_CodeStatementCollection_Works(CodeStatement[] value)
		{
			var collection = new CodeStatementCollection(new CodeStatementCollection(value));
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeStatementArray_Works(CodeStatement[] value)
		{
			var collection = new CodeStatementCollection();
			collection.AddRange(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeStatementCollection_Works(CodeStatement[] value)
		{
			var collection = new CodeStatementCollection();
			collection.AddRange(new CodeStatementCollection(value));
			VerifyCollection(collection, value);
		}

		[Fact]
		public void AddRange_Null_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeStatementCollection((CodeStatement[])null));
			Assert.Throws<ArgumentNullException>("value", () => new CodeStatementCollection((CodeStatementCollection)null));

			var collection = new CodeStatementCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeStatement[])null));
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeStatementCollection)null));
		}

		[Fact]
		public void AddRange_NullObjectInValue_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeStatementCollection(new CodeStatement[] { null }));

			var collection = new CodeStatementCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(new CodeStatement[] { null }));
		}

		[Fact]
		public void Add_CodeStatement_Insert_Remove()
		{
			var collection = new CodeStatementCollection();

			var value1 = new CodeStatement();
			Assert.Equal(0, collection.Add(value1));
			Assert.Equal(1, collection.Count);
			Assert.Equal(value1, collection[0]);

			var value2 = new CodeStatement();
			collection.Insert(0, value2);
			Assert.Equal(2, collection.Count);
			Assert.Same(value2, collection[0]);

			collection.Remove(value1);
			Assert.Equal(1, collection.Count);

			collection.Remove(value2);
			Assert.Equal(0, collection.Count);
		}

		public static IEnumerable<object[]> Add_CodeExpression_TestData()
		{
			yield return new object[] { null };
			yield return new object[] { new CodePrimitiveExpression("Value") };
		}

		[Theory]
		[MemberData(nameof(Add_CodeExpression_TestData))]
		public void Add_CodeExpression(CodeExpression expression)
		{
			var collection = new CodeStatementCollection();
			Assert.Equal(0, collection.Add(expression));
			Assert.Equal(expression, ((CodeExpressionStatement)collection[0]).Expression);
		}

		[Fact]
		public void Add_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeStatementCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Add((CodeStatement)null));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(1)]
		public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeStatementCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new CodeStatement()));
		}

		[Fact]
		public void Insert_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeStatementCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Insert(0, null));
		}

		[Fact]
		public void Remove_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeStatementCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Remove(null));
		}

		[Fact]
		public void Remove_NoSuchObject_ThrowsArgumentException()
		{
			var collection = new CodeStatementCollection();
			Assert.Throws<ArgumentException>(null, () => collection.Remove(new CodeStatement()));
		}

		[Fact]
		public void Contains_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeStatementCollection();
			Assert.False(collection.Contains(null));
			Assert.False(collection.Contains(new CodeStatement()));
		}

		[Fact]
		public void IndexOf_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeStatementCollection();
			Assert.Equal(-1, collection.IndexOf(null));
			Assert.Equal(-1, collection.IndexOf(new CodeStatement()));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Item_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeStatementCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new CodeStatement());
		}

		[Fact]
		public void ItemSet_Get_ReturnsExpected()
		{
			var value1 = new CodeStatement();
			var value2 = new CodeStatement();
			var collection = new CodeStatementCollection();
			collection.Add(value1);

			collection[0] = value2;
			Assert.Equal(1, collection.Count);
			Assert.Same(value2, collection[0]);
		}

		private static void VerifyCollection(CodeStatementCollection collection, CodeStatement[] contents)
		{
			Assert.Equal(contents.Length, collection.Count);
			for (int i = 0; i < contents.Length; i++)
			{
				CodeStatement content = contents[i];
				Assert.Equal(i, collection.IndexOf(content));
				Assert.True(collection.Contains(content));
				Assert.Same(content, collection[i]);
			}

			const int Index = 1;
			var copy = new CodeStatement[collection.Count + Index];
			collection.CopyTo(copy, Index);
			Assert.Null(copy[0]);
			for (int i = Index; i < copy.Length; i++)
			{
				Assert.Same(contents[i - Index], copy[i]);
			}
		}
	}
}
