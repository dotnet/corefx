﻿using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Expressions.Tests.IndexExpression
{
    internal class SampleClassWithProperties
    {
        internal readonly PropertyInfo DefaultIndexer = typeof(List<int>).GetProperty("Item");
        internal readonly ConstantExpression[] DefaultArguments = { Expression.Constant(0) };

        internal MemberExpression DefaultPropertyExpression => Expression.Property(Expression.Constant(this),
            typeof(SampleClassWithProperties).GetProperty(nameof(DefaultProperty)));

        internal Expressions.IndexExpression DefaultIndexExpression => Expression.MakeIndex(
            DefaultPropertyExpression,
            DefaultIndexer,
            DefaultArguments);

        public List<int> DefaultProperty { get; set; }

        public List<int> AlternativeProperty { get; set; }
    }
}
