// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.RegularExpressions
{
    internal class CharCategory
    {
        private const byte Q = 5;    // quantifier
        private const byte S = 4;    // ordinary stopper
        private const byte Z = 3;    // ScanBlank stopper
        private const byte X = 2;    // whitespace
        private const byte E = 1;    // should be escaped

        /// <summary>
        /// For categorizing ASCII characters.
        /// </summary>
        private static ReadOnlySpan<byte> Category => new byte[]
        {
            // 0 1 2 3 4 5 6 7 8 9 A B C D E F 0 1 2 3 4 5 6 7 8 9 A B C D E F
               0,0,0,0,0,0,0,0,0,X,X,0,X,X,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            //   ! " # $ % & ' ( ) * + , - . / 0 1 2 3 4 5 6 7 8 9 : ; < = > ?
               X,0,0,Z,S,0,0,0,S,S,Q,Q,0,0,S,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,Q,
            // @ A B C D E F G H I J K L M N O P Q R S T U V W X Y Z [ \ ] ^ _
               0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,S,S,0,S,0,
            // ' a b c d e f g h i j k l m n o p q r s t u v w x y z { | } ~
               0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,Q,S,0,0,0
        };

        /// <summary>
        /// Returns true for those characters that terminate a string of ordinary chars.
        /// </summary>
        public static bool IsSpecial(char ch) => (ch <= '|' && Category[ch] >= S);

        /// <summary>
        /// Returns true for those characters that terminate a string of ordinary chars.
        /// </summary>
        public static bool IsStopperX(char ch) => (ch <= '|' && Category[ch] >= X);

        /// <summary>
        /// Returns true for those characters that begin a quantifier.
        /// </summary>
        public static bool IsQuantifier(char ch) => (ch <= '{' && Category[ch] >= Q);

        /// <summary>
        /// Returns true for whitespace.
        /// </summary>
        public static bool IsSpace(char ch) => (ch <= ' ' && Category[ch] == X);

        /// <summary>
        /// Returns true for chars that should be escaped.
        /// </summary>
        public static bool IsMetachar(char ch) => (ch <= '|' && Category[ch] >= E);
    }
}
