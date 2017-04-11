using System;
using System.Collections.Generic;
using System.Xml;

namespace DesktopTestData
{
    public class PrimitiveDataContract : DataContract
    {
        static Dictionary<Type, PrimitiveDataContract> typeToContract = new Dictionary<Type, PrimitiveDataContract>();
        static Dictionary<XmlQualifiedName, PrimitiveDataContract> nameToContract = new Dictionary<XmlQualifiedName, PrimitiveDataContract>();
        static PrimitiveDataContract objectContract = new PrimitiveDataContract(typeof(object));

        static PrimitiveDataContract()
        {
            Add(new PrimitiveDataContract(typeof(char)));
            Add(new PrimitiveDataContract(typeof(bool)));
            Add(new PrimitiveDataContract(typeof(sbyte)));
            Add(new PrimitiveDataContract(typeof(byte)));
            Add(new PrimitiveDataContract(typeof(short)));
            Add(new PrimitiveDataContract(typeof(ushort)));
            Add(new PrimitiveDataContract(typeof(int)));
            Add(new PrimitiveDataContract(typeof(uint)));
            Add(new PrimitiveDataContract(typeof(long)));
            Add(new PrimitiveDataContract(typeof(ulong)));
            Add(new PrimitiveDataContract(typeof(float)));
            Add(new PrimitiveDataContract(typeof(double)));
            Add(new PrimitiveDataContract(typeof(decimal)));
            Add(new PrimitiveDataContract(typeof(DateTime)));
            Add(new PrimitiveDataContract(typeof(string)));
            Add(new PrimitiveDataContract(typeof(byte[])));
            Add(new PrimitiveDataContract(typeof(TimeSpan)));
            Add(new PrimitiveDataContract(typeof(Guid)));
            Add(new PrimitiveDataContract(typeof(Uri)));
            Add(objectContract);
        }
        static public void Add(PrimitiveDataContract primitiveContract)
        {
            typeToContract.Add(primitiveContract.UnderlyingType, primitiveContract);
            nameToContract.Add(primitiveContract.StableName, primitiveContract);
        }

        static public PrimitiveDataContract GetPrimitiveDataContract(Type type)
        {
            PrimitiveDataContract retVal = null;
            if (type.IsInterface)
                retVal = objectContract;
            else
                typeToContract.TryGetValue(type, out retVal);
            return retVal;
        }

        static public PrimitiveDataContract GetPrimitiveDataContract(string name, string ns)
        {
            PrimitiveDataContract retVal = null;
            nameToContract.TryGetValue(new XmlQualifiedName(name, ns), out retVal);
            return retVal;
        }

        PrimitiveDataContract(Type type) : base(type)
        {
            string name = null;
            string ns = Globals.SchemaNamespace;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                    name = "char";
                    ns = Globals.SerializationNamespace;
                    break;
                case TypeCode.Boolean:
                    name = "boolean";
                    break;
                case TypeCode.SByte:
                    name = "byte";
                    break;
                case TypeCode.Byte:
                    name = "unsignedByte";
                    break;
                case TypeCode.Int16:
                    name = "short";
                    break;
                case TypeCode.UInt16:
                    name = "unsignedShort";
                    break;
                case TypeCode.Int32:
                    name = "int";
                    break;
                case TypeCode.UInt32:
                    name = "unsignedInt";
                    break;
                case TypeCode.Int64:
                    name = "long";
                    break;
                case TypeCode.UInt64:
                    name = "unsignedLong";
                    break;
                case TypeCode.Single:
                    name = "float";
                    break;
                case TypeCode.Double:
                    name = "double";
                    break;
                case TypeCode.Decimal:
                    name = "decimal";
                    break;
                case TypeCode.DateTime:
                    name = "dateTime";
                    break;
                case TypeCode.String:
                    name = "string";
                    break;
                default:
                    if (type == Globals.TypeOfByteArray)
                    {
                        name = "base64Binary";
                        ns = Globals.SerializationNamespace;
                    }
                    else if (type == Globals.TypeOfObject)
                    {
                        name = "anyType";
                    }
                    else if (type == Globals.TypeOfTimeSpan)
                    {
                        name = "duration";
                    }
                    else if (type == Globals.TypeOfGuid)
                    {
                        name = "guid";
                    }
                    else if (type == Globals.TypeOfUri)
                    {
                        name = "anyURI";
                    }
                    else
                        throw new Exception(string.Format("{0} is an invalidPrimitiveType", type.FullName));
                    break;
            }
            StableName = new XmlQualifiedName(name, ns);
        }

        public override string TopLevelElementNamespace
        {
            get { return Globals.SerializationNamespace; }
        }
    }
}
