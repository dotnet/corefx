// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml.Schema
{
    /// <summary>An in-memory representation of an XML Schema, as specified in the World Wide Web Consortium (W3C) XML Schema Part 1: Structures and XML Schema Part 2: Datatypes specifications.</summary>
    public class XmlSchema
    {
        //Empty XmlSchema class to enable backward compatibility of interface method IXmlSerializable.GetSchema()        
        //Add private ctor to prevent constructing of this class
        private XmlSchema() { }
    }
}
