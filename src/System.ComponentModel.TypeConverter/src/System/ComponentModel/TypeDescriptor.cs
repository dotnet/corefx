// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace System.ComponentModel
{
    /// <summary>
    ///    Provides information about the properties and events
    ///    for a component. This class cannot be inherited.
    /// </summary>
    public sealed class TypeDescriptor
    {
        // Note: this is initialized at class load because we 
        // lock on it for thread safety.  It is used from nearly
        // every call to this class, so it will be created soon after
        // class load anyway.
        private static readonly WeakHashtable s_providerTable = new WeakHashtable();     // mapping of type or object hash to a provider list
        private static readonly Hashtable s_providerTypeTable = new Hashtable();         // A direct mapping from type to provider.
        private static readonly Hashtable s_defaultProviders = new Hashtable(); // A table of type -> default provider to track DefaultTypeDescriptionProviderAttributes.
        private static WeakHashtable s_associationTable;
        private static int s_metadataVersion;                          // a version stamp for our metadata.  Used by property descriptors to know when to rebuild attributes.

        // This is an index that we use to create a unique name for a property in the
        // event of a name collision.  The only time we should use this is when
        // a name collision happened on an extender property that has no site or
        // no name on its site.  Should be very rare.
        private static int s_collisionIndex;

        // For each stage of our filtering pipeline, the pipeline needs to know what it is filtering.
        private const int PIPELINE_ATTRIBUTES = 0x00;
        private const int PIPELINE_PROPERTIES = 0x01;
        private const int PIPELINE_EVENTS = 0x02;

        // And each stage of the pipeline needs to have its own
        // keys for its cache table.  We use guids because they
        // are unique and fast to compare.  The order for each of
        // these keys must match the Id's of the filter type above.
        private static readonly Guid[] s_pipelineInitializeKeys = new Guid[]
        {
            Guid.NewGuid(), // attributes
            Guid.NewGuid(), // properties
            Guid.NewGuid()  // events
        };

        private static readonly Guid[] s_pipelineMergeKeys = new Guid[]
        {
            Guid.NewGuid(), // attributes
            Guid.NewGuid(), // properties
            Guid.NewGuid()  // events
        };

        private static readonly Guid[] s_pipelineFilterKeys = new Guid[]
        {
            Guid.NewGuid(), // attributes
            Guid.NewGuid(), // properties
            Guid.NewGuid()  // events
        };

        private static readonly Guid[] s_pipelineAttributeFilterKeys = new Guid[]
        {
            Guid.NewGuid(), // attributes
            Guid.NewGuid(), // properties
            Guid.NewGuid()  // events
        };

        private static readonly object s_internalSyncObject = new object();

        private TypeDescriptor()
        {
        }

        /// <summary>
        ///     This property returns a Type object that can be passed to the various 
        ///     AddProvider methods to define a type description provider for interface types.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Type InterfaceType => typeof(TypeDescriptorInterface);

        /// <summary>
        ///     This value increments each time someone refreshes or changes metadata.
        /// </summary>
        internal static int MetadataVersion => s_metadataVersion;

        private static WeakHashtable AssociationTable => LazyInitializer.EnsureInitialized(ref s_associationTable, () => new WeakHashtable());

        /// <summary>
        ///    Occurs when Refreshed is raised for a component.
        /// </summary>
        public static event RefreshEventHandler Refreshed;

        /// <summary>
        ///     The AddAttributes method allows you to add class-level attributes for a 
        ///     type or an instance.  This method simply implements a type description provider 
        ///     that merges the provided attributes with the attributes that already exist on 
        ///     the class.  This is a short cut for such a behavior.  Adding additional 
        ///     attributes is common need for applications using the Windows Forms property 
        ///     window.  The return value form AddAttributes is the TypeDescriptionProvider 
        ///     that was used to add the attributes.  This provider can later be passed to 
        ///     RemoveProvider if the added attributes are no longer needed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static TypeDescriptionProvider AddAttributes(Type type, params Attribute[] attributes)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            TypeDescriptionProvider existingProvider = GetProvider(type);
            TypeDescriptionProvider provider = new AttributeProvider(existingProvider, attributes);
            TypeDescriptor.AddProvider(provider, type);
            return provider;
        }

        /// <summary>
        ///     The AddAttributes method allows you to add class-level attributes for a 
        ///     type or an instance.  This method simply implements a type description provider 
        ///     that merges the provided attributes with the attributes that already exist on 
        ///     the class.  This is a short cut for such a behavior.  Adding additional 
        ///     attributes is common need for applications using the Windows Forms property 
        ///     window.  The return value form AddAttributes is the TypeDescriptionProvider 
        ///     that was used to add the attributes.  This provider can later be passed to 
        ///     RemoveProvider if the added attributes are no longer needed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static TypeDescriptionProvider AddAttributes(object instance, params Attribute[] attributes)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            TypeDescriptionProvider existingProvider = GetProvider(instance);
            TypeDescriptionProvider provider = new AttributeProvider(existingProvider, attributes);
            TypeDescriptor.AddProvider(provider, instance);
            return provider;
        }

        /// <internalonly/>
        /// <summary>
        ///     Adds an editor table for the given editor base type. Typically, editors are
        ///     specified as metadata on an object. If no metadata for a requested editor
        ///     base type can be found on an object, however, the TypeDescriptor will search
        ///     an editor table for the editor type, if one can be found.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void AddEditorTable(Type editorBaseType, Hashtable table)
        {
            ReflectTypeDescriptionProvider.AddEditorTable(editorBaseType, table);
        }

        /// <summary>
        ///     Adds a type description provider that will be called on to provide 
        ///     type and instance information for any object that is of, or a subtype 
        ///     of, the provided type.  Type can be any type, including interfaces.  
        ///     For example, to provide custom type and instance information for all 
        ///     components, you would pass typeof(IComponent).  Passing typeof(object) 
        ///     will cause the provider to be called to provide type information for 
        ///     all types.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void AddProvider(TypeDescriptionProvider provider, Type type)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            lock (s_providerTable)
            {
                // Get the root node, hook it up, and stuff it back into
                // the provider cache.
                TypeDescriptionNode node = NodeFor(type, true);
                TypeDescriptionNode head = new TypeDescriptionNode(provider);
                head.Next = node;
                s_providerTable[type] = head;
                s_providerTypeTable.Clear();
            }

            Refresh(type);
        }

        /// <summary>
        ///     Adds a type description provider that will be called on to provide 
        ///     type information for a single object instance.  A provider added 
        ///     using this method will never have its CreateInstance method called 
        ///     because the instance already exists.  This method does not prevent 
        ///     the object from finalizing.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void AddProvider(TypeDescriptionProvider provider, object instance)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            bool refreshNeeded;

            // Get the root node, hook it up, and stuff it back into
            // the provider cache.
            lock (s_providerTable)
            {
                refreshNeeded = s_providerTable.ContainsKey(instance);
                TypeDescriptionNode node = NodeFor(instance, true);
                TypeDescriptionNode head = new TypeDescriptionNode(provider);
                head.Next = node;
                s_providerTable.SetWeak(instance, head);
                s_providerTypeTable.Clear();
            }

            if (refreshNeeded)
            {
                Refresh(instance, false);
            }
        }

        /// <summary>
        ///     Adds a type description provider that will be called on to provide 
        ///     type and instance information for any object that is of, or a subtype 
        ///     of, the provided type.  Type can be any type, including interfaces.  
        ///     For example, to provide custom type and instance information for all 
        ///     components, you would pass typeof(IComponent).  Passing typeof(object) 
        ///     will cause the provider to be called to provide type information for 
        ///     all types.
        ///     
        ///     This method can be called from partially trusted code. If 
        ///     <see cref="TypeDescriptorPermissionFlags.RestrictedRegistrationAccess"/>
        ///     is defined, the caller can register a provider for the specified type 
        ///     if it's also partially trusted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void AddProviderTransparent(TypeDescriptionProvider provider, Type type)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            AddProvider(provider, type);
        }

        /// <summary>
        ///     Adds a type description provider that will be called on to provide 
        ///     type information for a single object instance.  A provider added 
        ///     using this method will never have its CreateInstance method called 
        ///     because the instance already exists.  This method does not prevent 
        ///     the object from finalizing.
        ///     
        ///     This method can be called from partially trusted code. If 
        ///     <see cref="TypeDescriptorPermissionFlags.RestrictedRegistrationAccess"/>
        ///     is defined, the caller can register a provider for the specified instance 
        ///     if its type is also partially trusted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void AddProviderTransparent(TypeDescriptionProvider provider, object instance)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            Type type = instance.GetType();

            AddProvider(provider, instance);
        }

        /// <summary>
        ///     This method verifies that we have checked for the presence
        ///     of a default type description provider attribute for the
        ///     given type.
        /// </summary>
        private static void CheckDefaultProvider(Type type)
        {
            if (s_defaultProviders.ContainsKey(type))
            {
                return;
            }

            lock (s_internalSyncObject)
            {
                if (s_defaultProviders.ContainsKey(type))
                {
                    return;
                }

                // Immediately clear this. If we find a default provider
                // and it starts messing around with type information, 
                // this could infinitely recurse.
                //
                s_defaultProviders[type] = null;
            }

            // Always use core reflection when checking for
            // the default provider attribute.  If there is a
            // provider, we probably don't want to build up our
            // own cache state against the type.  There shouldn't be
            // more than one of these, but walk anyway.  Walk in 
            // reverse order so that the most derived takes precidence.
            //
            object[] attrs = type.GetCustomAttributes(typeof(TypeDescriptionProviderAttribute), false).ToArray();
            bool providerAdded = false;
            for (int idx = attrs.Length - 1; idx >= 0; idx--)
            {
                TypeDescriptionProviderAttribute pa = (TypeDescriptionProviderAttribute)attrs[idx];
                Type providerType = Type.GetType(pa.TypeName);
                if (providerType != null && typeof(TypeDescriptionProvider).IsAssignableFrom(providerType))
                {
                    TypeDescriptionProvider prov = (TypeDescriptionProvider)Activator.CreateInstance(providerType);
                    AddProvider(prov, type);
                    providerAdded = true;
                }
            }

            // If we did not add a provider, check the base class.  
            if (!providerAdded)
            {
                Type baseType = type.BaseType;
                if (baseType != null && baseType != type)
                {
                    CheckDefaultProvider(baseType);
                }
            }
        }

        /// <summary>
        ///     The CreateAssocation method creates an association between two objects.  
        ///     Once an association is created, a designer or other filtering mechanism 
        ///     can add properties that route to either object into the primary object's 
        ///     property set.  When a property invocation is made against the primary 
        ///     object, GetAssocation will be called to resolve the actual object 
        ///     instance that is related to its type parameter.  
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void CreateAssociation(object primary, object secondary)
        {
            if (primary == null)
            {
                throw new ArgumentNullException(nameof(primary));
            }

            if (secondary == null)
            {
                throw new ArgumentNullException(nameof(secondary));
            }

            if (primary == secondary)
            {
                throw new ArgumentException(SR.TypeDescriptorSameAssociation);
            }

            WeakHashtable associationTable = AssociationTable;
            IList associations = (IList)associationTable[primary];

            if (associations == null)
            {
                lock (associationTable)
                {
                    associations = (IList)associationTable[primary];
                    if (associations == null)
                    {
                        associations = new ArrayList(4);
                        associationTable.SetWeak(primary, associations);
                    }
                }
            }
            else
            {
                for (int idx = associations.Count - 1; idx >= 0; idx--)
                {
                    WeakReference r = (WeakReference)associations[idx];
                    if (r.IsAlive && r.Target == secondary)
                    {
                        throw new ArgumentException(SR.TypeDescriptorAlreadyAssociated);
                    }
                }
            }

            lock (associations)
            {
                associations.Add(new WeakReference(secondary));
            }
        }

        /// <summary>
        ///     This dynamically binds an EventDescriptor to a type.
        /// </summary>
        public static EventDescriptor CreateEvent(Type componentType, string name, Type type, params Attribute[] attributes)
        {
            return new ReflectEventDescriptor(componentType, name, type, attributes);
        }

        /// <summary>
        ///     This creates a new event descriptor identical to an existing event descriptor.  The new event descriptor
        ///     has the specified metadata attributes merged with the existing metadata attributes.
        /// </summary>
        public static EventDescriptor CreateEvent(Type componentType, EventDescriptor oldEventDescriptor, params Attribute[] attributes)
        {
            return new ReflectEventDescriptor(componentType, oldEventDescriptor, attributes);
        }

        /// <summary>
        ///     This method will search internal tables within TypeDescriptor for 
        ///     a TypeDescriptionProvider object that is associated with the given 
        ///     data type.  If it finds one, it will delegate the call to that object.  
        /// </summary>
        public static object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            if (argTypes != null)
            {
                if (args == null)
                {
                    throw new ArgumentNullException(nameof(args));
                }

                if (argTypes.Length != args.Length)
                {
                    throw new ArgumentException(SR.TypeDescriptorArgsCountMismatch);
                }
            }

            object instance = null;

            // See if the provider wants to offer a TypeDescriptionProvider to delegate to.  This allows
            // a caller to have complete control over all object instantiation.
            TypeDescriptionProvider p = provider?.GetService(typeof(TypeDescriptionProvider)) as TypeDescriptionProvider;
            if (p != null)
            {
                instance = p.CreateInstance(provider, objectType, argTypes, args);
            }

            return instance ?? NodeFor(objectType).CreateInstance(provider, objectType, argTypes, args);
        }
        /// <summary>
        ///     This dynamically binds a PropertyDescriptor to a type.
        /// </summary>
        public static PropertyDescriptor CreateProperty(Type componentType, string name, Type type, params Attribute[] attributes)
        {
            return new ReflectPropertyDescriptor(componentType, name, type, attributes);
        }

        /// <summary>
        ///     This creates a new property descriptor identical to an existing property descriptor.  The new property descriptor
        ///     has the specified metadata attributes merged with the existing metadata attributes.
        /// </summary>
        public static PropertyDescriptor CreateProperty(Type componentType, PropertyDescriptor oldPropertyDescriptor, params Attribute[] attributes)
        {
            // We must do some special case work here for extended properties.  If the old property descriptor is really
            // an extender property that is being surfaced on a component as a normal property, then we must
            // do work here or else ReflectPropertyDescriptor will fail to resolve the get and set methods.  We check
            // for the necessary ExtenderProvidedPropertyAttribute and if we find it, we create an
            // ExtendedPropertyDescriptor instead.  We only do this if the component class is the same, since the user
            // may want to re-route the property to a different target.
            //
            if (componentType == oldPropertyDescriptor.ComponentType)
            {
                ExtenderProvidedPropertyAttribute attr = (ExtenderProvidedPropertyAttribute)
                                                         oldPropertyDescriptor.Attributes[
                                                         typeof(ExtenderProvidedPropertyAttribute)];

                ReflectPropertyDescriptor reflectDesc = attr.ExtenderProperty as ReflectPropertyDescriptor;
                if (reflectDesc != null)
                {
                    return new ExtendedPropertyDescriptor(oldPropertyDescriptor, attributes);
                }
            }

            // This is either a normal prop or the caller has changed target classes.
            //
            return new ReflectPropertyDescriptor(componentType, oldPropertyDescriptor, attributes);
        }

        /// <summary>
        ///     This  API is used to remove any members from the given
        ///     collection that do not match the attribute array.  If members
        ///     need to be removed, a new ArrayList wil be created that
        ///     contains only the remaining members. The API returns
        ///     NULL if it did not need to filter any members.
        /// </summary>
        private static ArrayList FilterMembers(IList members, Attribute[] attributes)
        {
            ArrayList newMembers = null;
            int memberCount = members.Count;

            for (int idx = 0; idx < memberCount; idx++)
            {
                bool hide = false;

                for (int attrIdx = 0; attrIdx < attributes.Length; attrIdx++)
                {
                    if (ShouldHideMember((MemberDescriptor)members[idx], attributes[attrIdx]))
                    {
                        hide = true;
                        break;
                    }
                }

                if (hide)
                {
                    // We have to hide.  If this is the first time, we need to init
                    // newMembers to have all the valid members we have previously
                    // hit.
                    if (newMembers == null)
                    {
                        newMembers = new ArrayList(memberCount);
                        for (int validIdx = 0; validIdx < idx; validIdx++)
                        {
                            newMembers.Add(members[validIdx]);
                        }
                    }
                }
                else
                {
                    newMembers?.Add(members[idx]);
                }
            }

            return newMembers;
        }

        /// <summary>
        ///     The GetAssociation method returns the correct object to invoke 
        ///     for the requested type.  It never returns null.  
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static object GetAssociation(Type type, object primary)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (primary == null)
            {
                throw new ArgumentNullException(nameof(primary));
            }

            object associatedObject = primary;

            if (!type.IsInstanceOfType(primary))
            {
                // Check our association table for a match.
                //
                Hashtable assocTable = AssociationTable;
                IList associations = (IList) assocTable?[primary];
                if (associations != null)
                {
                    lock (associations)
                    {
                        for (int idx = associations.Count - 1; idx >= 0; idx--)
                        {
                            // Look for an associated object that has a type that
                            // matches the given type.
                            //
                            WeakReference weakRef = (WeakReference)associations[idx];
                            object secondary = weakRef.Target;
                            if (secondary == null)
                            {
                                associations.RemoveAt(idx);
                            }
                            else if (type.IsInstanceOfType(secondary))
                            {
                                associatedObject = secondary;
                            }
                        }
                    }
                }

#if FEATURE_IDESIGNERHOST
                // Not in our table.  We have a default association with a designer 
                // if that designer is a component.
                //
                if (associatedObject == primary)
                {
                    IComponent component = primary as IComponent;
                    if (component != null)
                    {
                        ISite site = component.Site;

                        if (site != null && site.DesignMode)
                        {
                            IDesignerHost host = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                            if (host != null)
                            {
                                object designer = host.GetDesigner(component);

                                // We only use the designer if it has a compatible class.  If we
                                // got here, we're probably hosed because the user just passed in
                                // an object that this PropertyDescriptor can't munch on, but it's
                                // clearer to use that object instance instead of it's designer.
                                //
                                if (designer != null && type.IsInstanceOfType(designer))
                                {
                                    associatedObject = designer;
                                }
                            }
                        }
                    }
                }
#endif
            }

            return associatedObject;
        }

        /// <summary>
        ///     Gets a collection of attributes for the specified type of component.
        /// </summary>
        public static AttributeCollection GetAttributes(Type componentType)
        {
            if (componentType == null)
            {
                Debug.Fail("COMPAT:  Returning an empty collection, but you should not pass null here");
                return new AttributeCollection((Attribute[])null);
            }

            AttributeCollection attributes = GetDescriptor(componentType, "componentType").GetAttributes();
            return attributes;
        }

        /// <summary>
        ///     Gets a collection of attributes for the specified component.
        /// </summary>
        public static AttributeCollection GetAttributes(object component)
        {
            return GetAttributes(component, false);
        }

        /// <summary>
        ///     Gets a collection of attributes for the specified component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static AttributeCollection GetAttributes(object component, bool noCustomTypeDesc)
        {
            if (component == null)
            {
                Debug.Fail("COMPAT:  Returning an empty collection, but you should not pass null here");
                return new AttributeCollection((Attribute[])null);
            }

            // We create a sort of pipeline for mucking with metadata.  The pipeline
            // goes through the following process:
            //
            // 1.  Merge metadata from extenders.
            // 2.  Allow services to filter the metadata
            // 3.  If an attribute filter was specified, apply that.
            // 
            // The goal here is speed.  We get speed by not copying or
            // allocating memory.  We do this by allowing each phase of the
            // pipeline to cache its data in the object cache.  If
            // a phase makes a change to the results, this change must cause
            // successive phases to recompute their results as well.  "Results" is
            // always a collection, and the various stages of the pipeline may
            // replace or modify this collection (depending on if it's a
            // read-only IList or not).  It is possible for the orignal
            // descriptor or attribute collection to pass through the entire
            // pipeline without modification.
            // 
            ICustomTypeDescriptor typeDesc = GetDescriptor(component, noCustomTypeDesc);
            ICollection results = typeDesc.GetAttributes();

            // If we are handed a custom type descriptor we have several choices of action
            // we can take.  If noCustomTypeDesc is true, it means that the custom type
            // descriptor is trying to find a baseline set of properties.  In this case
            // we should merge in extended properties, but we do not let designers filter
            // because we're not done with the property set yet.  If noCustomTypeDesc
            // is false, we don't do extender properties because the custom type descriptor
            // has already added them.  In this case, we are doing a final pass so we
            // want to apply filtering.  Finally, if the incoming object is not a custom
            // type descriptor, we do extenders and the filter.
            //
            if (component is ICustomTypeDescriptor)
            {
                if (noCustomTypeDesc)
                {
                    ICustomTypeDescriptor extDesc = GetExtendedDescriptor(component);
                    if (extDesc != null)
                    {
                        ICollection extResults = extDesc.GetAttributes();
                        results = PipelineMerge(PIPELINE_ATTRIBUTES, results, extResults, component, null);
                    }
                }
                else
                {
                    results = PipelineFilter(PIPELINE_ATTRIBUTES, results, component, null);
                }
            }
            else
            {
                IDictionary cache = GetCache(component);

                results = PipelineInitialize(PIPELINE_ATTRIBUTES, results, cache);

                ICustomTypeDescriptor extDesc = GetExtendedDescriptor(component);
                if (extDesc != null)
                {
                    ICollection extResults = extDesc.GetAttributes();
                    results = PipelineMerge(PIPELINE_ATTRIBUTES, results, extResults, component, cache);
                }

                results = PipelineFilter(PIPELINE_ATTRIBUTES, results, component, cache);
            }

            AttributeCollection attrs = results as AttributeCollection;
            if (attrs == null)
            {
                Attribute[] attrArray = new Attribute[results.Count];
                results.CopyTo(attrArray, 0);
                attrs = new AttributeCollection(attrArray);
            }

            return attrs;
        }

        /// <summary>
        ///     Helper function to obtain a cache for the given object.
        /// </summary>
        internal static IDictionary GetCache(object instance)
        {
            return NodeFor(instance).GetCache(instance);
        }

        /// <summary>
        ///     Gets the name of the class for the specified component.
        /// </summary>
        public static string GetClassName(object component)
        {
            return GetClassName(component, false);
        }

        /// <summary>
        ///     Gets the name of the class for the specified component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static string GetClassName(object component, bool noCustomTypeDesc)
        {
            return GetDescriptor(component, noCustomTypeDesc).GetClassName();
        }

        /// <summary>
        ///     Gets the name of the class for the specified type.
        /// </summary>
        public static string GetClassName(Type componentType)
        {
            return GetDescriptor(componentType, "componentType").GetClassName();
        }

        /// <summary>
        ///       The name of the class for the specified component.
        /// </summary>
        public static string GetComponentName(object component)
        {
            return GetComponentName(component, false);
        }

        /// <summary>
        ///    Gets the name of the class for the specified component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static string GetComponentName(object component, bool noCustomTypeDesc)
        {
            return GetDescriptor(component, noCustomTypeDesc).GetComponentName();
        }

        /// <summary>
        ///    Gets a type converter for the type of the specified component.
        /// </summary>
        public static TypeConverter GetConverter(object component)
        {
            return GetConverter(component, false);
        }
        /// <summary>
        ///    Gets a type converter for the type of the specified component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static TypeConverter GetConverter(object component, bool noCustomTypeDesc)
        {
            TypeConverter converter = GetDescriptor(component, noCustomTypeDesc).GetConverter();
            return converter;
        }

        /// <summary>
        ///    Gets a type converter for the specified type.
        /// </summary>
        public static TypeConverter GetConverter(Type type)
        {
            TypeConverter converter = GetDescriptor(type, "type").GetConverter();
            return converter;
        }

        /// <summary>
        ///     Gets the default event for the specified type of component.
        /// </summary>
        public static EventDescriptor GetDefaultEvent(Type componentType)
        {
            if (componentType == null)
            {
                Debug.Fail("COMPAT:  Returning null, but you should not pass null here");
                return null;
            }

            return GetDescriptor(componentType, "componentType").GetDefaultEvent();
        }

        /// <summary>
        ///     Gets the default event for the specified component.
        /// </summary>
        public static EventDescriptor GetDefaultEvent(object component)
        {
            return GetDefaultEvent(component, false);
        }

        /// <summary>
        ///     Gets the default event for a component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static EventDescriptor GetDefaultEvent(object component, bool noCustomTypeDesc)
        {
            if (component == null)
            {
                Debug.Fail("COMPAT:  Returning null, but you should not pass null here");
                return null;
            }

            return GetDescriptor(component, noCustomTypeDesc).GetDefaultEvent();
        }

        /// <summary>
        ///     Gets the default property for the specified type of component.
        /// </summary>
        public static PropertyDescriptor GetDefaultProperty(Type componentType)
        {
            if (componentType == null)
            {
                Debug.Fail("COMPAT:  Returning an empty collection, but you should not pass null here");
                return null;
            }

            return GetDescriptor(componentType, "componentType").GetDefaultProperty();
        }

        /// <summary>
        ///     Gets the default property for the specified component.
        /// </summary>
        public static PropertyDescriptor GetDefaultProperty(object component)
        {
            return GetDefaultProperty(component, false);
        }

        /// <summary>
        ///     Gets the default property for the specified component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static PropertyDescriptor GetDefaultProperty(object component, bool noCustomTypeDesc)
        {
            if (component == null)
            {
                Debug.Fail("COMPAT:  Returning null, but you should not pass null here");
                return null;
            }

            return GetDescriptor(component, noCustomTypeDesc).GetDefaultProperty();
        }

        /// <summary>
        ///     Returns a custom type descriptor for the given type.
        ///     Performs arg checking so callers don't have to.
        /// </summary>
        internal static ICustomTypeDescriptor GetDescriptor(Type type, string typeName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(typeName);
            }

            return NodeFor(type).GetTypeDescriptor(type);
        }

        /// <summary>
        ///     Returns a custom type descriptor for the given instance.
        ///     Performs arg checking so callers don't have to.  This
        ///     will call through to instance if it is a custom type
        ///     descriptor.
        /// </summary>
        internal static ICustomTypeDescriptor GetDescriptor(object component, bool noCustomTypeDesc)
        {
            if (component == null)
            {
                throw new ArgumentException(nameof(component));
            }

            if (component is IUnimplemented)
            {
                throw new NotSupportedException(SR.Format(SR.TypeDescriptorUnsupportedRemoteObject, component.GetType().FullName));
            }


            ICustomTypeDescriptor desc = NodeFor(component).GetTypeDescriptor(component);
            ICustomTypeDescriptor d = component as ICustomTypeDescriptor;
            if (!noCustomTypeDesc && d != null)
            {
                desc = new MergedTypeDescriptor(d, desc);
            }

            return desc;
        }

        /// <summary>
        ///     Returns an extended custom type descriptor for the given instance.
        /// </summary>
        internal static ICustomTypeDescriptor GetExtendedDescriptor(object component)
        {
            if (component == null)
            {
                throw new ArgumentException(nameof(component));
            }

            return NodeFor(component).GetExtendedTypeDescriptor(component);
        }

        /// <summary>
        ///     Gets an editor with the specified base type for the
        ///     specified component.
        /// </summary>
        public static object GetEditor(object component, Type editorBaseType)
        {
            return GetEditor(component, editorBaseType, false);
        }

        /// <summary>
        ///     Gets an editor with the specified base type for the
        ///     specified component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static object GetEditor(object component, Type editorBaseType, bool noCustomTypeDesc)
        {
            if (editorBaseType == null)
            {
                throw new ArgumentNullException(nameof(editorBaseType));
            }

            return GetDescriptor(component, noCustomTypeDesc).GetEditor(editorBaseType);
        }

        /// <summary>
        ///    Gets an editor with the specified base type for the specified type.
        /// </summary>
        public static object GetEditor(Type type, Type editorBaseType)
        {
            if (editorBaseType == null)
            {
                throw new ArgumentNullException(nameof(editorBaseType));
            }

            return GetDescriptor(type, "type").GetEditor(editorBaseType);
        }

        /// <summary>
        ///     Gets a collection of events for a specified type of component.
        /// </summary>
        public static EventDescriptorCollection GetEvents(Type componentType)
        {
            if (componentType == null)
            {
                Debug.Fail("COMPAT:  Returning an empty collection, but you should not pass null here");
                return new EventDescriptorCollection(null, true);
            }

            return GetDescriptor(componentType, "componentType").GetEvents();
        }

        /// <summary>
        ///     Gets a collection of events for a specified type of
        ///     component using a specified array of attributes as a filter.
        /// </summary>
        public static EventDescriptorCollection GetEvents(Type componentType, Attribute[] attributes)
        {
            if (componentType == null)
            {
                Debug.Fail("COMPAT:  Returning an empty collection, but you should not pass null here");
                return new EventDescriptorCollection(null, true);
            }

            EventDescriptorCollection events = GetDescriptor(componentType, "componentType").GetEvents(attributes);

            if (attributes != null && attributes.Length > 0)
            {
                ArrayList filteredEvents = FilterMembers(events, attributes);
                if (filteredEvents != null)
                {
                    events = new EventDescriptorCollection((EventDescriptor[])filteredEvents.ToArray(typeof(EventDescriptor)), true);
                }
            }

            return events;
        }

        /// <summary>
        ///     Gets a collection of events for a specified component.
        /// </summary>
        public static EventDescriptorCollection GetEvents(object component)
        {
            return GetEvents(component, null, false);
        }

        /// <summary>
        ///     Gets a collection of events for a specified component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static EventDescriptorCollection GetEvents(object component, bool noCustomTypeDesc)
        {
            return GetEvents(component, null, noCustomTypeDesc);
        }

        /// <summary>
        ///     Gets a collection of events for a specified component 
        ///     using a specified array of attributes as a filter.
        /// </summary>
        public static EventDescriptorCollection GetEvents(object component, Attribute[] attributes)
        {
            return GetEvents(component, attributes, false);
        }

        /// <summary>
        ///     Gets a collection of events for a specified component 
        ///     using a specified array of attributes as a filter.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static EventDescriptorCollection GetEvents(object component, Attribute[] attributes, bool noCustomTypeDesc)
        {
            if (component == null)
            {
                Debug.Fail("COMPAT:  Returning an empty collection, but you should not pass null here");
                return new EventDescriptorCollection(null, true);
            }

            // We create a sort of pipeline for mucking with metadata.  The pipeline
            // goes through the following process:
            //
            // 1.  Merge metadata from extenders.
            // 2.  Allow services to filter the metadata
            // 3.  If an attribute filter was specified, apply that.
            // 
            // The goal here is speed.  We get speed by not copying or
            // allocating memory.  We do this by allowing each phase of the
            // pipeline to cache its data in the object cache.  If
            // a phase makes a change to the results, this change must cause
            // successive phases to recompute their results as well.  "Results" is
            // always a collection, and the various stages of the pipeline may
            // replace or modify this collection (depending on if it's a
            // read-only IList or not).  It is possible for the orignal
            // descriptor or attribute collection to pass through the entire
            // pipeline without modification.
            // 
            ICustomTypeDescriptor typeDesc = GetDescriptor(component, noCustomTypeDesc);
            ICollection results;

            // If we are handed a custom type descriptor we have several choices of action
            // we can take.  If noCustomTypeDesc is true, it means that the custom type
            // descriptor is trying to find a baseline set of events.  In this case
            // we should merge in extended events, but we do not let designers filter
            // because we're not done with the event set yet.  If noCustomTypeDesc
            // is false, we don't do extender events because the custom type descriptor
            // has already added them.  In this case, we are doing a final pass so we
            // want to apply filtering.  Finally, if the incoming object is not a custom
            // type descriptor, we do extenders and the filter.
            //
            if (component is ICustomTypeDescriptor)
            {
                results = typeDesc.GetEvents(attributes);
                if (noCustomTypeDesc)
                {
                    ICustomTypeDescriptor extDesc = GetExtendedDescriptor(component);
                    if (extDesc != null)
                    {
                        ICollection extResults = extDesc.GetEvents(attributes);
                        results = PipelineMerge(PIPELINE_EVENTS, results, extResults, component, null);
                    }
                }
                else
                {
                    results = PipelineFilter(PIPELINE_EVENTS, results, component, null);
                    results = PipelineAttributeFilter(PIPELINE_EVENTS, results, attributes, component, null);
                }
            }
            else
            {
                IDictionary cache = GetCache(component);
                results = typeDesc.GetEvents(attributes);
                results = PipelineInitialize(PIPELINE_EVENTS, results, cache);
                ICustomTypeDescriptor extDesc = GetExtendedDescriptor(component);
                if (extDesc != null)
                {
                    ICollection extResults = extDesc.GetEvents(attributes);
                    results = PipelineMerge(PIPELINE_EVENTS, results, extResults, component, cache);
                }

                results = PipelineFilter(PIPELINE_EVENTS, results, component, cache);
                results = PipelineAttributeFilter(PIPELINE_EVENTS, results, attributes, component, cache);
            }

            EventDescriptorCollection evts = results as EventDescriptorCollection;
            if (evts == null)
            {
                EventDescriptor[] eventArray = new EventDescriptor[results.Count];
                results.CopyTo(eventArray, 0);
                evts = new EventDescriptorCollection(eventArray, true);
            }

            return evts;
        }

        /// <summary>
        ///     This method is invoked during filtering when a name
        ///     collision is encountered between two properties or events.  This returns
        ///     a suffix that can be appended to the name to make
        ///     it unique.  This will first attempt ot use the name of the
        ///     extender.  Failing that it will fall back to a static
        ///     index that is continually incremented.
        /// </summary>
        private static string GetExtenderCollisionSuffix(MemberDescriptor member)
        {
            string suffix = null;

            ExtenderProvidedPropertyAttribute exAttr = member.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;
            IExtenderProvider prov = exAttr?.Provider;

            if (prov != null)
            {
                string name = null;
                IComponent component = prov as IComponent;

                if (component != null && component.Site != null)
                {
                    name = component.Site.Name;
                }

                if (name == null || name.Length == 0)
                {
                    int ci = System.Threading.Interlocked.Increment(ref s_collisionIndex) - 1;
                    name = ci.ToString(CultureInfo.InvariantCulture);
                }

                suffix = string.Format(CultureInfo.InvariantCulture, "_{0}", name);
            }

            return suffix;
        }

        /// <summary>
        ///     The name of the specified component, or null if the component has no name.
        ///     In many cases this will return the same value as GetComponentName. If the
        ///     component resides in a nested container or has other nested semantics, it may
        ///     return a different fully qualified name.
        /// </summary>
        public static string GetFullComponentName(object component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            return GetProvider(component).GetFullComponentName(component);
        }

        private static Type GetNodeForBaseType(Type searchType)
        {
            if (searchType.IsInterface)
            {
                return InterfaceType;
            }
            else if (searchType == InterfaceType)
            {
                return null;
            }
            return searchType.BaseType;
        }

        /// <summary>
        ///     Gets a collection of properties for a specified type of component.
        /// </summary>
        public static PropertyDescriptorCollection GetProperties(Type componentType)
        {
            if (componentType == null)
            {
                Debug.Fail("COMPAT:  Returning an empty collection, but you should not pass null here");
                return new PropertyDescriptorCollection(null, true);
            }

            return GetDescriptor(componentType, "componentType").GetProperties();
        }

        /// <summary>
        ///    Gets a collection of properties for a specified type of 
        ///    component using a specified array of attributes as a filter.
        /// </summary>
        public static PropertyDescriptorCollection GetProperties(Type componentType, Attribute[] attributes)
        {
            if (componentType == null)
            {
                Debug.Fail("COMPAT:  Returning an empty collection, but you should not pass null here");
                return new PropertyDescriptorCollection(null, true);
            }

            PropertyDescriptorCollection properties = GetDescriptor(componentType, "componentType").GetProperties(attributes);

            if (attributes != null && attributes.Length > 0)
            {
                ArrayList filteredProperties = FilterMembers(properties, attributes);
                if (filteredProperties != null)
                {
                    properties = new PropertyDescriptorCollection((PropertyDescriptor[])filteredProperties.ToArray(typeof(PropertyDescriptor)), true);
                }
            }

            return properties;
        }

        /// <summary>
        ///     Gets a collection of properties for a specified component.
        /// </summary>
        public static PropertyDescriptorCollection GetProperties(object component)
        {
            return GetProperties(component, false);
        }

        /// <summary>
        ///     Gets a collection of properties for a specified component.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static PropertyDescriptorCollection GetProperties(object component, bool noCustomTypeDesc)
        {
            return GetPropertiesImpl(component, null, noCustomTypeDesc, true);
        }

        /// <summary>
        ///    Gets a collection of properties for a specified 
        ///    component using a specified array of attributes
        ///    as a filter.
        /// </summary>
        public static PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            return GetProperties(component, attributes, false);
        }

        /// <summary>
        ///    <para>Gets a collection of properties for a specified 
        ///       component using a specified array of attributes
        ///       as a filter.</para>
        /// </summary>
        public static PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes, bool noCustomTypeDesc)
        {
            return GetPropertiesImpl(component, attributes, noCustomTypeDesc, false);
        }

        /// <summary>
        ///    Gets a collection of properties for a specified component. Uses the attribute filter 
        ///    only if noAttributes is false. This is to preserve backward compat for the case when
        ///    no attribute filter was passed in (as against passing in null).
        /// </summary>
        private static PropertyDescriptorCollection GetPropertiesImpl(object component, Attribute[] attributes, bool noCustomTypeDesc, bool noAttributes)
        {
            if (component == null)
            {
                Debug.Fail("COMPAT:  Returning an empty collection, but you should not pass null here");
                return new PropertyDescriptorCollection(null, true);
            }

            // We create a sort of pipeline for mucking with metadata.  The pipeline
            // goes through the following process:
            //
            // 1.  Merge metadata from extenders.
            // 2.  Allow services to filter the metadata
            // 3.  If an attribute filter was specified, apply that.
            // 
            // The goal here is speed.  We get speed by not copying or
            // allocating memory.  We do this by allowing each phase of the
            // pipeline to cache its data in the object cache.  If
            // a phase makes a change to the results, this change must cause
            // successive phases to recompute their results as well.  "Results" is
            // always a collection, and the various stages of the pipeline may
            // replace or modify this collection (depending on if it's a
            // read-only IList or not).  It is possible for the orignal
            // descriptor or attribute collection to pass through the entire
            // pipeline without modification.
            // 
            ICustomTypeDescriptor typeDesc = GetDescriptor(component, noCustomTypeDesc);
            ICollection results;

            // If we are handed a custom type descriptor we have several choices of action
            // we can take.  If noCustomTypeDesc is true, it means that the custom type
            // descriptor is trying to find a baseline set of properties.  In this case
            // we should merge in extended properties, but we do not let designers filter
            // because we're not done with the property set yet.  If noCustomTypeDesc
            // is false, we don't do extender properties because the custom type descriptor
            // has already added them.  In this case, we are doing a final pass so we
            // want to apply filtering.  Finally, if the incoming object is not a custom
            // type descriptor, we do extenders and the filter.
            //
            if (component is ICustomTypeDescriptor)
            {
                results = noAttributes ? typeDesc.GetProperties() : typeDesc.GetProperties(attributes);
                if (noCustomTypeDesc)
                {
                    ICustomTypeDescriptor extDesc = GetExtendedDescriptor(component);
                    if (extDesc != null)
                    {
                        ICollection extResults = noAttributes ? extDesc.GetProperties() : extDesc.GetProperties(attributes);
                        results = PipelineMerge(PIPELINE_PROPERTIES, results, extResults, component, null);
                    }
                }
                else
                {
                    results = PipelineFilter(PIPELINE_PROPERTIES, results, component, null);
                    results = PipelineAttributeFilter(PIPELINE_PROPERTIES, results, attributes, component, null);
                }
            }
            else
            {
                IDictionary cache = GetCache(component);
                results = noAttributes ? typeDesc.GetProperties() : typeDesc.GetProperties(attributes);
                results = PipelineInitialize(PIPELINE_PROPERTIES, results, cache);
                ICustomTypeDescriptor extDesc = GetExtendedDescriptor(component);
                if (extDesc != null)
                {
                    ICollection extResults = noAttributes ? extDesc.GetProperties() : extDesc.GetProperties(attributes);
                    results = PipelineMerge(PIPELINE_PROPERTIES, results, extResults, component, cache);
                }

                results = PipelineFilter(PIPELINE_PROPERTIES, results, component, cache);
                results = PipelineAttributeFilter(PIPELINE_PROPERTIES, results, attributes, component, cache);
            }

            PropertyDescriptorCollection props = results as PropertyDescriptorCollection;
            if (props == null)
            {
                PropertyDescriptor[] propArray = new PropertyDescriptor[results.Count];
                results.CopyTo(propArray, 0);
                props = new PropertyDescriptorCollection(propArray, true);
            }

            return props;
        }

        /// <summary>
        ///     The GetProvider method returns a type description provider for 
        ///     the given object or type.  This will always return a type description 
        ///     provider.  Even the default TypeDescriptor implementation is built on 
        ///     a TypeDescriptionProvider, and this will be returned unless there is 
        ///     another provider that someone else has added.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static TypeDescriptionProvider GetProvider(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return NodeFor(type, true);
        }

        /// <summary>
        ///     The GetProvider method returns a type description provider for 
        ///     the given object or type.  This will always return a type description 
        ///     provider.  Even the default TypeDescriptor implementation is built on 
        ///     a TypeDescriptionProvider, and this will be returned unless there is 
        ///     another provider that someone else has added.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static TypeDescriptionProvider GetProvider(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return NodeFor(instance, true);
        }

        /// <summary>
        ///     This method returns a type description provider, but instead of creating
        ///     a delegating provider for the type, this will walk all base types until
        ///     it locates a provider.  The provider returned cannot be cached.  This
        ///     method is used by the DelegatingTypeDescriptionProvider to efficiently
        ///     locate the provider to delegate to.
        /// </summary>
        internal static TypeDescriptionProvider GetProviderRecursive(Type type)
        {
            return NodeFor(type, false);
        }

        /// <summary>
        ///     Returns an Type instance that can be used to perform reflection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Type GetReflectionType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return NodeFor(type).GetReflectionType(type);
        }


        /// <summary>
        ///     Returns an Type instance that can be used to perform reflection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Type GetReflectionType(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return NodeFor(instance).GetReflectionType(instance);
        }

        /// <summary>
        ///     Retrieves the head type description node for a type.
        ///     A head node pointing to a reflection based type description
        ///     provider will be created on demand.  This does not create
        ///     a delegator, in which case the node returned may be
        ///     a base type node.
        /// </summary>
        private static TypeDescriptionNode NodeFor(Type type)
        {
            return NodeFor(type, false);
        }

        /// <summary>
        ///     Retrieves the head type description node for a type.
        ///     A head node pointing to a reflection based type description
        ///     provider will be created on demand.
        ///
        ///     If createDelegator is true, this method will create a delegation
        ///     node for a type if the type has no node of its own.  Delegation
        ///     nodes should be created if you are going to hand this node
        ///     out to a user.  Without a delegation node, user code could
        ///     skip providers that are added after their call.  Delegation
        ///     nodes solve that problem.
        ///
        ///     If createDelegator is false, this method will recurse up the
        ///     base type chain looking for nodes.  
        /// </summary>
        private static TypeDescriptionNode NodeFor(Type type, bool createDelegator)
        {
            Debug.Assert(type != null, "Caller should validate");
            CheckDefaultProvider(type);

            // First, check our provider type table to see if we have a matching
            // provider for this type.  The provider type table is a cache that
            // matches types to providers.  When a new provider is added or
            // an existing one removed, the provider type table is torn
            // down and automatically rebuilt on demand.
            //
            TypeDescriptionNode node = null;
            Type searchType = type;

            while (node == null)
            {
                node = (TypeDescriptionNode)s_providerTypeTable[searchType] ??
                       (TypeDescriptionNode)s_providerTable[searchType];

                if (node == null)
                {
                    Type baseType = GetNodeForBaseType(searchType);

                    if (searchType == typeof(object) || baseType == null)
                    {
                        lock (s_providerTable)
                        {
                            node = (TypeDescriptionNode)s_providerTable[searchType];

                            if (node == null)
                            {
                                // The reflect type description provider is a default provider that
                                // can provide type information for all objects.
                                node = new TypeDescriptionNode(new ReflectTypeDescriptionProvider());
                                s_providerTable[searchType] = node;
                            }
                        }
                    }
                    else if (createDelegator)
                    {
                        node = new TypeDescriptionNode(new DelegatingTypeDescriptionProvider(baseType));
                        lock (s_providerTable)
                        {
                            s_providerTypeTable[searchType] = node;
                        }
                    }
                    else
                    {
                        // Continue our search
                        searchType = baseType;
                    }
                }
            }

            return node;
        }

        /// <summary>
        ///     Retrieves the head type description node for an instance.
        ///     Instance-based node lists are rare.  If a node list is not
        ///     available for a given instance, this will return the head node
        ///     for the instance's type.
        /// </summary>
        private static TypeDescriptionNode NodeFor(object instance)
        {
            return NodeFor(instance, false);
        }

        /// <summary>
        ///     Retrieves the head type description node for an instance.
        ///     Instance-based node lists are rare.  If a node list is not
        ///     available for a given instance, this will return the head node
        ///     for the instance's type.  This variation offers a bool called
        ///     createDelegator.  If true and there is no node list for this
        ///     instance, NodeFor will create a temporary "delegator node" that,
        ///     when queried, will delegate to the type stored in the instance.
        ///     This is done on demand, which means if someone else added a
        ///     type description provider for the instance's type the delegator
        ///     would pick up the new type.  If a query is being made that does
        ///     not involve publicly exposing the type description provider for
        ///     the instance, the query should pass in false (the default) for
        ///     createDelegator because no object will be created.
        /// </summary>
        private static TypeDescriptionNode NodeFor(object instance, bool createDelegator)
        {
            // For object instances, the provider cache key is not the object (that
            // would keep it in memory).  Instead, it is a subclass of WeakReference
            // that overrides GetHashCode and Equals to make it appear to be the
            // object it is wrapping.  A GC'd object causes WeakReference to return
            // false for all .Equals, but it always returns a valid hash code.

            Debug.Assert(instance != null, "Caller should validate");

            TypeDescriptionNode node = (TypeDescriptionNode)s_providerTable[instance];
            if (node == null)
            {
                Type type = instance.GetType();

                if (createDelegator)
                {
                    node = new TypeDescriptionNode(new DelegatingTypeDescriptionProvider(type));
                }
                else
                {
                    node = NodeFor(type);
                }
            }

            return node;
        }

        /// <summary>
        ///     Simple linked list code to remove an element
        ///     from the list.  Returns the new head to the
        ///     list.  If the head points to an instance of
        ///     DelegatingTypeDescriptionProvider, we clear the
        ///     node because all it is doing is delegating elsewhere.
        ///
        ///     Note that this behaves a little differently from normal
        ///     linked list code.  In a normal linked list, you remove 
        ///     then target node and fixup the links.  In this linked
        ///     list, we remove the node AFTER the target node, fixup
        ///     the links, and fixup the underlying providers that each
        ///     node references.  The reason for this is that most
        ///     providers keep a reference to the previous provider,
        ///     which is exposed as one of these nodes.  Therefore,
        ///     to remove a provider the node following is most likely
        ///     referenced by that provider
        /// </summary>
        private static void NodeRemove(object key, TypeDescriptionProvider provider)
        {
            lock (s_providerTable)
            {
                TypeDescriptionNode head = (TypeDescriptionNode)s_providerTable[key];
                TypeDescriptionNode target = head;
                TypeDescriptionNode prev = null;

                while (target != null && target.Provider != provider)
                {
                    prev = target;
                    target = target.Next;
                }

                if (target != null)
                {
                    // We have our target node.  There are three cases
                    // to consider:  the target is in the middle, the head,
                    // or the end.

                    if (target.Next != null)
                    {
                        // If there is a node after the target node,
                        // steal the node's provider and store it
                        // at the target location.  This removes
                        // the provider at the target location without
                        // the need to modify providers which may be
                        // pointing to "target".  
                        target.Provider = target.Next.Provider;

                        // Now remove target.Next from the list
                        target.Next = target.Next.Next;

                        // If the new provider we got is a delegating
                        // provider, we can remove this node from 
                        // the list.  The delegating provider should
                        // always be at the end of the node list.
                        if (target == head && target.Provider is DelegatingTypeDescriptionProvider)
                        {
                            Debug.Assert(target.Next == null, "Delegating provider should always be the last provider in the chain.");
                            s_providerTable.Remove(key);
                        }
                    }
                    else if (target != head)
                    {
                        // If target is the last node, we can't
                        // assign a new provider over to it.  What
                        // we can do, however, is assign a delegating
                        // provider into the target node.  This routes
                        // requests from the previous provider into
                        // the next base type provider list.

                        // We don't do this if the target is the head.
                        // In that case, we can remove the node
                        // altogether since no one is pointing to it.

                        Type keyType = key as Type ?? key.GetType();

                        target.Provider = new DelegatingTypeDescriptionProvider(keyType.BaseType);
                    }
                    else
                    {
                        s_providerTable.Remove(key);
                    }

                    // Finally, clear our cache of provider types; it might be invalid 
                    // now.
                    s_providerTypeTable.Clear();
                }
            }
        }

        /// <summary>
        ///     This is the last stage in our filtering pipeline.  Here, we apply any
        ///     user-defined filter.  
        /// </summary>
        private static ICollection PipelineAttributeFilter(int pipelineType, ICollection members, Attribute[] filter, object instance, IDictionary cache)
        {
            Debug.Assert(pipelineType != PIPELINE_ATTRIBUTES, "PipelineAttributeFilter is not supported for attributes");

            IList list = members as ArrayList;

            if (filter == null || filter.Length == 0)
            {
                return members;
            }

            // Now, check our cache.  The cache state is only valid
            // if the data coming into us is read-only.  If it is read-write,
            // that means something higher in the pipeline has already changed
            // it so we must recompute anyway.
            //
            if (cache != null && (list == null || list.IsReadOnly))
            {
                AttributeFilterCacheItem filterCache = cache[s_pipelineAttributeFilterKeys[pipelineType]] as AttributeFilterCacheItem;
                if (filterCache != null && filterCache.IsValid(filter))
                {
                    return filterCache.FilteredMembers;
                }
            }

            // Our cache did not contain the correct state, so generate it.
            //
            if (list == null || list.IsReadOnly)
            {
                list = new ArrayList(members);
            }

            ArrayList filterResult = FilterMembers(list, filter);
            if (filterResult != null) list = filterResult;

            // And, if we have a cache, store the updated state into it for future reference.
            //
            if (cache != null)
            {
                ICollection cacheValue;

                switch (pipelineType)
                {
                    case PIPELINE_PROPERTIES:
                        PropertyDescriptor[] propArray = new PropertyDescriptor[list.Count];
                        list.CopyTo(propArray, 0);
                        cacheValue = new PropertyDescriptorCollection(propArray, true);
                        break;

                    case PIPELINE_EVENTS:
                        EventDescriptor[] eventArray = new EventDescriptor[list.Count];
                        list.CopyTo(eventArray, 0);
                        cacheValue = new EventDescriptorCollection(eventArray, true);
                        break;

                    default:
                        Debug.Fail("unknown pipeline type");
                        cacheValue = null;
                        break;
                }

                AttributeFilterCacheItem filterCache = new AttributeFilterCacheItem(filter, cacheValue);
                cache[s_pipelineAttributeFilterKeys[pipelineType]] = filterCache;
            }

            return list;
        }

        /// <summary>
        ///     Metdata filtering is the third stage of our pipeline.  
        ///     In this stage we check to see if the given object is a
        ///     sited component that provides the ITypeDescriptorFilterService
        ///     object.  If it does, we allow the TDS to filter the metadata.
        ///     This will use the cache, if available, to store filtered
        ///     metdata.
        /// </summary>
        private static ICollection PipelineFilter(int pipelineType, ICollection members, object instance, IDictionary cache)
        {
            IComponent component = instance as IComponent;
            ITypeDescriptorFilterService componentFilter = null;

            ISite site = component?.Site;
            if (site != null)
            {
                componentFilter = site.GetService(typeof(ITypeDescriptorFilterService)) as ITypeDescriptorFilterService;
            }

            // If we have no filter, there is nothing for us to do.
            //
            IList list = members as ArrayList;

            if (componentFilter == null)
            {
                Debug.Assert(cache == null || list == null || !cache.Contains(s_pipelineFilterKeys[pipelineType]), "Earlier pipeline stage should have removed our cache");
                return members;
            }

            // Now, check our cache.  The cache state is only valid
            // if the data coming into us is read-only.  If it is read-write,
            // that means something higher in the pipeline has already changed
            // it so we must recompute anyway.
            //
            if (cache != null && (list == null || list.IsReadOnly))
            {
                FilterCacheItem cacheItem = cache[s_pipelineFilterKeys[pipelineType]] as FilterCacheItem;
                if (cacheItem != null && cacheItem.IsValid(componentFilter))
                {
                    return cacheItem.FilteredMembers;
                }
            }

            // Cache either is dirty or doesn't exist.  Re-filter the members.
            // We need to build an IDictionary of key->value pairs and invoke
            // Filter* on the filter service.
            //
            OrderedDictionary filterTable = new OrderedDictionary(members.Count);
            bool cacheResults;

            switch (pipelineType)
            {
                case PIPELINE_ATTRIBUTES:
                    foreach (Attribute attr in members)
                    {
                        filterTable[attr.TypeId] = attr;
                    }
                    cacheResults = componentFilter.FilterAttributes(component, filterTable);
                    break;

                case PIPELINE_PROPERTIES:
                case PIPELINE_EVENTS:
                    foreach (MemberDescriptor desc in members)
                    {
                        string descName = desc.Name;
                        // We must handle the case of duplicate property names
                        // because extender providers can provide any arbitrary
                        // name.  Our rule for this is simple:  If we find a
                        // duplicate name, resolve it back to the extender
                        // provider that offered it and append "_" + the
                        // provider name.  If the provider has no name,
                        // then append the object hash code.
                        //
                        if (filterTable.Contains(descName))
                        {
                            // First, handle the new property.  Because
                            // of the order in which we added extended
                            // properties earlier in the pipeline, we can be 
                            // sure that the new property is an extender.  We
                            // cannot be sure that the existing property
                            // in the table is an extender, so we will 
                            // have to check.
                            //
                            string suffix = GetExtenderCollisionSuffix(desc);
                            Debug.Assert(suffix != null, "Name collision with non-extender property.");
                            if (suffix != null)
                            {
                                filterTable[descName + suffix] = desc;
                            }

                            // Now, handle the original property.
                            //
                            MemberDescriptor origDesc = (MemberDescriptor)filterTable[descName];
                            suffix = GetExtenderCollisionSuffix(origDesc);
                            if (suffix != null)
                            {
                                filterTable.Remove(descName);
                                filterTable[origDesc.Name + suffix] = origDesc;
                            }
                        }
                        else
                        {
                            filterTable[descName] = desc;
                        }
                    }
                    if (pipelineType == PIPELINE_PROPERTIES)
                    {
                        cacheResults = componentFilter.FilterProperties(component, filterTable);
                    }
                    else
                    {
                        cacheResults = componentFilter.FilterEvents(component, filterTable);
                    }
                    break;

                default:
                    Debug.Fail("unknown pipeline type");
                    cacheResults = false;
                    break;
            }

            // See if we can re-use the IList were were passed.  If we can,
            // it is more efficient to re-use its slots than to generate new ones.
            //
            if (list == null || list.IsReadOnly)
            {
                list = new ArrayList(filterTable.Values);
            }
            else
            {
                list.Clear();
                foreach (object obj in filterTable.Values)
                {
                    list.Add(obj);
                }
            }

            // Component filter has requested that we cache these
            // new changes.  We store them as a correctly typed collection
            // so on successive invocations we can simply return.  Note that
            // we always return the IList so that successive stages in the
            // pipeline can modify it.
            //
            if (cacheResults && cache != null)
            {
                ICollection cacheValue;

                switch (pipelineType)
                {
                    case PIPELINE_ATTRIBUTES:
                        Attribute[] attrArray = new Attribute[list.Count];
                        try
                        {
                            list.CopyTo(attrArray, 0);
                        }
                        catch (InvalidCastException)
                        {
                            throw new ArgumentException(SR.Format(SR.TypeDescriptorExpectedElementType, typeof(Attribute).FullName));
                        }
                        cacheValue = new AttributeCollection(attrArray);
                        break;

                    case PIPELINE_PROPERTIES:
                        PropertyDescriptor[] propArray = new PropertyDescriptor[list.Count];
                        try
                        {
                            list.CopyTo(propArray, 0);
                        }
                        catch (InvalidCastException)
                        {
                            throw new ArgumentException(SR.Format(SR.TypeDescriptorExpectedElementType, typeof(PropertyDescriptor).FullName));
                        }
                        cacheValue = new PropertyDescriptorCollection(propArray, true);
                        break;

                    case PIPELINE_EVENTS:
                        EventDescriptor[] eventArray = new EventDescriptor[list.Count];
                        try
                        {
                            list.CopyTo(eventArray, 0);
                        }
                        catch (InvalidCastException)
                        {
                            throw new ArgumentException(SR.Format(SR.TypeDescriptorExpectedElementType, typeof(EventDescriptor).FullName));
                        }
                        cacheValue = new EventDescriptorCollection(eventArray, true);
                        break;

                    default:
                        Debug.Fail("unknown pipeline type");
                        cacheValue = null;
                        break;
                }

                FilterCacheItem cacheItem = new FilterCacheItem(componentFilter, cacheValue);
                cache[s_pipelineFilterKeys[pipelineType]] = cacheItem;
                cache.Remove(s_pipelineAttributeFilterKeys[pipelineType]);
            }

            return list;
        }

        /// <summary>
        /// This is the first stage in the pipeline.  This checks the incoming member collection and if it
        /// differs from what we have seen in the past, it invalidates all successive pipelines.
        /// </summary>
        private static ICollection PipelineInitialize(int pipelineType, ICollection members, IDictionary cache)
        {
            if (cache != null)
            {
                bool cacheValid = true;

                ICollection cachedMembers = cache[s_pipelineInitializeKeys[pipelineType]] as ICollection;
                if (cachedMembers != null && cachedMembers.Count == members.Count)
                {
                    IEnumerator cacheEnum = cachedMembers.GetEnumerator();
                    IEnumerator memberEnum = members.GetEnumerator();

                    while (cacheEnum.MoveNext() && memberEnum.MoveNext())
                    {
                        if (cacheEnum.Current != memberEnum.Current)
                        {
                            cacheValid = false;
                            break;
                        }
                    }
                }

                if (!cacheValid)
                {
                    // The cache wasn't valid.  Remove all subsequent cache layers
                    // and then save off new data.
                    cache.Remove(s_pipelineMergeKeys[pipelineType]);
                    cache.Remove(s_pipelineFilterKeys[pipelineType]);
                    cache.Remove(s_pipelineAttributeFilterKeys[pipelineType]);
                    cache[s_pipelineInitializeKeys[pipelineType]] = members;
                }
            }

            return members;
        }

        /// <summary>
        ///     Metadata merging is the second stage of our metadata pipeline.  This stage
        ///     merges extended metdata with primary metadata, and stores it in 
        ///     the cache if it is available.
        /// </summary>
        private static ICollection PipelineMerge(int pipelineType, ICollection primary, ICollection secondary, object instance, IDictionary cache)
        {
            // If there is no secondary collection, there is nothing to merge.
            //
            if (secondary == null || secondary.Count == 0)
            {
                return primary;
            }

            // Next, if we were given a cache, see if it has accurate data.
            //
            ICollection mergeCache = cache?[s_pipelineMergeKeys[pipelineType]] as ICollection;
            if (mergeCache != null && mergeCache.Count == (primary.Count + secondary.Count))
            {
                // Walk the merge cache.
                IEnumerator mergeEnum = mergeCache.GetEnumerator();
                IEnumerator primaryEnum = primary.GetEnumerator();
                bool match = true;

                while (primaryEnum.MoveNext() && mergeEnum.MoveNext())
                {
                    if (primaryEnum.Current != mergeEnum.Current)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    IEnumerator secondaryEnum = secondary.GetEnumerator();

                    while (secondaryEnum.MoveNext() && mergeEnum.MoveNext())
                    {
                        if (secondaryEnum.Current != mergeEnum.Current)
                        {
                            match = false;
                            break;
                        }
                    }
                }

                if (match)
                {
                    return mergeCache;
                }
            }

            // Our cache didn't match.  We need to merge metadata and return
            // the merged copy.  We create an array list here, rather than
            // an array, because we want successive sections of the 
            // pipeline to be able to modify it.
            //
            ArrayList list = new ArrayList(primary.Count + secondary.Count);
            foreach (object obj in primary)
            {
                list.Add(obj);
            }
            foreach (object obj in secondary)
            {
                list.Add(obj);
            }

            if (cache != null)
            {
                ICollection cacheValue;

                switch (pipelineType)
                {
                    case PIPELINE_ATTRIBUTES:
                        Attribute[] attrArray = new Attribute[list.Count];
                        list.CopyTo(attrArray, 0);
                        cacheValue = new AttributeCollection(attrArray);
                        break;

                    case PIPELINE_PROPERTIES:
                        PropertyDescriptor[] propArray = new PropertyDescriptor[list.Count];
                        list.CopyTo(propArray, 0);
                        cacheValue = new PropertyDescriptorCollection(propArray, true);
                        break;

                    case PIPELINE_EVENTS:
                        EventDescriptor[] eventArray = new EventDescriptor[list.Count];
                        list.CopyTo(eventArray, 0);
                        cacheValue = new EventDescriptorCollection(eventArray, true);
                        break;

                    default:
                        Debug.Fail("unknown pipeline type");
                        cacheValue = null;
                        break;
                }

                cache[s_pipelineMergeKeys[pipelineType]] = cacheValue;
                cache.Remove(s_pipelineFilterKeys[pipelineType]);
                cache.Remove(s_pipelineAttributeFilterKeys[pipelineType]);
            }

            return list;
        }

        private static void RaiseRefresh(object component)
        {
            // This volatility prevents the JIT from making certain optimizations 
            // that could cause this firing pattern to break. Although the likelihood 
            // the JIT makes those changes is mostly theoretical
            RefreshEventHandler handler = Volatile.Read(ref Refreshed);

            handler?.Invoke(new RefreshEventArgs(component));
        }

        private static void RaiseRefresh(Type type)
        {
            RefreshEventHandler handler = Volatile.Read(ref Refreshed);

            handler?.Invoke(new RefreshEventArgs(type));
        }

        /// <summary>
        ///    Clears the properties and events for the specified 
        ///    component from the cache.
        /// </summary>
        public static void Refresh(object component)
        {
            Refresh(component, true);
        }

        private static void Refresh(object component, bool refreshReflectionProvider)
        {
            if (component == null)
            {
                Debug.Fail("COMPAT:  Returning, but you should not pass null here");
                return;
            }

            // Build up a list of type description providers for
            // each type that is a derived type of the given
            // object.  We will invalidate the metadata at
            // each of these levels.
            bool found = false;

            if (refreshReflectionProvider)
            {
                Type type = component.GetType();

                lock (s_providerTable)
                {
                    // ReflectTypeDescritionProvider is only bound to object, but we
                    // need go to through the entire table to try to find custom
                    // providers.  If we find one, will clear our cache.
                    // Manual use of IDictionaryEnumerator instead of foreach to avoid
                    // DictionaryEntry box allocations.
                    IDictionaryEnumerator e = s_providerTable.GetEnumerator();
                    while (e.MoveNext())
                    {
                        DictionaryEntry de = e.Entry;
                        Type nodeType = de.Key as Type;
                        if (nodeType != null && type.IsAssignableFrom(nodeType) || nodeType == typeof(object))
                        {
                            TypeDescriptionNode node = (TypeDescriptionNode)de.Value;
                            while (node != null && !(node.Provider is ReflectTypeDescriptionProvider))
                            {
                                found = true;
                                node = node.Next;
                            }

                            if (node != null)
                            {
                                ReflectTypeDescriptionProvider provider = (ReflectTypeDescriptionProvider)node.Provider;
                                if (provider.IsPopulated(type))
                                {
                                    found = true;
                                    provider.Refresh(type);
                                }
                            }
                        }
                    }
                }
            }

            // We need to clear our filter even if no typedescriptionprovider had data.
            // This is because if you call Refresh(instance1) and Refresh(instance2)
            // and instance1 and instance2 are of the same type, you will end up not
            // actually deleting the dictionary cache on instance2 if you skip this
            // when you don't find a typedescriptionprovider.
            // However, we do not need to fire the event if we did not find any loaded
            // typedescriptionprovider AND the cache is empty (if someone repeatedly calls
            // Refresh on an instance).

            // Now, clear any cached data for the instance.
            //
            IDictionary cache = GetCache(component);
            if (found || cache != null)
            {
                if (cache != null)
                {
                    for (int idx = 0; idx < s_pipelineFilterKeys.Length; idx++)
                    {
                        cache.Remove(s_pipelineFilterKeys[idx]);
                        cache.Remove(s_pipelineMergeKeys[idx]);
                        cache.Remove(s_pipelineAttributeFilterKeys[idx]);
                    }
                }

                Interlocked.Increment(ref s_metadataVersion);

                // And raise the event.
                //
                RaiseRefresh(component);
            }
        }

        /// <summary>
        ///    Clears the properties and events for the specified type 
        ///    of component from the cache.
        /// </summary>
        public static void Refresh(Type type)
        {
            if (type == null)
            {
                Debug.Fail("COMPAT:  Returning, but you should not pass null here");
                return;
            }

            // Build up a list of type description providers for
            // each type that is a derived type of the given
            // type.  We will invalidate the metadata at
            // each of these levels.

            bool found = false;

            lock (s_providerTable)
            {
                // ReflectTypeDescritionProvider is only bound to object, but we
                // need go to through the entire table to try to find custom
                // providers.  If we find one, will clear our cache.
                // Manual use of IDictionaryEnumerator instead of foreach to avoid
                // DictionaryEntry box allocations.
                IDictionaryEnumerator e = s_providerTable.GetEnumerator();
                while (e.MoveNext())
                {
                    DictionaryEntry de = e.Entry;
                    Type nodeType = de.Key as Type;
                    if (nodeType != null && type.IsAssignableFrom(nodeType) || nodeType == typeof(object))
                    {
                        TypeDescriptionNode node = (TypeDescriptionNode)de.Value;
                        while (node != null && !(node.Provider is ReflectTypeDescriptionProvider))
                        {
                            found = true;
                            node = node.Next;
                        }

                        if (node != null)
                        {
                            ReflectTypeDescriptionProvider provider = (ReflectTypeDescriptionProvider)node.Provider;
                            if (provider.IsPopulated(type))
                            {
                                found = true;
                                provider.Refresh(type);
                            }
                        }
                    }
                }
            }

            // We only clear our filter and fire the refresh event if there was one or
            // more type description providers that were populated with metdata.
            // This prevents us from doing a lot of extra work and raising 
            // a ton more events than we need to.
            //
            if (found)
            {
                Interlocked.Increment(ref s_metadataVersion);

                // And raise the event.
                //
                RaiseRefresh(type);
            }
        }

        /// <summary>
        ///    Clears the properties and events for the specified 
        ///    module from the cache.
        /// </summary>
        public static void Refresh(Module module)
        {
            if (module == null)
            {
                Debug.Fail("COMPAT:  Returning, but you should not pass null here");
                return;
            }

            // Build up a list of type description providers for
            // each type that is a derived type of the given
            // object.  We will invalidate the metadata at
            // each of these levels.
            Hashtable refreshedTypes = null;

            lock (s_providerTable)
            {
                // Manual use of IDictionaryEnumerator instead of foreach to avoid DictionaryEntry box allocations.
                IDictionaryEnumerator e = s_providerTable.GetEnumerator();
                while (e.MoveNext())
                {
                    DictionaryEntry de = e.Entry;
                    Type nodeType = de.Key as Type;
                    if (nodeType != null && nodeType.Module.Equals(module) || nodeType == typeof(object))
                    {
                        TypeDescriptionNode node = (TypeDescriptionNode)de.Value;
                        while (node != null && !(node.Provider is ReflectTypeDescriptionProvider))
                        {
                            if (refreshedTypes == null)
                            {
                                refreshedTypes = new Hashtable();
                            }
                            refreshedTypes[nodeType] = nodeType;
                            node = node.Next;
                        }

                        if (node != null)
                        {
                            ReflectTypeDescriptionProvider provider = (ReflectTypeDescriptionProvider)node.Provider;
                            Type[] populatedTypes = provider.GetPopulatedTypes(module);

                            foreach (Type populatedType in populatedTypes)
                            {
                                provider.Refresh(populatedType);
                                if (refreshedTypes == null)
                                {
                                    refreshedTypes = new Hashtable();
                                }
                                refreshedTypes[populatedType] = populatedType;
                            }
                        }
                    }
                }
            }

            // And raise the event if types were refresh and handlers are attached.
            //
            if (refreshedTypes != null && Refreshed != null)
            {
                foreach (Type t in refreshedTypes.Keys)
                {
                    RaiseRefresh(t);
                }
            }
        }

        /// <summary>
        ///    Clears the properties and events for the specified 
        ///    assembly from the cache.
        /// </summary>
        public static void Refresh(Assembly assembly)
        {
            if (assembly == null)
            {
                Debug.Fail("COMPAT:  Returning, but you should not pass null here");
                return;
            }

            foreach (Module mod in assembly.GetModules())
            {
                Refresh(mod);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Type ComObjectType => typeof(TypeDescriptor.TypeDescriptorComObject);

        public static IDesigner CreateDesigner(IComponent component, Type designerBaseType)
        {
            Type type = null;
            IDesigner result = null;
            AttributeCollection attributes = TypeDescriptor.GetAttributes(component);
            for (int i = 0; i < attributes.Count; i++)
            {
                DesignerAttribute designerAttribute = attributes[i] as DesignerAttribute;
                if (designerAttribute != null)
                {
                    Type type2 = Type.GetType(designerAttribute.DesignerBaseTypeName);
                    if (type2 != null && type2 == designerBaseType)
                    {
                        ISite site = component.Site;
                        bool flag = false;
                        ITypeResolutionService typeResolutionService = (ITypeResolutionService) site?.GetService(typeof(ITypeResolutionService));
                        if (typeResolutionService != null)
                        {
                            flag = true;
                            type = typeResolutionService.GetType(designerAttribute.DesignerTypeName);
                        }
                        if (!flag)
                        {
                            type = Type.GetType(designerAttribute.DesignerTypeName);
                        }
                        if (type != null)
                        {
                            break;
                        }
                    }
                }
            }
            if (type != null)
            {
                result = (IDesigner)SecurityUtils.SecureCreateInstance(type);
            }
            return result;
        }

        [Obsolete("This property has been deprecated.  Use a type description provider to supply type information for COM types instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public static IComNativeDescriptorHandler ComNativeDescriptorHandler
        {
            get
            {
                TypeDescriptor.TypeDescriptionNode typeDescriptionNode = TypeDescriptor.NodeFor(TypeDescriptor.ComObjectType);
                TypeDescriptor.ComNativeDescriptionProvider comNativeDescriptionProvider;
                do
                {
                    comNativeDescriptionProvider = (typeDescriptionNode.Provider as TypeDescriptor.ComNativeDescriptionProvider);
                    typeDescriptionNode = typeDescriptionNode.Next;
                }
                while (typeDescriptionNode != null && comNativeDescriptionProvider == null);
                return comNativeDescriptionProvider?.Handler;
            }
            set
            {
                TypeDescriptor.TypeDescriptionNode typeDescriptionNode = TypeDescriptor.NodeFor(TypeDescriptor.ComObjectType);
                while (typeDescriptionNode != null && !(typeDescriptionNode.Provider is TypeDescriptor.ComNativeDescriptionProvider))
                {
                    typeDescriptionNode = typeDescriptionNode.Next;
                }
                if (typeDescriptionNode == null)
                {
                    TypeDescriptor.AddProvider(new TypeDescriptor.ComNativeDescriptionProvider(value), TypeDescriptor.ComObjectType);
                    return;
                }
                ((TypeDescriptor.ComNativeDescriptionProvider)typeDescriptionNode.Provider).Handler = value;
            }
        }


        /// <summary>
        ///     The RemoveAssociation method removes an association with an object.  
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RemoveAssociation(object primary, object secondary)
        {
            if (primary == null)
            {
                throw new ArgumentNullException(nameof(primary));
            }

            if (secondary == null)
            {
                throw new ArgumentNullException(nameof(secondary));
            }

            Hashtable assocTable = AssociationTable;
            IList associations = (IList) assocTable?[primary];
            if (associations != null)
            {
                lock (associations)
                {
                    for (int idx = associations.Count - 1; idx >= 0; idx--)
                    {
                        // Look for an associated object that has a type that
                        // matches the given type.
                        //
                        WeakReference weakRef = (WeakReference)associations[idx];
                        object secondaryItem = weakRef.Target;
                        if (secondaryItem == null || secondaryItem == secondary)
                        {
                            associations.RemoveAt(idx);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     The RemoveAssociations method removes all associations for a primary object.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RemoveAssociations(object primary)
        {
            if (primary == null)
            {
                throw new ArgumentNullException(nameof(primary));
            }

            Hashtable assocTable = AssociationTable;
            assocTable?.Remove(primary);
        }

        /// <summary>
        ///     The RemoveProvider method removes a previously added type 
        ///     description provider.  Removing a provider causes a Refresh 
        ///     event to be raised for the object or type the provider is 
        ///     associated with.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RemoveProvider(TypeDescriptionProvider provider, Type type)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Walk the nodes until we find the right one, and then remove it.
            NodeRemove(type, provider);
            RaiseRefresh(type);
        }

        /// <summary>
        ///     The RemoveProvider method removes a previously added type 
        ///     description provider.  Removing a provider causes a Refresh 
        ///     event to be raised for the object or type the provider is 
        ///     associated with.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RemoveProvider(TypeDescriptionProvider provider, object instance)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            // Walk the nodes until we find the right one, and then remove it.
            NodeRemove(instance, provider);
            RaiseRefresh(instance);
        }


        /// <summary>
        ///     The RemoveProvider method removes a previously added type 
        ///     description provider.  Removing a provider causes a Refresh 
        ///     event to be raised for the object or type the provider is 
        ///     associated with.
        ///     
        ///     This method can be called from partially trusted code. If 
        ///     <see cref="TypeDescriptorPermissionFlags.RestrictedRegistrationAccess"/>
        ///     is defined, the caller can unregister a provider for the specified type
        ///     if it's also partially trusted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RemoveProviderTransparent(TypeDescriptionProvider provider, Type type)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            RemoveProvider(provider, type);
        }

        /// <summary>
        ///     The RemoveProvider method removes a previously added type 
        ///     description provider.  Removing a provider causes a Refresh 
        ///     event to be raised for the object or type the provider is 
        ///     associated with.
        ///     
        ///     This method can be called from partially trusted code. If 
        ///     <see cref="TypeDescriptorPermissionFlags.RestrictedRegistrationAccess"/>
        ///     is defined, the caller can register a provider for the specified instance 
        ///     if its type is also partially trusted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RemoveProviderTransparent(TypeDescriptionProvider provider, object instance)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            Type type = instance.GetType();

            RemoveProvider(provider, instance);
        }

        /// <summary> 
        ///     This function takes a member descriptor and an attribute and determines whether 
        ///     the member satisfies the particular attribute.  This either means that the member 
        ///     contains the attribute or the member does not contain the attribute and the default 
        ///     for the attribute matches the passed in attribute. 
        /// </summary> 
        private static bool ShouldHideMember(MemberDescriptor member, Attribute attribute)
        {
            if (member == null || attribute == null)
            {
                return true;
            }

            Attribute memberAttribute = member.Attributes[attribute.GetType()];
            if (memberAttribute == null)
            {
                return !attribute.IsDefaultAttribute();
            }
            else
            {
                return !attribute.Equals(memberAttribute);
            }
        }

        /// <summary>
        ///     Sorts descriptors by name of the descriptor.
        /// </summary>
        public static void SortDescriptorArray(IList infos)
        {
            if (infos == null)
            {
                throw new ArgumentNullException(nameof(infos));
            }

            ArrayList.Adapter(infos).Sort(MemberDescriptorComparer.Instance);
        }

        /// <summary>
        ///     This class is a type description provider that works with the IComNativeDescriptorHandler
        ///     interface.
        /// </summary>
        private sealed class ComNativeDescriptionProvider : TypeDescriptionProvider
        {
#pragma warning disable 618
            internal ComNativeDescriptionProvider(IComNativeDescriptorHandler handler)
            {
                Handler = handler;
            }

            /// <summary>
            ///     Returns the COM handler object.
            /// </summary>
            internal IComNativeDescriptorHandler Handler { get; set; }
#pragma warning restore 618
            
            /// <summary>
            ///     Implements GetTypeDescriptor.  This creates a custom type
            ///     descriptor that walks the linked list for each of its calls.
            /// </summary>
            
            [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                if (objectType == null)
                {
                    throw new ArgumentNullException(nameof(objectType));
                }

                if (instance == null)
                {
                    return null;
                }

                if (!objectType.IsInstanceOfType(instance))
                {
                    throw new ArgumentException(SR.Format(SR.ConvertToException, nameof(objectType), instance.GetType()) , nameof(instance));
                }

                return new ComNativeTypeDescriptor(Handler, instance);
            }

            /// <summary>
            ///     This type descriptor sits on top of a native
            ///     descriptor handler.
            /// </summary>
            private sealed class ComNativeTypeDescriptor : ICustomTypeDescriptor
            {
#pragma warning disable 618
                private readonly IComNativeDescriptorHandler _handler;
                private readonly object _instance;

                /// <summary>
                ///     Creates a new ComNativeTypeDescriptor.
                /// </summary>
                internal ComNativeTypeDescriptor(IComNativeDescriptorHandler handler, object instance)
                {
                    _handler = handler;
                    _instance = instance;
                }
#pragma warning restore 618

                AttributeCollection ICustomTypeDescriptor.GetAttributes()
                {
                    return _handler.GetAttributes(_instance);
                }

                string ICustomTypeDescriptor.GetClassName()
                {
                    return _handler.GetClassName(_instance);
                }

                string ICustomTypeDescriptor.GetComponentName()
                {
                    return null;
                }

                TypeConverter ICustomTypeDescriptor.GetConverter()
                {
                    return _handler.GetConverter(_instance);
                }

                EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
                {
                    return _handler.GetDefaultEvent(_instance);
                }

                PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
                {
                    return _handler.GetDefaultProperty(_instance);
                }

                object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
                {
                    return _handler.GetEditor(_instance, editorBaseType);
                }

                EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
                {
                    return _handler.GetEvents(_instance);
                }

                EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
                {
                    return _handler.GetEvents(_instance, attributes);
                }

                PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
                {
                    return _handler.GetProperties(_instance, null);
                }

                PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
                {
                    return _handler.GetProperties(_instance, attributes);
                }

                object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
                {
                    return _instance;
                }
            }
        }

        /// <summary>
        ///     This is a type description provider that adds the given
        ///     array of attributes to a class or instance, preserving the rest
        ///     of the metadata in the process.
        /// </summary>
        private sealed class AttributeProvider : TypeDescriptionProvider
        {
            private Attribute[] _attrs;

            /// <summary>
            ///     Creates a new attribute provider.
            /// </summary>
            internal AttributeProvider(TypeDescriptionProvider existingProvider, params Attribute[] attrs) : base(existingProvider)
            {
                _attrs = attrs;
            }

            /// <summary>
            ///     Creates a custom type descriptor that replaces the attributes.
            /// </summary>
            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                return new AttributeTypeDescriptor(_attrs, base.GetTypeDescriptor(objectType, instance));
            }

            /// <summary>
            ///     Our custom type descriptor.
            /// </summary>
            private class AttributeTypeDescriptor : CustomTypeDescriptor
            {
                private Attribute[] _attributeArray;

                /// <summary>
                ///     Creates a new custom type descriptor that can merge 
                ///     the provided set of attributes with the existing set.
                /// </summary>
                internal AttributeTypeDescriptor(Attribute[] attrs, ICustomTypeDescriptor parent) : base(parent)
                {
                    _attributeArray = attrs;
                }

                /// <summary>
                ///     Retrieves the merged set of attributes.  We do not cache
                ///     this because there is always the possibility that someone
                ///     changed our parent provider's metadata.  TypeDescriptor
                ///     will cache this for us anyhow.
                /// </summary>
                public override AttributeCollection GetAttributes()
                {
                    Attribute[] finalAttr = null;
                    AttributeCollection existing = base.GetAttributes();
                    Attribute[] newAttrs = _attributeArray;
                    Attribute[] newArray = new Attribute[existing.Count + newAttrs.Length];
                    int actualCount = existing.Count;
                    existing.CopyTo(newArray, 0);

                    for (int idx = 0; idx < newAttrs.Length; idx++)
                    {
                        Debug.Assert(newAttrs[idx] != null, "_attributes contains a null member");

                        // We must see if this attribute is already in the existing
                        // array.  If it is, we replace it.
                        bool match = false;
                        for (int existingIdx = 0; existingIdx < existing.Count; existingIdx++)
                        {
                            if (newArray[existingIdx].TypeId.Equals(newAttrs[idx].TypeId))
                            {
                                match = true;
                                newArray[existingIdx] = newAttrs[idx];
                                break;
                            }
                        }

                        if (!match)
                        {
                            newArray[actualCount++] = newAttrs[idx];
                        }
                    }

                    // Now, if we collapsed some attributes, create a new array.
                    //
                    if (actualCount < newArray.Length)
                    {
                        finalAttr = new Attribute[actualCount];
                        Array.Copy(newArray, 0, finalAttr, 0, actualCount);
                    }
                    else
                    {
                        finalAttr = newArray;
                    }

                    return new AttributeCollection(finalAttr);
                }
            }
        }

        /// <summary>
        ///     This is a simple class that is used to store a filtered
        ///     set of members in an object's dictionary cache.  It is
        ///     used by the PipelineAttributeFilter method.
        /// </summary>
        private sealed class AttributeFilterCacheItem
        {
            private readonly Attribute[] _filter;
            internal readonly ICollection FilteredMembers;

            internal AttributeFilterCacheItem(Attribute[] filter, ICollection filteredMembers)
            {
                _filter = filter;
                FilteredMembers = filteredMembers;
            }

            internal bool IsValid(Attribute[] filter)
            {
                if (_filter.Length != filter.Length) return false;

                for (int idx = 0; idx < filter.Length; idx++)
                {
                    if (_filter[idx] != filter[idx])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// This small class contains cache information for the filter stage of our
        /// caching algorithm.  It is used by the PipelineFilter method.
        /// </summary>
        private sealed class FilterCacheItem
        {
            private readonly ITypeDescriptorFilterService _filterService;
            internal readonly ICollection FilteredMembers;

            internal FilterCacheItem(ITypeDescriptorFilterService filterService, ICollection filteredMembers)
            {
                _filterService = filterService;
                FilteredMembers = filteredMembers;
            }

            internal bool IsValid(ITypeDescriptorFilterService filterService)
            {
                if (!Object.ReferenceEquals(_filterService, filterService)) return false;
                return true;
            }
        }

        /// <summary>
        ///     An unimplemented interface.  What is this?  It is an interface that nobody ever
        ///     implements, of course? Where and why would it be used?  Why, to find cross-process
        ///     remoted objects, of course!  If a well-known object comes in from a cross process
        ///     connection, the remoting layer does contain enough type information to determine
        ///     if an object implements an interface.  It assumes that if you are going to cast
        ///     an object to an interface that you know what you're doing, and allows the cast,
        ///     even for objects that DON'T actually implement the interface.  The error here
        ///     is raised later when you make your first call on that interface pointer:  you
        ///     get a remoting exception.
        ///
        ///     This is a big problem for code that does "is" and "as" checks to detect the
        ///     presence of an interface.  We do that all over the place here, so we do a check
        ///     during parameter validation to see if an object implements IUnimplemented.  If it
        ///     does, we know that what we really have is a lying remoting proxy, and we bail.
        /// </summary>
        private interface IUnimplemented { }

        /// <summary>
        ///     This comparer compares member descriptors for sorting.
        /// </summary>
        private sealed class MemberDescriptorComparer : IComparer
        {
            public static readonly MemberDescriptorComparer Instance = new MemberDescriptorComparer();

            public int Compare(object left, object right)
            {
                return CultureInfo.InvariantCulture.CompareInfo.Compare(((MemberDescriptor)left).Name, ((MemberDescriptor)right).Name);
            }
        }

        private sealed class TypeDescriptorComObject
        {
        }

        /// <summary>
        ///     This is a merged type descriptor that can merge the output of
        ///     a primary and secondary type descriptor.  If the primary doesn't
        ///     provide the needed information, the request is passed on to the 
        ///     secondary.
        /// </summary>
        private sealed class MergedTypeDescriptor : ICustomTypeDescriptor
        {
            private readonly ICustomTypeDescriptor _primary;
            private readonly ICustomTypeDescriptor _secondary;

            /// <summary>
            ///     Creates a new MergedTypeDescriptor.
            /// </summary>
            internal MergedTypeDescriptor(ICustomTypeDescriptor primary, ICustomTypeDescriptor secondary)
            {
                _primary = primary;
                _secondary = secondary;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
                AttributeCollection attrs = _primary.GetAttributes() ?? _secondary.GetAttributes();

                Debug.Assert(attrs != null, "Someone should have handled this");
                return attrs;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            string ICustomTypeDescriptor.GetClassName()
            {
                string className = _primary.GetClassName() ?? _secondary.GetClassName();

                Debug.Assert(className != null, "Someone should have handled this");
                return className;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            string ICustomTypeDescriptor.GetComponentName()
            {
                string name = _primary.GetComponentName() ?? _secondary.GetComponentName();

                return name;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            TypeConverter ICustomTypeDescriptor.GetConverter()
            {
                TypeConverter converter = _primary.GetConverter() ?? _secondary.GetConverter();

                Debug.Assert(converter != null, "Someone should have handled this");
                return converter;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
                EventDescriptor evt = _primary.GetDefaultEvent() ?? _secondary.GetDefaultEvent();

                return evt;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
                PropertyDescriptor prop = _primary.GetDefaultProperty() ?? _secondary.GetDefaultProperty();

                return prop;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
                if (editorBaseType == null)
                {
                    throw new ArgumentNullException(nameof(editorBaseType));
                }

                object editor = _primary.GetEditor(editorBaseType) ?? _secondary.GetEditor(editorBaseType);

                return editor;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                EventDescriptorCollection events = _primary.GetEvents() ?? _secondary.GetEvents();

                Debug.Assert(events != null, "Someone should have handled this");
                return events;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
                EventDescriptorCollection events = _primary.GetEvents(attributes) ?? _secondary.GetEvents(attributes);

                Debug.Assert(events != null, "Someone should have handled this");
                return events;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
                PropertyDescriptorCollection properties = _primary.GetProperties() ?? _secondary.GetProperties();

                Debug.Assert(properties != null, "Someone should have handled this");
                return properties;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
                PropertyDescriptorCollection properties = _primary.GetProperties(attributes);
                if (properties == null)
                {
                    properties = _secondary.GetProperties(attributes);
                }

                Debug.Assert(properties != null, "Someone should have handled this");
                return properties;
            }

            /// <summary>
            ///     ICustomTypeDescriptor implementation.
            /// </summary>
            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
            {
                object owner = _primary.GetPropertyOwner(pd) ?? _secondary.GetPropertyOwner(pd);

                return owner;
            }
        }

        /// <summary>
        ///     This is a linked list node that is comprised of a type
        ///     description provider.  Each node contains a Next pointer
        ///     to the next node in the list and also a Provider pointer
        ///     which contains the type description provider this node
        ///     represents.  The implementation of TypeDescriptionProvider
        ///     that the node provides simply invokes the corresponding
        ///     method on the node's provider.
        /// </summary>
        private sealed class TypeDescriptionNode : TypeDescriptionProvider
        {
            internal TypeDescriptionNode Next;
            internal TypeDescriptionProvider Provider;

            /// <summary>
            ///     Creates a new type description node.
            /// </summary>
            internal TypeDescriptionNode(TypeDescriptionProvider provider)
            {
                Provider = provider;
            }

            /// <summary>
            ///     Implements CreateInstance.  This just walks the linked list
            ///     looking for someone who implements the call.
            /// </summary>
            public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
            {
                if (objectType == null)
                {
                    throw new ArgumentNullException(nameof(objectType));
                }

                if (argTypes != null)
                {
                    if (args == null)
                    {
                        throw new ArgumentNullException(nameof(args));
                    }

                    if (argTypes.Length != args.Length)
                    {
                        throw new ArgumentException(SR.TypeDescriptorArgsCountMismatch);
                    }
                }

                return Provider.CreateInstance(provider, objectType, argTypes, args);
            }

            /// <summary>
            ///     Implements GetCache.  This just walks the linked
            ///     list looking for someone who implements the call.
            /// </summary>
            public override IDictionary GetCache(object instance)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }

                return Provider.GetCache(instance);
            }

            /// <summary>
            ///     Implements GetExtendedTypeDescriptor.  This creates a custom type
            ///     descriptor that walks the linked list for each of its calls.
            /// </summary>
            public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }

                return new DefaultExtendedTypeDescriptor(this, instance);
            }

            protected internal override IExtenderProvider[] GetExtenderProviders(object instance)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }

                return Provider.GetExtenderProviders(instance);
            }

            /// <summary>
            ///     The name of the specified component, or null if the component has no name.
            ///     In many cases this will return the same value as GetComponentName. If the
            ///     component resides in a nested container or has other nested semantics, it may
            ///     return a different fully qualified name.
            ///
            ///     If not overridden, the default implementation of this method will call
            ///     GetTypeDescriptor.GetComponentName.
            /// </summary>
            public override string GetFullComponentName(object component)
            {
                if (component == null)
                {
                    throw new ArgumentNullException(nameof(component));
                }

                return Provider.GetFullComponentName(component);
            }

            /// <summary>
            ///     Implements GetReflectionType.  This just walks the linked list
            ///     looking for someone who implements the call.
            /// </summary>
            public override Type GetReflectionType(Type objectType, object instance)
            {
                if (objectType == null)
                {
                    throw new ArgumentNullException(nameof(objectType));
                }

                return Provider.GetReflectionType(objectType, instance);
            }

            public override Type GetRuntimeType(Type objectType)
            {
                if (objectType == null)
                {
                    throw new ArgumentNullException(nameof(objectType));
                }

                return Provider.GetRuntimeType(objectType);
            }

            /// <summary>
            ///     Implements GetTypeDescriptor.  This creates a custom type
            ///     descriptor that walks the linked list for each of its calls.
            /// </summary>
            [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                if (objectType == null)
                {
                    throw new ArgumentNullException(nameof(objectType));
                }

                if (instance != null && !objectType.IsInstanceOfType(instance))
                {
                    throw new ArgumentException(nameof(instance));
                }

                return new DefaultTypeDescriptor(this, objectType, instance);
            }

            public override bool IsSupportedType(Type type)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }
                return Provider.IsSupportedType(type);
            }

            /// <summary>
            ///     A type descriptor for extended types.  This type descriptor
            ///     looks at the head node in the linked list.
            /// </summary>
            private readonly struct DefaultExtendedTypeDescriptor : ICustomTypeDescriptor
            {
                private readonly TypeDescriptionNode _node;
                private readonly object _instance;

                /// <summary>
                ///     Creates a new WalkingExtendedTypeDescriptor.
                /// </summary>
                internal DefaultExtendedTypeDescriptor(TypeDescriptionNode node, object instance)
                {
                    _node = node;
                    _instance = instance;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                AttributeCollection ICustomTypeDescriptor.GetAttributes()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedAttributes(_instance);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    AttributeCollection attrs = desc.GetAttributes();
                    if (attrs == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetAttributes"));
                    return attrs;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                string ICustomTypeDescriptor.GetClassName()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedClassName(_instance);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    string name = desc.GetClassName() ?? _instance.GetType().FullName;
                    return name;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                string ICustomTypeDescriptor.GetComponentName()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedComponentName(_instance);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    return desc.GetComponentName();
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                TypeConverter ICustomTypeDescriptor.GetConverter()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedConverter(_instance);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    TypeConverter converter = desc.GetConverter();
                    if (converter == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetConverter"));
                    return converter;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedDefaultEvent(_instance);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    return desc.GetDefaultEvent();
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedDefaultProperty(_instance);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    return desc.GetDefaultProperty();
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
                {
                    if (editorBaseType == null)
                    {
                        throw new ArgumentNullException(nameof(editorBaseType));
                    }

                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedEditor(_instance, editorBaseType);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    return desc.GetEditor(editorBaseType);
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedEvents(_instance);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    EventDescriptorCollection events = desc.GetEvents();
                    if (events == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEvents"));
                    return events;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        // There is no need to filter these events.  For extended objects, they
                        // are accessed through our pipeline code, which always filters before
                        // returning.  So any filter we do here is redundant.  Note that we do
                        // pass a valid filter to a custom descriptor so it can optimize if it wants.
                        EventDescriptorCollection events = rp.GetExtendedEvents(_instance);
                        return events;
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    EventDescriptorCollection evts = desc.GetEvents(attributes);
                    if (evts == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEvents"));
                    return evts;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedProperties(_instance);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    PropertyDescriptorCollection properties = desc.GetProperties();
                    if (properties == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetProperties"));
                    return properties;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        // There is no need to filter these properties.  For extended objects, they
                        // are accessed through our pipeline code, which always filters before
                        // returning.  So any filter we do here is redundant.  Note that we do
                        // pass a valid filter to a custom descriptor so it can optimize if it wants.
                        PropertyDescriptorCollection props = rp.GetExtendedProperties(_instance);
                        return props;
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    PropertyDescriptorCollection properties = desc.GetProperties(attributes);
                    if (properties == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetProperties"));
                    return properties;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;

                    if (rp != null)
                    {
                        return rp.GetExtendedPropertyOwner(_instance, pd);
                    }

                    ICustomTypeDescriptor desc = p.GetExtendedTypeDescriptor(_instance);
                    if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"));
                    object owner = desc.GetPropertyOwner(pd) ?? _instance;
                    return owner;
                }
            }

            /// <summary>
            ///     The default type descriptor.
            /// </summary>
            private readonly struct DefaultTypeDescriptor : ICustomTypeDescriptor
            {
                private readonly TypeDescriptionNode _node;
                private readonly Type _objectType;
                private readonly object _instance;

                /// <summary>
                ///     Creates a new WalkingTypeDescriptor.
                /// </summary>
                internal DefaultTypeDescriptor(TypeDescriptionNode node, Type objectType, object instance)
                {
                    _node = node;
                    _objectType = objectType;
                    _instance = instance;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                AttributeCollection ICustomTypeDescriptor.GetAttributes()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    AttributeCollection attrs;

                    if (rp != null)
                    {
                        attrs = rp.GetAttributes(_objectType);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        attrs = desc.GetAttributes();
                        if (attrs == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetAttributes"));
                    }

                    return attrs;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                string ICustomTypeDescriptor.GetClassName()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    string name;

                    if (rp != null)
                    {
                        name = rp.GetClassName(_objectType);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        name = desc.GetClassName() ?? _objectType.FullName;
                    }

                    return name;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                string ICustomTypeDescriptor.GetComponentName()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    string name;

                    if (rp != null)
                    {
                        name = rp.GetComponentName(_objectType, _instance);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        name = desc.GetComponentName();
                    }

                    return name;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                TypeConverter ICustomTypeDescriptor.GetConverter()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    TypeConverter converter;

                    if (rp != null)
                    {
                        converter = rp.GetConverter(_objectType, _instance);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        converter = desc.GetConverter();
                        if (converter == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetConverter"));
                    }

                    return converter;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    EventDescriptor defaultEvent;

                    if (rp != null)
                    {
                        defaultEvent = rp.GetDefaultEvent(_objectType, _instance);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        defaultEvent = desc.GetDefaultEvent();
                    }

                    return defaultEvent;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    PropertyDescriptor defaultProperty;

                    if (rp != null)
                    {
                        defaultProperty = rp.GetDefaultProperty(_objectType, _instance);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        defaultProperty = desc.GetDefaultProperty();
                    }

                    return defaultProperty;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
                {
                    if (editorBaseType == null)
                    {
                        throw new ArgumentNullException(nameof(editorBaseType));
                    }

                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    object editor;

                    if (rp != null)
                    {
                        editor = rp.GetEditor(_objectType, _instance, editorBaseType);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        editor = desc.GetEditor(editorBaseType);
                    }

                    return editor;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    EventDescriptorCollection events;

                    if (rp != null)
                    {
                        events = rp.GetEvents(_objectType);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        events = desc.GetEvents();
                        if (events == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEvents"));
                    }

                    return events;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    EventDescriptorCollection events;

                    if (rp != null)
                    {
                        events = rp.GetEvents(_objectType);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        events = desc.GetEvents(attributes);
                        if (events == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEvents"));
                    }

                    return events;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    PropertyDescriptorCollection properties;

                    if (rp != null)
                    {
                        properties = rp.GetProperties(_objectType);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        properties = desc.GetProperties();
                        if (properties == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetProperties"));
                    }

                    return properties;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    PropertyDescriptorCollection properties;

                    if (rp != null)
                    {
                        properties = rp.GetProperties(_objectType);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        properties = desc.GetProperties(attributes);
                        if (properties == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetProperties"));
                    }

                    return properties;
                }

                /// <summary>
                ///     ICustomTypeDescriptor implementation.
                /// </summary>
                object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
                {
                    // Check to see if the provider we get is a ReflectTypeDescriptionProvider.
                    // If so, we can call on it directly rather than creating another
                    // custom type descriptor

                    TypeDescriptionProvider p = _node.Provider;
                    ReflectTypeDescriptionProvider rp = p as ReflectTypeDescriptionProvider;
                    object owner;

                    if (rp != null)
                    {
                        owner = rp.GetPropertyOwner(_objectType, _instance, pd);
                    }
                    else
                    {
                        ICustomTypeDescriptor desc = p.GetTypeDescriptor(_objectType, _instance);
                        if (desc == null) throw new InvalidOperationException(SR.Format(SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"));
                        owner = desc.GetPropertyOwner(pd) ?? _instance;
                    }

                    return owner;
                }
            }
        }

        /// <summary>
        ///     This is a simple internal type that allows external parties to
        ///     register a custom type description provider for all interface types.
        /// </summary>
        private sealed class TypeDescriptorInterface
        {
        }
    }
}
