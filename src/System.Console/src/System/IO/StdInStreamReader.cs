// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace System.IO
{
    /* This class is used by for reading from the stdin.
     * It is designed to read stdin in raw mode for interpreting
     * key press events and maintain its own buffer for the same.
     * which is then used for all the Read operations
     */
    internal class StdInStreamReader : StreamReader
    {
        char[] unprocessedBufferToBeRead; // Buffer that might have already been read from stdin but not yet processed.
        const int bytesToBeRead = 255; // No. of bytes to be read from the stream at a time.
        int startIndex; // First unprocessed index in the buffer;
        int endIndex; // Last unprocessed index in the buffer;

        internal StdInStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
        {
            unprocessedBufferToBeRead = new char[encoding.GetMaxCharCount(bufferSize)];
            startIndex = endIndex = -1; // There is no unprocessed index yet.
        }

        /// <summary> Checks whether the unprocessed buffer is empty. </summary>
        bool IsExtraBufferEmpty()
        {
            if (startIndex == -1 && endIndex == -1) return true;

            if (startIndex > endIndex) // Everything has been processed;
            {
                startIndex = -1;
                endIndex = -1;
                return true;
            }

            return false;
        }

        /// <summary> Reads the buffer in the raw mode. </summary>
        unsafe void ReadExtraBuffer()
        {
            Debug.Assert(IsExtraBufferEmpty());

            byte[] buffer = new byte[bytesToBeRead];
            fixed (byte* bufPtr = buffer)
            {
                int result;
                Interop.CheckIo(result = Interop.Sys.ReadStdinUnbuffered((byte*)bufPtr, bytesToBeRead));
                Debug.Assert(result <= bytesToBeRead);

                // Now we need to convert the byte buffer to char buffer.
                int charCount = CurrentEncoding.GetMaxCharCount(result);
                Debug.Assert(charCount < unprocessedBufferToBeRead.Length);

                CurrentEncoding.GetChars(buffer, 0, buffer.Length, unprocessedBufferToBeRead, ++startIndex);
                endIndex += charCount;

            }
        }

        public override string ReadLine()
        {
            StringBuilder sb = new StringBuilder();

            //Check if there is anything left in the unprocessedBufferToBeRead.
            while (!IsExtraBufferEmpty())
            {
                ConsoleKeyInfo keyInfo = ReadKeyInternal();

                if (keyInfo.Key == ConsoleKey.Enter)
                    return sb.ToString();

                else if (keyInfo.Key == ConsoleKey.Backspace && sb.Length > 0)
                    sb.Length--;

                if (keyInfo.KeyChar != '\0') // We ignore all non-printable keys
                    sb.Append(keyInfo.KeyChar);
            }

            return sb.Append(base.ReadLine()).ToString();
        }


        public override int Read()
        {
            int x;

            // Convert byte to char.
            if (IsExtraBufferEmpty())
            {
                x = base.Read();
            }
            else
            {
                x = (int)unprocessedBufferToBeRead[startIndex++];
            }

            return x;
        }

        internal ConsoleKey GetKeyFromCharValue(ref char x, out bool isShift, out bool isCtrl)
        {
            isShift = false;
            isCtrl = false;

            switch (x)
            {
                case '\b':
                    x = '\0'; // Non-printable character.
                    return ConsoleKey.Backspace;

                case '\t':
                    x = '\0'; // Non-printable character.
                    return ConsoleKey.Tab;

                case (char)(int)(0x0A):
                    x = '\0'; // Non-printable character.
                    return ConsoleKey.Enter;

                case (char)(int)(0x1B):
                    x = '\0'; // Non-printable character.
                    return ConsoleKey.Escape;

                case (char)(int)(0x2A): return ConsoleKey.Multiply;

                case (char)(int)(0x2B): return ConsoleKey.Add;

                case (char)(int)(0x2D): return ConsoleKey.Subtract;

                case (char)(int)(0x2F): return ConsoleKey.Divide;

                case (char)(int)(0x2E): return ConsoleKey.Decimal;

                case (char)(int)(0x7F):
                    x = '\0'; // Non-printable character.
                    return ConsoleKey.Delete;

                case (char)(int)(0x20): return ConsoleKey.Spacebar;

                case (char)(int)(0x21):
                    isShift = true;
                    return ConsoleKey.D1;
                    
                case (char)(int)(0x40):
                    isShift = true;
                    return ConsoleKey.D2;

                case (char)(int)(0x23):
                    isShift = true;
                    return ConsoleKey.D3;

                case (char)(int)(0x24):
                    isShift = true;
                    return ConsoleKey.D4;

                case (char)(int)(0x25):
                    isShift = true;
                    return ConsoleKey.D5;

                case (char)(int)(0x5E):
                    isShift = true;
                    return ConsoleKey.D6;

                case (char)(int)(0x26):
                    isShift = true;
                    return ConsoleKey.D7;

                case (char)(int)(0x28):
                    isShift = true;
                    return ConsoleKey.D9;

                case (char)(int)(0x29):
                    isShift = true;
                    return ConsoleKey.D0;

                case (char)(int)(0x60): return ConsoleKey.Oem3;

                case (char)(int)(0x7E):
                    isShift = true;
                    return ConsoleKey.Oem3;

                case (char)(int)(0x5B): return ConsoleKey.Oem4;

                case (char)(int)(0x7B): 
                    isShift = true;
                    return ConsoleKey.Oem4;

                case (char)(int)(0x5D): return ConsoleKey.Oem6;

                case (char)(int)(0x7D):
                    isShift = true;
                    return ConsoleKey.Oem6;

                case (char)(int)(0x3B): return ConsoleKey.Oem1;

                case (char)(int)(0x3A):
                    isShift = true;
                    return ConsoleKey.Oem1;

                case (char)(int)(0x27): return ConsoleKey.Oem7;

                case (char)(int)(0x22):
                    isShift = true;
                    return ConsoleKey.Oem7;

                case (char)(int)(0x2C): return ConsoleKey.OemComma;

                case (char)(int)(0x3C):
                    isShift = true;
                    return ConsoleKey.OemComma;

                case (char)(int)(0x3D): return ConsoleKey.OemPlus;

                case (char)(int)(0x5F): return ConsoleKey.OemMinus;

                case (char)(int)(0x3E): return ConsoleKey.OemPeriod;

                case (char)(int)(0x3F):
                    isShift = true;
                    return ConsoleKey.Oem2;

                case (char)(int)(0x5C): return ConsoleKey.Oem5;

                case (char)(int)(0x7C):
                    isShift = true;
                    return ConsoleKey.Oem5;

                default:
                    // 1. Ctrl A to Ctrl Z.
                    if (x >= 1 && x <= 26)
                    {
                        isCtrl = true;
                        return ConsoleKey.A + x - 1;
                    }

                    // 2. Numbers from 0 to 9.
                    if (x >= '0' && x <= '9')
                    {
                        return ConsoleKey.D0 + x - '0';
                    }

                    //3. A to Z
                    if (x >= 'A' && x <= 'Z')
                    {
                        isShift = true;
                        return ConsoleKey.A + (x - 'A');
                    }

                    // 4. a to z.
                    if (x >= 'a' && x <= 'z')
                    {
                        return ConsoleKey.A + (x - 'a');
                    }

                    break;
            }

            return default(ConsoleKey);
        }

        internal bool TryMapBufferToConsoleKey(out ConsoleKey key, out char ch, out bool isShift, out bool isAlt, out bool isCtrl)
        {
            Debug.Assert(!IsExtraBufferEmpty());

            isAlt = isCtrl = isShift = false;
            int keyLength;

            // 1. Try to get the special key match from the TermInfo static information.
            if (ConsolePal.TryGetSpecialConsoleKey(unprocessedBufferToBeRead, startIndex, endIndex, out key, out keyLength))
            {
                startIndex += keyLength;
                ch = '\0'; //Special keys are non-printable.
                return true;
            }

            // 1. Check if we can match Esc + combination and guess if alt was pressed.
            if (isAlt == false && unprocessedBufferToBeRead[startIndex] == (char)(int)(0x1B) /*Alt is send as an escape character*/ && endIndex - startIndex + 1 >= 2 /*We have at least two characters to read.*/)
            {
                isAlt = true; // Since the alt is pressed.
                startIndex++;
                if (TryMapBufferToConsoleKey(out key, out ch, out isShift, out isAlt, out isCtrl))
                {
                    isAlt = true;
                    return true;
                }
                else
                {
                    // We could not find a matching key here so, Alt+ combination assumption is in-correct.
                    // The current key needs to be marked as Esc key.
                    // Also, we do not increment startIndex as we already did it.
                    key = ConsoleKey.Escape;
                    ch = (char)(int)(0x1B);
                    isAlt = false;
                    return true;
                }
            }

            // Try reading the first char in the buffer and interpret it as a key.
            ch = unprocessedBufferToBeRead[startIndex++];
            key = GetKeyFromCharValue(ref ch, out isShift, out isCtrl);
            return key != (ConsoleKey)0;

        }

        internal ConsoleKeyInfo ReadKeyInternal()
        {
            ConsoleKey key;
            char ch;
            bool isAlt, isCtrl, isShift;

            if (IsExtraBufferEmpty())
            {
                ReadExtraBuffer();
            }

            if(TryMapBufferToConsoleKey(out key, out ch, out isShift, out isAlt, out isCtrl))
            {
                return new ConsoleKeyInfo(ch, key, isShift, isAlt, isCtrl);
            }

            return default(ConsoleKeyInfo);
        }


        /// <summary> Try to intercept the key pressed. </summary>
        public ConsoleKeyInfo ReadKey()
        {
            ConsoleKeyInfo keyInfo = default(ConsoleKeyInfo);
            do
            {
                /* Unlike Windows Unix has no concept of virtual key codes.
                 * Hence, in case we do not recognize a key, we can't really get the ConsoleKey
                 * key code associated with it.
                 * As a result, we try to recognize the key, and if that does not work,
                 * we simply ignore and try to get the next batch of bytes and match them.
                 */
                keyInfo = ReadKeyInternal();
            } while (keyInfo == default(ConsoleKeyInfo));

            return keyInfo;
        }
    }
}
