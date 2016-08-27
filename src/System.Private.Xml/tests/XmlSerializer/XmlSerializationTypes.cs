// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace SerializationTypes
{

    public class TypeWithSoapAttributes
    {
        [SoapAttribute(Namespace = "http://www.cpandl.com")]
        public string GroupName;

        [SoapAttribute(DataType = "base64Binary")]
        public byte[] GroupNumber;

        [SoapAttribute(DataType = "date", AttributeName = "CreationDate")]
        public DateTime Today;
        [SoapElement(DataType = "nonNegativeInteger", ElementName = "PosInt")]
        public string PostitiveInt;
        // This is ignored when serialized unless it is overridden.
        [SoapIgnore]
        public bool IgnoreThis;

        public GroupType Grouptype;

        [SoapInclude(typeof(Car))]
        public Vehicle myCar(string licNumber)
        {
            Vehicle v;
            if (licNumber == "")
            {
                v = new Car();
                v.licenseNumber = "!!!!!!";
            }
            else
            {
                v = new Car();
                v.licenseNumber = licNumber;
            }
            return v;
        }
    }

    public class Car : Vehicle
    {
    }

    public abstract class Vehicle
    {
        public string licenseNumber;
        public DateTime makeDate;
    }

    public enum GroupType
    {
        small,
        large
    }
}