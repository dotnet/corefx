﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
    internal sealed class XPathScanner
    {
        private string _xpathExpr;
        private int _xpathExprIndex;
        private LexKind _kind;
        private char _currentChar;
        private string _name;
        private string _prefix;
        private string _stringValue;
        private double _numberValue = double.NaN;
        private bool _canBeFunction;
        private XmlCharType _xmlCharType = XmlCharType.Instance;

        public XPathScanner(string xpathExpr)
        {
            if (xpathExpr == null)
            {
                throw XPathException.Create(SR.Xp_ExprExpected, string.Empty);
            }
            this._xpathExpr = xpathExpr;
            NextChar();
            NextLex();
        }

        public string SourceText { get { return this._xpathExpr; } }

        private char CurrentChar { get { return _currentChar; } }

        private bool NextChar()
        {
            Debug.Assert(0 <= _xpathExprIndex && _xpathExprIndex <= _xpathExpr.Length);
            if (_xpathExprIndex < _xpathExpr.Length)
            {
                _currentChar = _xpathExpr[_xpathExprIndex++];
                return true;
            }
            else
            {
                _currentChar = '\0';
                return false;
            }
        }

#if XML10_FIFTH_EDITION
        private char PeekNextChar()
        {
            Debug.Assert(0 <= xpathExprIndex && xpathExprIndex <= xpathExpr.Length);
            if (xpathExprIndex < xpathExpr.Length)
            {
                return xpathExpr[xpathExprIndex];
            }
            else
            {
                Debug.Assert(xpathExprIndex == xpathExpr.Length);
                return '\0';
            }
        }
#endif

        public LexKind Kind { get { return this._kind; } }

        public string Name
        {
            get
            {
                Debug.Assert(this._kind == LexKind.Name || this._kind == LexKind.Axe);
                Debug.Assert(this._name != null);
                return this._name;
            }
        }

        public string Prefix
        {
            get
            {
                Debug.Assert(this._kind == LexKind.Name);
                Debug.Assert(this._prefix != null);
                return this._prefix;
            }
        }

        public string StringValue
        {
            get
            {
                Debug.Assert(this._kind == LexKind.String);
                Debug.Assert(this._stringValue != null);
                return this._stringValue;
            }
        }

        public double NumberValue
        {
            get
            {
                Debug.Assert(this._kind == LexKind.Number);
                Debug.Assert(!double.IsNaN(this._numberValue));
                return this._numberValue;
            }
        }

        // To parse PathExpr we need a way to distinct name from function. 
        // THis distinction can't be done without context: "or (1 != 0)" this this a function or 'or' in OrExp 
        public bool CanBeFunction
        {
            get
            {
                Debug.Assert(this._kind == LexKind.Name);
                return this._canBeFunction;
            }
        }

        void SkipSpace()
        {
            while (_xmlCharType.IsWhiteSpace(this.CurrentChar) && NextChar()) ;
        }

        public bool NextLex()
        {
            SkipSpace();
            switch (this.CurrentChar)
            {
                case '\0':
                    _kind = LexKind.Eof;
                    return false;
                case ',':
                case '@':
                case '(':
                case ')':
                case '|':
                case '*':
                case '[':
                case ']':
                case '+':
                case '-':
                case '=':
                case '#':
                case '$':
                    _kind = (LexKind)Convert.ToInt32(this.CurrentChar, CultureInfo.InvariantCulture);
                    NextChar();
                    break;
                case '<':
                    _kind = LexKind.Lt;
                    NextChar();
                    if (this.CurrentChar == '=')
                    {
                        _kind = LexKind.Le;
                        NextChar();
                    }
                    break;
                case '>':
                    _kind = LexKind.Gt;
                    NextChar();
                    if (this.CurrentChar == '=')
                    {
                        _kind = LexKind.Ge;
                        NextChar();
                    }
                    break;
                case '!':
                    _kind = LexKind.Bang;
                    NextChar();
                    if (this.CurrentChar == '=')
                    {
                        _kind = LexKind.Ne;
                        NextChar();
                    }
                    break;
                case '.':
                    _kind = LexKind.Dot;
                    NextChar();
                    if (this.CurrentChar == '.')
                    {
                        _kind = LexKind.DotDot;
                        NextChar();
                    }
                    else if (XmlCharType.IsDigit(this.CurrentChar))
                    {
                        _kind = LexKind.Number;
                        _numberValue = ScanFraction();
                    }
                    break;
                case '/':
                    _kind = LexKind.Slash;
                    NextChar();
                    if (this.CurrentChar == '/')
                    {
                        _kind = LexKind.SlashSlash;
                        NextChar();
                    }
                    break;
                case '"':
                case '\'':
                    this._kind = LexKind.String;
                    this._stringValue = ScanString();
                    break;
                default:
                    if (XmlCharType.IsDigit(this.CurrentChar))
                    {
                        _kind = LexKind.Number;
                        _numberValue = ScanNumber();
                    }
                    else if (_xmlCharType.IsStartNCNameSingleChar(this.CurrentChar)
#if XML10_FIFTH_EDITION
                    || xmlCharType.IsNCNameHighSurrogateChar(this.CurerntChar)
#endif
                    )
                    {
                        _kind = LexKind.Name;
                        this._name = ScanName();
                        this._prefix = string.Empty;
                        // "foo:bar" is one lexem not three because it doesn't allow spaces in between
                        // We should distinct it from "foo::" and need process "foo ::" as well
                        if (this.CurrentChar == ':')
                        {
                            NextChar();
                            // can be "foo:bar" or "foo::"
                            if (this.CurrentChar == ':')
                            {   // "foo::"
                                NextChar();
                                _kind = LexKind.Axe;
                            }
                            else
                            {                          // "foo:*", "foo:bar" or "foo: "
                                this._prefix = this._name;
                                if (this.CurrentChar == '*')
                                {
                                    NextChar();
                                    this._name = "*";
                                }
                                else if (_xmlCharType.IsStartNCNameSingleChar(this.CurrentChar)
#if XML10_FIFTH_EDITION
                                || xmlCharType.IsNCNameHighSurrogateChar(this.CurerntChar)
#endif
                                )
                                {
                                    this._name = ScanName();
                                }
                                else
                                {
                                    throw XPathException.Create(SR.Xp_InvalidName, SourceText);
                                }
                            }
                        }
                        else
                        {
                            SkipSpace();
                            if (this.CurrentChar == ':')
                            {
                                NextChar();
                                // it can be "foo ::" or just "foo :"
                                if (this.CurrentChar == ':')
                                {
                                    NextChar();
                                    _kind = LexKind.Axe;
                                }
                                else
                                {
                                    throw XPathException.Create(SR.Xp_InvalidName, SourceText);
                                }
                            }
                        }
                        SkipSpace();
                        this._canBeFunction = (this.CurrentChar == '(');
                    }
                    else
                    {
                        throw XPathException.Create(SR.Xp_InvalidToken, SourceText);
                    }
                    break;
            }
            return true;
        }

        private double ScanNumber()
        {
            Debug.Assert(this.CurrentChar == '.' || XmlCharType.IsDigit(this.CurrentChar));
            int start = _xpathExprIndex - 1;
            int len = 0;
            while (XmlCharType.IsDigit(this.CurrentChar))
            {
                NextChar(); len++;
            }
            if (this.CurrentChar == '.')
            {
                NextChar(); len++;
                while (XmlCharType.IsDigit(this.CurrentChar))
                {
                    NextChar(); len++;
                }
            }
            return XmlConvertEx.ToXPathDouble(this._xpathExpr.Substring(start, len));
        }

        private double ScanFraction()
        {
            Debug.Assert(XmlCharType.IsDigit(this.CurrentChar));
            int start = _xpathExprIndex - 2;
            Debug.Assert(0 <= start && this._xpathExpr[start] == '.');
            int len = 1; // '.'
            while (XmlCharType.IsDigit(this.CurrentChar))
            {
                NextChar(); len++;
            }
            return XmlConvertEx.ToXPathDouble(this._xpathExpr.Substring(start, len));
        }

        private string ScanString()
        {
            char endChar = this.CurrentChar;
            NextChar();
            int start = _xpathExprIndex - 1;
            int len = 0;
            while (this.CurrentChar != endChar)
            {
                if (!NextChar())
                {
                    throw XPathException.Create(SR.Xp_UnclosedString);
                }
                len++;
            }
            Debug.Assert(this.CurrentChar == endChar);
            NextChar();
            return this._xpathExpr.Substring(start, len);
        }

        private string ScanName()
        {
            Debug.Assert(_xmlCharType.IsStartNCNameSingleChar(this.CurrentChar)
#if XML10_FIFTH_EDITION
                || xmlCharType.IsNCNameHighSurrogateChar(this.CurerntChar)
#endif
                );
            int start = _xpathExprIndex - 1;
            int len = 0;

            for (; ;)
            {
                if (_xmlCharType.IsNCNameSingleChar(this.CurrentChar))
                {
                    NextChar();
                    len++;
                }
#if XML10_FIFTH_EDITION
                else if (xmlCharType.IsNCNameSurrogateChar(this.PeekNextChar(), this.CurerntChar))
                {
                    NextChar();
                    NextChar();
                    len += 2;
                }
#endif
                else
                {
                    break;
                }
            }
            return this._xpathExpr.Substring(start, len);
        }

        public enum LexKind
        {
            Comma = ',',
            Slash = '/',
            At = '@',
            Dot = '.',
            LParens = '(',
            RParens = ')',
            LBracket = '[',
            RBracket = ']',
            Star = '*',
            Plus = '+',
            Minus = '-',
            Eq = '=',
            Lt = '<',
            Gt = '>',
            Bang = '!',
            Dollar = '$',
            Apos = '\'',
            Quote = '"',
            Union = '|',
            Ne = 'N',   // !=
            Le = 'L',   // <=
            Ge = 'G',   // >=
            And = 'A',   // &&
            Or = 'O',   // ||
            DotDot = 'D',   // ..
            SlashSlash = 'S',   // //
            Name = 'n',   // XML _Name
            String = 's',   // Quoted string constant
            Number = 'd',   // _Number constant
            Axe = 'a',   // Axe (like child::)
            Eof = 'E',
        };
    }
}

