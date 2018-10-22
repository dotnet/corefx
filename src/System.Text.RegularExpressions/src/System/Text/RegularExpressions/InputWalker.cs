// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.RegularExpressions
{
    internal ref struct InputWalker
    {
        public ReadOnlySpan<char> Input { get; }

        public int Position { get; private set; }

        public InputWalker(ReadOnlySpan<char> input)
        {
            Input = input;
            Position = 0;
        }

        /// <summary>
        /// Returns the char at the right of the current parsing position and advances to the right.
        /// </summary>
        public char RightCharMoveRight()
        {
            return Input[Position++];
        }

        /// <summary>
        /// Moves the current parsing position one to the left.
        /// </summary>
        public void MoveLeft()
        {
            --Position;
        }

        /// <summary>
        /// Number of characters to the right of the current parsing position.
        /// </summary>
        public int CharsRight()
        {
            return Input.Length - Position;
        }

        /// <summary>
        /// Returns the char right of the current parsing position.
        /// </summary>
        /// <returns></returns>
        public char RightChar()
        {
            return Input[Position];
        }

        /// <summary>
        /// Moves the current position to the right.
        /// </summary>
        public void MoveRight(int i = 1)
        {
            Position += i;
        }

        /// <summary>
        /// Returns the current parsing position.
        /// </summary>
        public int Textpos()
        {
            return Position;
        }

        /// <summary>
        /// Zaps to a specific parsing position.
        /// </summary>
        /// <param name="position"></param>
        public void Textto(int position)
        {
            Position = position;
        }
    }
}
