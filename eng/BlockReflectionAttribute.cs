/*
  Providing a definition for __BlockReflectionAttribute in an assembly is a signal to the .NET Native toolchain 
  to remove the metadata for all non-public APIs. This both reduces size and disables private reflection on those 
  APIs in libraries that include this. The attribute can also be applied to individual public APIs to similarly block them.
  This file is consumed by Build.Common.Targets.
*/

using System;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.All)]
    internal class __BlockReflectionAttribute : Attribute { }
}
