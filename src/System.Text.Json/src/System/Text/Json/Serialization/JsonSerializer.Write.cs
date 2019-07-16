﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        // There are three conditions to consider for an object (primitive value, enumerable or object) being processed here:
        // 1) The object type was specified as the root-level return type to a Parse\Read method.
        // 2) The object is property on a parent object.
        // 3) The object is an element in an enumerable.
        private static bool Write(
            Utf8JsonWriter writer,
            int originalWriterDepth,
            int flushThreshold,
            JsonSerializerOptions options,
            ref WriteStack state)
        {
            bool finishedSerializing;

            try
            {
                do
                {
                    WriteStackFrame current = state.Current;
                    switch (current.JsonClassInfo.ClassType)
                    {
                        case ClassType.Enumerable:
                            finishedSerializing = HandleEnumerable(current.JsonClassInfo.ElementClassInfo, options, writer, ref state);
                            break;
                        case ClassType.Value:
                            Debug.Assert(current.JsonPropertyInfo.ClassType == ClassType.Value);
                            current.JsonPropertyInfo.Write(ref state, writer);
                            finishedSerializing = true;
                            break;
                        case ClassType.Object:
                            finishedSerializing = WriteObject(options, writer, ref state);
                            break;
                        case ClassType.Dictionary:
                        case ClassType.IDictionaryConstructible:
                            finishedSerializing = HandleDictionary(current.JsonClassInfo.ElementClassInfo, options, writer, ref state);
                            break;
                        default:
                            Debug.Assert(state.Current.JsonClassInfo.ClassType == ClassType.Unknown);

                            // Treat typeof(object) as an empty object.
                            finishedSerializing = WriteObject(options, writer, ref state);
                            break;
                    }

                    if (finishedSerializing)
                    {
                        if (writer.CurrentDepth == 0 || writer.CurrentDepth == originalWriterDepth)
                        {
                            break;
                        }
                    }
                    else if (writer.CurrentDepth >= options.EffectiveMaxDepth)
                    {
                        ThrowHelper.ThrowJsonException_DepthTooLarge(writer.CurrentDepth, state, options);
                    }

                    // If serialization is not yet end and we surpass beyond flush threshold return false and flush stream.
                    if (flushThreshold >= 0 && writer.BytesPending > flushThreshold)
                    {
                        return false;
                    }
                } while (true);
            }
            catch (InvalidOperationException ex) when (ex.Source == ThrowHelper.ExceptionSourceValueToRethrowAsJsonException)
            {
                ThrowHelper.ReThrowWithPath(state, ex);
            }
            catch (JsonException ex)
            {
                ThrowHelper.AddExceptionInformation(state, ex);
                throw;
            }

            return true;
        }
    }
}
