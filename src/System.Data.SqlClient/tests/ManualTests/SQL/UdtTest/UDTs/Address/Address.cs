// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Data.SqlTypes;

using Microsoft.SqlServer.Server;

[Serializable]
[SqlUserDefinedType(Format.UserDefined, IsByteOrdered = false, MaxByteSize = 500)]
public class Address : INullable, IBinarySerialize
{
    //public static readonly Address Null = new Address(true);
    public static Address Null { get { return new Address(true); } }

    //******************************************************
    // Constructors
    //******************************************************

    // Constructor for a null value: Address.Null
    // fNull is not used but needed because compiler doesn't let
    // struct to have parameterless constructors
    private Address(bool fNull)
    {
        m_fNotNull = false;
        m_firstline = SqlString.Null;
        m_secondline = SqlString.Null;
    }

    public Address(SqlString line1, SqlString line2)
    {
        m_firstline = line1;
        m_secondline = line2;
        m_fNotNull = true;
    }

    public Address()
    {
        m_fNotNull = false;
        m_firstline = SqlString.Null;
        m_secondline = SqlString.Null;
    }

    //******************************************************
    // INullable interface
    //******************************************************

    // INullable
    public bool IsNull
    {
        get { return !m_fNotNull; }
    }

    //******************************************************
    // Common static and instance methods for SQL UDTs
    //******************************************************

    public const int MaxByteSize = 500;
    public const bool IsFixedLength = false;
    public const bool IsByteOrdered = true;

    public void Read(BinaryReader r)
    {
        m_firstline = new SqlString(r.ReadString());
        m_secondline = new SqlString(r.ReadString());
        m_fNotNull = BitConverter.ToBoolean(r.ReadBytes(1), 0);
    }

    public void Write(BinaryWriter w)
    {
        w.Write(m_firstline.ToString());
        w.Write(m_secondline.ToString());
        w.Write(m_fNotNull);
    }

    public void FillFromBytes(SqlBytes value)
    {
        // todo: throw if bigger than MaxByteSize bytes

        if (value.IsNull)
        {
            m_fNotNull = false;
            m_firstline = SqlString.Null;
            m_secondline = SqlString.Null;
            return;
        }

        System.Text.UnicodeEncoding e = new System.Text.UnicodeEncoding();
        String str = e.GetString(value.Buffer);

        String[] twolines = new String[2];
        Char[] seperator = { '|' };
        twolines = str.Split(seperator);

        m_firstline = twolines[0];
        m_secondline = twolines.Length > 1 ? twolines[1] : SqlString.Null;
        m_fNotNull = true;

        return;
    }


    public void FillBytes(SqlBytes value)
    {
        if (this.IsNull)
        {
            if (value.IsNull)
                return;
            else
            {
                value.SetNull();
                return;
            }
        }

        SqlString str;
        if ((Object)m_secondline == null || m_secondline.IsNull)
            str = m_firstline;
        else
        {
            str = String.Concat(m_firstline, "|");
            str = String.Concat(str, m_secondline);
        }

        byte[] stringData = str.GetUnicodeBytes();
        int i;
        for (i = 0; i < stringData.Length; i++)
            value[i] = stringData[i];
        value.SetLength(i);

        return;
    }


    /*    Not in M1

        public void FillFromBytes( SqlBinary value ) ;

        public static Address FromXmlString( SqlString s );

        public SqlString ToXmlString();

    */
    public override string ToString()
    {
        if (IsNull)
            return "Null";

        else
            return Value.ToString();
    }

    public static Address Parse(SqlString s) // formerly FromString
    {
        if (s.IsNull)
            return Address.Null;


        // todo:throw if bigger than MaxByteSize bytes

        String str = s.ToString();
        String[] twolines = new String[2];
        // using || to indicate the seperation between
        // address line 1 and 2, assume it won't appear
        // in any address
        Char[] seperator = { '|', '|' };
        twolines = str.Split(seperator);

        if (twolines.Length == 2)
            return new Address(twolines[0], twolines[1]);
        else
            return new Address(twolines[0], SqlString.Null);

    }


    //******************************************************
    // Address Specific Methods
    //******************************************************

    public SqlString GetFirstLine()
    {
        return m_firstline;
    }

    public SqlString GetSecondLine()
    {
        return m_secondline;
    }


    public SqlString Value
    {
        get
        {
            if (m_fNotNull)
            {
                if (m_secondline.IsNull)
                    return m_firstline;
                else
                    return String.Concat(m_firstline, m_secondline);
            }
            else
                throw new SqlNullValueException();
        }
    }

    //******************************************************
    // Address Private Members
    //******************************************************
    private SqlString m_firstline;
    private SqlString m_secondline;
    private bool m_fNotNull; //false if null, default ctor makes it null
}

