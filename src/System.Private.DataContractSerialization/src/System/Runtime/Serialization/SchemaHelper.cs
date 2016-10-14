﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Collections;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
    using SchemaObjectDictionary = System.Collections.Generic.Dictionary<System.Xml.XmlQualifiedName, SchemaObjectInfo>;

    internal class SchemaObjectInfo
    {
        internal XmlSchemaType type;
        internal XmlSchemaElement element;
        internal XmlSchema schema;
        internal List<XmlSchemaType> knownTypes;

        internal SchemaObjectInfo(XmlSchemaType type, XmlSchemaElement element, XmlSchema schema, List<XmlSchemaType> knownTypes)
        {
            this.type = type;
            this.element = element;
            this.schema = schema;
            this.knownTypes = knownTypes;
        }
    }

    internal static class SchemaHelper
    {

        internal static bool NamespacesEqual(string ns1, string ns2)
        {
            if (ns1 == null || ns1.Length == 0)
                return (ns2 == null || ns2.Length == 0);
            else
                return ns1 == ns2;
        }

        internal static XmlSchemaType GetSchemaType(XmlSchemaSet schemas, XmlQualifiedName typeQName, out XmlSchema outSchema)
        {
            outSchema = null;
            ICollection currentSchemas = schemas.Schemas();
            string ns = typeQName.Namespace;
            foreach (XmlSchema schema in currentSchemas)
            {
                if (NamespacesEqual(ns, schema.TargetNamespace))
                {
                    outSchema = schema;
                    foreach (XmlSchemaObject schemaObj in schema.Items)
                    {
                        XmlSchemaType schemaType = schemaObj as XmlSchemaType;
                        if (schemaType != null && schemaType.Name == typeQName.Name)
                        {
                            return schemaType;
                        }
                    }
                }
            }
            return null;
        }

        internal static XmlSchemaType GetSchemaType(SchemaObjectDictionary schemaInfo, XmlQualifiedName typeName)
        {
            SchemaObjectInfo schemaObjectInfo;
            if (schemaInfo.TryGetValue(typeName, out schemaObjectInfo))
            {
                return schemaObjectInfo.type;
            }
            return null;
        }

        internal static XmlSchema GetSchemaWithType(SchemaObjectDictionary schemaInfo, XmlSchemaSet schemas, XmlQualifiedName typeName)
        {
            SchemaObjectInfo schemaObjectInfo;
            if (schemaInfo.TryGetValue(typeName, out schemaObjectInfo))
            {
                if (schemaObjectInfo.schema != null)
                    return schemaObjectInfo.schema;
            }
            ICollection currentSchemas = schemas.Schemas();
            string ns = typeName.Namespace;
            foreach (XmlSchema schema in currentSchemas)
            {
                if (NamespacesEqual(ns, schema.TargetNamespace))
                {
                    return schema;
                }
            }
            return null;
        }

        internal static XmlSchemaElement GetSchemaElement(XmlSchemaSet schemas, XmlQualifiedName elementQName, out XmlSchema outSchema)
        {
            outSchema = null;
            ICollection currentSchemas = schemas.Schemas();
            string ns = elementQName.Namespace;
            foreach (XmlSchema schema in currentSchemas)
            {
                if (NamespacesEqual(ns, schema.TargetNamespace))
                {
                    outSchema = schema;
                    foreach (XmlSchemaObject schemaObj in schema.Items)
                    {
                        XmlSchemaElement schemaElement = schemaObj as XmlSchemaElement;
                        if (schemaElement != null && schemaElement.Name == elementQName.Name)
                        {
                            return schemaElement;
                        }
                    }
                }
            }
            return null;
        }

        internal static XmlSchemaElement GetSchemaElement(SchemaObjectDictionary schemaInfo, XmlQualifiedName elementName)
        {
            SchemaObjectInfo schemaObjectInfo;
            if (schemaInfo.TryGetValue(elementName, out schemaObjectInfo))
            {
                return schemaObjectInfo.element;
            }
            return null;
        }

        internal static XmlSchema GetSchema(string ns, XmlSchemaSet schemas)
        {
            if (ns == null) { ns = String.Empty; }

            ICollection currentSchemas = schemas.Schemas();
            foreach (XmlSchema schema in currentSchemas)
            {
                if ((schema.TargetNamespace == null && ns.Length == 0) || ns.Equals(schema.TargetNamespace))
                {
                    return schema;
                }
            }
            return CreateSchema(ns, schemas);
        }

        static XmlSchema CreateSchema(string ns, XmlSchemaSet schemas)
        {
            XmlSchema schema = new XmlSchema();

            schema.ElementFormDefault = XmlSchemaForm.Qualified;
            if (ns.Length > 0)
            {
                schema.TargetNamespace = ns;
                schema.Namespaces.Add(Globals.TnsPrefix, ns);
            }


            schemas.Add(schema);
            return schema;
        }

        internal static void AddElementForm(XmlSchemaElement element, XmlSchema schema)
        {
            if (schema.ElementFormDefault != XmlSchemaForm.Qualified)
            {
                element.Form = XmlSchemaForm.Qualified;
            }
        }

        internal static void AddSchemaImport(string ns, XmlSchema schema)
        {
            if (SchemaHelper.NamespacesEqual(ns, schema.TargetNamespace) || SchemaHelper.NamespacesEqual(ns, Globals.SchemaNamespace) || SchemaHelper.NamespacesEqual(ns, Globals.SchemaInstanceNamespace))
                return;

            foreach (object item in schema.Includes)
            {
                if (item is XmlSchemaImport)
                {
                    if (SchemaHelper.NamespacesEqual(ns, ((XmlSchemaImport)item).Namespace))
                        return;
                }
            }

            XmlSchemaImport import = new XmlSchemaImport();
            if (ns != null && ns.Length > 0)
                import.Namespace = ns;
            schema.Includes.Add(import);
        }

        internal static XmlSchema GetSchemaWithGlobalElementDeclaration(XmlSchemaElement element, XmlSchemaSet schemas)
        {
            ICollection currentSchemas = schemas.Schemas();
            foreach (XmlSchema schema in currentSchemas)
            {
                foreach (XmlSchemaObject schemaObject in schema.Items)
                {
                    XmlSchemaElement schemaElement = schemaObject as XmlSchemaElement;
                    if (schemaElement == null)
                        continue;

                    if (schemaElement == element)
                    {
                        return schema;
                    }
                }
            }
            return null;
        }

        internal static XmlQualifiedName GetGlobalElementDeclaration(XmlSchemaSet schemas, XmlQualifiedName typeQName, out bool isNullable)
        {
            ICollection currentSchemas = schemas.Schemas();
            string ns = typeQName.Namespace;
            if (ns == null)
                ns = string.Empty;
            isNullable = false;
            foreach (XmlSchema schema in currentSchemas)
            {
                foreach (XmlSchemaObject schemaObject in schema.Items)
                {
                    XmlSchemaElement schemaElement = schemaObject as XmlSchemaElement;
                    if (schemaElement == null)
                        continue;

                    if (schemaElement.SchemaTypeName.Equals(typeQName))
                    {
                        isNullable = schemaElement.IsNillable;
                        return new XmlQualifiedName(schemaElement.Name, schema.TargetNamespace);
                    }
                }
            }
            return null;
        }

    }
}
