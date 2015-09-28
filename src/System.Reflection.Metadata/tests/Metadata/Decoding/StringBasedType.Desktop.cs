// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection.Metadata.Decoding
{
    public class StringBasedAssembly  : Assembly
    {
        private readonly AssemblyName _name;

        public StringBasedAssembly(AssemblyName name)
        {
            _name = name;
        }

        public override string ToString()
        {
            string name = "|" + _name.Name + "(name)";

            if (_name.Version != null)
            {
                name = AddComponent(name, "Version", _name.Version.ToString());
            }

            byte[] token = _name.GetPublicKeyToken();
            if (token != null)
            {
                name = AddComponent(name, "PublicKeyToken", token.Length > 0 ? String.Join("", token.Select(b => b.ToString("x2"))) : "null");
            }

            if (_name.CultureName != null)
            {
                name = AddComponent(name, "Culture", _name.CultureName.Length > 0 ? _name.CultureName : "neutral");
            }

            if ((_name.Flags & AssemblyNameFlags.Retargetable) == AssemblyNameFlags.Retargetable)
            {
                name = AddComponent(name, "Retargetable", "Yes");
            }

            name += "|";

            return name;
        }

        private string AddComponent(string name, string componentName, string componentValue)
        {
            return name + componentName + "(componentName)" + componentValue + "(componentValue)";
        }
    }

    public class StringBasedType : Type
    {
        private readonly string _typeName;
        private readonly Assembly _assembly;

        public static string ParseTypeName(string typeName)
        {
            StringBasedType type =  (StringBasedType)Type.GetType(typeName, (name) => new StringBasedAssembly(name), (assembly, t, throwOnError) => new StringBasedType(assembly, Unescape(t) + "(simple)"), true);

            return type.AssemblyQualifiedName;
        }
        
        private static string Unescape(string typeName)
        {
            StringBuilder builder = new StringBuilder();

            bool skipNextEscape = false;

            for (int i = 0; i < typeName.Length; i++)
            {
                if (!skipNextEscape && typeName[i] == '\\')
                {
                    skipNextEscape = true;
                    continue;
                }

                skipNextEscape = false;
                builder.Append(typeName[i]);
            }

            return builder.ToString();
        }

        public StringBasedType(Assembly assembly, string typeName)
        {
            _assembly = assembly;
            _typeName = typeName;
        }

        public override string AssemblyQualifiedName
        {
            get 
            {
                if (_assembly == null)
                    return _typeName;

                return _assembly.ToString() + _typeName; }
        }

        public override Type MakeArrayType()
        {
            return new StringBasedType(_assembly, _typeName + "{}");
        }

        public override Type MakeArrayType(int rank)
        {
            return new StringBasedType(_assembly, _typeName + "{" + rank + "}");
        }

        public override Type MakeByRefType()
        {
            return new StringBasedType(_assembly, _typeName + "(reference)");
        }

        public override Type MakePointerType()
        {
            return new StringBasedType(_assembly, _typeName + "(pointer)");
        }

        public override Type MakeGenericType(params Type[] typeArguments)
        {
            IEnumerable<string> names = typeArguments.Select(t => t.AssemblyQualifiedName);

            return new StringBasedType(_assembly, _typeName + "<" + String.Join(",", names) + ">");
        }

        public override Type GetNestedType(string name, System.Reflection.BindingFlags bindingAttr)
        {
            return new StringBasedType(_assembly, _typeName + "-" + name);
        }

        public override Assembly Assembly
        {
            get { return _assembly; }
        }

        public override Type BaseType
        {
            get { throw new NotImplementedException(); }
        }

        public override string FullName
        {
            get { throw new NotImplementedException(); }
        }

        public override Guid GUID
        {
            get { throw new NotImplementedException(); }
        }

        protected override System.Reflection.TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException();
        }

        protected override System.Reflection.ConstructorInfo GetConstructorImpl(System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, Type[] types, System.Reflection.ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        public override System.Reflection.ConstructorInfo[] GetConstructors(System.Reflection.BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }

        public override System.Reflection.EventInfo GetEvent(string name, System.Reflection.BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override System.Reflection.EventInfo[] GetEvents(System.Reflection.BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override System.Reflection.FieldInfo GetField(string name, System.Reflection.BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException();
        }

        public override System.Reflection.MemberInfo[] GetMembers(System.Reflection.BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        protected override System.Reflection.MethodInfo GetMethodImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, Type[] types, System.Reflection.ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        public override System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetNestedTypes(System.Reflection.BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        protected override System.Reflection.PropertyInfo GetPropertyImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, Type returnType, Type[] types, System.Reflection.ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException();
        }

        public override object InvokeMember(string name, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args, System.Reflection.ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters)
        {
            throw new NotImplementedException();
        }

        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException();
        }

        public override System.Reflection.Module Module
        {
            get { throw new NotImplementedException(); }
        }

        public override string Namespace
        {
            get { throw new NotImplementedException(); }
        }

        public override Type UnderlyingSystemType
        {
            get { throw new NotImplementedException(); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }
    }
}
