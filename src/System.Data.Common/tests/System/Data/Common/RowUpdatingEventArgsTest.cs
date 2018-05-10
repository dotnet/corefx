﻿using Xunit;

namespace System.Data.Common
{
    public class RowUpdatingEventArgsTest
    {
        [Fact]
        public void Ctor_InvalidArgument_ThrowsArgumentException()
        {
            var table = new DataTable();
            AssertExtensions.Throws<ArgumentNullException>("dataRow", () => new RowUpdatingEventArgs(null, null, StatementType.Select, null));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(nameof(StatementType), () => new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Batch, new DataTableMapping()));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(nameof(StatementType), () => new RowUpdatingEventArgs(table.NewRow(), null, (StatementType)100, new DataTableMapping()));
            AssertExtensions.Throws<ArgumentNullException>("tableMapping", () => new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Select, null));
        }

        [Fact]
        public void Ctor_AssignsCorrectPropertyValues()
        {
            var table = new DataTable();
            var mapping = new DataTableMapping();
            var args = new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Insert, mapping);
            Assert.NotNull(args.Row);
            Assert.Equal(table, args.Row.Table);
            Assert.Null(args.Command);
            Assert.Equal(StatementType.Insert, args.StatementType);
            Assert.Equal(mapping, args.TableMapping);
        }

        [Fact]
        public void Status_SetInvalidValue_ThrowsArgumentOutOfRangeException()
        {
            var table = new DataTable();
            var args = new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Select, new DataTableMapping());
            AssertExtensions.Throws<ArgumentOutOfRangeException>(nameof(UpdateStatus), () => args.Status = (UpdateStatus)100);
        }
    }
}
