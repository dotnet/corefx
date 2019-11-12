using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    internal enum ResolvedReferenceHandling
    {
        Ignore,
        Preserve,
        IsReference,
        // Maybe add WriteReference to use that state instead of writeAsReference.
        //Ignore or Error were selected but no loop was found.
        None
    }

    public static partial class JsonSerializer
    {

        private static ResolvedReferenceHandling ResolveReferenceHandling(JsonSerializerOptions options, ref WriteStack state, out string referenceId, out bool writeAsReference, object currentPropertyValue = null)
        {
            ReferenceHandlingOnSerialize handling = options.ReferenceHandlingOnSerialize;
            object value = currentPropertyValue ?? state.Current.CurrentValue;

            switch (handling)
            {
                case ReferenceHandlingOnSerialize.Error:
                case ReferenceHandlingOnSerialize.Ignore:
                    referenceId = default;
                    writeAsReference = default;
                    return ResolveReferenceLoop(handling, value, ref state);
                case ReferenceHandlingOnSerialize.Preserve:

                    writeAsReference = state.GetPreservedReference(value, out referenceId);//ResolvePreserveReference(out referenceId, value, ref state);
                    return ResolvedReferenceHandling.Preserve;

                default:
                    referenceId = default;
                    writeAsReference = default;
                    return ResolvedReferenceHandling.None;
            }
        }

        private static ResolvedReferenceHandling ResolveReferenceLoop(ReferenceHandlingOnSerialize handling, object value, ref WriteStack state)
        {
            if (!state.AddStackReference(value))
            {
                if (handling == ReferenceHandlingOnSerialize.Error)
                {
                    //Nice to have: include the name of the property in this message.
                    throw new JsonException("Invalid Reference Loop Detected!.");
                }

                //if reference wasn't added to the set, it means it was already there, therefore we should ignore it BUT not remove it from the set in order to keep validating against further references.
                state.Current.KeepReferenceInSet = true;
                return ResolvedReferenceHandling.Ignore;
            }

            //New reference in the stack.
            return ResolvedReferenceHandling.None;
        }
    }
}
