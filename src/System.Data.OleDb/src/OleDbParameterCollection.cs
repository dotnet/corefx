// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Data.OleDb {

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.ProviderBase;
    using System.Diagnostics;

    [
    //Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, " + AssemblyRef.MicrosoftVSDesigner, "System.Drawing.Design.UITypeEditor, " + AssemblyRef.SystemDrawing),
    ListBindable(false)
    ]
    public sealed partial class OleDbParameterCollection : DbParameterCollection {
        private int _changeID;

        private static Type ItemType = typeof(OleDbParameter);

        internal OleDbParameterCollection() : base() {
        }

        internal int ChangeID {
            get {
                return _changeID;
            }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        new public OleDbParameter this[int index] {
            get {
                return (OleDbParameter)GetParameter(index);
            }
            set {
                SetParameter(index, value);
            }
        }

        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        new public OleDbParameter this[string parameterName] {
            get {
                 return (OleDbParameter)GetParameter(parameterName);
            }
            set {
                 SetParameter(parameterName, value);
            }
        }

        public OleDbParameter Add(OleDbParameter value) { // MDAC 59206
            Add((object)value);
            return value;
        }

        [ EditorBrowsableAttribute(EditorBrowsableState.Never) ]
        [ ObsoleteAttribute("Add(String parameterName, Object value) has been deprecated.  Use AddWithValue(String parameterName, Object value).  http://go.microsoft.com/fwlink/?linkid=14202", false) ] // 79027
        public OleDbParameter Add(string parameterName, object value) { // MDAC 59206
            return Add(new OleDbParameter(parameterName, value));
        }

        public OleDbParameter AddWithValue(string parameterName, object value) { // MDAC 79027
            return Add(new OleDbParameter(parameterName, value));
        }

        public OleDbParameter Add(string parameterName, OleDbType oleDbType) {
            return Add(new OleDbParameter(parameterName, oleDbType));
        }

        public OleDbParameter Add(string parameterName, OleDbType oleDbType, int size) {
            return Add(new OleDbParameter(parameterName, oleDbType, size));
        }

        public OleDbParameter Add(string parameterName, OleDbType oleDbType, int size, string sourceColumn) {
            return Add(new OleDbParameter(parameterName, oleDbType, size, sourceColumn));
        }

        public void AddRange(OleDbParameter[] values) { // V1.2.3300
            AddRange((Array)values);
        }

        override public bool Contains(string value) { // WebData 97349
            return (-1 != IndexOf(value));
        }

        public bool Contains(OleDbParameter value) {
            return (-1 != IndexOf(value));
        }

        public void CopyTo(OleDbParameter[] array, int index) {
            CopyTo((Array)array, index);
        }
        
        public int IndexOf(OleDbParameter value) {
            return IndexOf((object)value);
        }
    
        public void Insert(int index, OleDbParameter value) {
            Insert(index, (object)value);
        }
        
        private void OnChange() {
            unchecked { _changeID++; }
        }
        
        public void Remove(OleDbParameter value) {
            Remove((object)value);
        }    

    }
}
