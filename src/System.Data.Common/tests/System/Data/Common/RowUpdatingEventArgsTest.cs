using Xunit;

namespace System.Data.Common
{
    public class RowUpdatingEventArgsTest
    {
        [Fact]
        public void ConstructorFail()
        {
            var table = new DataTable();
            try
            {
                new RowUpdatingEventArgs(null, null, StatementType.Select, null);
                Assert.False(true);
            }
            catch (ArgumentNullException ex)
            {
                Assert.Equal(typeof(ArgumentNullException), ex.GetType());
                Assert.Equal(ex.ParamName, "dataRow");
            }

            try
            {
                new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Batch, new DataTableMapping());
                Assert.False(true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.Equal(typeof(ArgumentOutOfRangeException), ex.GetType());
                Assert.Equal(ex.ParamName, nameof(StatementType));
            }

            try
            {
                new RowUpdatingEventArgs(table.NewRow(), null, (StatementType)666, new DataTableMapping());
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

            try
            {
                new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Select, null);
                Assert.False(true);
            }
            catch (ArgumentNullException ex)
            {
                Assert.Equal(typeof(ArgumentNullException), ex.GetType());
                Assert.Equal(ex.ParamName, "tableMapping");
            }
        }

        [Fact]
        public void ConstructorPass()
        {
            var table = new DataTable();
            var ev = new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Insert, new DataTableMapping());
            Assert.NotNull(ev);
            Assert.NotNull(ev.Row);
            Assert.Equal(ev.Row.Table, table);
            Assert.Null(ev.Command);
            Assert.Equal(ev.StatementType, StatementType.Insert);
            Assert.NotNull(ev.TableMapping);
        }

        [Fact]
        public void Status_set()
        {
            var table = new DataTable();
            var ev = new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Select, new DataTableMapping());
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
    }
}
