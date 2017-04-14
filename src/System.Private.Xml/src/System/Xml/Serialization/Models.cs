// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if XMLSERIALIZERGENERATOR
namespace Microsoft.XmlSerializer.Generator
#else
namespace System.Xml.Serialization
#endif
{
    using System;
    using System.Reflection;
    using System.Collections;
    using System.Diagnostics;
    using System.Collections.Generic;

    // These classes define the abstract serialization model, e.g. the rules for WHAT is serialized.  
    // The answer of HOW the values are serialized is answered by a particular reflection importer 
    // by looking for a particular set of custom attributes specific to the serialization format
    // and building an appropriate set of accessors/mappings.

    internal class ModelScope
    {
        private TypeScope _typeScope;
        private readonly Dictionary<Type, TypeModel> _models = new Dictionary<Type, TypeModel>();
        private readonly Dictionary<Type, TypeModel> _arrayModels = new Dictionary<Type, TypeModel>();

        internal ModelScope(TypeScope typeScope)
        {
            _typeScope = typeScope;
        }

        internal TypeScope TypeScope
        {
            get { return _typeScope; }
        }

        internal TypeModel GetTypeModel(Type type)
        {
            return GetTypeModel(type, true);
        }

        internal TypeModel GetTypeModel(Type type, bool directReference)
        {
            TypeModel model;
            if (_models.TryGetValue(type, out model))
                return model;
            TypeDesc typeDesc = _typeScope.GetTypeDesc(type, null, directReference);

            switch (typeDesc.Kind)
            {
                case TypeKind.Enum:
                    model = new EnumModel(type, typeDesc, this);
                    break;
                case TypeKind.Primitive:
                    model = new PrimitiveModel(type, typeDesc, this);
                    break;
                case TypeKind.Array:
                case TypeKind.Collection:
                case TypeKind.Enumerable:
                    model = new ArrayModel(type, typeDesc, this);
                    break;
                case TypeKind.Root:
                case TypeKind.Class:
                case TypeKind.Struct:
                    model = new StructModel(type, typeDesc, this);
                    break;
                default:
                    if (!typeDesc.IsSpecial) throw new NotSupportedException(SR.Format(SR.XmlUnsupportedTypeKind, type.FullName));
                    model = new SpecialModel(type, typeDesc, this);
                    break;
            }

            _models.Add(type, model);
            return model;
        }

        internal ArrayModel GetArrayModel(Type type)
        {
            TypeModel model;
            if (!_arrayModels.TryGetValue(type, out model))
            {
                model = GetTypeModel(type);
                if (!(model is ArrayModel))
                {
                    TypeDesc typeDesc = _typeScope.GetArrayTypeDesc(type);
                    model = new ArrayModel(type, typeDesc, this);
                }
                _arrayModels.Add(type, model);
            }
            return (ArrayModel)model;
        }
    }

    internal abstract class TypeModel
    {
        private TypeDesc _typeDesc;
        private Type _type;
        private ModelScope _scope;

        protected TypeModel(Type type, TypeDesc typeDesc, ModelScope scope)
        {
            _scope = scope;
            _type = type;
            _typeDesc = typeDesc;
        }

        internal Type Type
        {
            get { return _type; }
        }

        internal ModelScope ModelScope
        {
            get { return _scope; }
        }

        internal TypeDesc TypeDesc
        {
            get { return _typeDesc; }
        }
    }

    internal class ArrayModel : TypeModel
    {
        internal ArrayModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }

        internal TypeModel Element
        {
            get { return ModelScope.GetTypeModel(TypeScope.GetArrayElementType(Type, null)); }
        }
    }

    internal class PrimitiveModel : TypeModel
    {
        internal PrimitiveModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }
    }

    internal class SpecialModel : TypeModel
    {
        internal SpecialModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }
    }

    internal class StructModel : TypeModel
    {
        internal StructModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }

        internal MemberInfo[] GetMemberInfos()
        {
            // we use to return Type.GetMembers() here, the members were returned in a different order: fields first, properties last
            // Current System.Reflection code returns members in opposite order: properties first, then fields.
            // This code make sure that returns members in the Everett order.
            MemberInfo[] members = Type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            MemberInfo[] fieldsAndProps = new MemberInfo[members.Length];

            int cMember = 0;
            // first copy all non-property members over
            for (int i = 0; i < members.Length; i++)
            {
                if (!(members[i] is PropertyInfo))
                {
                    fieldsAndProps[cMember++] = members[i];
                }
            }
            // now copy all property members over
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] is PropertyInfo)
                {
                    fieldsAndProps[cMember++] = members[i];
                }
            }
            return fieldsAndProps;
        }

        internal FieldModel GetFieldModel(MemberInfo memberInfo)
        {
            FieldModel model = null;
            if (memberInfo is FieldInfo)
                model = GetFieldModel((FieldInfo)memberInfo);
            else if (memberInfo is PropertyInfo)
                model = GetPropertyModel((PropertyInfo)memberInfo);
            if (model != null)
            {
                if (model.ReadOnly && model.FieldTypeDesc.Kind != TypeKind.Collection && model.FieldTypeDesc.Kind != TypeKind.Enumerable)
                    return null;
            }
            return model;
        }

        private void CheckSupportedMember(TypeDesc typeDesc, MemberInfo member, Type type)
        {
            if (typeDesc == null)
                return;
            if (typeDesc.IsUnsupported)
            {
                if (typeDesc.Exception == null)
                {
                    typeDesc.Exception = new NotSupportedException(SR.Format(SR.XmlSerializerUnsupportedType, typeDesc.FullName));
                }
                throw new InvalidOperationException(SR.Format(SR.XmlSerializerUnsupportedMember, member.DeclaringType.FullName + "." + member.Name, type.FullName), typeDesc.Exception);
            }
            CheckSupportedMember(typeDesc.BaseTypeDesc, member, type);
            CheckSupportedMember(typeDesc.ArrayElementTypeDesc, member, type);
        }

        private FieldModel GetFieldModel(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsStatic) return null;
            if (fieldInfo.DeclaringType != Type) return null;

            TypeDesc typeDesc = ModelScope.TypeScope.GetTypeDesc(fieldInfo.FieldType, fieldInfo, true, false);
            if (fieldInfo.IsInitOnly && typeDesc.Kind != TypeKind.Collection && typeDesc.Kind != TypeKind.Enumerable)
                return null;

            CheckSupportedMember(typeDesc, fieldInfo, fieldInfo.FieldType);
            return new FieldModel(fieldInfo, fieldInfo.FieldType, typeDesc);
        }

        private FieldModel GetPropertyModel(PropertyInfo propertyInfo)
        {
            if (propertyInfo.DeclaringType != Type) return null;
            if (CheckPropertyRead(propertyInfo))
            {
                TypeDesc typeDesc = ModelScope.TypeScope.GetTypeDesc(propertyInfo.PropertyType, propertyInfo, true, false);
                // Fix for CSDMain 100492, please contact arssrvlt if you need to change this line
                if (!propertyInfo.CanWrite && typeDesc.Kind != TypeKind.Collection && typeDesc.Kind != TypeKind.Enumerable)
                    return null;
                CheckSupportedMember(typeDesc, propertyInfo, propertyInfo.PropertyType);
                return new FieldModel(propertyInfo, propertyInfo.PropertyType, typeDesc);
            }
            return null;
        }

        //CheckProperty
        internal static bool CheckPropertyRead(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead) return false;

            MethodInfo getMethod = propertyInfo.GetMethod;
            if (getMethod.IsStatic) return false;
            ParameterInfo[] parameters = getMethod.GetParameters();
            if (parameters.Length > 0) return false;
            return true;
        }
    }

    internal enum SpecifiedAccessor
    {
        None,
        ReadOnly,
        ReadWrite,
    }

    internal class FieldModel
    {
        private SpecifiedAccessor _checkSpecified = SpecifiedAccessor.None;
        private MemberInfo _memberInfo;
        private MemberInfo _checkSpecifiedMemberInfo;
        private MethodInfo _checkShouldPersistMethodInfo;
        private bool _checkShouldPersist;
        private bool _readOnly = false;
        private bool _isProperty = false;
        private Type _fieldType;
        private string _name;
        private TypeDesc _fieldTypeDesc;

        internal FieldModel(string name, Type fieldType, TypeDesc fieldTypeDesc, bool checkSpecified, bool checkShouldPersist) :
            this(name, fieldType, fieldTypeDesc, checkSpecified, checkShouldPersist, false)
        {
        }
        internal FieldModel(string name, Type fieldType, TypeDesc fieldTypeDesc, bool checkSpecified, bool checkShouldPersist, bool readOnly)
        {
            _fieldTypeDesc = fieldTypeDesc;
            _name = name;
            _fieldType = fieldType;
            _checkSpecified = checkSpecified ? SpecifiedAccessor.ReadWrite : SpecifiedAccessor.None;
            _checkShouldPersist = checkShouldPersist;
            _readOnly = readOnly;
        }

        internal FieldModel(MemberInfo memberInfo, Type fieldType, TypeDesc fieldTypeDesc)
        {
            _name = memberInfo.Name;
            _fieldType = fieldType;
            _fieldTypeDesc = fieldTypeDesc;
            _memberInfo = memberInfo;
            _checkShouldPersistMethodInfo = memberInfo.DeclaringType.GetMethod("ShouldSerialize" + memberInfo.Name, Array.Empty<Type>());
            _checkShouldPersist = _checkShouldPersistMethodInfo != null;

            FieldInfo specifiedField = memberInfo.DeclaringType.GetField(memberInfo.Name + "Specified");
            if (specifiedField != null)
            {
                if (specifiedField.FieldType != typeof(bool))
                {
                    throw new InvalidOperationException(SR.Format(SR.XmlInvalidSpecifiedType, specifiedField.Name, specifiedField.FieldType.FullName, typeof(bool).FullName));
                }
                _checkSpecified = specifiedField.IsInitOnly ? SpecifiedAccessor.ReadOnly : SpecifiedAccessor.ReadWrite;
                _checkSpecifiedMemberInfo = specifiedField;
            }
            else
            {
                PropertyInfo specifiedProperty = memberInfo.DeclaringType.GetProperty(memberInfo.Name + "Specified");
                if (specifiedProperty != null)
                {
                    if (StructModel.CheckPropertyRead(specifiedProperty))
                    {
                        _checkSpecified = specifiedProperty.CanWrite ? SpecifiedAccessor.ReadWrite : SpecifiedAccessor.ReadOnly;
                        _checkSpecifiedMemberInfo = specifiedProperty;
                    }
                    if (_checkSpecified != SpecifiedAccessor.None && specifiedProperty.PropertyType != typeof(bool))
                    {
                        throw new InvalidOperationException(SR.Format(SR.XmlInvalidSpecifiedType, specifiedProperty.Name, specifiedProperty.PropertyType.FullName, typeof(bool).FullName));
                    }
                }
            }
            if (memberInfo is PropertyInfo)
            {
                _readOnly = !((PropertyInfo)memberInfo).CanWrite;
                _isProperty = true;
            }
            else if (memberInfo is FieldInfo)
            {
                _readOnly = ((FieldInfo)memberInfo).IsInitOnly;
            }
        }

        internal string Name
        {
            get { return _name; }
        }

        internal Type FieldType
        {
            get { return _fieldType; }
        }

        internal TypeDesc FieldTypeDesc
        {
            get { return _fieldTypeDesc; }
        }

        internal bool CheckShouldPersist
        {
            get { return _checkShouldPersist; }
        }

        internal SpecifiedAccessor CheckSpecified
        {
            get { return _checkSpecified; }
        }

        internal MemberInfo MemberInfo
        {
            get { return _memberInfo; }
        }
        internal MemberInfo CheckSpecifiedMemberInfo
        {
            get { return _checkSpecifiedMemberInfo; }
        }
        internal MethodInfo CheckShouldPersistMethodInfo
        {
            get { return _checkShouldPersistMethodInfo; }
        }

        internal bool ReadOnly
        {
            get { return _readOnly; }
        }

        internal bool IsProperty
        {
            get { return _isProperty; }
        }
    }

    internal class ConstantModel
    {
        private FieldInfo _fieldInfo;
        private long _value;

        internal ConstantModel(FieldInfo fieldInfo, long value)
        {
            _fieldInfo = fieldInfo;
            _value = value;
        }

        internal string Name
        {
            get { return _fieldInfo.Name; }
        }

        internal long Value
        {
            get { return _value; }
        }

        internal FieldInfo FieldInfo
        {
            get { return _fieldInfo; }
        }
    }

    internal class EnumModel : TypeModel
    {
        private ConstantModel[] _constants;

        internal EnumModel(Type type, TypeDesc typeDesc, ModelScope scope) : base(type, typeDesc, scope) { }

        internal ConstantModel[] Constants
        {
            get
            {
                if (_constants == null)
                {
                    var list = new List<ConstantModel>();
                    FieldInfo[] fields = Type.GetFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        FieldInfo field = fields[i];
                        ConstantModel constant = GetConstantModel(field);
                        if (constant != null) list.Add(constant);
                    }
                    _constants = list.ToArray();
                }
                return _constants;
            }
        }

        private ConstantModel GetConstantModel(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsSpecialName) return null;
            return new ConstantModel(fieldInfo, ((IConvertible)fieldInfo.GetValue(null)).ToInt64(null));
        }
    }
}

