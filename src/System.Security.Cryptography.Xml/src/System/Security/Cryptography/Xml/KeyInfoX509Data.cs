// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
    public class KeyInfoX509Data : KeyInfoClause
    {
        // An array of certificates representing the certificate chain 
        private ArrayList _certificates = null;
        // An array of issuer serial structs
        private ArrayList _issuerSerials = null;
        // An array of SKIs
        private ArrayList _subjectKeyIds = null;
        // An array of subject names
        private ArrayList _subjectNames = null;
        // A raw byte data representing a certificate revocation list
        private byte[] _CRL = null;

        //
        // public constructors
        //

        public KeyInfoX509Data() { }

        public KeyInfoX509Data(byte[] rgbCert)
        {
            X509Certificate2 certificate = new X509Certificate2(rgbCert);
            AddCertificate(certificate);
        }

        public KeyInfoX509Data(X509Certificate cert)
        {
            AddCertificate(cert);
        }

        public KeyInfoX509Data(X509Certificate cert, X509IncludeOption includeOption)
        {
            if (cert == null)
                throw new ArgumentNullException("cert");

            X509Certificate2 certificate = new X509Certificate2(cert);
            X509ChainElementCollection elements = null;
            X509Chain chain = null;
            switch (includeOption)
            {
                case X509IncludeOption.ExcludeRoot:
                    // Build the certificate chain
                    chain = new X509Chain();
                    chain.Build(certificate);

                    // Can't honor the option if we only have a partial chain.
                    if ((chain.ChainStatus.Length > 0) &&
                        ((chain.ChainStatus[0].Status & X509ChainStatusFlags.PartialChain) == X509ChainStatusFlags.PartialChain))
                    throw new CryptographicException(unchecked((int)Interop.Crypt32.CertChainPolicyErrors.CERT_E_CHAINING));

                elements = (X509ChainElementCollection)chain.ChainElements;
                    for (int index = 0; index < (Utils.IsSelfSigned(chain) ? 1 : elements.Count - 1); index++)
                    {
                        AddCertificate(elements[index].Certificate);
                    }
                    break;
                case X509IncludeOption.EndCertOnly:
                    AddCertificate(certificate);
                    break;
                case X509IncludeOption.WholeChain:
                    // Build the certificate chain
                    chain = new X509Chain();
                    chain.Build(certificate);

                    // Can't honor the option if we only have a partial chain.
                    if ((chain.ChainStatus.Length > 0) &&
                        ((chain.ChainStatus[0].Status & X509ChainStatusFlags.PartialChain) == X509ChainStatusFlags.PartialChain))
                        throw new CryptographicException(SR.GetResourceString("Cryptography_Xml_E_Chaining"));

                    elements = (X509ChainElementCollection)chain.ChainElements;
                    foreach (X509ChainElement element in elements)
                    {
                        AddCertificate(element.Certificate);
                    }
                    break;
            }
        }

        //
        // public properties
        //

        public ArrayList Certificates
        {
            get { return _certificates; }
        }

        public void AddCertificate(X509Certificate certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException("certificate");

            if (_certificates == null)
                _certificates = new ArrayList();

            X509Certificate2 x509 = new X509Certificate2(certificate);
            _certificates.Add(x509);
        }

        public ArrayList SubjectKeyIds
        {
            get { return _subjectKeyIds; }
        }

        public void AddSubjectKeyId(byte[] subjectKeyId)
        {
            if (_subjectKeyIds == null)
                _subjectKeyIds = new ArrayList();
            _subjectKeyIds.Add(subjectKeyId);
        }

        public void AddSubjectKeyId(string subjectKeyId)
        {
            if (_subjectKeyIds == null)
                _subjectKeyIds = new ArrayList();
            _subjectKeyIds.Add(Utils.DecodeHexString(subjectKeyId));
        }

        public ArrayList SubjectNames
        {
            get { return _subjectNames; }
        }

        public void AddSubjectName(string subjectName)
        {
            if (_subjectNames == null)
                _subjectNames = new ArrayList();
            _subjectNames.Add(subjectName);
        }

        public ArrayList IssuerSerials
        {
            get { return _issuerSerials; }
        }

        public void AddIssuerSerial(string issuerName, string serialNumber)
        {
            BigInt h = new BigInt();
            h.FromHexadecimal(serialNumber);
            if (_issuerSerials == null)
                _issuerSerials = new ArrayList();
            _issuerSerials.Add(new X509IssuerSerial(issuerName, h.ToDecimal()));
        }

        // When we load an X509Data from Xml, we know the serial number is in decimal representation.
        internal void InternalAddIssuerSerial(string issuerName, string serialNumber)
        {
            if (_issuerSerials == null)
                _issuerSerials = new ArrayList();
            _issuerSerials.Add(new X509IssuerSerial(issuerName, serialNumber));
        }

        public byte[] CRL
        {
            get { return _CRL; }
            set { _CRL = value; }
        }

        //
        // private methods
        //

        private void Clear()
        {
            _CRL = null;
            if (_subjectKeyIds != null) _subjectKeyIds.Clear();
            if (_subjectNames != null) _subjectNames.Clear();
            if (_issuerSerials != null) _issuerSerials.Clear();
            if (_certificates != null) _certificates.Clear();
        }

        //
        // public methods
        //

        public override XmlElement GetXml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            return GetXml(xmlDocument);
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            XmlElement x509DataElement = xmlDocument.CreateElement("X509Data", SignedXml.XmlDsigNamespaceUrl);

            if (_issuerSerials != null)
            {
                foreach (X509IssuerSerial issuerSerial in _issuerSerials)
                {
                    XmlElement issuerSerialElement = xmlDocument.CreateElement("X509IssuerSerial", SignedXml.XmlDsigNamespaceUrl);
                    XmlElement issuerNameElement = xmlDocument.CreateElement("X509IssuerName", SignedXml.XmlDsigNamespaceUrl);
                    issuerNameElement.AppendChild(xmlDocument.CreateTextNode(issuerSerial.IssuerName));
                    issuerSerialElement.AppendChild(issuerNameElement);
                    XmlElement serialNumberElement = xmlDocument.CreateElement("X509SerialNumber", SignedXml.XmlDsigNamespaceUrl);
                    serialNumberElement.AppendChild(xmlDocument.CreateTextNode(issuerSerial.SerialNumber));
                    issuerSerialElement.AppendChild(serialNumberElement);
                    x509DataElement.AppendChild(issuerSerialElement);
                }
            }

            if (_subjectKeyIds != null)
            {
                foreach (byte[] subjectKeyId in _subjectKeyIds)
                {
                    XmlElement subjectKeyIdElement = xmlDocument.CreateElement("X509SKI", SignedXml.XmlDsigNamespaceUrl);
                    subjectKeyIdElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(subjectKeyId)));
                    x509DataElement.AppendChild(subjectKeyIdElement);
                }
            }

            if (_subjectNames != null)
            {
                foreach (string subjectName in _subjectNames)
                {
                    XmlElement subjectNameElement = xmlDocument.CreateElement("X509SubjectName", SignedXml.XmlDsigNamespaceUrl);
                    subjectNameElement.AppendChild(xmlDocument.CreateTextNode(subjectName));
                    x509DataElement.AppendChild(subjectNameElement);
                }
            }

            if (_certificates != null)
            {
                foreach (X509Certificate certificate in _certificates)
                {
                    XmlElement x509Element = xmlDocument.CreateElement("X509Certificate", SignedXml.XmlDsigNamespaceUrl);
                    x509Element.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(certificate.GetRawCertData())));
                    x509DataElement.AppendChild(x509Element);
                }
            }

            if (_CRL != null)
            {
                XmlElement crlElement = xmlDocument.CreateElement("X509CRL", SignedXml.XmlDsigNamespaceUrl);
                crlElement.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(_CRL)));
                x509DataElement.AppendChild(crlElement);
            }

            return x509DataElement;
        }

        public override void LoadXml(XmlElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            XmlNamespaceManager nsm = new XmlNamespaceManager(element.OwnerDocument.NameTable);
            nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);

            XmlNodeList x509IssuerSerialNodes = element.SelectNodes("ds:X509IssuerSerial", nsm);
            XmlNodeList x509SKINodes = element.SelectNodes("ds:X509SKI", nsm);
            XmlNodeList x509SubjectNameNodes = element.SelectNodes("ds:X509SubjectName", nsm);
            XmlNodeList x509CertificateNodes = element.SelectNodes("ds:X509Certificate", nsm);
            XmlNodeList x509CRLNodes = element.SelectNodes("ds:X509CRL", nsm);

            if ((x509CRLNodes.Count == 0 && x509IssuerSerialNodes.Count == 0 && x509SKINodes.Count == 0
                    && x509SubjectNameNodes.Count == 0 && x509CertificateNodes.Count == 0)) // Bad X509Data tag, or Empty tag
                throw new CryptographicException(SR.GetResourceString("Cryptography_Xml_InvalidElement"), "X509Data");

            // Flush anything in the lists
            Clear();

            if (x509CRLNodes.Count != 0)
                _CRL = Convert.FromBase64String(Utils.DiscardWhiteSpaces(x509CRLNodes.Item(0).InnerText));

            foreach (XmlNode issuerSerialNode in x509IssuerSerialNodes)
            {
                XmlNode x509IssuerNameNode = issuerSerialNode.SelectSingleNode("ds:X509IssuerName", nsm);
                XmlNode x509SerialNumberNode = issuerSerialNode.SelectSingleNode("ds:X509SerialNumber", nsm);
                if (x509IssuerNameNode == null || x509SerialNumberNode == null)
                    throw new CryptographicException(SR.GetResourceString("Cryptography_Xml_InvalidElement"), "IssuerSerial");
                InternalAddIssuerSerial(x509IssuerNameNode.InnerText.Trim(), x509SerialNumberNode.InnerText.Trim());
            }

            foreach (XmlNode node in x509SKINodes)
            {
                AddSubjectKeyId(Convert.FromBase64String(Utils.DiscardWhiteSpaces(node.InnerText)));
            }

            foreach (XmlNode node in x509SubjectNameNodes)
            {
                AddSubjectName(node.InnerText.Trim());
            }

            foreach (XmlNode node in x509CertificateNodes)
            {
                AddCertificate(new X509Certificate2(Convert.FromBase64String(Utils.DiscardWhiteSpaces(node.InnerText))));
            }
        }

        //
        // This is a pretty "crude" implementation of BigInt arithmetic operations.
        // This class is used in particular to convert certificate serial numbers from
        // hexadecimal representation to decimal format and vice versa.
        //
        // We are not very concerned about the perf characterestics of this implementation
        // for now. We perform all operations up to 128 bytes (which is enough for the current
        // purposes although this constant can be increased). Any overflow bytes are going to be lost.
        //
        // A BigInt is represented as a little endian byte array of size 128 bytes. All
        // arithmetic operations are performed in base 0x100 (256). The algorithms used
        // are simply the common primary school techniques for doing operations in base 10.
        //

        internal sealed class BigInt
        {
            private byte[] m_elements;
            private const int m_maxbytes = 128; // 128 bytes is the maximum we can handle.
                                                // This means any overflow bits beyond 128 bytes
                                                // will be lost when doing arithmetic operations.
            private const int m_base     = 0x100;
            private int m_size           = 0;

            internal BigInt()
            {
                m_elements = new byte[m_maxbytes];
            }

            internal BigInt(byte b)
            {
                m_elements = new byte[m_maxbytes];
                SetDigit(0, b);
            }

            //
            // Gets or sets the size of a BigInt.
            //

            internal int Size
            {
                get
                {
                    return m_size;
                }
                set
                {
                    if (value > m_maxbytes)
                        m_size = m_maxbytes;
                    if (value < 0)
                        m_size = 0;
                    m_size = value;
                }
            }

            //
            // Gets the digit at the specified index.
            //

            internal byte GetDigit(int index)
            {
                if (index < 0 || index >= m_size)
                    return 0;

                return m_elements[index];
            }

            //
            // Sets the digit at the specified index.
            //

            internal void SetDigit(int index, byte digit)
            {
                if (index >= 0 && index < m_maxbytes)
                {
                    m_elements[index] = digit;
                    if (index >= m_size && digit != 0)
                        m_size = (index + 1);
                    if (index == m_size - 1 && digit == 0)
                        m_size--;
                }
            }

            internal void SetDigit(int index, byte digit, ref int size)
            {
                if (index >= 0 && index < m_maxbytes)
                {
                    m_elements[index] = digit;
                    if (index >= size && digit != 0)
                        size = (index + 1);
                    if (index == size - 1 && digit == 0)
                        size = (size - 1);
                }
            }

            //
            // overloaded operators.
            //

            public static bool operator <(BigInt value1, BigInt value2)
            {
                if (value1 == null)
                    return true;
                else if (value2 == null)
                    return false;

                int Len1 = value1.Size;
                int Len2 = value2.Size;

                if (Len1 != Len2)
                    return (Len1 < Len2);

                while (Len1-- > 0)
                {
                    if (value1.m_elements[Len1] != value2.m_elements[Len1])
                        return (value1.m_elements[Len1] < value2.m_elements[Len1]);
                }

                return false;
            }

            public static bool operator >(BigInt value1, BigInt value2)
            {
                if (value1 == null)
                    return false;
                else if (value2 == null)
                    return true;

                int Len1 = value1.Size;
                int Len2 = value2.Size;

                if (Len1 != Len2)
                    return (Len1 > Len2);

                while (Len1-- > 0)
                {
                    if (value1.m_elements[Len1] != value2.m_elements[Len1])
                        return (value1.m_elements[Len1] > value2.m_elements[Len1]);
                }

                return false;
            }

            public static bool operator ==(BigInt value1, BigInt value2)
            {
                if ((Object)value1 == null)
                    return ((Object)value2 == null);
                else if ((Object)value2 == null)
                    return ((Object)value1 == null);

                int Len1 = value1.Size;
                int Len2 = value2.Size;

                if (Len1 != Len2)
                    return false;

                for (int index = 0; index < Len1; index++)
                {
                    if (value1.m_elements[index] != value2.m_elements[index])
                        return false;
                }

                return true;
            }

            public static bool operator !=(BigInt value1, BigInt value2)
            {
                return !(value1 == value2);
            }

            public override bool Equals(Object obj)
            {
                if (obj is BigInt)
                {
                    return (this == (BigInt)obj);
                }
                return false;
            }

            public override int GetHashCode()
            {
                int hash = 0;
                for (int index = 0; index < m_size; index++)
                {
                    hash += GetDigit(index);
                }
                return hash;
            }

            //
            // Adds a and b and outputs the result in c.
            //

            internal static void Add(BigInt a, byte b, ref BigInt c)
            {
                byte carry = b;
                int sum = 0;

                int size = a.Size;
                int newSize = 0;
                for (int index = 0; index < size; index++)
                {
                    sum = a.GetDigit(index) + carry;
                    c.SetDigit(index, (byte)(sum & 0xFF), ref newSize);
                    carry = (byte)((sum >> 8) & 0xFF);
                }

                if (carry != 0)
                    c.SetDigit(a.Size, carry, ref newSize);

                c.Size = newSize;
            }

            //
            // Negates a BigInt value. Each byte is complemented, then we add 1 to it.
            //

            internal static void Negate(ref BigInt a)
            {
                int newSize = 0;
                for (int index = 0; index < m_maxbytes; index++)
                {
                    a.SetDigit(index, (byte)(~a.GetDigit(index) & 0xFF), ref newSize);
                }
                for (int index = 0; index < m_maxbytes; index++)
                {
                    a.SetDigit(index, (byte)(a.GetDigit(index) + 1), ref newSize);
                    if ((a.GetDigit(index) & 0xFF) != 0)
                        break;
                    a.SetDigit(index, (byte)(a.GetDigit(index) & 0xFF), ref newSize);
                }
                a.Size = newSize;
            }

            //
            // Subtracts b from a and outputs the result in c.
            //

            internal static void Subtract(BigInt a, BigInt b, ref BigInt c)
            {
                byte borrow = 0;
                int diff = 0;

                if (a < b)
                {
                    Subtract(b, a, ref c);
                    Negate(ref c);
                    return;
                }

                int index = 0;
                int size = a.Size;
                int newSize = 0;
                for (index = 0; index < size; index++)
                {
                    diff = a.GetDigit(index) - b.GetDigit(index) - borrow;
                    borrow = 0;
                    if (diff < 0)
                    {
                        diff += m_base;
                        borrow = 1;
                    }
                    c.SetDigit(index, (byte)(diff & 0xFF), ref newSize);
                }

                c.Size = newSize;
            }

            //
            // multiplies a BigInt by an integer.
            //

            private void Multiply(int b)
            {
                if (b == 0)
                {
                    Clear();
                    return;
                }

                int carry = 0, product = 0;
                int size = this.Size;
                int newSize = 0;
                for (int index = 0; index < size; index++)
                {
                    product = b * GetDigit(index) + carry;
                    carry = product / m_base;
                    SetDigit(index, (byte)(product % m_base), ref newSize);
                }

                if (carry != 0)
                {
                    byte[] bytes = BitConverter.GetBytes(carry);
                    for (int index = 0; index < bytes.Length; index++)
                    {
                        SetDigit(size + index, bytes[index], ref newSize);
                    }
                }

                this.Size = newSize;
            }

            private static void Multiply(BigInt a, int b, ref BigInt c)
            {
                if (b == 0)
                {
                    c.Clear();
                    return;
                }

                int carry = 0, product = 0;
                int size = a.Size;
                int newSize = 0;
                for (int index = 0; index < size; index++)
                {
                    product = b * a.GetDigit(index) + carry;
                    carry = product / m_base;
                    c.SetDigit(index, (byte)(product % m_base), ref newSize);
                }

                if (carry != 0)
                {
                    byte[] bytes = BitConverter.GetBytes(carry);
                    for (int index = 0; index < bytes.Length; index++)
                    {
                        c.SetDigit(size + index, bytes[index], ref newSize);
                    }
                }

                c.Size = newSize;
            }

            //
            // Divides a BigInt by a single byte.
            //

            private void Divide(int b)
            {
                int carry = 0, quotient = 0;
                int bLen = this.Size;

                int newSize = 0;
                while (bLen-- > 0)
                {
                    quotient = m_base * carry + GetDigit(bLen);
                    carry = quotient % b;
                    SetDigit(bLen, (byte)(quotient / b), ref newSize);
                }

                this.Size = newSize;
            }

            //
            // Integer division of one BigInt by another.
            //

            internal static void Divide(BigInt numerator, BigInt denominator, ref BigInt quotient, ref BigInt remainder)
            {
                // Avoid extra computations in special cases.

                if (numerator < denominator)
                {
                    quotient.Clear();
                    remainder.CopyFrom(numerator);
                    return;
                }

                if (numerator == denominator)
                {
                    quotient.Clear();
                    quotient.SetDigit(0, 1);
                    remainder.Clear();
                    return;
                }

                BigInt dividend = new BigInt();
                dividend.CopyFrom(numerator);
                BigInt divisor = new BigInt();
                divisor.CopyFrom(denominator);

                uint zeroCount = 0;
                // We pad the divisor with zeros until its size equals that of the dividend.
                while (divisor.Size < dividend.Size)
                {
                    divisor.Multiply(m_base);
                    zeroCount++;
                }

                if (divisor > dividend)
                {
                    divisor.Divide(m_base);
                    zeroCount--;
                }

                // Use school division techniques, make a guess for how many times
                // divisor goes into dividend, making adjustment if necessary.
                int a = 0;
                int b = 0;
                int c = 0;

                BigInt hold = new BigInt();
                quotient.Clear();
                for (int index = 0; index <= zeroCount; index++)
                {
                    a = dividend.Size == divisor.Size ? dividend.GetDigit(dividend.Size - 1) :
                                                        m_base * dividend.GetDigit(dividend.Size - 1) + dividend.GetDigit(dividend.Size - 2);
                    b = divisor.GetDigit(divisor.Size - 1);
                    c = a / b;

                    if (c >= m_base)
                        c = 0xFF;

                    Multiply(divisor, c, ref hold);
                    while (hold > dividend)
                    {
                        c--;
                        Multiply(divisor, c, ref hold);
                    }

                    quotient.Multiply(m_base);
                    Add(quotient, (byte)c, ref quotient);
                    Subtract(dividend, hold, ref dividend);
                    divisor.Divide(m_base);
                }
                remainder.CopyFrom(dividend);
            }

            //
            // copies a BigInt value.
            //

            internal void CopyFrom(BigInt a)
            {
                Array.Copy(a.m_elements, m_elements, m_maxbytes);
                m_size = a.m_size;
            }

            //
            // This method returns true if the BigInt is equal to 0, false otherwise.
            //

            internal bool IsZero()
            {
                for (int index = 0; index < m_size; index++)
                {
                    if (m_elements[index] != 0)
                        return false;
                }
                return true;
            }

            //
            // returns the array in machine format, i.e. little endian format (as an integer).
            //

            internal byte[] ToByteArray()
            {
                byte[] result = new byte[this.Size];
                Array.Copy(m_elements, result, this.Size);
                return result;
            }

            //
            // zeroizes the content of the internal array.
            //

            internal void Clear()
            {
                m_size = 0;
            }

            //
            // Imports a hexadecimal string into a BigInt bit representation.
            //

            internal void FromHexadecimal(string hexNum)
            {
                byte[] hex = Utils.DecodeHexString(hexNum);
                Array.Reverse(hex);
                int size = Utils.GetHexArraySize(hex);
                Array.Copy(hex, m_elements, size);
                this.Size = size;
            }

            //
            // Imports a decimal string into a BigInt bit representation.
            //

            internal void FromDecimal(string decNum)
            {
                BigInt c = new BigInt();
                BigInt tmp = new BigInt();
                int length = decNum.Length;
                for (int index = 0; index < length; index++)
                {
                    // just ignore invalid characters. No need to raise an exception.
                    if (decNum[index] > '9' || decNum[index] < '0')
                        continue;
                    Multiply(c, 10, ref tmp);
                    Add(tmp, (byte)(decNum[index] - '0'), ref c);
                }
                CopyFrom(c);
            }

            //
            // Exports the BigInt representation as a decimal string.
            //

            private static readonly char[] decValues = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            internal string ToDecimal()
            {
                if (IsZero())
                    return "0";

                BigInt ten = new BigInt(0xA);
                BigInt numerator = new BigInt();
                BigInt quotient = new BigInt();
                BigInt remainder = new BigInt();

                numerator.CopyFrom(this);

                // Each hex digit can account for log(16) = 1.21 decimal digits. Times two hex digits in a byte
                // and m_size bytes used in this BigInt, yields the maximum number of characters for the decimal
                // representation of the BigInt.
                char[] dec = new char[(int)Math.Ceiling(m_size * 2 * 1.21)];

                int index = 0;
                do
                {
                    Divide(numerator, ten, ref quotient, ref remainder);
                    dec[index++] = decValues[remainder.IsZero() ? 0 : (int)remainder.m_elements[0]];
                    numerator.CopyFrom(quotient);
                } while (quotient.IsZero() == false);

                Array.Reverse(dec, 0, index);
                return new String(dec, 0, index);
            }
        }
    }
}
