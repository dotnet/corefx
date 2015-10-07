﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

using Microsoft.Internal;

namespace System.Composition.Convention
{
    /// <summary>
    /// Configures a type as a MEF part.
    /// </summary>
    /// <typeparam name="T">The type of the part, or a type to which the part is assignable.</typeparam>
    public class PartConventionBuilder<T> : PartConventionBuilder
    {
        private class MethodExpressionAdapter
        {
            private readonly MethodInfo _methodInfo;

            public MethodExpressionAdapter(Expression<Action<T>> methodSelector)
            {
                _methodInfo = SelectMethods(methodSelector);
            }

            public bool VerifyMethodInfo(MethodInfo mi)
            {
                return mi == _methodInfo;
            }

            private static MethodInfo SelectMethods(Expression<Action<T>> methodSelector)
            {
                Requires.NotNull(methodSelector, "methodSelector");

                var expr = Reduce(methodSelector).Body;
                if (expr.NodeType == ExpressionType.Call)
                {
                    var memberInfo = ((MethodCallExpression)expr).Method as MethodInfo;
                    if (memberInfo != null)
                    {
                        return memberInfo;
                    }
                }

                // An error occurred the expression must be a void Method() Member Expression
                throw ExceptionBuilder.Argument_ExpressionMustBeVoidMethodWithNoArguments("methodSelector");
            }

            protected static Expression<Func<T, object>> Reduce(Expression<Func<T, object>> expr)
            {
                while (expr.CanReduce)
                {
                    expr = (Expression<Func<T, object>>)expr.Reduce();
                }
                return expr;
            }

            protected static Expression<Action<T>> Reduce(Expression<Action<T>> expr)
            {
                while (expr.CanReduce)
                {
                    expr = (Expression<Action<T>>)expr.Reduce();
                }
                return expr;
            }
        }

        private class PropertyExpressionAdapter
        {
            private readonly PropertyInfo _propertyInfo;
            private readonly Action<ImportConventionBuilder> _configureImport;
            private readonly Action<ExportConventionBuilder> _configureExport;

            public PropertyExpressionAdapter(
                Expression<Func<T, object>> propertySelector,
                Action<ImportConventionBuilder> configureImport = null,
                Action<ExportConventionBuilder> configureExport = null)
            {
                _propertyInfo = SelectProperties(propertySelector);
                _configureImport = configureImport;
                _configureExport = configureExport;
            }

            public bool VerifyPropertyInfo(PropertyInfo pi)
            {
                return pi == _propertyInfo;
            }

            public void ConfigureImport(PropertyInfo propertyInfo, ImportConventionBuilder importBuilder)
            {
                if (_configureImport != null)
                {
                    _configureImport(importBuilder);
                }
            }

            public void ConfigureExport(PropertyInfo propertyInfo, ExportConventionBuilder exportBuilder)
            {
                if (_configureExport != null)
                {
                    _configureExport(exportBuilder);
                }
            }

            private static PropertyInfo SelectProperties(Expression<Func<T, object>> propertySelector)
            {
                Requires.NotNull(propertySelector, "propertySelector");

                var expr = Reduce(propertySelector).Body;
                if (expr.NodeType == ExpressionType.MemberAccess)
                {
                    var memberInfo = ((MemberExpression)expr).Member as PropertyInfo;
                    if (memberInfo != null)
                    {
                        return memberInfo;
                    }
                }

                // An error occurred the expression must be a Property Member Expression
                throw ExceptionBuilder.Argument_ExpressionMustBePropertyMember("propertySelector");
            }

            protected static Expression<Func<T, object>> Reduce(Expression<Func<T, object>> expr)
            {
                while (expr.CanReduce)
                {
                    expr = (Expression<Func<T, object>>)expr.Reduce();
                }
                return expr;
            }
        }

        private class ConstructorExpressionAdapter
        {
            private ConstructorInfo _constructorInfo = null;
            private Dictionary<ParameterInfo, Action<ImportConventionBuilder>> _importBuilders = null;

            public ConstructorExpressionAdapter(Expression<Func<ParameterImportConventionBuilder, T>> selectConstructor)
            {
                ParseSelectConstructor(selectConstructor);
            }

            public ConstructorInfo SelectConstructor(IEnumerable<ConstructorInfo> constructorInfos)
            {
                return _constructorInfo;
            }

            public void ConfigureConstructorImports(ParameterInfo parameterInfo, ImportConventionBuilder importBuilder)
            {
                if (_importBuilders != null)
                {
                    Action<ImportConventionBuilder> parameterImportBuilder;
                    if (_importBuilders.TryGetValue(parameterInfo, out parameterImportBuilder))
                    {
                        parameterImportBuilder(importBuilder);
                    }
                }

                return;
            }

            private void ParseSelectConstructor(Expression<Func<ParameterImportConventionBuilder, T>> constructorSelector)
            {
                Requires.NotNull(constructorSelector, "constructorSelector");

                var expr = Reduce(constructorSelector).Body;
                if (expr.NodeType != ExpressionType.New)
                {
                    throw ExceptionBuilder.Argument_ExpressionMustBeNew("constructorSelector");
                }
                var newExpression = (NewExpression)expr;
                _constructorInfo = newExpression.Constructor;

                int index = 0;
                var parameterInfos = _constructorInfo.GetParameters();

                foreach (var argument in newExpression.Arguments)
                {
                    if (argument.NodeType == ExpressionType.Call)
                    {
                        var methodCallExpression = (MethodCallExpression)argument;
                        if (methodCallExpression.Arguments.Count() == 1)
                        {
                            var parameter = methodCallExpression.Arguments[0];
                            if (parameter.NodeType == ExpressionType.Lambda)
                            {
                                var lambdaExpression = (LambdaExpression)parameter;
                                var importDelegate = lambdaExpression.Compile();
                                if (_importBuilders == null)
                                {
                                    _importBuilders = new Dictionary<ParameterInfo, Action<ImportConventionBuilder>>();
                                }
                                _importBuilders.Add(parameterInfos[index], (Action<ImportConventionBuilder>)importDelegate);
                                ++index;
                            }
                        }
                    }
                }
            }

            private static Expression<Func<ParameterImportConventionBuilder, T>> Reduce(Expression<Func<ParameterImportConventionBuilder, T>> expr)
            {
                while (expr.CanReduce)
                {
                    expr.Reduce();
                }
                return expr;
            }
        }

        internal PartConventionBuilder(Predicate<Type> selectType) : base(selectType)
        {
        }

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Expression that selects a single constructor.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> SelectConstructor(Expression<Func<ParameterImportConventionBuilder, T>> constructorSelector)
        {
            Requires.NotNull(constructorSelector, "constructorSelector");

            var adapter = new ConstructorExpressionAdapter(constructorSelector);
            base.SelectConstructor(adapter.SelectConstructor, adapter.ConfigureConstructorImports);
            return this;
        }

        /// <summary>
        /// Select a property on the part to export.
        /// </summary>
        /// <param name="propertySelector">Expression that selects the exported property.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> ExportProperty(Expression<Func<T, object>> propertySelector)
        {
            return ExportProperty(propertySelector, null);
        }

        /// <summary>
        /// Select a property on the part to export.
        /// </summary>
        /// <param name="propertySelector">Expression that selects the exported property.</param>
        /// <param name="exportConfiguration">Action to configure selected properties.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> ExportProperty(
            Expression<Func<T, object>> propertySelector,
            Action<ExportConventionBuilder> exportConfiguration)
        {
            Requires.NotNull(propertySelector, "propertySelector");

            var adapter = new PropertyExpressionAdapter(propertySelector, null, exportConfiguration);
            base.ExportProperties(adapter.VerifyPropertyInfo, adapter.ConfigureExport);
            return this;
        }


        /// <summary>
        /// Select a property to export from the part.
        /// </summary>
        /// <typeparam name="TContract">Contract type to export.</typeparam>
        /// <param name="propertySelector">Expression to select the matching property.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> ExportProperty<TContract>(Expression<Func<T, object>> propertySelector)
        {
            return ExportProperty<TContract>(propertySelector, null);
        }

        /// <summary>
        /// Select a property to export from the part.
        /// </summary>
        /// <typeparam name="TContract">Contract type to export.</typeparam>
        /// <param name="propertySelector">Expression to select the matching property.</param>
        /// <param name="exportConfiguration">Action to configure selected properties.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> ExportProperty<TContract>(
            Expression<Func<T, object>> propertySelector,
            Action<ExportConventionBuilder> exportConfiguration)
        {
            Requires.NotNull(propertySelector, "propertySelector");

            var adapter = new PropertyExpressionAdapter(propertySelector, null, exportConfiguration);
            base.ExportProperties<TContract>(adapter.VerifyPropertyInfo, adapter.ConfigureExport);
            return this;
        }

        /// <summary>
        /// Select a property on the part to import.
        /// </summary>
        /// <param name="propertySelector">Expression selecting the property.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> ImportProperty(Expression<Func<T, object>> propertySelector)
        {
            return ImportProperty(propertySelector, null);
        }

        /// <summary>
        /// Select a property on the part to import.
        /// </summary>
        /// <param name="propertySelector">Expression selecting the property.</param>
        /// <param name="importConfiguration">Action configuring the imported property.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> ImportProperty(
            Expression<Func<T, object>> propertySelector,
            Action<ImportConventionBuilder> importConfiguration)
        {
            Requires.NotNull(propertySelector, "propertySelector");

            var adapter = new PropertyExpressionAdapter(propertySelector, importConfiguration, null);
            base.ImportProperties(adapter.VerifyPropertyInfo, adapter.ConfigureImport);
            return this;
        }

        /// <summary>
        /// Select a property on the part to import.
        /// </summary>
        /// <typeparam name="TContract">Contract type to import.</typeparam>
        /// <param name="propertySelector">Expression selecting the property.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> ImportProperty<TContract>(Expression<Func<T, object>> propertySelector)
        {
            return ImportProperty<TContract>(propertySelector, null);
        }

        /// <summary>
        /// Select a property on the part to import.
        /// </summary>
        /// <typeparam name="TContract">Contract type to import.</typeparam>
        /// <param name="propertySelector">Expression selecting the property.</param>
        /// <param name="importConfiguration">Action configuring the imported property.</param>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> ImportProperty<TContract>(
            Expression<Func<T, object>> propertySelector,
            Action<ImportConventionBuilder> importConfiguration)
        {
            Requires.NotNull(propertySelector, "propertySelector");

            var adapter = new PropertyExpressionAdapter(propertySelector, importConfiguration, null);
            base.ImportProperties<TContract>(adapter.VerifyPropertyInfo, adapter.ConfigureImport);
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public PartConventionBuilder<T> NotifyImportsSatisfied(Expression<Action<T>> methodSelector)
        {
            Requires.NotNull(methodSelector, "methodSelector");

            var adapter = new MethodExpressionAdapter(methodSelector);
            base.NotifyImportsSatisfied(adapter.VerifyMethodInfo);
            return this;
        }
    }
}
