// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Exclude until we determine the dependency should work on System.IO.Pipelines
#if false

using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        [System.CLSCompliantAttribute(false)]
        public static ValueTask<TValue> ReadAsync<TValue>(PipeReader utf8Json, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            if (utf8Json == null)
                throw new ArgumentNullException(nameof(utf8Json));

            return ReadAsync<TValue>(utf8Json, typeof(TValue), options, cancellationToken);
        }

        [System.CLSCompliantAttribute(false)]
        public static ValueTask<object> ReadAsync(PipeReader utf8Json, Type returnType, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            if (utf8Json == null)
                throw new ArgumentNullException(nameof(utf8Json));

            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            return ReadAsync<object>(utf8Json, returnType, options, cancellationToken);
        }

        private static async ValueTask<TValue> ReadAsync<TValue>(PipeReader utf8Json, Type returnType, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            if (options == null)
                options = s_defaultSettings;

            ReadStack state = default;
            JsonClassInfo classInfo = options.GetOrAddClass(returnType);
            state.Current.ClassInfo = classInfo;
            if (classInfo.ClassType != ClassType.Object)
            {
                state.Current.PropertyInfo = classInfo.GetPolicyProperty();
            }

            var readerState = new JsonReaderState(options: options.ReaderOptions);

            ReadResult result;
            do
            {
                result = await utf8Json.ReadAsync(cancellationToken).ConfigureAwait(false);
                ReadOnlySequence<byte> buffer = result.Buffer;

                ReadCore(
                    ref readerState,
                    result.IsCompleted,
                    buffer,
                    options,
                    ref state);

                utf8Json.AdvanceTo(buffer.GetPosition(readerState.BytesConsumed), buffer.End);
            } while (!result.IsCompleted);

            return (TValue)state.Current.ReturnValue;
        }

        private static void ReadCore(
            ref JsonReaderState readerState,
            bool isFinalBlock,
            ReadOnlySequence<byte> buffer,
            JsonSerializerOptions options,
            ref ReadStack state)
        {
            Utf8JsonReader reader = new Utf8JsonReader(buffer, isFinalBlock, readerState);

            ReadCore(
                options,
                ref reader,
                ref state);

            readerState = reader.CurrentState;
        }
    }
}
#endif
