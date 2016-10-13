// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.CodeDom.Tests
{
	public class CodeTypeParameterCollectionTests
	{
		[Fact]
		public void Ctor_IsEmpty()
		{
			var collection = new CodeTypeParameterCollection();
			Assert.Equal(0, collection.Count);
		}

		public static IEnumerable<object[]> AddRange_TestData()
		{
			yield return new object[] { new CodeTypeParameter[0] };
			yield return new object[] { new CodeTypeParameter[] { new CodeTypeParameter() } };
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor__CodeTypeParameterArray_Works(CodeTypeParameter[] value)
		{
			var collection = new CodeTypeParameterCollection(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor_CodeTypeParameterCollection_Works(CodeTypeParameter[] value)
		{
			var collection = new CodeTypeParameterCollection(new CodeTypeParameterCollection(value));
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeTypeParameterArray_Works(CodeTypeParameter[] value)
		{
			var collection = new CodeTypeParameterCollection();
			collection.AddRange(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CodeTypeParameterCollection_Works(CodeTypeParameter[] value)
		{
			var collection = new CodeTypeParameterCollection();
			collection.AddRange(new CodeTypeParameterCollection(value));
			VerifyCollection(collection, value);
		}

		[Fact]
		public void AddRange_Null_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeTypeParameterCollection((CodeTypeParameter[])null));
			Assert.Throws<ArgumentNullException>("value", () => new CodeTypeParameterCollection((CodeTypeParameterCollection)null));

			var collection = new CodeTypeParameterCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeTypeParameter[])null));
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CodeTypeParameterCollection)null));
		}

		[Fact]
		public void AddRange_NullObjectInValue_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CodeTypeParameterCollection(new CodeTypeParameter[] { null }));

			var collection = new CodeTypeParameterCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(new CodeTypeParameter[] { null }));
		}

		[Fact]
		public void Add_CodeTypeParameter_Insert_Remove()
		{
			var collection = new CodeTypeParameterCollection();

			var value1 = new CodeTypeParameter();
			Assert.Equal(0, collection.Add(value1));
			Assert.Equal(1, collection.Count);
			Assert.Equal(value1, collection[0]);

			var value2 = new CodeTypeParameter();
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
		[InlineData("Name")]
		public void Add_String(string name)
		{
			var collection = new CodeTypeParameterCollection();
			collection.Add(name);
			Assert.Equal(new CodeTypeParameter(name).Name, collection[0].Name);
		}

		[Fact]
		public void Add_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeTypeParameterCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Add((CodeTypeParameter)null));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(1)]
		public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeTypeParameterCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new CodeTypeParameter()));
		}

		[Fact]
		public void Insert_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeTypeParameterCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Insert(0, null));
		}

		[Fact]
		public void Remove_Null_ThrowsArgumentNullException()
		{
			var collection = new CodeTypeParameterCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Remove(null));
		}

		[Fact]
		public void Remove_NoSuchObject_ThrowsArgumentException()
		{
			var collection = new CodeTypeParameterCollection();
			Assert.Throws<ArgumentException>(null, () => collection.Remove(new CodeTypeParameter()));
		}

		[Fact]
		public void Contains_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeTypeParameterCollection();
			Assert.False(collection.Contains(null));
			Assert.False(collection.Contains(new CodeTypeParameter()));
		}

		[Fact]
		public void IndexOf_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CodeTypeParameterCollection();
			Assert.Equal(-1, collection.IndexOf(null));
			Assert.Equal(-1, collection.IndexOf(new CodeTypeParameter()));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Item_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CodeTypeParameterCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new CodeTypeParameter());
		}

		[Fact]
		public void ItemSet_Get_ReturnsExpected()
		{
			var value1 = new CodeTypeParameter();
			var value2 = new CodeTypeParameter();
			var collection = new CodeTypeParameterCollection();
			collection.Add(value1);

			collection[0] = value2;
			Assert.Equal(1, collection.Count);
			Assert.Same(value2, collection[0]);
		}

		private static void VerifyCollection(CodeTypeParameterCollection collection, CodeTypeParameter[] contents)
		{
			Assert.Equal(contents.Length, collection.Count);
			for (int i = 0; i < contents.Length; i++)
			{
				CodeTypeParameter content = contents[i];
				Assert.Equal(i, collection.IndexOf(content));
				Assert.True(collection.Contains(content));
				Assert.Same(content, collection[i]);
			}

			const int Index = 1;
			var copy = new CodeTypeParameter[collection.Count + Index];
			collection.CopyTo(copy, Index);
			Assert.Null(copy[0]);
			for (int i = Index; i < copy.Length; i++)
			{
				Assert.Same(contents[i - Index], copy[i]);
			}
		}
	}
}
