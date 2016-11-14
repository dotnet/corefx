// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Net.Http.Headers
{
    public class ProductInfoHeaderValue : ICloneable
    {
        private ProductHeaderValue _product;
        private string _comment;

        public ProductHeaderValue Product
        {
            get { return _product; }
        }

        public string Comment
        {
            get { return _comment; }
        }

        public ProductInfoHeaderValue(string productName, string productVersion)
            : this(new ProductHeaderValue(productName, productVersion))
        {
        }

        public ProductInfoHeaderValue(ProductHeaderValue product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            _product = product;
        }

        public ProductInfoHeaderValue(string comment)
        {
            HeaderUtilities.CheckValidComment(comment, nameof(comment));
            _comment = comment;
        }

        private ProductInfoHeaderValue(ProductInfoHeaderValue source)
        {
            Debug.Assert(source != null);

            _product = source._product;
            _comment = source._comment;
        }

        private ProductInfoHeaderValue()
        {
        }

        public override string ToString()
        {
            if (_product == null)
            {
                return _comment;
            }
            return _product.ToString();
        }

        public override bool Equals(object obj)
        {
            ProductInfoHeaderValue other = obj as ProductInfoHeaderValue;

            if (other == null)
            {
                return false;
            }

            if (_product == null)
            {
                // We compare comments using case-sensitive comparison.
                return string.Equals(_comment, other._comment, StringComparison.Ordinal);
            }

            return _product.Equals(other._product);
        }

        public override int GetHashCode()
        {
            if (_product == null)
            {
                return _comment.GetHashCode();
            }
            return _product.GetHashCode();
        }

        public static ProductInfoHeaderValue Parse(string input)
        {
            int index = 0;
            object result = ProductInfoHeaderParser.SingleValueParser.ParseValue(
                input, null, ref index);
            if (index < input.Length)
            {
                // There is some invalid leftover data. Normally BaseHeaderParser.TryParseValue would 
                // handle this, but ProductInfoHeaderValue does not derive from BaseHeaderParser.
                throw new FormatException(string.Format(System.Globalization.CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, input.Substring(index)));
            }
            return (ProductInfoHeaderValue)result;
        }

        public static bool TryParse(string input, out ProductInfoHeaderValue parsedValue)
        {
            int index = 0;
            object output;
            parsedValue = null;

            if (ProductInfoHeaderParser.SingleValueParser.TryParseValue(input, null, ref index, out output))
            {
                if (index < input.Length)
                {
                    // There is some invalid leftover data. Normally BaseHeaderParser.TryParseValue would 
                    // handle this, but ProductInfoHeaderValue does not derive from BaseHeaderParser.
                    return false;
                }
                parsedValue = (ProductInfoHeaderValue)output;
                return true;
            }
            return false;
        }

        internal static int GetProductInfoLength(string input, int startIndex, out ProductInfoHeaderValue parsedValue)
        {
            Debug.Assert(startIndex >= 0);

            parsedValue = null;

            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }

            int current = startIndex;

            // Caller must remove leading whitespace.
            string comment = null;
            ProductHeaderValue product = null;
            if (input[current] == '(')
            {
                int commentLength = 0;
                if (HttpRuleParser.GetCommentLength(input, current, out commentLength) != HttpParseResult.Parsed)
                {
                    return 0;
                }

                comment = input.Substring(current, commentLength);

                current = current + commentLength;
                current = current + HttpRuleParser.GetWhitespaceLength(input, current);
            }
            else
            {
                // Trailing whitespace is removed by GetProductLength().
                int productLength = ProductHeaderValue.GetProductLength(input, current, out product);

                if (productLength == 0)
                {
                    return 0;
                }

                current = current + productLength;
            }

            parsedValue = new ProductInfoHeaderValue();
            parsedValue._product = product;
            parsedValue._comment = comment;
            return current - startIndex;
        }

        object ICloneable.Clone()
        {
            return new ProductInfoHeaderValue(this);
        }
    }
}
