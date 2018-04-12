using Xunit;

namespace System.Data.Common
{
    public class RowUpdatedEventArgsTest
    {
        [Fact]
        public void ConstructorFail()
        {
            try
            {
                new RowUpdatedEventArgs(null, null, (StatementType)666, null);
                Assert.False(true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.Equal(typeof(ArgumentOutOfRangeException), ex.GetType());
                Assert.Null(ex.InnerException);
                Assert.True(ex.Message.IndexOf(nameof(StatementType)) != -1);
                Assert.True(ex.Message.IndexOf("666") != -1);
                Assert.Equal(ex.ParamName, nameof(StatementType));
            }
        }

        [Fact]
        public void ConstructorPass()
        {
            var ev = new RowUpdatedEventArgs(null, null, 0, null);
            Assert.NotNull(ev);
            Assert.Null(ev.Row);
            Assert.Null(ev.Command);
            Assert.Null(ev.TableMapping);
            Assert.Equal(ev.StatementType, (StatementType)0);
            Assert.Equal(ev.RowCount, 0);
            Assert.Equal(ev.RecordsAffected, 0);

            var table = new DataTable();
            var ev2 = new RowUpdatedEventArgs(table.NewRow(), null, StatementType.Update, new DataTableMapping());
            Assert.NotNull(ev2);
            Assert.NotNull(ev2.Row);
            Assert.Equal(ev2.Row.Table, table);
            Assert.Equal(ev2.RowCount, 1);
            Assert.Null(ev2.Command);
            Assert.Equal(ev2.StatementType, StatementType.Update);
            Assert.NotNull(ev2.TableMapping);
        }

        [Fact]
        public void Status_set()
        {
            var ev = new RowUpdatedEventArgs(null, null, 0, null);
            try
            {
                ev.Status = (UpdateStatus)666;
                Assert.False(true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.Equal(typeof(ArgumentOutOfRangeException), ex.GetType());
                Assert.Null(ex.InnerException);
                Assert.True(ex.Message.IndexOf(nameof(UpdateStatus)) != -1);
                Assert.True(ex.Message.IndexOf("666") != -1);
                Assert.Equal(ex.ParamName, nameof(UpdateStatus));
            }
        }

        [Fact]
        public void CopyToRows()
        {
            var table = new DataTable();
            var ev = new RowUpdatedEventArgs(table.NewRow(), null, StatementType.Update, null);

            Assert.Throws<ArgumentNullException>(() => ev.CopyToRows(null));

            var newRows = new DataRow[2];

            ev.CopyToRows(newRows, 1);
            Assert.Null(newRows[0]);
            Assert.NotNull(newRows[1]);
            Assert.Equal(newRows[1].Table, table);

            Assert.Throws<IndexOutOfRangeException>(() => ev.CopyToRows(newRows, 2));
        }
    }
}
