// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Reflection
{
    /// <summary>
    /// Contains static methods for retrieving custom attributes.
    /// </summary>
    public static partial class CustomAttributeExtensions
    {
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A custom attribute that matches <paramref name="attributeType" />, or null if no such attribute
        /// is found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        public static System.Attribute GetCustomAttribute(this System.Reflection.Assembly element, System.Type attributeType) { return default(System.Attribute); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A custom attribute that matches <paramref name="attributeType" />, or null if no such attribute
        /// is found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Attribute GetCustomAttribute(this System.Reflection.MemberInfo element, System.Type attributeType) { return default(System.Attribute); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified member, and
        /// optionally inspects the ancestors of that member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <returns>
        /// A custom attribute that matches <paramref name="attributeType" />, or null if no such attribute
        /// is found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Attribute GetCustomAttribute(this System.Reflection.MemberInfo element, System.Type attributeType, bool inherit) { return default(System.Attribute); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A custom attribute that matches <paramref name="attributeType" />, or null if no such attribute
        /// is found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        public static System.Attribute GetCustomAttribute(this System.Reflection.Module element, System.Type attributeType) { return default(System.Attribute); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A custom attribute that matches <paramref name="attributeType" />, or null if no such attribute
        /// is found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Attribute GetCustomAttribute(this System.Reflection.ParameterInfo element, System.Type attributeType) { return default(System.Attribute); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified parameter,
        /// and optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <returns>
        /// A custom attribute matching <paramref name="attributeType" />, or null if no such attribute
        /// is found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Attribute GetCustomAttribute(this System.Reflection.ParameterInfo element, System.Type attributeType, bool inherit) { return default(System.Attribute); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A custom attribute that matches <paramref name="T" />, or null if no such attribute is found.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        public static T GetCustomAttribute<T>(this System.Reflection.Assembly element) where T : System.Attribute { return default(T); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A custom attribute that matches <paramref name="T" />, or null if no such attribute is found.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static T GetCustomAttribute<T>(this System.Reflection.MemberInfo element) where T : System.Attribute { return default(T); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified member, and
        /// optionally inspects the ancestors of that member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A custom attribute that matches <paramref name="T" />, or null if no such attribute is found.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static T GetCustomAttribute<T>(this System.Reflection.MemberInfo element, bool inherit) where T : System.Attribute { return default(T); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A custom attribute that matches <paramref name="T" />, or null if no such attribute is found.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        public static T GetCustomAttribute<T>(this System.Reflection.Module element) where T : System.Attribute { return default(T); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A custom attribute that matches <paramref name="T" />, or null if no such attribute is found.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static T GetCustomAttribute<T>(this System.Reflection.ParameterInfo element) where T : System.Attribute { return default(T); }
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified parameter,
        /// and optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A custom attribute that matches <paramref name="T" />, or null if no such attribute is found.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        /// More than one of the requested attributes was found.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static T GetCustomAttribute<T>(this System.Reflection.ParameterInfo element, bool inherit) where T : System.Attribute { return default(T); }
        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" />, or
        /// an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.Assembly element) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="attributeType" />, or an empty collection if no such attributes
        /// exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.Assembly element, System.Type attributeType) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" />, or
        /// an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.MemberInfo element) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified member, and optionally
        /// inspects the ancestors of that member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> that
        /// match the specified criteria, or an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.MemberInfo element, bool inherit) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="attributeType" />, or an empty collection if no such attributes
        /// exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.MemberInfo element, System.Type attributeType) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// member, and optionally inspects the ancestors of that member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="attributeType" />, or an empty collection if no such attributes
        /// exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.MemberInfo element, System.Type attributeType, bool inherit) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" />, or
        /// an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.Module element) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="attributeType" />, or an empty collection if no such attributes
        /// exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.Module element, System.Type attributeType) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" />, or
        /// an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.ParameterInfo element) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified parameter, and
        /// optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" />, or
        /// an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.ParameterInfo element, bool inherit) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="attributeType" />, or an empty collection if no such attributes
        /// exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.ParameterInfo element, System.Type attributeType) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// parameter, and optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="attributeType" />, or an empty collection if no such attributes
        /// exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.ParameterInfo element, System.Type attributeType, bool inherit) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="T" />, or an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.Assembly element) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="T" />, or an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.MemberInfo element) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// member, and optionally inspects the ancestors of that member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="T" />, or an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.MemberInfo element, bool inherit) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="T" />, or an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.Module element) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="T" />, or an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.ParameterInfo element) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified
        /// parameter, and optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <returns>
        /// A collection of the custom attributes that are applied to <paramref name="element" /> and
        /// that match <paramref name="T" />, or an empty collection if no such attributes exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="element" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.ParameterInfo element, bool inherit) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <param name="attributeType">The type of the attribute to search for.</param>
        /// <returns>
        /// true if an attribute of the specified type is applied to <paramref name="element" />; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        public static bool IsDefined(this System.Reflection.Assembly element, System.Type attributeType) { return default(bool); }
        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// true if an attribute of the specified type is applied to <paramref name="element" />; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        public static bool IsDefined(this System.Reflection.MemberInfo element, System.Type attributeType) { return default(bool); }
        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified member,
        /// and, optionally, applied to its ancestors.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of the attribute to search for.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <returns>
        /// true if an attribute of the specified type is applied to <paramref name="element" />; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="element" /> is not a constructor, method, property, event, type, or field.
        /// </exception>
        public static bool IsDefined(this System.Reflection.MemberInfo element, System.Type attributeType, bool inherit) { return default(bool); }
        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// true if an attribute of the specified type is applied to <paramref name="element" />; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        public static bool IsDefined(this System.Reflection.Module element, System.Type attributeType) { return default(bool); }
        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// true if an attribute of the specified type is applied to <paramref name="element" />; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        public static bool IsDefined(this System.Reflection.ParameterInfo element, System.Type attributeType) { return default(bool); }
        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified parameter,
        /// and, optionally, applied to its ancestors.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="element" />; otherwise, false.</param>
        /// <returns>
        /// true if an attribute of the specified type is applied to <paramref name="element" />; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> or <paramref name="attributeType" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributeType" /> is not derived from <see cref="Attribute" />.
        /// </exception>
        public static bool IsDefined(this System.Reflection.ParameterInfo element, System.Type attributeType, bool inherit) { return default(bool); }
    }
    /// <summary>
    /// Retrieves the mapping of an interface into the actual methods on a class that implements that
    /// interface.
    /// </summary>
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct InterfaceMapping
    {
        /// <summary>
        /// Shows the methods that are defined on the interface.
        /// </summary>
        public System.Reflection.MethodInfo[] InterfaceMethods;
        /// <summary>
        /// Shows the type that represents the interface.
        /// </summary>
        public System.Type InterfaceType;
        /// <summary>
        /// Shows the methods that implement the interface.
        /// </summary>
        public System.Reflection.MethodInfo[] TargetMethods;
        /// <summary>
        /// Represents the type that was used to create the interface mapping.
        /// </summary>
        public System.Type TargetType;
    }
    /// <summary>
    /// Provides methods that retrieve information about types at run time.
    /// </summary>
    public static partial class RuntimeReflectionExtensions
    {
        /// <summary>
        /// Gets an object that represents the method represented by the specified delegate.
        /// </summary>
        /// <param name="del">The delegate to examine.</param>
        /// <returns>
        /// An object that represents the method.
        /// </returns>
        public static System.Reflection.MethodInfo GetMethodInfo(this System.Delegate del) { return default(System.Reflection.MethodInfo); }
        /// <summary>
        /// Retrieves an object that represents the specified method on the direct or indirect base class
        /// where the method was first declared.
        /// </summary>
        /// <param name="method">The method to retrieve information about.</param>
        /// <returns>
        /// An object that represents the specified method's initial declaration on a base class.
        /// </returns>
        public static System.Reflection.MethodInfo GetRuntimeBaseDefinition(this System.Reflection.MethodInfo method) { return default(System.Reflection.MethodInfo); }
        /// <summary>
        /// Retrieves an object that represents the specified event.
        /// </summary>
        /// <param name="type">The type that contains the event.</param>
        /// <param name="name">The name of the event.</param>
        /// <returns>
        /// An object that represents the specified event, or null if the event is not found.
        /// </returns>
        public static System.Reflection.EventInfo GetRuntimeEvent(this System.Type type, string name) { return default(System.Reflection.EventInfo); }
        /// <summary>
        /// Retrieves a collection that represents all the events defined on a specified type.
        /// </summary>
        /// <param name="type">The type that contains the events.</param>
        /// <returns>
        /// A collection of events for the specified type.
        /// </returns>
        public static System.Collections.Generic.IEnumerable<System.Reflection.EventInfo> GetRuntimeEvents(this System.Type type) { return default(System.Collections.Generic.IEnumerable<System.Reflection.EventInfo>); }
        /// <summary>
        /// Retrieves an object that represents a specified field.
        /// </summary>
        /// <param name="type">The type that contains the field.</param>
        /// <param name="name">The name of the field.</param>
        /// <returns>
        /// An object that represents the specified field, or null if the field is not found.
        /// </returns>
        public static System.Reflection.FieldInfo GetRuntimeField(this System.Type type, string name) { return default(System.Reflection.FieldInfo); }
        /// <summary>
        /// Retrieves a collection that represents all the fields defined on a specified type.
        /// </summary>
        /// <param name="type">The type that contains the fields.</param>
        /// <returns>
        /// A collection of fields for the specified type.
        /// </returns>
        public static System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo> GetRuntimeFields(this System.Type type) { return default(System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo>); }
        /// <summary>
        /// Returns an interface mapping for the specified type and the specified interface.
        /// </summary>
        /// <param name="typeInfo">The type to retrieve a mapping for.</param>
        /// <param name="interfaceType">The interface to retrieve a mapping for.</param>
        /// <returns>
        /// An object that represents the interface mapping for the specified interface and type.
        /// </returns>
        public static System.Reflection.InterfaceMapping GetRuntimeInterfaceMap(this System.Reflection.TypeInfo typeInfo, System.Type interfaceType) { return default(System.Reflection.InterfaceMapping); }
        /// <summary>
        /// Retrieves an object that represents a specified method.
        /// </summary>
        /// <param name="type">The type that contains the method.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="parameters">An array that contains the method's parameters.</param>
        /// <returns>
        /// An object that represents the specified method, or null if the method is not found.
        /// </returns>
        public static System.Reflection.MethodInfo GetRuntimeMethod(this System.Type type, string name, System.Type[] parameters) { return default(System.Reflection.MethodInfo); }
        /// <summary>
        /// Retrieves a collection that represents all methods defined on a specified type.
        /// </summary>
        /// <param name="type">The type that contains the methods.</param>
        /// <returns>
        /// A collection of methods for the specified type.
        /// </returns>
        public static System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo> GetRuntimeMethods(this System.Type type) { return default(System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>); }
        /// <summary>
        /// Retrieves a collection that represents all the properties defined on a specified type.
        /// </summary>
        /// <param name="type">The type that contains the properties.</param>
        /// <returns>
        /// A collection of properties for the specified type.
        /// </returns>
        public static System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo> GetRuntimeProperties(this System.Type type) { return default(System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo>); }
        /// <summary>
        /// Retrieves an object that represents a specified property.
        /// </summary>
        /// <param name="type">The type that contains the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        /// An object that represents the specified property, or null if the property is not found.
        /// </returns>
        public static System.Reflection.PropertyInfo GetRuntimeProperty(this System.Type type, string name) { return default(System.Reflection.PropertyInfo); }
    }
}
