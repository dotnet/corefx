// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Linq
{
    /// <summary>
    ///   The base class that represents a single node within a mutable JSON document.
    /// </summary>
    public abstract partial class JNode
    {
        private protected JNode() { }

        /// <summary>
        ///   Transforms this instance into <see cref="JsonElement"/> representation.
        ///   Operations performed on this instance will modify the returned <see cref="JsonElement"/>.
        /// </summary>
        /// <returns>Mutable <see cref="JsonElement"/> with <see cref="JNode"/> underneath.</returns>
        public JsonElement AsJsonElement() => new JsonElement(this);

        /// <summary>
        ///   The <see cref="JsonValueKind"/> that the node is.
        /// </summary>
        public abstract JsonValueKind ValueKind { get; }

        /// <summary>
        ///   Gets the <see cref="JNode"/> represented by <paramref name="jsonElement"/>.
        ///   Operations performed on the returned <see cref="JNode"/> will modify the <paramref name="jsonElement"/>.
        /// </summary>
        /// <param name="jsonElement"><see cref="JsonElement"/> to get the <see cref="JNode"/> from.</param>
        /// <returns><see cref="JNode"/> represented by <paramref name="jsonElement"/>.</returns>
        /// <exception cref="ArgumentException">
        ///   Provided <see cref="JsonElement"/> was not build from <see cref="JNode"/>.
        /// </exception>
        public static JNode GetNode(JsonElement jsonElement) => !jsonElement.IsImmutable ? (JNode)jsonElement._parent : throw new ArgumentException(SR.NotNodeJsonElementParent);

        /// <summary>
        ///    Gets the <see cref="JNode"/> represented by the <paramref name="jsonElement"/>.
        ///    Operations performed on the returned <see cref="JNode"/> will modify the <paramref name="jsonElement"/>.
        ///    A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="jsonElement"><see cref="JsonElement"/> to get the <see cref="JNode"/> from.</param>
        /// <param name="jsonNode"><see cref="JNode"/> represented by <paramref name="jsonElement"/>.</param>
        /// <returns>
        ///  <see langword="true"/> if the operation succeded;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public static bool TryGetNode(JsonElement jsonElement, out JNode jsonNode)
        {
            if (!jsonElement.IsImmutable)
            {
                jsonNode = (JNode)jsonElement._parent;
                return true;
            }

            jsonNode = null;
            return false;
        }

        /// <summary>
        ///   Performs a deep copy operation on this instance.
        /// </summary>
        /// <returns><see cref="JNode"/> which is a clone of this instance.</returns>
        public abstract JNode Clone();

        /// <summary>
        ///   Converts a <see cref="string"/> to a <see cref="JString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(string value)
        {
            if (value == null)
            {
                return new JNull();
            }

            return new JString(value);
        }

        /// <summary>
        ///   Converts a <see cref="DateTime"/> to a <see cref="JString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(DateTime value) => new JString(value);

        /// <summary>
        ///   Converts a <see cref="DateTimeOffset"/> to a <see cref="JString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(DateTimeOffset value) => new JString(value);

        /// <summary>
        ///   Converts a <see cref="Guid"/> to a <see cref="JString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(Guid value) => new JString(value);

        /// <summary>
        ///   Converts a <see cref="bool"/> to a <see cref="JBoolean"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(bool value) => new JBoolean(value);

        /// <summary>
        ///   Converts a <see cref="byte"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(byte value) => new JNumber(value);

        /// <summary>
        ///   Converts a <see cref="short"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(short value) => new JNumber(value);

        /// <summary>
        ///   Converts an <see cref="int"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(int value) => new JNumber(value);

        /// <summary>
        ///   Converts a <see cref="long"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(long value) => new JNumber(value);

        /// <summary>
        ///    Converts a <see cref="float"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(float value)
        {
            if (float.IsInfinity(value) || float.IsNaN(value))
            {
                return new JString(value.ToString());
            }

            return new JNumber(value);
        }

        /// <summary>
        ///    Converts a <see cref="double"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                return new JString(value.ToString());
            }

            return new JNumber(value);
        }

        /// <summary>
        ///    Converts a <see cref="sbyte"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JNode(sbyte value) => new JNumber(value);

        /// <summary>
        ///    Converts a <see cref="ushort"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JNode(ushort value) => new JNumber(value);

        /// <summary>
        ///    Converts a <see cref="uint"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JNode(uint value) => new JNumber(value);

        /// <summary>
        ///    Converts a <see cref="ulong"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JNode(ulong value) => new JNumber(value);

        /// <summary>
        ///    Converts a <see cref="decimal"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JNode(decimal value) => new JNumber(value);
    }
}
