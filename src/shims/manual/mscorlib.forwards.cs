// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Add any internal types that we need to forward from mscorlib. 

// These types are required for Desktop to Core serialization as they are not covered by GenAPI because they are marked as internal.
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.CultureAwareComparer))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.OrdinalComparer))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.GenericComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.NullableComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.ObjectComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.GenericEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.NullableEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.ObjectEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.NonRandomizedStringEqualityComparer))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.ByteEqualityComparer))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.EnumEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.SByteEnumEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.ShortEnumEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.Generic.LongEnumEqualityComparer<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Collections.ListDictionaryInternal))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.UnitySerializationHolder))]

// This is temporary as we are building the mscorlib shim against netfx461 which doesn't contain ValueTuples.
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ValueTuple))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ValueTuple<>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ValueTuple<,>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ValueTuple<,,>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ValueTuple<,,,>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ValueTuple<,,,,>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ValueTuple<,,,,,>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ValueTuple<,,,,,,>))]
[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ValueTuple<,,,,,,,>))]
