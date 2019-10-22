// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json
{
    /// <summary>
    /// Represents a property that is metadata and is prefixed with "$"/>.
    /// </summary>
    internal sealed class JsonPropertyInfoMetadata : JsonPropertyInfoCommon<string, string, string, string>
    {
        public override Type GetDictionaryConcreteType()
        {
            throw new NotImplementedException();
        }

        protected override void OnRead(ref ReadStack state, ref Utf8JsonReader reader)
        {
            if (Converter == null)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(RuntimePropertyType);
            }

            string key = Converter.Read(ref reader, RuntimePropertyType, Options);

            if (MetadataProperty == MetadataPropertyName.Id)
            {
                state.AddReference(key, GetValueToPreserve(ref state));
            }
            else if (MetadataProperty == MetadataPropertyName.Ref)
            {
                state.Current.ReferenceId = key;
                state.Current.ShouldHandleReference = true;
            }
        }

        protected override void OnReadEnumerable(ref ReadStack state, ref Utf8JsonReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void OnWrite(ref WriteStackFrame current, Utf8JsonWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void OnWriteEnumerable(ref WriteStackFrame current, Utf8JsonWriter writer)
        {
            throw new NotImplementedException();
        }

        private static object GetValueToPreserve(ref ReadStack state)
        {
            return state.Current.IsProcessingProperty(ClassType.Dictionary) ? state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue) : state.Current.ReturnValue;
        }
    }
}
