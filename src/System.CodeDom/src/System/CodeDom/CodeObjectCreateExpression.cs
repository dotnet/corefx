// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.CodeDom
{
    public class CodeObjectCreateExpression : CodeExpression
    {
        private CodeTypeReference _createType;

        public CodeObjectCreateExpression() { }

        public CodeObjectCreateExpression(CodeTypeReference createType, params CodeExpression[] parameters)
        {
            CreateType = createType;
            Parameters.AddRange(parameters);
        }

        public CodeObjectCreateExpression(string createType, params CodeExpression[] parameters)
        {
            CreateType = new CodeTypeReference(createType);
            Parameters.AddRange(parameters);
        }

        public CodeObjectCreateExpression(Type createType, params CodeExpression[] parameters)
        {
            CreateType = new CodeTypeReference(createType);
            Parameters.AddRange(parameters);
        }

        public CodeTypeReference CreateType
        {
            get { return _createType ?? (_createType = new CodeTypeReference("")); }
            set { _createType = value; }
        }

        public CodeExpressionCollection Parameters { get; } = new CodeExpressionCollection();
    }
}
