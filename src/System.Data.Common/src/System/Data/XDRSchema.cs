// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using System.Data.Common;

namespace System.Data
{
    internal sealed class XDRSchema : XMLSchema
    {
        internal string _schemaName;
        internal string _schemaUri;
        internal XmlElement _schemaRoot;
        internal DataSet _ds;
        private static readonly char[] s_colonArray = new char[] { ':' };

        internal XDRSchema(DataSet ds, bool fInline)
        {
            _schemaUri = string.Empty;
            _schemaName = string.Empty;
            _schemaRoot = null;
            _ds = ds;
        }

        internal void LoadSchema(XmlElement schemaRoot, DataSet ds)
        {
            if (schemaRoot == null)
                return;

            _schemaRoot = schemaRoot;
            _ds = ds;
            _schemaName = schemaRoot.GetAttribute(Keywords.NAME);


            _schemaUri = string.Empty;
            Debug.Assert(FEqualIdentity(schemaRoot, Keywords.XDR_SCHEMA, Keywords.XDRNS), "Illegal node");

            // Get Locale and CaseSensitive properties

            if (_schemaName == null || _schemaName.Length == 0)
                _schemaName = "NewDataSet";

            ds.Namespace = _schemaUri;

            // Walk all the top level Element tags.  
            for (XmlNode n = schemaRoot.FirstChild; n != null; n = n.NextSibling)
            {
                if (!(n is XmlElement))
                    continue;

                XmlElement child = (XmlElement)n;

                if (FEqualIdentity(child, Keywords.XDR_ELEMENTTYPE, Keywords.XDRNS))
                {
                    HandleTable(child);
                }
            }

            _schemaName = XmlConvert.DecodeName(_schemaName);
            if (ds.Tables[_schemaName] == null)
                ds.DataSetName = _schemaName;
        }

        internal XmlElement FindTypeNode(XmlElement node)
        {
            string strType;
            XmlNode vn;
            XmlNode vnRoof;

            Debug.Assert(FEqualIdentity(node, Keywords.XDR_ELEMENT, Keywords.XDRNS) ||
                         FEqualIdentity(node, Keywords.XDR_SCHEMA, Keywords.XDRNS) ||
                         FEqualIdentity(node, Keywords.XDR_ATTRIBUTE, Keywords.XDRNS) ||
                         FEqualIdentity(node, Keywords.XDR_ELEMENTTYPE, Keywords.XDRNS),
                         "Invalid node type " + node.LocalName);

            if (FEqualIdentity(node, Keywords.XDR_ELEMENTTYPE, Keywords.XDRNS))
                return node;

            strType = node.GetAttribute(Keywords.TYPE);

            if (FEqualIdentity(node, Keywords.XDR_ELEMENT, Keywords.XDRNS) ||
                FEqualIdentity(node, Keywords.XDR_ATTRIBUTE, Keywords.XDRNS))
            {
                if (strType == null || strType.Length == 0)
                    return null;

                // Find an ELEMENTTYPE or ATTRIBUTETYPE with name=strType
                vn = node.OwnerDocument.FirstChild;
                vnRoof = node.OwnerDocument;

                while (vn != vnRoof)
                {
                    if ((FEqualIdentity(vn, Keywords.XDR_ELEMENTTYPE, Keywords.XDRNS) &&
                         FEqualIdentity(node, Keywords.XDR_ELEMENT, Keywords.XDRNS)) ||
                        (FEqualIdentity(vn, Keywords.XDR_ATTRIBUTETYPE, Keywords.XDRNS) &&
                         FEqualIdentity(node, Keywords.XDR_ATTRIBUTE, Keywords.XDRNS)))
                    {
                        if (vn is XmlElement && ((XmlElement)vn).GetAttribute(Keywords.NAME) == strType)
                            return (XmlElement)vn;
                    }

                    // Move vn node
                    if (vn.FirstChild != null)
                        vn = vn.FirstChild;
                    else if (vn.NextSibling != null)
                        vn = vn.NextSibling;
                    else
                    {
                        while (vn != vnRoof)
                        {
                            vn = vn.ParentNode;
                            if (vn.NextSibling != null)
                            {
                                vn = vn.NextSibling;
                                break;
                            }
                        }
                    }
                }

                return null;
            }

            return null;
        }

        internal bool IsTextOnlyContent(XmlElement node)
        {
            Debug.Assert(FEqualIdentity(node, Keywords.XDR_ELEMENTTYPE, Keywords.XDRNS), "Invalid node type " + node.LocalName);

            string value = node.GetAttribute(Keywords.CONTENT);
            if (value == null || value.Length == 0)
            {
                string type = node.GetAttribute(Keywords.DT_TYPE, Keywords.DTNS);
                return !string.IsNullOrEmpty(type);
            }

            if (value == Keywords.EMPTY || value == Keywords.ELTONLY || value == Keywords.ELEMENTONLY || value == Keywords.MIXED)
            {
                return false;
            }
            if (value == Keywords.TEXTONLY)
            {
                return true;
            }

            throw ExceptionBuilder.InvalidAttributeValue("content", value);
        }

        internal bool IsXDRField(XmlElement node, XmlElement typeNode)
        {
            int min = 1;
            int max = 1;

            if (!IsTextOnlyContent(typeNode))
                return false;

            for (XmlNode n = typeNode.FirstChild; n != null; n = n.NextSibling)
            {
                if (FEqualIdentity(n, Keywords.XDR_ELEMENT, Keywords.XDRNS) ||
                    FEqualIdentity(n, Keywords.XDR_ATTRIBUTE, Keywords.XDRNS))
                    return false;
            }

            if (FEqualIdentity(node, Keywords.XDR_ELEMENT, Keywords.XDRNS))
            {
                GetMinMax(node, ref min, ref max);
                if (max == -1 || max > 1)
                    return false;
            }

            return true;
        }

        internal DataTable HandleTable(XmlElement node)
        {
            XmlElement typeNode;

            Debug.Assert(FEqualIdentity(node, Keywords.XDR_ELEMENTTYPE, Keywords.XDRNS) ||
                         FEqualIdentity(node, Keywords.XDR_ELEMENT, Keywords.XDRNS), "Invalid node type");


            // Figure out if this really is a table.  If not, bail out.
            typeNode = FindTypeNode(node);

            string occurs = node.GetAttribute(Keywords.MINOCCURS);

            if (occurs != null && occurs.Length > 0)
                if ((Convert.ToInt32(occurs, CultureInfo.InvariantCulture) > 1) && (typeNode == null))
                {
                    return InstantiateSimpleTable(_ds, node);
                }

            occurs = node.GetAttribute(Keywords.MAXOCCURS);

            if (occurs != null && occurs.Length > 0)
                if (!string.Equals(occurs, "1", StringComparison.Ordinal) && (typeNode == null))
                {
                    return InstantiateSimpleTable(_ds, node);
                }


            if (typeNode == null)
                return null;

            if (IsXDRField(node, typeNode))
                return null;

            return InstantiateTable(_ds, node, typeNode);
        }

        private sealed class NameType : IComparable
        {
            public string name;
            public Type type;
            public NameType(string n, Type t)
            {
                name = n;
                type = t;
            }
            public int CompareTo(object obj) { return string.Compare(name, (string)obj, StringComparison.Ordinal); }
        };

        // XDR spec: http://www.ltg.ed.ac.uk/~ht/XMLData-Reduced.htm
        private static NameType[] s_mapNameTypeXdr = {
            new NameType("bin.base64"          , typeof(byte[])  ), /* XDR */ 
            new NameType("bin.hex"             , typeof(byte[])  ), /* XDR */ 
            new NameType("boolean"             , typeof(bool)    ), /* XDR */ 
            new NameType("byte"                , typeof(sbyte)   ), /* XDR */
            new NameType("char"                , typeof(char)    ), /* XDR */ 
            new NameType("date"                , typeof(DateTime)), /* XDR */ 
            new NameType("dateTime"            , typeof(DateTime)), /* XDR */ 
            new NameType("dateTime.tz"         , typeof(DateTime)), /* XDR */ 
            new NameType("entities"            , typeof(string)  ), /* XDR */ 
            new NameType("entity"              , typeof(string)  ), /* XDR */ 
            new NameType("enumeration"         , typeof(string)  ), /* XDR */ 
            new NameType("fixed.14.4"          , typeof(decimal) ), /* XDR */ 
            new NameType("float"               , typeof(double)  ), /* XDR */
            new NameType("i1"                  , typeof(sbyte)   ), /* XDR */ 
            new NameType("i2"                  , typeof(short)   ), /* XDR */ 
            new NameType("i4"                  , typeof(int)   ), /* XDR */ 
            new NameType("i8"                  , typeof(long)   ), /* XDR */         
            new NameType("id"                  , typeof(string)  ), /* XDR */ 
            new NameType("idref"               , typeof(string)  ), /* XDR */ 
            new NameType("idrefs"              , typeof(string)  ), /* XDR */ 
            new NameType("int"                 , typeof(int)   ), /* XDR */ 
            new NameType("nmtoken"             , typeof(string)  ), /* XDR */ 
            new NameType("nmtokens"            , typeof(string)  ), /* XDR */ 
            new NameType("notation"            , typeof(string)  ), /* XDR */ 
            new NameType("number"              , typeof(decimal) ), /* XDR */ 
            new NameType("r4"                  , typeof(float)  ), /* XDR */ 
            new NameType("r8"                  , typeof(double)  ), /* XDR */ 
            new NameType("string"              , typeof(string)  ), /* XDR */ 
            new NameType("time"                , typeof(DateTime)), /* XDR */ 
            new NameType("time.tz"             , typeof(DateTime)), /* XDR */ 
            new NameType("ui1"                 , typeof(byte)    ), /* XDR */ 
            new NameType("ui2"                 , typeof(ushort)  ), /* XDR */ 
            new NameType("ui4"                 , typeof(uint)  ), /* XDR */ 
            new NameType("ui8"                 , typeof(ulong)  ), /* XDR */ 
            new NameType("uri"                 , typeof(string)  ), /* XDR */ 
            new NameType("uuid"                , typeof(Guid)    ), /* XDR */
        };

        private static NameType FindNameType(string name)
        {
#if DEBUG
            for (int i = 1; i < s_mapNameTypeXdr.Length; ++i)
            {
                Debug.Assert((s_mapNameTypeXdr[i - 1].CompareTo(s_mapNameTypeXdr[i].name)) < 0, "incorrect sorting");
            }
#endif
            int index = Array.BinarySearch(s_mapNameTypeXdr, name);
            if (index < 0)
            {
#if DEBUG
                // Let's check that we realy don't have this name:
                foreach (NameType nt in s_mapNameTypeXdr)
                {
                    Debug.Assert(nt.name != name, "FindNameType('" + name + "') -- failed. Existed name not found");
                }
#endif
                throw ExceptionBuilder.UndefinedDatatype(name);
            }
            Debug.Assert(s_mapNameTypeXdr[index].name == name, "FindNameType('" + name + "') -- failed. Wrong name found");
            return s_mapNameTypeXdr[index];
        }

        private static NameType s_enumerationNameType = FindNameType("enumeration");

        private Type ParseDataType(string dt, string dtValues)
        {
            string strType = dt;
            string[] parts = dt.Split(s_colonArray);  // ":"

            if (parts.Length > 2)
            {
                throw ExceptionBuilder.InvalidAttributeValue("type", dt);
            }
            else if (parts.Length == 2)
            {
                // CONSIDER: check that we have valid prefix
                strType = parts[1];
            }

            NameType nt = FindNameType(strType);
            if (nt == s_enumerationNameType && (dtValues == null || dtValues.Length == 0))
                throw ExceptionBuilder.MissingAttribute("type", Keywords.DT_VALUES);
            return nt.type;
        }

        internal string GetInstanceName(XmlElement node)
        {
            string instanceName;

            if (FEqualIdentity(node, Keywords.XDR_ELEMENTTYPE, Keywords.XDRNS) ||
                FEqualIdentity(node, Keywords.XDR_ATTRIBUTETYPE, Keywords.XDRNS))
            {
                instanceName = node.GetAttribute(Keywords.NAME);
                if (instanceName == null || instanceName.Length == 0)
                {
                    throw ExceptionBuilder.MissingAttribute("Element", Keywords.NAME);
                }
            }
            else
            {
                instanceName = node.GetAttribute(Keywords.TYPE);
                if (instanceName == null || instanceName.Length == 0)
                    throw ExceptionBuilder.MissingAttribute("Element", Keywords.TYPE);
            }

            return instanceName;
        }

        internal void HandleColumn(XmlElement node, DataTable table)
        {
            Debug.Assert(FEqualIdentity(node, Keywords.XDR_ELEMENT, Keywords.XDRNS) ||
                         FEqualIdentity(node, Keywords.XDR_ATTRIBUTE, Keywords.XDRNS), "Illegal node type");

            string instanceName;
            string strName;
            Type type;
            string strType;
            string strValues;
            int minOccurs = 0;
            int maxOccurs = 1;
            string strDefault;
            DataColumn column;

            string strUse = node.GetAttribute(Keywords.USE);



            // Get the name
            if (node.Attributes.Count > 0)
            {
                string strRef = node.GetAttribute(Keywords.REF);

                if (strRef != null && strRef.Length > 0)
                    return; //skip ref nodes. B2 item

                strName = instanceName = GetInstanceName(node);
                column = table.Columns[instanceName, _schemaUri];
                if (column != null)
                {
                    if (column.ColumnMapping == MappingType.Attribute)
                    {
                        if (FEqualIdentity(node, Keywords.XDR_ATTRIBUTE, Keywords.XDRNS))
                            throw ExceptionBuilder.DuplicateDeclaration(strName);
                    }
                    else
                    {
                        if (FEqualIdentity(node, Keywords.XDR_ELEMENT, Keywords.XDRNS))
                        {
                            throw ExceptionBuilder.DuplicateDeclaration(strName);
                        }
                    }
                    instanceName = GenUniqueColumnName(strName, table);
                }
            }
            else
            {
                strName = instanceName = string.Empty;
            }

            // Now get the type
            XmlElement typeNode = FindTypeNode(node);

            SimpleType xsdType = null;

            if (typeNode == null)
            {
                strType = node.GetAttribute(Keywords.TYPE);
                throw ExceptionBuilder.UndefinedDatatype(strType);
            }

            strType = typeNode.GetAttribute(Keywords.DT_TYPE, Keywords.DTNS);
            strValues = typeNode.GetAttribute(Keywords.DT_VALUES, Keywords.DTNS);
            if (strType == null || strType.Length == 0)
            {
                strType = string.Empty;
                type = typeof(string);
            }
            else
            {
                type = ParseDataType(strType, strValues);
                // HACK: temp work around special types
                if (strType == "float")
                {
                    strType = string.Empty;
                }

                if (strType == "char")
                {
                    strType = string.Empty;
                    xsdType = SimpleType.CreateSimpleType(StorageType.Char, type);
                }


                if (strType == "enumeration")
                {
                    strType = string.Empty;
                    xsdType = SimpleType.CreateEnumeratedType(strValues);
                }

                if (strType == "bin.base64")
                {
                    strType = string.Empty;
                    xsdType = SimpleType.CreateByteArrayType("base64");
                }

                if (strType == "bin.hex")
                {
                    strType = string.Empty;
                    xsdType = SimpleType.CreateByteArrayType("hex");
                }
            }

            bool isAttribute = FEqualIdentity(node, Keywords.XDR_ATTRIBUTE, Keywords.XDRNS);

            GetMinMax(node, isAttribute, ref minOccurs, ref maxOccurs);

            strDefault = null;

            // Does XDR has default?
            strDefault = node.GetAttribute(Keywords.DEFAULT);


            bool bNullable = false;

            column = new DataColumn(XmlConvert.DecodeName(instanceName), type, null,
                isAttribute ? MappingType.Attribute : MappingType.Element);

            SetProperties(column, node.Attributes); // xmlschema.SetProperties will skipp setting expressions
            column.XmlDataType = strType;
            column.SimpleType = xsdType;
            column.AllowDBNull = (minOccurs == 0) || bNullable;
            column.Namespace = (isAttribute) ? string.Empty : _schemaUri;

            // We will skip handling expression columns in SetProperties, so we need set the expressions here
            if (node.Attributes != null)
            {
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    if (node.Attributes[i].NamespaceURI == Keywords.MSDNS)
                    {
                        if (node.Attributes[i].LocalName == "Expression")
                        {
                            column.Expression = node.Attributes[i].Value;
                            break;
                        }
                    }
                }
            }

            string targetNamespace = node.GetAttribute(Keywords.TARGETNAMESPACE);
            if (targetNamespace != null && targetNamespace.Length > 0)
                column.Namespace = targetNamespace;

            table.Columns.Add(column);
            if (strDefault != null && strDefault.Length != 0)
                try
                {
                    column.DefaultValue = SqlConvert.ChangeTypeForXML(strDefault, type);
                }
                catch (System.FormatException)
                {
                    throw ExceptionBuilder.CannotConvert(strDefault, type.FullName);
                }
        }

        internal void GetMinMax(XmlElement elNode, ref int minOccurs, ref int maxOccurs)
        {
            GetMinMax(elNode, false, ref minOccurs, ref maxOccurs);
        }

        internal void GetMinMax(XmlElement elNode, bool isAttribute, ref int minOccurs, ref int maxOccurs)
        {
            string occurs = elNode.GetAttribute(Keywords.MINOCCURS);
            if (occurs != null && occurs.Length > 0)
            {
                try
                {
                    minOccurs = int.Parse(occurs, CultureInfo.InvariantCulture);
                }
                catch (Exception e) when (ADP.IsCatchableExceptionType(e))
                {
                    throw ExceptionBuilder.AttributeValues(nameof(minOccurs), "0", "1");
                }
            }
            occurs = elNode.GetAttribute(Keywords.MAXOCCURS);

            if (occurs != null && occurs.Length > 0)
            {
                int bZeroOrMore = string.Compare(occurs, Keywords.STAR, StringComparison.Ordinal);
                if (bZeroOrMore == 0)
                {
                    maxOccurs = -1;
                }
                else
                {
                    try
                    {
                        maxOccurs = int.Parse(occurs, CultureInfo.InvariantCulture);
                    }
                    catch (Exception e) when (ADP.IsCatchableExceptionType(e))
                    {
                        throw ExceptionBuilder.AttributeValues(nameof(maxOccurs), "1", Keywords.STAR);
                    }
                    if (maxOccurs != 1)
                    {
                        throw ExceptionBuilder.AttributeValues(nameof(maxOccurs), "1", Keywords.STAR);
                    }
                }
            }
        }


        internal void HandleTypeNode(XmlElement typeNode, DataTable table, ArrayList tableChildren)
        {
            DataTable tableChild;

            for (XmlNode n = typeNode.FirstChild; n != null; n = n.NextSibling)
            {
                if (!(n is XmlElement))
                    continue;

                if (FEqualIdentity(n, Keywords.XDR_ELEMENT, Keywords.XDRNS))
                {
                    tableChild = HandleTable((XmlElement)n);
                    if (tableChild != null)
                    {
                        tableChildren.Add(tableChild);
                        continue;
                    }
                }

                if (FEqualIdentity(n, Keywords.XDR_ATTRIBUTE, Keywords.XDRNS) ||
                    FEqualIdentity(n, Keywords.XDR_ELEMENT, Keywords.XDRNS))
                {
                    HandleColumn((XmlElement)n, table);
                    continue;
                }
            }
        }

        internal DataTable InstantiateTable(DataSet dataSet, XmlElement node, XmlElement typeNode)
        {
            string typeName = string.Empty;
            XmlAttributeCollection attrs = node.Attributes;
            DataTable table;
            int minOccurs = 1;
            int maxOccurs = 1;
            string keys = null;
            ArrayList tableChildren = new ArrayList();



            if (attrs.Count > 0)
            {
                typeName = GetInstanceName(node);
                table = dataSet.Tables.GetTable(typeName, _schemaUri);
                if (table != null)
                {
                    return table;
                }
            }

            table = new DataTable(XmlConvert.DecodeName(typeName));
            // fxcop: new DataTable should inherit the CaseSensitive, Locale from DataSet and possibly updating during SetProperties

            table.Namespace = _schemaUri;

            GetMinMax(node, ref minOccurs, ref maxOccurs);
            table.MinOccurs = minOccurs;
            table.MaxOccurs = maxOccurs;

            _ds.Tables.Add(table);

            HandleTypeNode(typeNode, table, tableChildren);

            SetProperties(table, attrs);

            // check to see if we fave unique constraint

            if (keys != null)
            {
                string[] list = keys.TrimEnd(null).Split(null);
                int keyLength = list.Length;

                var cols = new DataColumn[keyLength];

                for (int i = 0; i < keyLength; i++)
                {
                    DataColumn col = table.Columns[list[i], _schemaUri];
                    if (col == null)
                        throw ExceptionBuilder.ElementTypeNotFound(list[i]);
                    cols[i] = col;
                }
                table.PrimaryKey = cols;
            }


            foreach (DataTable _tableChild in tableChildren)
            {
                DataRelation relation = null;

                DataRelationCollection childRelations = table.ChildRelations;

                for (int j = 0; j < childRelations.Count; j++)
                {
                    if (!childRelations[j].Nested)
                        continue;

                    if (_tableChild == childRelations[j].ChildTable)
                        relation = childRelations[j];
                }

                if (relation != null)
                    continue;

                DataColumn parentKey = table.AddUniqueKey();
                // foreign key in the child table
                DataColumn childKey = _tableChild.AddForeignKey(parentKey);
                // create relationship
                // setup relationship between parent and this table
                relation = new DataRelation(table.TableName + "_" + _tableChild.TableName, parentKey, childKey, true);

                relation.CheckMultipleNested = false; // disable the check for multiple nested parent

                relation.Nested = true;
                _tableChild.DataSet.Relations.Add(relation);
                relation.CheckMultipleNested = true; // enable the check for multiple nested parent
            }

            return table;
        }

        internal DataTable InstantiateSimpleTable(DataSet dataSet, XmlElement node)
        {
            string typeName;
            XmlAttributeCollection attrs = node.Attributes;
            DataTable table;
            int minOccurs = 1;
            int maxOccurs = 1;

            typeName = GetInstanceName(node);
            table = dataSet.Tables.GetTable(typeName, _schemaUri);
            if (table != null)
            {
                throw ExceptionBuilder.DuplicateDeclaration(typeName);
            }
            string tbName = XmlConvert.DecodeName(typeName);
            table = new DataTable(tbName);
            // fxcop: new DataTable will either inherit the CaseSensitive, Locale from DataSet or be set during SetProperties
            table.Namespace = _schemaUri;
            GetMinMax(node, ref minOccurs, ref maxOccurs);
            table.MinOccurs = minOccurs;
            table.MaxOccurs = maxOccurs;
            SetProperties(table, attrs);
            table._repeatableElement = true;

            HandleColumn(node, table);

            table.Columns[0].ColumnName = tbName + "_Column";
            _ds.Tables.Add(table);


            return table;
        }
    }
}
