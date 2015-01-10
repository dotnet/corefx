// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using Microsoft.Composition.Demos.Web.Mvc;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Web;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Microsoft.Composition.Demos.Web.Mvc")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("Microsoft.Composition.Demos.Web.Mvc")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("93f9b9f4-f2fb-4a6e-bcae-37db5193893d")]

[assembly: PreApplicationStartMethod(typeof(RequestCompositionScopeModule), "Register")]

[assembly: InternalsVisibleTo("Microsoft.Composition.Demos.Web.Mvc.UnitTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100616470ad6a034af669d130b58deedb7ad8544920d8a21d95bc5bb535ca673d8a49b228c5163f78f34b8df3b015fc2b99ff45b7536830a596f711b8b09f80b48a4bf20883ee5b97f50462d7e0f33440f024dae7d8f7eaf875b747619f1e772131a24dea9d5f80e5d54d95f0704f78fe84ac4b3774ce17eb00a764c295846d43e3")]

[assembly: SecurityTransparent]
[assembly: NeutralResourcesLanguage("en")]
