// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Expressions.Tests
{
    partial class ExpressionCatalog
    {
        private static IEnumerable<Expression> MemberAccess()
        {
            var instance = Expression.Constant(new Members());

            foreach (var member in typeof(Members).GetMembers().Where(m => m is FieldInfo || m is PropertyInfo))
            {
                var isStatic = false;
                if (member is FieldInfo)
                {
                    var field = (FieldInfo)member;
                    isStatic = field.IsStatic || field.IsLiteral;
                }
                else
                {
                    var property = (PropertyInfo)member;
                    isStatic = property.GetGetMethod().IsStatic;
                }

                if (isStatic)
                {
                    yield return Expression.MakeMemberAccess(null, member);
                }
                else
                {
                    yield return Expression.MakeMemberAccess(instance, member);
                    yield return Expression.MakeMemberAccess(Expression.Default(typeof(Members)), member);
                }
            }
        }
    }

    class Members
    {
        public static readonly int A = 42;

        public const int B = 43;

        public static int C
        {
            get { return 44; }
        }

        public static readonly string D = "bar";

        public const string E = "foo";

        public static string F
        {
            get { return "qux"; }
        }

        public int G = 45;

        public int H
        {
            get { return 46; }
        }

        public string I = "baz";

        public string J
        {
            get { return "foz"; }
        }

        public static int K = 47;
    }
}
