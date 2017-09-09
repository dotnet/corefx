// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Data.Common
{
    public abstract class DbParameter : MarshalByRefObject, IDbDataParameter
    {
        protected DbParameter() : base() { }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.All)]
        public abstract DbType DbType { get; set; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract void ResetDbType();

        [DefaultValue(ParameterDirection.Input)]
        [RefreshProperties(RefreshProperties.All)]
        public abstract ParameterDirection Direction { get; set; }

        [Browsable(false)]
        [DesignOnly(true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract bool IsNullable { get; set; }

        [DefaultValue("")]
        public abstract string ParameterName { get; set; }

        // These properties pick up the implementation of IDbDataParameter.Precision and Scale
        // so that Db-agnostic code via IDbConnection actually works.
        public virtual byte Precision
        {
            get { return 0; }
            set { }
        }

        public virtual byte Scale
        {
            get { return 0; }
            set { }
        }

        public abstract int Size { get; set; }

        [DefaultValue("")]
        public abstract string SourceColumn { get; set; }

        [DefaultValue(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [RefreshProperties(RefreshProperties.All)]
        public abstract bool SourceColumnNullMapping { get; set; }

        [DefaultValue(DataRowVersion.Current)]
        public virtual DataRowVersion SourceVersion
        {
            get { return DataRowVersion.Default; }
            set { }
        }

        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.All)]
        public abstract object Value { get; set; }
    }
}
