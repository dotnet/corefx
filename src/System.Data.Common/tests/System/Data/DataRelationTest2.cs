// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

// Copyright (c) 2004 Mainsoft Co.
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Xunit;
//using System.ComponentModel;


namespace System.Data.Tests
{
    public class DataRelationTest2
    {
        [Fact]
        public void ChildColumns()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            // ChildColumns 1
            Assert.Single(dRel.ChildColumns);

            // ChildColumns 2
            Assert.Equal(dtChild.Columns[0], dRel.ChildColumns[0]);
        }

        [Fact]
        public void ChildKeyConstraint()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            // ChildKeyConstraint 1
            Assert.Equal(dtChild.Constraints[0], dRel.ChildKeyConstraint);
        }

        [Fact]
        public void ChildTable()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            // ChildTable
            Assert.Equal(dtChild, dRel.ChildTable);
        }

        [Fact]
        public void DataSet()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            // DataSet
            Assert.Equal(ds, dRel.DataSet);
        }

        [Fact]
        public void ParentColumns()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            // ParentColumns 1
            Assert.Single(dRel.ParentColumns);

            // ParentColumns 2
            Assert.Equal(dtParent.Columns[0], dRel.ParentColumns[0]);
        }

        [Fact]
        public void ParentKeyConstraint()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            // ChildKeyConstraint 1
            Assert.Equal(dtParent.Constraints[0], dRel.ParentKeyConstraint);
        }

        [Fact]
        public void ParentTable()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            // ParentTable
            Assert.Equal(dtParent, dRel.ParentTable);
        }

        [Fact]
        public new void ToString()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation(null, dtParent.Columns[0], dtChild.Columns[0]);

            // ToString 1
            Assert.Equal(string.Empty, dRel.ToString());

            ds.Relations.Add(dRel);

            // ToString 2
            Assert.Equal("Relation1", dRel.ToString());

            dRel.RelationName = "myRelation";

            // ToString 3
            Assert.Equal("myRelation", dRel.ToString());
        }

        [Fact]
        public void ctor_ByNameDataColumns()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            // DataRelation - CTor
            Assert.False(dRel == null);

            // DataRelation - parent Constraints
            Assert.Single(dtParent.Constraints);

            // DataRelation - child Constraints
            Assert.Single(dtChild.Constraints);

            // DataRelation - child relations
            Assert.Equal(dRel, dtParent.ChildRelations[0]);

            // DataRelation - parent relations
            Assert.Equal(dRel, dtChild.ParentRelations[0]);

            // DataRelation - name
            Assert.Equal("MyRelation", dRel.RelationName);

            // DataRelation - parent UniqueConstraint
            Assert.Equal(typeof(UniqueConstraint), dtParent.Constraints[0].GetType());

            // DataRelation - Child ForeignKeyConstraint
            Assert.Equal(typeof(ForeignKeyConstraint), dtChild.Constraints[0].GetType());

            ds.Relations.Clear();
            // Remove DataRelation - Parent Constraints
            Assert.Single(dtParent.Constraints);

            // Remove DataRelation - Child Constraints
            Assert.Single(dtChild.Constraints);

            // Remove DataRelation - child relations
            Assert.Empty(dtParent.ChildRelations);

            // Remove DataRelation - parent relations
            Assert.Empty(dtChild.ParentRelations);

            //add relation which will create invalid constraint
            dtChild.Constraints.Clear();
            dtParent.Constraints.Clear();
            //add duplicated row
            dtParent.Rows.Add(dtParent.Rows[0].ItemArray);
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);

            // Add relation which will create invalid constraint
            AssertExtensions.Throws<ArgumentException>(null, () =>
            {
                ds.Relations.Add(dRel);
            });
        }

        [Fact]
        public void ctor_ByNameDataColumnsCreateConstraints()
        {
            DataRelation dRel;
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();

            var ds = new DataSet();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            //parameter createConstraints = true

            bool createConstraints = true;
            for (int i = 0; i <= 1; i++)
            {
                if (i == 0)
                    createConstraints = false;
                else
                    createConstraints = true;

                ds.Relations.Clear();
                dtParent.Constraints.Clear();
                dtChild.Constraints.Clear();

                //add duplicated row
                dtParent.Rows.Add(dtParent.Rows[0].ItemArray);
                dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0], createConstraints);
                // Add relation which will create invalid constraint
                if (createConstraints == true)
                {
                    AssertExtensions.Throws<ArgumentException>(null, () =>
                    {
                        ds.Relations.Add(dRel);
                    });
                }
                else
                    ds.Relations.Add(dRel);

                dtParent.Rows.Remove(dtParent.Rows[dtParent.Rows.Count - 1]);
                ds.Relations.Clear();
                dtParent.Constraints.Clear();
                dtChild.Constraints.Clear();
                dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0], createConstraints);
                ds.Relations.Add(dRel);

                // DataRelation - CTor,createConstraints=
                Assert.False(dRel == null);

                // DataRelation - parent Constraints,createConstraints=
                Assert.Equal(i, dtParent.Constraints.Count);

                // DataRelation - child Constraints,createConstraints=
                Assert.Equal(i, dtChild.Constraints.Count);

                // DataRelation - child relations,createConstraints=
                Assert.Equal(dRel, dtParent.ChildRelations[0]);

                // DataRelation - parent relations,createConstraints=
                Assert.Equal(dRel, dtChild.ParentRelations[0]);

                // DataRelation - name
                Assert.Equal("MyRelation", dRel.RelationName);
            }
        }

        [Fact]
        public void ctor_ByNameDataColumnsArrays()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;

            //check some exception 
            // DataRelation - CTor ArgumentException, two columns child
            AssertExtensions.Throws<ArgumentException>(null, () =>
            {
                dRel = new DataRelation("MyRelation", new DataColumn[] { dtParent.Columns[0] }, new DataColumn[] { dtChild.Columns[0], dtChild.Columns[2] });
            });

            dRel = new DataRelation("MyRelation", new DataColumn[] { dtParent.Columns[0], dtParent.Columns[1] }, new DataColumn[] { dtChild.Columns[0], dtChild.Columns[2] });
            // DataRelation - Add Relation ArgumentException, fail on creating child Constraints
            AssertExtensions.Throws<ArgumentException>(null, () =>
            {
                ds.Relations.Add(dRel);
            });

            // DataRelation ArgumentException - parent Constraints
            Assert.Single(dtParent.Constraints);

            // DataRelation ArgumentException - child Constraints
            Assert.Empty(dtChild.Constraints);

            // DataRelation ArgumentException - DataSet.Relation count
            Assert.Single(ds.Relations);

            //begin to check the relation ctor
            dtParent.Constraints.Clear();
            dtChild.Constraints.Clear();
            ds.Relations.Clear();
            dRel = new DataRelation("MyRelation", new DataColumn[] { dtParent.Columns[0] }, new DataColumn[] { dtChild.Columns[0] });
            ds.Relations.Add(dRel);

            // DataSet DataRelation count
            Assert.Single(ds.Relations);

            // DataRelation - CTor
            Assert.False(dRel == null);

            // DataRelation - parent Constraints
            Assert.Single(dtParent.Constraints);

            // DataRelation - child Constraints
            Assert.Single(dtChild.Constraints);

            // DataRelation - child relations
            Assert.Equal(dRel, dtParent.ChildRelations[0]);

            // DataRelation - parent relations
            Assert.Equal(dRel, dtChild.ParentRelations[0]);

            // DataRelation - name
            Assert.Equal("MyRelation", dRel.RelationName);
        }

        [Fact]
        public void ctor_ByNameDataColumnsArraysCreateConstraints()
        {
            DataRelation dRel;
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();

            var ds = new DataSet();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            //parameter createConstraints = true

            bool createConstraints = true;
            for (int i = 0; i <= 1; i++)
            {
                if (i == 0)
                    createConstraints = false;
                else
                    createConstraints = true;

                ds.Relations.Clear();
                dtParent.Constraints.Clear();
                dtChild.Constraints.Clear();

                //add duplicated row
                dtParent.Rows.Add(dtParent.Rows[0].ItemArray);
                dRel = new DataRelation("MyRelation", new DataColumn[] { dtParent.Columns[0] }, new DataColumn[] { dtChild.Columns[0] }, createConstraints);
                // Add relation which will create invalid constraint
                if (createConstraints == true)
                {
                    AssertExtensions.Throws<ArgumentException>(null, () => ds.Relations.Add(dRel));
                }
                else
                    ds.Relations.Add(dRel);

                ds.Relations.Clear();
                dtParent.Constraints.Clear();
                dtChild.Constraints.Clear();
                dtParent.Rows.Remove(dtParent.Rows[dtParent.Rows.Count - 1]);

                dRel = new DataRelation("MyRelation", new DataColumn[] { dtParent.Columns[0] }, new DataColumn[] { dtChild.Columns[0] }, createConstraints);
                ds.Relations.Add(dRel);

                // DataRelation - CTor,createConstraints=
                Assert.False(dRel == null);

                // DataRelation - parent Constraints,createConstraints=
                Assert.Equal(i, dtParent.Constraints.Count);

                // DataRelation - child Constraints,createConstraints=
                Assert.Equal(i, dtChild.Constraints.Count);

                // DataRelation - child relations,createConstraints=
                Assert.Equal(dRel, dtParent.ChildRelations[0]);

                // DataRelation - parent relations,createConstraints=
                Assert.Equal(dRel, dtChild.ParentRelations[0]);

                // DataRelation - name
                Assert.Equal("MyRelation", dRel.RelationName);
            }
        }

        [Fact]
        public void extendedProperties()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation("MyRelation", dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            PropertyCollection pc;
            pc = dRel.ExtendedProperties;

            // Checking ExtendedProperties default 
            Assert.True(pc != null);

            // Checking ExtendedProperties count 
            Assert.Empty(pc);
        }

        [Fact]
        public void nested()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation(null, dtParent.Columns[0], dtChild.Columns[0]);
            ds.Relations.Add(dRel);

            // Nested default 
            Assert.False(dRel.Nested);

            dRel.Nested = true;

            // Nested get/set
            Assert.True(dRel.Nested);
        }

        [Fact]
        public void relationName()
        {
            var ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            DataRelation dRel;
            dRel = new DataRelation(null, dtParent.Columns[0], dtChild.Columns[0]);

            // RelationName default 1
            Assert.Equal(string.Empty, dRel.RelationName);

            ds.Relations.Add(dRel);

            // RelationName default 2
            Assert.Equal("Relation1", dRel.RelationName);

            dRel.RelationName = "myRelation";

            // RelationName get/set
            Assert.Equal("myRelation", dRel.RelationName);
        }

        [Fact]
        public void bug79233()
        {
            DataSet ds = new DataSet();
            DataTable dtChild = DataProvider.CreateChildDataTable();
            DataTable dtParent = DataProvider.CreateParentDataTable();
            ds.Tables.Add(dtParent);
            ds.Tables.Add(dtChild);

            dtParent.Rows.Clear();
            dtChild.Rows.Clear();

            DataRelation dr = dtParent.ChildRelations.Add(dtParent.Columns[0], dtChild.Columns[0]);
            Assert.Equal("Relation1", dr.RelationName);
            dr = dtChild.ChildRelations.Add(dtChild.Columns[0], dtParent.Columns[0]);
            Assert.Equal("Relation2", dr.RelationName);
        }

        [Fact]
        public void DataRelationTest()
        {
            var ds = new DataSet();

            DataTable t1 = new DataTable("t1");
            t1.Columns.Add("col", typeof(DateTime));

            DataTable t2 = new DataTable("t2");
            t2.Columns.Add("col", typeof(DateTime));
            t2.Columns[0].DateTimeMode = DataSetDateTime.Unspecified;

            ds.Tables.Add(t1);
            ds.Tables.Add(t2);
            ds.Relations.Add("rel", t1.Columns[0], t2.Columns[0], false);

            ds.Relations.Clear();
            t2.Columns[0].DateTimeMode = DataSetDateTime.Local;

            try
            {
                ds.Relations.Add("rel", t1.Columns[0], t2.Columns[0], false);
                Assert.False(true);
            }
            catch (InvalidConstraintException) { }
        }
    }
}
