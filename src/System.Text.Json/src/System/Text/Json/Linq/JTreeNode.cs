// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Linq
{
    /// <summary>
    ///   The base class that represents a single node within a mutable JSON document. See also <see cref="JTreeObject"/> and <see cref="JTreeArray"/> which derive from <see cref="JTreeNode"/>.
    /// </summary>
    /// <seealso cref="JTreeObject"/>
    /// <seealso cref="JTreeArray"/>
    public abstract partial class JTreeNode
    {
        private protected JTreeNode() { }

        /// <summary>
        ///   Transforms this instance into <see cref="JsonElement"/> representation.
        ///   Operations performed on this instance will modify the returned <see cref="JsonElement"/>.
        /// </summary>
        /// <returns>Mutable <see cref="JsonElement"/> with <see cref="JTreeNode"/> underneath.</returns>
        public JsonElement AsJsonElement() => new JsonElement(this);

        /// <summary>
        ///   The <see cref="JsonValueKind"/> that the node is.
        /// </summary>
        public abstract JsonValueKind ValueKind { get; }

        /// <summary>
        ///   Gets the <see cref="JTreeNode"/> represented by <paramref name="jsonElement"/> if it was already backed by one.
        ///   Operations performed on the returned <see cref="JTreeNode"/> will modify the <paramref name="jsonElement"/>.
        /// </summary>
        /// <param name="jsonElement"><see cref="JsonElement"/> to get the <see cref="JTreeNode"/> from.</param>
        /// <returns><see cref="JTreeNode"/> represented by <paramref name="jsonElement"/>.</returns>
        /// <exception cref="ArgumentException">
        ///   Provided <see cref="JsonElement"/> was not build from <see cref="JTreeNode"/>.
        /// </exception>
        public static JTreeNode GetOriginatingNode(JsonElement jsonElement) => !jsonElement.IsImmutable ? (JTreeNode)jsonElement._parent : throw new ArgumentException(SR.NotNodeJsonElementParent);

        /// <summary>
        ///    Gets the <see cref="JTreeNode"/> represented by the <paramref name="jsonElement"/> if it was already backed by one.
        ///    Operations performed on the returned <see cref="JTreeNode"/> will modify the <paramref name="jsonElement"/>.
        ///    A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="jsonElement"><see cref="JsonElement"/> to get the <see cref="JTreeNode"/> from.</param>
        /// <param name="jsonNode"><see cref="JTreeNode"/> represented by <paramref name="jsonElement"/>.</param>
        /// <returns>
        ///  <see langword="true"/> if the operation succeded;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public static bool TryGetOriginatingNode(JsonElement jsonElement, out JTreeNode jsonNode)
        {
            if (!jsonElement.IsImmutable)
            {
                jsonNode = (JTreeNode)jsonElement._parent;
                return true;
            }

            jsonNode = null;
            return false;
        }

        /// <summary>
        ///   Performs a deep copy operation on this instance.
        /// </summary>
        /// <returns><see cref="JTreeNode"/> which is a clone of this instance.</returns>
        public abstract JTreeNode Clone();

        /// <summary>
        ///   Converts a <see cref="string"/> to a <see cref="JTreeString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(string value)
        {
            if (value == null)
            {
                return new JTreeNull();
            }

            return new JTreeString(value);
        }

        /// <summary>
        ///   Converts a <see cref="DateTime"/> to a <see cref="JTreeString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(DateTime value) => new JTreeString(value);

        /// <summary>
        ///   Converts a <see cref="DateTimeOffset"/> to a <see cref="JTreeString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(DateTimeOffset value) => new JTreeString(value);

        /// <summary>
        ///   Converts a <see cref="Guid"/> to a <see cref="JTreeString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(Guid value) => new JTreeString(value);

        /// <summary>
        ///   Converts a <see cref="bool"/> to a <see cref="JTreeBoolean"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(bool value) => new JTreeBoolean(value);

        /// <summary>
        ///   Converts a <see cref="byte"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(byte value) => new JTreeNumber(value);

        /// <summary>
        ///   Converts a <see cref="short"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(short value) => new JTreeNumber(value);

        /// <summary>
        ///   Converts an <see cref="int"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(int value) => new JTreeNumber(value);

        /// <summary>
        ///   Converts a <see cref="long"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(long value) => new JTreeNumber(value);

        /// <summary>
        ///    Converts a <see cref="float"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(float value)
        {
            if (float.IsInfinity(value) || float.IsNaN(value))
            {
                return new JTreeString(value.ToString());
            }

            return new JTreeNumber(value);
        }

        /// <summary>
        ///    Converts a <see cref="double"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                return new JTreeString(value.ToString());
            }

            return new JTreeNumber(value);
        }

        /// <summary>
        ///    Converts a <see cref="sbyte"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JTreeNode(sbyte value) => new JTreeNumber(value);

        /// <summary>
        ///    Converts a <see cref="ushort"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JTreeNode(ushort value) => new JTreeNumber(value);

        /// <summary>
        ///    Converts a <see cref="uint"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JTreeNode(uint value) => new JTreeNumber(value);

        /// <summary>
        ///    Converts a <see cref="ulong"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JTreeNode(ulong value) => new JTreeNumber(value);

        /// <summary>
        ///    Converts a <see cref="decimal"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JTreeNode(decimal value) => new JTreeNumber(value);
    }
}
