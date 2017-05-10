// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.
//// See the LICENSE file in the project root for more information.

//using System;
//using System.Reflection;

//namespace System.Runtime.Serialization.Xml.Canonicalization.Tests
//{   
//    public static class ReflectionHelper // ReflectionHelper
//    {
//        const string SecPrefix = "System.ServiceModel.Security.";
//        const string SecProtocolPrefix = "System.ServiceModel.Security.";
//        const string SecTokenPrefix = "System.ServiceModel.Security.Tokens.";
//        const string ChannelPrefix = "System.ServiceModel.Channels.";
//        const string IdentityModelPrefix = "System.IdentityModel.";

//        const BindingFlags createInstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags getMethodFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags invokeMethodFlags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags invokeStaticMethodFlags = BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic;

//        const BindingFlags getFieldFlags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags getStaticFieldFlags = BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags getPropertyFlags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags getStaticPropertyFlags = BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags setFieldFlags = BindingFlags.SetField | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags setStaticFieldFlags = BindingFlags.SetField | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags setPropertyFlags = BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//        const BindingFlags setStaticPropertyFlags = BindingFlags.SetProperty | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

//        static readonly Assembly identityModelAssembly = typeof(System.IdentityModel.Tokens.SecurityToken).Assembly;        

//        public static object New(Assembly assembly, string typeName, params object[] args)
//        {
//            return CheckNew(assembly.CreateInstance(typeName, false, createInstanceFlags, null, args, null, null), typeName);
//        }

//        public static object NewIdentityModel(string unqualifiedTypeName, params object[] args)
//        {
//            return New(identityModelAssembly, IdentityModelPrefix + unqualifiedTypeName, args);
//        }

//        static object CheckNew(object instance, string typeName)
//        {
//            if (instance == null)
//            {
//                throw new Exception(string.Format("Unable to create instance of type '{0}'", typeName));
//            }
//            return instance;
//        }

//        public static object Call(object instance, string methodName, params object[] args)
//        {
//            return instance.GetType().InvokeMember(methodName, invokeMethodFlags, null, instance, args);
//        }

//        public static object SetProperty(object instance, string propertyName, object value)
//        {
//            return instance.GetType().InvokeMember(propertyName, setPropertyFlags, null, instance, new object[] {value});
//        }
//    }
//}
