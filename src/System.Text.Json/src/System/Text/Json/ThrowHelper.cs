// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentException_MaxDepthMustBePositive()
        {
            throw GetArgumentException(SR.MaxDepthMustBePositive);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ArgumentException GetArgumentException(string message)
        {
            return new ArgumentException(message);
        }

        public static void ThrowJsonReaderException(ref Utf8JsonReader json, ExceptionResource resource = ExceptionResource.Default, byte nextByte = default, ReadOnlySpan<byte> bytes = default)
        {
            throw GetJsonReaderException(ref json, resource, nextByte, bytes);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static JsonReaderException GetJsonReaderException(ref Utf8JsonReader json, ExceptionResource resource, byte nextByte, ReadOnlySpan<byte> bytes)
        {
            string message = GetResourceString(ref json, resource, (char)nextByte, Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Length));

            long lineNumber = json.CurrentState._lineNumber;
            long position = json.CurrentState._lineBytePosition;

            message += $" LineNumber: {lineNumber} | BytePosition: {position}.";
            return new JsonReaderException(message, lineNumber, position);
        }

        // This function will convert an ExceptionResource enum value to the resource string.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetResourceString(ref Utf8JsonReader json, ExceptionResource resource, char character, string characters)
        {
            Debug.Assert(Enum.IsDefined(typeof(ExceptionResource), resource),
                "The enum value is not defined, please check the ExceptionResource Enum.");

            string message = "";
            switch (resource)
            {
                case ExceptionResource.ArrayDepthTooLarge:
                    message = string.Format(SR.ArrayDepthTooLarge, json.CurrentDepth + 1, json.CurrentState.MaxDepth);
                    break;
                case ExceptionResource.ArrayEndWithinObject:
                    message = json.CurrentDepth <= 0 ?
                        string.Format(SR.DepthMustBePositive, json.CurrentDepth) :
                        string.Format(SR.ArrayEndWithinObject);
                    break;
                case ExceptionResource.EndOfStringNotFound:
                    message = SR.EndOfStringNotFound;
                    break;
                case ExceptionResource.ExpectedDigitNotFound:
                    message = string.Format(SR.ExpectedDigitNotFound, character);
                    break;
                case ExceptionResource.ExpectedDigitNotFoundEndOfData:
                    message = string.Format(SR.ExpectedDigitNotFoundEndOfData, character);
                    break;
                case ExceptionResource.ExpectedEndAfterSingleJson:
                    message = string.Format(SR.ExpectedEndAfterSingleJson, character);
                    break;
                case ExceptionResource.ExpectedEndOfDigitNotFound:
                    message = string.Format(SR.ExpectedEndOfDigitNotFound, character);
                    break;
                case ExceptionResource.ExpectedNextDigitComponentNotFound:
                    message = string.Format(SR.ExpectedNextDigitComponentNotFound, character);
                    break;
                case ExceptionResource.ExpectedNextDigitEValueNotFound:
                    message = string.Format(SR.ExpectedNextDigitEValueNotFound, character);
                    break;
                case ExceptionResource.ExpectedSeparaterAfterPropertyNameNotFound:
                    message = string.Format(SR.ExpectedSeparaterAfterPropertyNameNotFound, character);
                    break;
                case ExceptionResource.ExpectedStartOfPropertyNotFound:
                    message = string.Format(SR.ExpectedStartOfPropertyNotFound, character);
                    break;
                case ExceptionResource.ExpectedStartOfPropertyOrValueNotFound:
                    message = SR.ExpectedStartOfPropertyOrValueNotFound;
                    break;
                case ExceptionResource.ExpectedStartOfValueNotFound:
                    message = string.Format(SR.ExpectedStartOfValueNotFound, character);
                    break;
                case ExceptionResource.ExpectedValueAfterPropertyNameNotFound:
                    message = SR.ExpectedValueAfterPropertyNameNotFound;
                    break;
                case ExceptionResource.FoundInvalidCharacter:
                    message = string.Format(SR.FoundInvalidCharacter, character);
                    break;
                case ExceptionResource.InvalidEndOfJson:
                    message = string.Format(SR.InvalidEndOfJson, json.TokenType);
                    break;
                case ExceptionResource.ObjectDepthTooLarge:
                    message = string.Format(SR.ObjectDepthTooLarge, json.CurrentDepth + 1, json.CurrentState.MaxDepth);
                    break;
                case ExceptionResource.ObjectEndWithinArray:
                    message = json.CurrentDepth <= 0 ?
                        string.Format(SR.DepthMustBePositive, json.CurrentDepth) :
                        string.Format(SR.ObjectEndWithinArray);
                    break;
                case ExceptionResource.Default:
                    break;
                case ExceptionResource.ExpectedFalse:
                    message = string.Format(SR.ExpectedFalse, characters);
                    break;
                case ExceptionResource.ExpectedNull:
                    message = string.Format(SR.ExpectedNull, characters);
                    break;
                case ExceptionResource.ExpectedTrue:
                    message = string.Format(SR.ExpectedTrue, characters);
                    break;
                // This case is covered between ArrayEndWithinObject and ObjectEndWithinArray
                /*case ExceptionResource.DepthMustBePositive:
                    break;*/
                case ExceptionResource.InvalidCharacterWithinString:
                    message = string.Format(SR.InvalidCharacterWithinString, character);
                    break;
                case ExceptionResource.EndOfCommentNotFound:
                    message = SR.EndOfCommentNotFound;
                    break;
            }

            return message;
        }
    }

    internal enum ExceptionResource
    {
        ArrayDepthTooLarge,
        ArrayEndWithinObject,
        Default,
        DepthMustBePositive,
        EndOfCommentNotFound,
        EndOfStringNotFound,
        ExpectedDigitNotFound,
        ExpectedDigitNotFoundEndOfData,
        ExpectedEndAfterSingleJson,
        ExpectedEndOfDigitNotFound,
        ExpectedFalse,
        ExpectedNextDigitComponentNotFound,
        ExpectedNextDigitEValueNotFound,
        ExpectedNull,
        ExpectedSeparaterAfterPropertyNameNotFound,
        ExpectedStartOfPropertyNotFound,
        ExpectedStartOfPropertyOrValueNotFound,
        ExpectedStartOfValueNotFound,
        ExpectedTrue,
        ExpectedValueAfterPropertyNameNotFound,
        FoundInvalidCharacter,
        InvalidCharacterWithinString,
        InvalidEndOfJson,
        ObjectDepthTooLarge,
        ObjectEndWithinArray,
    }
}
