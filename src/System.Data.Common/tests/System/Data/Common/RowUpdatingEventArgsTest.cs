using Xunit;

namespace System.Data.Common
{
    public class RowUpdatingEventArgsTest
    {
        [Fact]
        public void ConstructorFail()
        {
            var table = new DataTable();
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new RowUpdatingEventArgs(null, null, StatementType.Select, null));
            Assert.Equal(typeof(ArgumentNullException), ex.GetType());
            Assert.Equal(ex.ParamName, "dataRow");

            ArgumentOutOfRangeException ex2 = Assert.Throws<ArgumentOutOfRangeException>(() => new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Batch, new DataTableMapping()));
            Assert.Equal(typeof(ArgumentOutOfRangeException), ex2.GetType());
            Assert.Equal(ex2.ParamName, nameof(StatementType));

            ArgumentOutOfRangeException ex3 = Assert.Throws<ArgumentOutOfRangeException>(() => new RowUpdatingEventArgs(table.NewRow(), null, (StatementType)100, new DataTableMapping()));
            Assert.Equal(typeof(ArgumentOutOfRangeException), ex3.GetType());
            Assert.Null(ex3.InnerException);
            Assert.True(ex3.Message.IndexOf(nameof(StatementType)) != -1);
            Assert.True(ex3.Message.IndexOf("100") != -1);
            Assert.Equal(ex3.ParamName, nameof(StatementType));

            ArgumentNullException ex4 = Assert.Throws<ArgumentNullException>(() => new RowUpdatingEventArgs(table.NewRow(), null, StatementType.Select, null));
            Assert.Equal(typeof(ArgumentNullException), ex4.GetType());
            Assert.Equal(ex4.ParamName, "tableMapping");
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
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => ev.Status = (UpdateStatus)100);
            Assert.Equal(typeof(ArgumentOutOfRangeException), ex.GetType());
            Assert.Null(ex.InnerException);
            Assert.True(ex.Message.IndexOf(nameof(UpdateStatus)) != -1);
            Assert.True(ex.Message.IndexOf("100") != -1);
            Assert.Equal(ex.ParamName, nameof(UpdateStatus));
        }
    }
}
