// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom.Compiler;
using System.Collections.Generic;
using Xunit;

namespace System.CodeDom.Tests
{
	public class CompilerErrorCollectionTests
	{
		[Fact]
		public void Ctor_IsEmpty()
		{
			var collection = new CompilerErrorCollection();
			Assert.Equal(0, collection.Count);
		}

		public static IEnumerable<object[]> AddRange_TestData()
		{
			yield return new object[] { new CompilerError[0] };
			yield return new object[] { new CompilerError[] { new CompilerError() } };
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor__CompilerErrorArray_Works(CompilerError[] value)
		{
			var collection = new CompilerErrorCollection(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void Ctor_CompilerErrorCollection_Works(CompilerError[] value)
		{
			var collection = new CompilerErrorCollection(new CompilerErrorCollection(value));
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CompilerErrorArray_Works(CompilerError[] value)
		{
			var collection = new CompilerErrorCollection();
			collection.AddRange(value);
			VerifyCollection(collection, value);
		}

		[Theory]
		[MemberData(nameof(AddRange_TestData))]
		public void AddRange_CompilerErrorCollection_Works(CompilerError[] value)
		{
			var collection = new CompilerErrorCollection();
			collection.AddRange(new CompilerErrorCollection(value));
			VerifyCollection(collection, value);
		}

		[Fact]
		public void AddRange_Null_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CompilerErrorCollection((CompilerError[])null));
			Assert.Throws<ArgumentNullException>("value", () => new CompilerErrorCollection((CompilerErrorCollection)null));

			var collection = new CompilerErrorCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CompilerError[])null));
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((CompilerErrorCollection)null));
		}

		[Fact]
		public void AddRange_NullObjectInValue_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("value", () => new CompilerErrorCollection(new CompilerError[] { null }));

			var collection = new CompilerErrorCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(new CompilerError[] { null }));
		}

		[Fact]
		public void Add_Insert_Remove()
		{
			var collection = new CompilerErrorCollection();

			var value1 = new CompilerError();
			Assert.Equal(0, collection.Add(value1));
			Assert.Equal(1, collection.Count);
			Assert.Equal(value1, collection[0]);

			var value2 = new CompilerError();
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
			var collection = new CompilerErrorCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Add(null));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(1)]
		public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CompilerErrorCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new CompilerError()));
		}

		[Fact]
		public void Insert_Null_ThrowsArgumentNullException()
		{
			var collection = new CompilerErrorCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Insert(0, null));
		}

		[Fact]
		public void Remove_Null_ThrowsArgumentNullException()
		{
			var collection = new CompilerErrorCollection();
			Assert.Throws<ArgumentNullException>("value", () => collection.Remove(null));
		}

		[Fact]
		public void Remove_NoSuchObject_ThrowsArgumentException()
		{
			var collection = new CompilerErrorCollection();
			Assert.Throws<ArgumentException>(null, () => collection.Remove(new CompilerError()));
		}

		[Fact]
		public void Contains_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CompilerErrorCollection();
			Assert.False(collection.Contains(null));
			Assert.False(collection.Contains(new CompilerError()));
		}

		[Fact]
		public void IndexOf_NoSuchObject_ReturnsMinusOne()
		{
			var collection = new CompilerErrorCollection();
			Assert.Equal(-1, collection.IndexOf(null));
			Assert.Equal(-1, collection.IndexOf(new CompilerError()));
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Item_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
		{
			var collection = new CompilerErrorCollection();
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
			Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new CompilerError());
		}

		[Fact]
		public void ItemSet_Get_ReturnsExpected()
		{
			var value1 = new CompilerError();
			var value2 = new CompilerError();
			var collection = new CompilerErrorCollection();
			collection.Add(value1);

			collection[0] = value2;
			Assert.Equal(1, collection.Count);
			Assert.Same(value2, collection[0]);
		}

		[Fact]
		public void HasWarnings_Empty_ReturnsFalse()
		{
			var collection = new CompilerErrorCollection();
			Assert.False(collection.HasWarnings);
		}

		[Fact]
		public void HasWarnings_OnlyErrors_ReturnsFalse()
		{
			var collection = new CompilerErrorCollection();
			collection.Add(new CompilerError() { IsWarning = false });
			Assert.False(collection.HasWarnings);
		}

		[Fact]
		public void HasWarnings_OnlyWarnings_ReturnsTrue()
		{
			var collection = new CompilerErrorCollection();
			collection.Add(new CompilerError() { IsWarning = true });
			Assert.True(collection.HasWarnings);
		}

		[Fact]
		public void HasWarnings_WarningsAndErrors_ReturnsTrue()
		{
			var collection = new CompilerErrorCollection();
			collection.Add(new CompilerError() { IsWarning = false });
			collection.Add(new CompilerError() { IsWarning = true });
			Assert.True(collection.HasWarnings);
		}

		[Fact]
		public void HasErrors_Empty_ReturnsFalse()
		{
			var collection = new CompilerErrorCollection();
			Assert.False(collection.HasErrors);
		}

		[Fact]
		public void HasErrors_OnlyErrors_ReturnsTrue()
		{
			var collection = new CompilerErrorCollection();
			collection.Add(new CompilerError() { IsWarning = false });
			Assert.True(collection.HasErrors);
		}

		[Fact]
		public void HasErrors_OnlyWarnings_ReturnsFalse()
		{
			var collection = new CompilerErrorCollection();
			collection.Add(new CompilerError() { IsWarning = true });
			Assert.False(collection.HasErrors);
		}

		[Fact]
		public void HasErrors_WarningsAndErrors_ReturnsTrue()
		{
			var collection = new CompilerErrorCollection();
			collection.Add(new CompilerError() { IsWarning = false });
			collection.Add(new CompilerError() { IsWarning = true });
			Assert.True(collection.HasErrors);
		}

		private static void VerifyCollection(CompilerErrorCollection collection, CompilerError[] contents)
		{
			Assert.Equal(contents.Length, collection.Count);
			for (int i = 0; i < contents.Length; i++)
			{
				CompilerError content = contents[i];
				Assert.Equal(i, collection.IndexOf(content));
				Assert.True(collection.Contains(content));
				Assert.Same(content, collection[i]);
			}

			const int Index = 1;
			var copy = new CompilerError[collection.Count + Index];
			collection.CopyTo(copy, Index);
			Assert.Null(copy[0]);
			for (int i = Index; i < copy.Length; i++)
			{
				Assert.Same(contents[i - Index], copy[i]);
			}
		}
	}
}
