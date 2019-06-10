# System.Text.Json Features

Here's a summary of the supported (and not-supported) features for the built-in JSON stack as part of .NET Core 3.0.

## Features within the 3.0 release
- RFC Compliant strict mode across the stack (Reader/Writer, Document, (De)Serializer)
    - Optionally ignore trailing commas
    - Optionally ignore (or read/write) comments
- Support for restricting maximum depth while reading (and default limit while writing)
- Support for primitive/built-in types (int, string, DateTime, Guid, etc.)
    - Effectively, the subset of types that are supported by the [Utf8Parser](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8parser?view=netstandard-2.1)/[Formatter](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8formatter?view=netstandard-2.1)
- Write/Serialize formatted/indented and minimized
- Async APIs on the (De)Serializer
- User-defined classes with public properties with default/parameterless constructor
- Base64 support for byte arrays
- Support for ignoring nulls, and customize property names (such as camel casing)
- Allow overflow JSON to round-trip (via `JsonExtensionDataAttribute`)
- Detect (and throw) when observing cycles in the object graph
- Support for collections such as IList, ICollection, IDictionary, etc.
    - For deserializing dictionaries, only `string: object` and `string: JsonElement` pairs are supported
- [preview 7+] Converters for customizing (De)Serializer behavior
- [preview 7+] Treating enums as strings
- [preview 7+] Ability to control escaping
- [preview 7+] Opt-in polymorphic support for serialization

## Features for vNext (and beyond)
- Modifiable JsonDocument/Document Object Model (DOM)
- Other built-in types (BigInteger, Uri, etc.)
- Dynamic objects (like ExpandoObject)
- User-defined structs
- Fields and custom constructors
- F# discriminated unions
- F# record and anonymous record types
- Option to ignore cycles in the object graph
- Option to allow customizing behavior for duplicate properties
- Private properties, setters/getters
- Preserving object references

## Features not supported (with no plans to do so)
- Non-RFC complaint default behavior
- Deserializing based on types defined within the payload (without a closed known types set)
- Built-in support for inferring/guessing .NET types from the payload (such as parsing ints from a string)
- Support for octal/hex numbers
- Default escaping behavior will be based on an allow list and strict
