// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Security.Cryptography.Xml
{
    public struct X509IssuerSerial
    {
        private string _issuerName;
        private string _serialNumber;

        internal X509IssuerSerial(string issuerName, string serialNumber)
        {
            if (issuerName == null || issuerName.Length == 0)
                throw new ArgumentException(SR.Arg_EmptyOrNullString, nameof(issuerName));
            if (serialNumber == null || serialNumber.Length == 0)
                throw new ArgumentException(SR.Arg_EmptyOrNullString, nameof(serialNumber));
            _issuerName = issuerName;
            _serialNumber = serialNumber;
        }


        public string IssuerName
        {
            get
            {
                return _issuerName;
            }
            set
            {
                _issuerName = value;
            }
        }

        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }
            set
            {
                _serialNumber = value;
            }
        }
    }
}
