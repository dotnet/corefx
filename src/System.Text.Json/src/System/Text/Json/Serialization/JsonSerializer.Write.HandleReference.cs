﻿using System.Text.Json.Serialization;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        private enum ResolvedReferenceHandling
        {
            Ignore,
            Preserve,
            //Ignore or Error were selected but no loop was found.
            None
        }

        private static ResolvedReferenceHandling ResolveReferenceHandling(JsonSerializerOptions options, ref WriteStack state, out int referenceId, out bool writeAsReference, object currentPropertyValue = null)
        {
            //if (currentFramePropertyInfo != state.Current.JsonPropertyInfo)
            //{
            //    throw new Exception("Does not always match");
            //}

            //if jsonPropertyInfo == null
            //We are in the root object.

            ReferenceHandling handling;
            //JsonProperty is null if is either root object or and object element within an array.
            JsonReferenceHandlingAttribute attr = JsonPropertyInfo.GetAttribute<JsonReferenceHandlingAttribute>(state.Current.JsonPropertyInfo?.PropertyInfo);

            //JsonReferenceHandlingAttribute classAttr = JsonClassInfo.

            if (attr != null)
            {
                handling = attr.Handling;
            }
            //else if()
            //TODO: Add support for class-level attributes... here.
            else
            {
                handling = options.ReferenceHandling;
            }

            object value = currentPropertyValue ?? state.Current.CurrentValue;

            switch (handling)
            {
                case ReferenceHandling.Error:
                case ReferenceHandling.Ignore:
                    referenceId = default;
                    writeAsReference = default;
                    return ResolveReferenceLoop(handling, value, ref state);
                case ReferenceHandling.Preserve:
                    writeAsReference = ResolvePreserveReference(out referenceId, value, ref state);
                    return ResolvedReferenceHandling.Preserve;//return ResolvePreserveReference(currentValue, ref state, writer);
                default:
                    referenceId = default;
                    writeAsReference = default;
                    return ResolvedReferenceHandling.None;
            }
        }

        private static ResolvedReferenceHandling ResolveReferenceLoop(ReferenceHandling handling, object value, ref WriteStack state)
        {
            if (!state.AddStackReference(value))
            {
                if (handling == ReferenceHandling.Error)
                {
                    throw new JsonException("Invalid Reference Loop Detected!");
                }

                //if reference wasn't added to the set, it means it was already there, therefore we should ignore it BUT not remove it from the set in order to keep validating against further references.
                state.Current.KeepReferenceInSet = true;
                return ResolvedReferenceHandling.Ignore;
            }

            //New reference in the stack.
            return ResolvedReferenceHandling.None;
        }

        private static bool ResolvePreserveReference(out int id, object value, ref WriteStack state) => !state.AddPreservedReference(value, out id);
    }
}
