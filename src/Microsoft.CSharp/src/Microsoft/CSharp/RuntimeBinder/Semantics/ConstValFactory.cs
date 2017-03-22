// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // ConstValFactory owns the construction of CONSTVALs
    // This should be the only place in the code which dynamically
    // allocates memory for CONSTVALs.
    //
    // One important client of ConstValFactory is the lexer/parser,
    // so ConstValFactory should not take a dependency on TYPESYMs
    // or predefined types.

    internal sealed class ConstValFactory
    {
        public static CONSTVAL GetDefaultValue(ConstValKind kind)
        {
            switch (kind)
            {
                case ConstValKind.Int:
                    return new CONSTVAL(0);

                case ConstValKind.Double:
                    return new CONSTVAL(0.0);

                case ConstValKind.Long:
                    return new CONSTVAL(0L);

                case ConstValKind.Decimal:
                    return new CONSTVAL(0M);

                case ConstValKind.Float:
                    return new CONSTVAL(0F);

                case ConstValKind.Boolean:
                    return new CONSTVAL(false);
            }

            return new CONSTVAL();
        }

        public static CONSTVAL GetNullRef()
        {
            return new CONSTVAL();
        }

        public static CONSTVAL GetBool(bool value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL GetInt(int value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL GetUInt(uint value)
        {
            return new CONSTVAL(value);
        }

        public CONSTVAL Create(decimal value)
        {
            return new CONSTVAL(value);
        }

        public CONSTVAL Create(string value)
        {
            return new CONSTVAL(value);
        }

        public CONSTVAL Create(float value)
        {
            return new CONSTVAL(value);
        }

        public CONSTVAL Create(double value)
        {
            return new CONSTVAL(value);
        }

        public CONSTVAL Create(long value)
        {
            return new CONSTVAL(value);
        }

        public CONSTVAL Create(ulong value)
        {
            return new CONSTVAL(value);
        }

        internal CONSTVAL Create(bool value)
        {
            return new CONSTVAL(value);
        }

        internal CONSTVAL Create(object p)
        {
            return new CONSTVAL(p);
        }
    }
}
