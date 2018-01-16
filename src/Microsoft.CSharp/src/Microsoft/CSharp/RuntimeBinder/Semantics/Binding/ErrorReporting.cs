// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Errors;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed partial class ExpressionBinder
    {
        private RuntimeBinderException ReportReadOnlyError(ExprField field, bool isNested)
        {
            Debug.Assert(field != null);

            FieldWithType fieldWithType = field.FieldWithType;
            bool isStatic = fieldWithType.Field().isStatic;
            ErrArg[] args;
            ErrorCode err;
            if (isNested)
            {
                args = new ErrArg[]{ fieldWithType };
                err = isStatic ? ErrorCode.ERR_AssgReadonlyStatic2 : ErrorCode.ERR_AssgReadonly2;
            }
            else
            {
                args = Array.Empty<ErrArg>();
                err = isStatic ? ErrorCode.ERR_AssgReadonlyStatic : ErrorCode.ERR_AssgReadonly;
            }

            return ErrorContext.Error(err, args);
        }

        private void TryReportLvalueFailure(Expr expr, CheckLvalueKind kind)
        {
            Debug.Assert(expr != null);
            Debug.Assert(!(expr is ExprLocal));

            // We have a lvalue failure. Was the reason because this field
            // was marked readonly? Give special messages for this case.

            Debug.Assert(expr != null);

            // We've already reported read-only-property errors.
            Debug.Assert(!(expr is ExprProperty), "No other property readonly failure possible.");
            if (expr is ExprField field)
            {
				Debug.Assert(field.FieldWithType.Field().isReadOnly);
                throw ReportReadOnlyError(field, false);
            }

            throw ErrorContext.Error(GetStandardLvalueError(kind));
        }
    }
}
