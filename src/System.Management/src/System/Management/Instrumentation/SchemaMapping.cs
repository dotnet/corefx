//------------------------------------------------------------------------------
// <copyright from='1997' to='2001' company='Microsoft Corporation'>           
//    Copyright (c) Microsoft Corporation. All Rights Reserved.                
//    Information Contained Herein is Proprietary and Confidential.            
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Management.Instrumentation
{
	using System;
	using System.Reflection;
	using System.Management;
	using System.Collections;

	class SchemaMapping
	{
		Type classType;
		ManagementClass newClass;
		string className;
		string classPath;
		string codeClassName;
		CodeWriter code = new CodeWriter();

		public Type ClassType { get { return classType; } }
		public ManagementClass NewClass { get { return newClass; } }
		public string ClassName { get { return className; } }
		public string ClassPath { get { return classPath; } }
		public CodeWriter Code { get { return code; } }
		public string CodeClassName { get {return codeClassName; } }

		InstrumentationType instrumentationType;
		public InstrumentationType InstrumentationType { get { return instrumentationType; } }

		static public void ThrowUnsupportedMember(MemberInfo mi)
		{
			ThrowUnsupportedMember(mi, null);
		}
		static public void ThrowUnsupportedMember(MemberInfo mi, Exception innerException)
		{
			throw new ArgumentException(String.Format(RC.GetString("UNSUPPORTEDMEMBER_EXCEPT"), mi.Name), mi.Name, innerException);
		}
		public SchemaMapping(Type type, SchemaNaming naming, Hashtable mapTypeToConverterClassName)
		{
			codeClassName = (string)mapTypeToConverterClassName[type];
			classType = type;

			bool hasGenericEmbeddedObject = false;

			string baseClassName = ManagedNameAttribute.GetBaseClassName(type);
			className = ManagedNameAttribute.GetMemberName(type);
			instrumentationType = InstrumentationClassAttribute.GetAttribute(type).InstrumentationType;

			classPath = naming.NamespaceName + ":" + className;

			if(null == baseClassName)
			{
				newClass = new ManagementClass(naming.NamespaceName, "", null);
				newClass.SystemProperties ["__CLASS"].Value = className;
			}
			else
			{
				ManagementClass baseClass = new ManagementClass(naming.NamespaceName + ":" + baseClassName);
				if(instrumentationType == InstrumentationType.Instance)
				{
					bool baseAbstract = false;
					try
					{
						QualifierData o = baseClass.Qualifiers["abstract"];
						if(o.Value is bool)
							baseAbstract = (bool)o.Value;
					}
					catch(ManagementException e)
					{
						if(e.ErrorCode != ManagementStatus.NotFound)
							throw;
					}
					if(!baseAbstract)
						throw new Exception(RC.GetString("CLASSINST_EXCEPT"));

				}

				newClass = baseClass.Derive(className);
			}


			// Create the converter class
			CodeWriter codeClass = code.AddChild("public class "+codeClassName+" : IWmiConverter");

			// Create code block for one line Members
			CodeWriter codeOneLineMembers = codeClass.AddChild(new CodeWriter());
			codeOneLineMembers.Line("static ManagementClass managementClass = new ManagementClass(@\"" + classPath + "\");");
			codeOneLineMembers.Line("static IntPtr classWbemObjectIP;");
			codeOneLineMembers.Line("static Guid iidIWbemObjectAccess = new Guid(\"49353C9A-516B-11D1-AEA6-00C04FB68820\");");
			codeOneLineMembers.Line("internal ManagementObject instance = managementClass.CreateInstance();");
			codeOneLineMembers.Line("object reflectionInfoTempObj = null ; ");
			codeOneLineMembers.Line("FieldInfo reflectionIWbemClassObjectField = null ; ");
			codeOneLineMembers.Line("IntPtr emptyWbemObject = IntPtr.Zero ; ");
			codeOneLineMembers.Line("IntPtr originalObject = IntPtr.Zero ; ");
			codeOneLineMembers.Line("bool toWmiCalled = false ; ");
			
			//
			// Reuters VSQFE#: 750	[marioh] see comments above
			// Used as a temporary pointer to the newly created instance that we create to avoid re-using the same
			// object causing unbound memory usage in IWbemClassObject implementation.
			codeOneLineMembers.Line("IntPtr theClone = IntPtr.Zero;");
			codeOneLineMembers.Line("public static ManagementObject emptyInstance = managementClass.CreateInstance();");

			// TODO: Make these non-public
			codeOneLineMembers.Line("public IntPtr instWbemObjectAccessIP;");

			// Create static constructor to initialize handles
			CodeWriter codeCCTOR = codeClass.AddChild("static "+codeClassName+"()");
			codeCCTOR.Line("classWbemObjectIP = (IntPtr)managementClass;");
			codeCCTOR.Line("IntPtr wbemObjectAccessIP;");
			codeCCTOR.Line("Marshal.QueryInterface(classWbemObjectIP, ref iidIWbemObjectAccess, out wbemObjectAccessIP);");
			codeCCTOR.Line("int cimType;");
            
			// Create constructor
			CodeWriter codeCTOR = codeClass.AddChild("public "+codeClassName+"()");
			codeCTOR.Line("IntPtr wbemObjectIP = (IntPtr)instance;");
       	       codeCTOR.Line("originalObject = (IntPtr)instance;");
			codeCTOR.Line("Marshal.QueryInterface(wbemObjectIP, ref iidIWbemObjectAccess, out instWbemObjectAccessIP);");

			//
			// Reuters VSQFE#: 750	[marioh] 
			// In the CCTOR we set things up only once:
			//  1. We get the IWbemClassObjectFreeThreaded object '_wbemObject' from the ManagementObject instance
			//  2. We then get the actual IntPtr to the underlying WMI object
			//  3. Finally, the simple cast to IntPtr from the ManagementObject instance
			// These fields will be used later during the ToWMI call.
			codeCTOR.Line ("FieldInfo tempField = instance.GetType().GetField ( \"_wbemObject\", BindingFlags.Instance | BindingFlags.NonPublic );" );
			codeCTOR.Line("if ( tempField == null )");
			codeCTOR.Line("{");
			codeCTOR.Line("   tempField = instance.GetType().GetField ( \"wbemObject\", BindingFlags.Instance | BindingFlags.NonPublic ) ;");
			codeCTOR.Line("}");

			codeCTOR.Line ("reflectionInfoTempObj = tempField.GetValue (instance) ;");
			codeCTOR.Line("reflectionIWbemClassObjectField = reflectionInfoTempObj.GetType().GetField (\"pWbemClassObject\", BindingFlags.Instance | BindingFlags.NonPublic );");
			codeCTOR.Line("emptyWbemObject = (IntPtr) emptyInstance;");

			// Create destructor that will be called at process cleanup
			CodeWriter codeDTOR = codeClass.AddChild("~"+codeClassName+"()");
			codeDTOR.AddChild("if(instWbemObjectAccessIP != IntPtr.Zero)").Line("Marshal.Release(instWbemObjectAccessIP);");
			// [marioh] Make sure we release the initial instance so that we dont leak
			codeDTOR.Line("if ( toWmiCalled == true )");
			codeDTOR.Line("{");
			codeDTOR.Line("	Marshal.Release (originalObject);");
			codeDTOR.Line("}");
			

			// Create method to convert from managed code to WMI
			CodeWriter codeToWMI = codeClass.AddChild("public void ToWMI(object obj)");

			//
			// Reuters VSQFE#: 750	[marioh] see comments above
			// Ensure the release of the WbemObjectAccess interface pointer.
			codeToWMI.Line( "toWmiCalled = true ;" ) ;
			codeToWMI.Line( "if(instWbemObjectAccessIP != IntPtr.Zero)" ) ;
			codeToWMI.Line( "{" ) ;
			codeToWMI.Line("    Marshal.Release(instWbemObjectAccessIP);" ) ;
			codeToWMI.Line("    instWbemObjectAccessIP = IntPtr.Zero;" ) ;
			codeToWMI.Line( "}" ) ;

			codeToWMI.Line( "if(theClone != IntPtr.Zero)" ) ;
			codeToWMI.Line( "{" ) ;
			codeToWMI.Line("    Marshal.Release(theClone);" ) ;
			codeToWMI.Line("    theClone = IntPtr.Zero;" ) ;
			codeToWMI.Line( "}" ) ;

			codeToWMI.Line( "IWOA.Clone_f(12, emptyWbemObject, out theClone) ;" ) ;
			codeToWMI.Line( "Marshal.QueryInterface(theClone, ref iidIWbemObjectAccess, out instWbemObjectAccessIP) ;" ) ;
			codeToWMI.Line( "reflectionIWbemClassObjectField.SetValue ( reflectionInfoTempObj, theClone ) ;" ) ;

			codeToWMI.Line(String.Format("{0} instNET = ({0})obj;", type.FullName.Replace('+', '.'))); // bug#92918 - watch for nested classes

			// Explicit cast to IntPtr
			CodeWriter codeIntPtrCast = codeClass.AddChild("public static explicit operator IntPtr("+codeClassName+" obj)");
			codeIntPtrCast.Line("return obj.instWbemObjectAccessIP;");

			// Add GetInstance
			codeOneLineMembers.Line("public ManagementObject GetInstance() {return instance;}");

			PropertyDataCollection props = newClass.Properties;

			// type specific info
			switch(instrumentationType)
			{
				case InstrumentationType.Event:
					break;
				case InstrumentationType.Instance:
					props.Add("ProcessId", CimType.String, false);
					props.Add("InstanceId", CimType.String, false);
					props["ProcessId"].Qualifiers.Add("key", true);
					props["InstanceId"].Qualifiers.Add("key", true);
					newClass.Qualifiers.Add("dynamic", true, false, false, false, true);
					newClass.Qualifiers.Add("provider", naming.DecoupledProviderInstanceName, false, false, false, true);
					break;
				case InstrumentationType.Abstract:
					newClass.Qualifiers.Add("abstract", true, false, false, false, true);
					break;
				default:
					break;
			}
			
			int propCount = 0;
			bool needsNullObj = false;
			foreach(MemberInfo field in type.GetMembers())
			{
				if(!(field is FieldInfo || field is PropertyInfo))
					continue;

				if(field.GetCustomAttributes(typeof(IgnoreMemberAttribute), false).Length > 0)
					continue;

				if(field is FieldInfo)
				{
					FieldInfo fi = field as FieldInfo;

					// We ignore statics
					if(fi.IsStatic)
						ThrowUnsupportedMember(field);
				}
				else if (field is PropertyInfo)
				{
					PropertyInfo pi = field as PropertyInfo;
					// We must have a 'get' property accessor
					if(!pi.CanRead)
						ThrowUnsupportedMember(field);

					// We ignore static properties
					MethodInfo mi = pi.GetGetMethod();
					if(null == mi || mi.IsStatic)
						ThrowUnsupportedMember(field);

					// We don't support parameters on properties
					if(mi.GetParameters().Length > 0)
						ThrowUnsupportedMember(field);
				}
                
				String propName = ManagedNameAttribute.GetMemberName(field);


#if SUPPORTS_ALTERNATE_WMI_PROPERTY_TYPE
                Type t2 = ManagedTypeAttribute.GetManagedType(field);
#else
				Type t2;
				if(field is FieldInfo)
					t2 = (field as FieldInfo).FieldType;
				else
					t2 = (field as PropertyInfo).PropertyType;
#endif
				bool isArray = false;
				if(t2.IsArray)
				{
					// We only support one dimensional arrays in this version
					if(t2.GetArrayRank() != 1)
						ThrowUnsupportedMember(field);

					isArray = true;
					t2 = t2.GetElementType();
				}

				string embeddedTypeName = null;
				string embeddedConverterName = null;
				if(mapTypeToConverterClassName.Contains(t2))
				{
					embeddedConverterName = (string)mapTypeToConverterClassName[t2];
					embeddedTypeName = ManagedNameAttribute.GetMemberName(t2);
				}

				bool isGenericEmbeddedObject = false;
				if(t2 == typeof(object))
				{
					isGenericEmbeddedObject = true;
					if(hasGenericEmbeddedObject == false)
					{
						hasGenericEmbeddedObject = true;
						// Add map
						codeOneLineMembers.Line("static Hashtable mapTypeToConverter = new Hashtable();");
						foreach(DictionaryEntry entry in mapTypeToConverterClassName)
						{
							codeCCTOR.Line(String.Format("mapTypeToConverter[typeof({0})] = typeof({1});", ((Type)entry.Key).FullName.Replace('+', '.'), (string)entry.Value)); // bug#92918 - watch for nested classes
						}
					}

				}

				string propFieldName = "prop_" + (propCount);
				string handleFieldName = "handle_" + (propCount++);

				// Add handle for field, which is static accross all instances
				codeOneLineMembers.Line("static int " + handleFieldName + ";");
				codeCCTOR.Line(String.Format("IWOA.GetPropertyHandle_f27(27, wbemObjectAccessIP, \"{0}\", out cimType, out {1});", propName, handleFieldName));

				// Add PropertyData for field, which is specific to each instance
				codeOneLineMembers.Line("PropertyData " + propFieldName + ";");
				codeCTOR.Line(String.Format("{0} = instance.Properties[\"{1}\"];", propFieldName, propName));
				
				if(isGenericEmbeddedObject)
				{
					CodeWriter codeNotNull = codeToWMI.AddChild(String.Format("if(instNET.{0} != null)", field.Name));
					CodeWriter codeElse = codeToWMI.AddChild("else");
					codeElse.Line(String.Format("{0}.Value = null;", propFieldName));
					if(isArray)
					{
						codeNotNull.Line(String.Format("int len = instNET.{0}.Length;", field.Name));
						codeNotNull.Line("ManagementObject[] embeddedObjects = new ManagementObject[len];");
						codeNotNull.Line("IWmiConverter[] embeddedConverters = new IWmiConverter[len];");

						CodeWriter codeForLoop = codeNotNull.AddChild("for(int i=0;i<len;i++)");

						CodeWriter codeFoundType = codeForLoop.AddChild(String.Format("if((instNET.{0}[i] != null) && mapTypeToConverter.Contains(instNET.{0}[i].GetType()))", field.Name));
						codeFoundType.Line(String.Format("Type type = (Type)mapTypeToConverter[instNET.{0}[i].GetType()];", field.Name));
						codeFoundType.Line("embeddedConverters[i] = (IWmiConverter)Activator.CreateInstance(type);");
						codeFoundType.Line(String.Format("embeddedConverters[i].ToWMI(instNET.{0}[i]);", field.Name));
						codeFoundType.Line("embeddedObjects[i] = embeddedConverters[i].GetInstance();");

						codeForLoop.AddChild("else").Line(String.Format("embeddedObjects[i] = SafeAssign.GetManagementObject(instNET.{0}[i]);", field.Name));

						codeNotNull.Line(String.Format("{0}.Value = embeddedObjects;", propFieldName));
					}
					else
					{
						CodeWriter codeFoundType = codeNotNull.AddChild(String.Format("if(mapTypeToConverter.Contains(instNET.{0}.GetType()))", field.Name));
						codeFoundType.Line(String.Format("Type type = (Type)mapTypeToConverter[instNET.{0}.GetType()];", field.Name));
						codeFoundType.Line("IWmiConverter converter = (IWmiConverter)Activator.CreateInstance(type);");
						codeFoundType.Line(String.Format("converter.ToWMI(instNET.{0});", field.Name));
						codeFoundType.Line(String.Format("{0}.Value = converter.GetInstance();", propFieldName));

						codeNotNull.AddChild("else").Line(String.Format("{0}.Value = SafeAssign.GetInstance(instNET.{1});", propFieldName, field.Name));
					}
				}
				else if(embeddedTypeName != null)
				{
					// If this is an embedded struct, it cannot be null
					CodeWriter codeNotNull;
					if(t2.IsValueType)
						codeNotNull = codeToWMI;
					else
					{
						codeNotNull = codeToWMI.AddChild(String.Format("if(instNET.{0} != null)", field.Name));
						CodeWriter codeElse = codeToWMI.AddChild("else");
						codeElse.Line(String.Format("{0}.Value = null;", propFieldName));
					}

					if(isArray)
					{
						codeNotNull.Line(String.Format("int len = instNET.{0}.Length;", field.Name));
						codeNotNull.Line("ManagementObject[] embeddedObjects = new ManagementObject[len];");
						codeNotNull.Line(String.Format("{0}[] embeddedConverters = new {0}[len];", embeddedConverterName));

						CodeWriter codeForLoop = codeNotNull.AddChild("for(int i=0;i<len;i++)");
						codeForLoop.Line(String.Format("embeddedConverters[i] = new {0}();", embeddedConverterName));

						// If this is a struct array, the elements are never null
						if(t2.IsValueType)
						{
							codeForLoop.Line(String.Format("embeddedConverters[i].ToWMI(instNET.{0}[i]);", field.Name));
						}
						else
						{
							CodeWriter codeArrayElementNotNull = codeForLoop.AddChild(String.Format("if(instNET.{0}[i] != null)", field.Name));
							codeArrayElementNotNull.Line(String.Format("embeddedConverters[i].ToWMI(instNET.{0}[i]);", field.Name));
						}
						codeForLoop.Line("embeddedObjects[i] = embeddedConverters[i].instance;");

						codeNotNull.Line(String.Format("{0}.Value = embeddedObjects;", propFieldName));
					}
					else
					{

						// We cannot create an instance of 'embeddedConverterName' because it may be the
						// same type as we are defining (in other words, a cyclic loop, such as class XXX
						// having an instance of an XXX as a member).  To prevent an infinite loop of constructing
						// converter classes, we create a 'lazy' variable that is initialized to NULL, and the first
						// time it is used, we set it to a 'new embeddedConverterName'.
						codeOneLineMembers.Line(String.Format("{0} lazy_embeddedConverter_{1} = null;", embeddedConverterName, propFieldName));
						CodeWriter codeConverterProp = codeClass.AddChild(String.Format("{0} embeddedConverter_{1}", embeddedConverterName, propFieldName));
						CodeWriter codeGet = codeConverterProp.AddChild("get");
						CodeWriter codeIf = codeGet.AddChild(String.Format("if(null == lazy_embeddedConverter_{0})", propFieldName));
						codeIf.Line(String.Format("lazy_embeddedConverter_{0} = new {1}();", propFieldName, embeddedConverterName));
						codeGet.Line(String.Format("return lazy_embeddedConverter_{0};", propFieldName));

						codeNotNull.Line(String.Format("embeddedConverter_{0}.ToWMI(instNET.{1});", propFieldName, field.Name));
						codeNotNull.Line(String.Format("{0}.Value = embeddedConverter_{0}.instance;", propFieldName));
					}

				}
				else if(!isArray)
				{
					if(t2 == typeof(Byte) || t2 == typeof(SByte))
					{
						//
						// [PS#128409, marioh] CS0206 Compile error occured when instrumentated types contains public properties of type SByte, Int16, and UInt16	
						// Properties can not be passed as ref and therefore we store the property value in a tmp local variable before calling WritePropertyValue.
						// 
						codeToWMI.Line(String.Format("{0} instNET_{1} = instNET.{1} ;", t2, field.Name));
						codeToWMI.Line(String.Format("IWOA.WritePropertyValue_f28(28, instWbemObjectAccessIP, {0}, 1, ref instNET_{1});", handleFieldName, field.Name));
					}
					else if(t2 == typeof(Int16) || t2 == typeof(UInt16) || t2 == typeof(Char))
					{
						//
						// [PS#128409, marioh] CS0206 Compile error occured when instrumentated types contains public properties of type SByte, Int16, and UInt16	
						// Properties can not be passed as ref and therefore we store the property value in a tmp local variable before calling WritePropertyValue.
						// 
						codeToWMI.Line(String.Format("{0} instNET_{1} = instNET.{1} ;", t2, field.Name));	
						codeToWMI.Line(String.Format("IWOA.WritePropertyValue_f28(28, instWbemObjectAccessIP, {0}, 2, ref instNET_{1});", handleFieldName, field.Name));
					}
					else if(t2 == typeof(UInt32) || t2 == typeof(Int32) || t2 == typeof(Single))
						codeToWMI.Line(String.Format("IWOA.WriteDWORD_f31(31, instWbemObjectAccessIP, {0}, instNET.{1});", handleFieldName, field.Name));
					else if(t2 == typeof(UInt64) || t2 == typeof(Int64) || t2 == typeof(Double))
						codeToWMI.Line(String.Format("IWOA.WriteQWORD_f33(33, instWbemObjectAccessIP, {0}, instNET.{1});", handleFieldName, field.Name));
					else if(t2 == typeof(Boolean))
					{
						// TODO: Fix this to use IWOA
						//                        codeToWMI.Line(String.Format("{0}.Value = instNET.{1};", propFieldName, field.Name));
						codeToWMI.Line(String.Format("if(instNET.{0})", field.Name));
						codeToWMI.Line(String.Format("    IWOA.WritePropertyValue_f28(28, instWbemObjectAccessIP, {0}, 2, ref SafeAssign.boolTrue);", handleFieldName));
						codeToWMI.Line("else");
						codeToWMI.Line(String.Format("    IWOA.WritePropertyValue_f28(28, instWbemObjectAccessIP, {0}, 2, ref SafeAssign.boolFalse);", handleFieldName));
					}
					else if(t2 == typeof(String))
					{
						CodeWriter codeQuickString = codeToWMI.AddChild(String.Format("if(null != instNET.{0})", field.Name));
						codeQuickString.Line(String.Format("IWOA.WritePropertyValue_f28(28, instWbemObjectAccessIP, {0}, (instNET.{1}.Length+1)*2, instNET.{1});", handleFieldName, field.Name));
						//                        codeToWMI.AddChild("else").Line(String.Format("{0}.Value = instNET.{1};", propFieldName, field.Name));
						codeToWMI.AddChild("else").Line(String.Format("IWOA.Put_f5(5, instWbemObjectAccessIP, \"{0}\", 0, ref nullObj, 8);", propName));
						if(needsNullObj == false)
						{
							needsNullObj = true;

							// Bug#111623 - This line used to say 'nullObj = null;'  When nullObj was passed
							// to IWOA.Put, this did NOT set the value of a string variable to NULL.  The correct
							// thing to do was to pass a reference to DBNull.Value to IWOA.Put instead.
							codeOneLineMembers.Line("object nullObj = DBNull.Value;");
						}
					}
					else if(t2 == typeof(DateTime) || t2 == typeof(TimeSpan))
					{
						codeToWMI.Line(String.Format("IWOA.WritePropertyValue_f28(28, instWbemObjectAccessIP, {0}, 52, SafeAssign.WMITimeToString(instNET.{1}));", handleFieldName, field.Name));
						//                        codeToWMI.Line(String.Format("{0}.Value = SafeAssign.DateTimeToString(instNET.{1});", propFieldName, field.Name));
					}
					else
						codeToWMI.Line(String.Format("{0}.Value = instNET.{1};", propFieldName, field.Name));
				}
				else
				{
					// We have an array type
					if(t2 == typeof(DateTime) || t2 == typeof(TimeSpan))
					{
						codeToWMI.AddChild(String.Format("if(null == instNET.{0})", field.Name)).Line(String.Format("{0}.Value = null;", propFieldName));
						codeToWMI.AddChild("else").Line(String.Format("{0}.Value = SafeAssign.WMITimeArrayToStringArray(instNET.{1});", propFieldName, field.Name));
					}
					else
					{
						// This handles arrays of all primative types
						codeToWMI.Line(String.Format("{0}.Value = instNET.{1};", propFieldName, field.Name));
					}
				}


				CimType cimtype = CimType.String;

				if(field.DeclaringType != type)
					continue;


#if REQUIRES_EXPLICIT_DECLARATION_OF_INHERITED_PROPERTIES
                if(InheritedPropertyAttribute.GetAttribute(field) != null)
                    continue;
#else
				// See if this field already exists on the WMI class
				// In other words, is it inherited from a base class
				// TODO: Make this more efficient
				//  - If we have a null base class name, all property names
				//    should be new
				//  - We could get all base class property names into a
				//    hashtable, and look them up from there
				bool propertyExists = true;
				try
				{
					PropertyData prop = newClass.Properties[propName];
					// HACK for bug#96863 - The above line used to throw a
					// not found exception.  This was changed with a recent
					// checkin.  If this functionality is not reverted, the
					// following statement should force the necessary 'not
					// found' exception that we are looking for.
					CimType cimType = prop.Type;

					// Make sure that if the property exists, it is inherited
					// If it is local, they probably named two properties with
					// the same name
					if(prop.IsLocal)
					{
						throw new ArgumentException(String.Format(RC.GetString("MEMBERCONFLILCT_EXCEPT"), field.Name), field.Name);
					}
				}
				catch(ManagementException e)
				{
					if(e.ErrorCode != ManagementStatus.NotFound)
						throw;
					else
						propertyExists = false;
				}
				if(propertyExists)
					continue;
#endif


				if(embeddedTypeName != null)
					cimtype = CimType.Object;
				else if(isGenericEmbeddedObject)
					cimtype = CimType.Object;
				else if(t2 == typeof(ManagementObject))
					cimtype = CimType.Object;
				else if(t2 == typeof(SByte))
					cimtype = CimType.SInt8;
				else if(t2 == typeof(Byte))
					cimtype = CimType.UInt8;
				else if(t2 == typeof(Int16))
					cimtype = CimType.SInt16;
				else if(t2 == typeof(UInt16))
					cimtype = CimType.UInt16;
				else if(t2 == typeof(Int32))
					cimtype = CimType.SInt32;
				else if(t2 == typeof(UInt32))
					cimtype = CimType.UInt32;
				else if(t2 == typeof(Int64))
					cimtype = CimType.SInt64;
				else if(t2 == typeof(UInt64))
					cimtype = CimType.UInt64;
				else if(t2 == typeof(Single))
					cimtype = CimType.Real32;
				else if(t2 == typeof(Double))
					cimtype = CimType.Real64;
				else if(t2 == typeof(Boolean))
					cimtype = CimType.Boolean;
				else if(t2 == typeof(String))
					cimtype = CimType.String;
				else if(t2 == typeof(Char))
					cimtype = CimType.Char16;
				else if(t2 == typeof(DateTime))
					cimtype = CimType.DateTime;
				else if(t2 == typeof(TimeSpan))
					cimtype = CimType.DateTime;
				else
					ThrowUnsupportedMember(field);
				// HACK: The following line cause a strange System.InvalidProgramException when run through InstallUtil
				//				throw new Exception("Unsupported type for event member - " + t2.Name);


				//              TODO: if(t2 == typeof(Decimal))

#if SUPPORTS_WMI_DEFAULT_VAULES
                Object defaultValue = ManagedDefaultValueAttribute.GetManagedDefaultValue(field);

                // TODO: Is it safe to make this one line?
                if(null == defaultValue)
                    props.Add(propName, cimtype, false);
                else
                    props.Add(propName, defaultValue, cimtype);
#else
				try
				{
					props.Add(propName, cimtype, isArray);
				}
				catch(ManagementException e)
				{
					ThrowUnsupportedMember(field, e);
				}
#endif

				// Must at 'interval' SubType on TimeSpans
				if(t2 == typeof(TimeSpan))
				{
					PropertyData prop = props[propName];
					prop.Qualifiers.Add("SubType", "interval", false, true, true, true);
				}

				if(embeddedTypeName != null)
				{
					PropertyData prop = props[propName];
					prop.Qualifiers["CIMTYPE"].Value = "object:"+embeddedTypeName;
				}
			}
			codeCCTOR.Line("Marshal.Release(wbemObjectAccessIP);");
			//            codeToWMI.Line("Console.WriteLine(instance.GetText(TextFormat.Mof));");
		}
	}
}
