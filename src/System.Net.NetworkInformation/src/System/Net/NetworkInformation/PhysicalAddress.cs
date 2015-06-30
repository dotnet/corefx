// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;

namespace System.Net.NetworkInformation
{
    public class PhysicalAddress
    {
        byte[] address = null;
        bool changed = true;
        int hash = 0;

        // FxCop: if this class is ever made mutable (like, given any non-readonly fields),
        // the readonly should be removed from the None decoration.
        public static readonly PhysicalAddress None = new PhysicalAddress(Array.Empty<byte>());

        // constructors
        public PhysicalAddress(byte[] address)
        {
            this.address = address;
        }

        public override int GetHashCode()
        {
            if (changed)
            {
                changed = false;
                hash = 0;

                int i;
                int size = address.Length & ~3;

                for (i = 0; i < size; i += 4)
                {
                    hash ^= (int)address[i]
                            | ((int)address[i + 1] << 8)
                            | ((int)address[i + 2] << 16)
                            | ((int)address[i + 3] << 24);
                }
                if ((address.Length & 3) != 0)
                {
                    int remnant = 0;
                    int shift = 0;

                    for (; i < address.Length; ++i)
                    {
                        remnant |= ((int)address[i]) << shift;
                        shift += 8;
                    }
                    hash ^= remnant;
                }
            }
            return hash;
        }

        public override bool Equals(object comparand)
        {
            PhysicalAddress address = comparand as PhysicalAddress;
            if (address == null)
                return false;

            if (this.address.Length != address.address.Length)
            {
                return false;
            }
            for (int i = 0; i < address.address.Length; i++)
            {
                if (this.address[i] != address.address[i])
                    return false;
            }
            return true;
        }


        public override string ToString()
        {
            StringBuilder addressString = new StringBuilder();

            foreach (byte value in address)
            {
                int tmp = (value >> 4) & 0x0F;

                for (int i = 0; i < 2; i++)
                {
                    if (tmp < 0x0A)
                    {
                        addressString.Append((char)(tmp + 0x30));
                    }
                    else
                    {
                        addressString.Append((char)(tmp + 0x37));
                    }
                    tmp = ((int)value & 0x0F);
                }
            }
            return addressString.ToString();
        }

        public byte[] GetAddressBytes()
        {
            byte[] tmp = new byte[address.Length];
            Buffer.BlockCopy(address, 0, tmp, 0, address.Length);
            return tmp;
        }



        public static PhysicalAddress Parse(string address)
        {
            int validCount = 0;
            bool hasDashes = false;
            byte[] buffer = null;

            if (address == null)
            {
                return PhysicalAddress.None;
            }

            //has dashes?
            if (address.IndexOf('-') >= 0)
            {
                hasDashes = true;
                buffer = new byte[(address.Length + 1) / 3];
            }
            else
            {
                if (address.Length % 2 > 0)
                {  //should be even
                    throw new FormatException(SR.net_bad_mac_address);
                }

                buffer = new byte[address.Length / 2];
            }

            int j = 0;
            for (int i = 0; i < address.Length; i++)
            {
                int value = (int)address[i];

                if (value >= 0x30 && value <= 0x39)
                {
                    value -= 0x30;
                }
                else if (value >= 0x41 && value <= 0x46)
                {
                    value -= 0x37;
                }
                else if (value == (int)'-')
                {
                    if (validCount == 2)
                    {
                        validCount = 0;
                        continue;
                    }
                    else
                    {
                        throw new FormatException(SR.net_bad_mac_address);
                    }
                }
                else
                {
                    throw new FormatException(SR.net_bad_mac_address);
                }

                //we had too many characters after the last dash
                if (hasDashes && validCount >= 2)
                {
                    throw new FormatException(SR.net_bad_mac_address);
                }

                if (validCount % 2 == 0)
                {
                    buffer[j] = (byte)(value << 4);
                }
                else
                {
                    buffer[j++] |= (byte)value;
                }

                validCount++;
            }

            //we too few characters after the last dash
            if (validCount < 2)
            {
                throw new FormatException(SR.net_bad_mac_address);
            }

            return new PhysicalAddress(buffer);
        }
    }
}

