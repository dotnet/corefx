// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Represents accessing a field or property.
    /// </summary>
    [DebuggerTypeProxy(typeof(Expression.MemberExpressionProxy))]
    public class MemberExpression : Expression
    {
        private readonly Expression _expression;

        /// <summary>
        /// Gets the field or property to be accessed.
        /// </summary>
        public MemberInfo Member
        {
            get { return GetMember(); }
        }

        /// <summary>
        /// Gets the containing object of the field or property.
        /// </summary>
        public Expression Expression
        {
            get { return _expression; }
        }

        // param order: factories args in order, then other args
        internal MemberExpression(Expression expression)
        {
            _expression = expression;
        }

        internal static PropertyExpression Make(Expression expression, PropertyInfo property)
        {
            Debug.Assert(property != null);
            return new PropertyExpression(expression, property);
        }

        internal static FieldExpression Make(Expression expression, FieldInfo field)
        {
            Debug.Assert(field != null);
            return new FieldExpression(expression, field);
        }

        internal static MemberExpression Make(Expression expression, MemberInfo member)
        {
            FieldInfo fi = member as FieldInfo;
            return fi == null ? (MemberExpression)Make(expression, (PropertyInfo)member) : Make(expression, fi);
        }

        /// <summary>
        /// Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        public sealed override ExpressionType NodeType
        {
            get { return ExpressionType.MemberAccess; }
        }

        [ExcludeFromCodeCoverage] // Unreachable
        internal virtual MemberInfo GetMember()
        {
            throw ContractUtils.Unreachable;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitMember(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public MemberExpression Update(Expression expression)
        {
            if (expression == Expression)
            {
                return this;
            }
            return Expression.MakeMemberAccess(expression, Member);
        }
    }

    internal class FieldExpression : MemberExpression
    {
        private readonly FieldInfo _field;

        public FieldExpression(Expression expression, FieldInfo member)
            : base(expression)
        {
            _field = member;
        }

        internal override MemberInfo GetMember()
        {
            return _field;
        }

        public sealed override Type Type
        {
            get { return _field.FieldType; }
        }
    }

    internal class PropertyExpression : MemberExpression
    {
        private readonly PropertyInfo _property;
        public PropertyExpression(Expression expression, PropertyInfo member)
            : base(expression)
        {
            _property = member;
        }

        internal override MemberInfo GetMember()
        {
            return _property;
        }

        public sealed override Type Type
        {
            get { return _property.PropertyType; }
        }
    }

    public partial class Expression
    {
        #region Field

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a field.
        /// </summary>
        /// <param name="expression">The containing object of the field.  This can be null for static fields.</param>
        /// <param name="field">The field to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames")]
        public static MemberExpression Field(Expression expression, FieldInfo field)
        {
            ContractUtils.RequiresNotNull(field, nameof(field));

            if (field.IsStatic)
            {
                if (expression != null) throw new ArgumentException(Strings.OnlyStaticFieldsHaveNullInstance, nameof(expression));
            }
            else
            {
                if (expression == null) throw new ArgumentException(Strings.OnlyStaticFieldsHaveNullInstance, nameof(field));
                RequiresCanRead(expression, nameof(expression));
                if (!TypeUtils.AreReferenceAssignable(field.DeclaringType, expression.Type))
                {
                    throw Error.FieldInfoNotDefinedForType(field.DeclaringType, field.Name, expression.Type);
                }
            }
            return MemberExpression.Make(expression, field);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a field.
        /// </summary>
        /// <param name="expression">The containing object of the field.  This can be null for static fields.</param>
        /// <param name="fieldName">The field to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Field(Expression expression, string fieldName)
        {
            RequiresCanRead(expression, nameof(expression));

            // bind to public names first
            FieldInfo fi = expression.Type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (fi == null)
            {
                fi = expression.Type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            }
            if (fi == null)
            {
                throw Error.InstanceFieldNotDefinedForType(fieldName, expression.Type);
            }
            return Expression.Field(expression, fi);
        }


        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a field.
        /// </summary>
        /// <param name="expression">The containing object of the field.  This can be null for static fields.</param>
        /// <param name="type">The <see cref="Type"/> containing the field.</param>
        /// <param name="fieldName">The field to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames")]
        public static MemberExpression Field(Expression expression, Type type, string fieldName)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));

            // bind to public names first
            FieldInfo fi = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (fi == null)
            {
                fi = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            }

            if (fi == null)
            {
                throw Error.FieldNotDefinedForType(fieldName, type);
            }
            return Expression.Field(expression, fi);
        }
        #endregion

        #region Property

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property.
        /// </summary>
        /// <param name="expression">The containing object of the property.  This can be null for static properties.</param>
        /// <param name="propertyName">The property to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Property(Expression expression, string propertyName)
        {
            RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(propertyName, nameof(propertyName));
            // bind to public names first
            PropertyInfo pi = expression.Type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (pi == null)
            {
                pi = expression.Type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            }
            if (pi == null)
            {
                throw Error.InstancePropertyNotDefinedForType(propertyName, expression.Type);
            }
            return Property(expression, pi);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property.
        /// </summary>
        /// <param name="expression">The containing object of the property.  This can be null for static properties.</param>
        /// <param name="type">The <see cref="Type"/> containing the property.</param>
        /// <param name="propertyName">The property to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Property(Expression expression, Type type, string propertyName)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));
            ContractUtils.RequiresNotNull(propertyName, nameof(propertyName));
            // bind to public names first
            PropertyInfo pi = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (pi == null)
            {
                pi = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            }
            if (pi == null)
            {
                throw Error.PropertyNotDefinedForType(propertyName, type);
            }
            return Property(expression, pi);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property.
        /// </summary>
        /// <param name="expression">The containing object of the property.  This can be null for static properties.</param>
        /// <param name="property">The property to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames")]
        public static MemberExpression Property(Expression expression, PropertyInfo property)
        {
            ContractUtils.RequiresNotNull(property, nameof(property));

            MethodInfo mi = property.GetGetMethod(true);

            if (mi == null)
            {
                mi = property.GetSetMethod(true);

                if (mi == null)
                {
                    throw Error.PropertyDoesNotHaveAccessor(property);
                }
                else if (mi.GetParametersCached().Length != 1)
                {
                    throw Error.IncorrectNumberOfMethodCallArguments(mi);
                }
            }
            else if (mi.GetParametersCached().Length != 0)
            {
                throw Error.IncorrectNumberOfMethodCallArguments(mi);
            }

            if (mi.IsStatic)
            {
                if (expression != null) throw new ArgumentException(Strings.OnlyStaticPropertiesHaveNullInstance, nameof(expression));
            }
            else
            {
                if (expression == null) throw new ArgumentException(Strings.OnlyStaticPropertiesHaveNullInstance, nameof(property));
                RequiresCanRead(expression, nameof(expression));
                if (!TypeUtils.IsValidInstanceType(property, expression.Type))
                {
                    throw Error.PropertyNotDefinedForType(property, expression.Type);
                }
            }

            return MemberExpression.Make(expression, property);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property.
        /// </summary>
        /// <param name="expression">The containing object of the property.  This can be null for static properties.</param>
        /// <param name="propertyAccessor">An accessor method of the property to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor)
        {
            ContractUtils.RequiresNotNull(propertyAccessor, nameof(propertyAccessor));
            ValidateMethodInfo(propertyAccessor);
            return Property(expression, GetProperty(propertyAccessor));
        }

        private static PropertyInfo GetProperty(MethodInfo mi)
        {
            Type type = mi.DeclaringType;
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic;
            flags |= (mi.IsStatic) ? BindingFlags.Static : BindingFlags.Instance;
            var props = type.GetProperties(flags);
            foreach (PropertyInfo pi in props)
            {
                if (pi.CanRead && CheckMethod(mi, pi.GetGetMethod(true)))
                {
                    return pi;
                }
                if (pi.CanWrite && CheckMethod(mi, pi.GetSetMethod(true)))
                {
                    return pi;
                }
            }
            throw Error.MethodNotPropertyAccessor(mi.DeclaringType, mi.Name);
        }

        private static bool CheckMethod(MethodInfo method, MethodInfo propertyMethod)
        {
            if (method.Equals(propertyMethod))
            {
                return true;
            }
            // If the type is an interface then the handle for the method got by the compiler will not be the 
            // same as that returned by reflection.
            // Check for this condition and try and get the method from reflection.
            Type type = method.DeclaringType;
            if (type.GetTypeInfo().IsInterface && method.Name == propertyMethod.Name && type.GetMethod(method.Name) == propertyMethod)
            {
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property or field.
        /// </summary>
        /// <param name="expression">The containing object of the member.  This can be null for static members.</param>
        /// <param name="propertyOrFieldName">The member to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression PropertyOrField(Expression expression, string propertyOrFieldName)
        {
            RequiresCanRead(expression, nameof(expression));
            // bind to public names first
            PropertyInfo pi = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (pi != null)
                return Property(expression, pi);
            FieldInfo fi = expression.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (fi != null)
                return Field(expression, fi);
            pi = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (pi != null)
                return Property(expression, pi);
            fi = expression.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (fi != null)
                return Field(expression, fi);

            throw Error.NotAMemberOfType(propertyOrFieldName, expression.Type);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property or field.
        /// </summary>
        /// <param name="expression">The containing object of the member.  This can be null for static members.</param>
        /// <param name="member">The member to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression MakeMemberAccess(Expression expression, MemberInfo member)
        {
            ContractUtils.RequiresNotNull(member, nameof(member));

            FieldInfo fi = member as FieldInfo;
            if (fi != null)
            {
                return Expression.Field(expression, fi);
            }
            PropertyInfo pi = member as PropertyInfo;
            if (pi != null)
            {
                return Expression.Property(expression, pi);
            }
            throw Error.MemberNotFieldOrProperty(member);
        }
    }
}
