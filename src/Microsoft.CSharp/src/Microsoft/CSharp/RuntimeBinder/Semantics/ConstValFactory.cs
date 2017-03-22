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

    internal static class ConstValFactory
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

        public static CONSTVAL Get(bool value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(int value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(uint value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(decimal value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(string value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(float value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(double value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(long value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(ulong value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(object p)
        {
            return new CONSTVAL(p);
        }
    }
}
