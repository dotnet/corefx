// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ImportingMember : ImportingItem
    {
        private readonly ReflectionWritableMember _member;

        public ImportingMember(ContractBasedImportDefinition definition, ReflectionWritableMember member, ImportType importType)
            : base(definition, importType)
        {
            Assumes.NotNull(definition, member);

            this._member = member;
        }

        public void SetExportedValue(object instance, object value)
        {
            if (RequiresCollectionNormalization())
            {
                this.SetCollectionMemberValue(instance, (IEnumerable)value);
            }
            else
            {
                this.SetSingleMemberValue(instance, value);
            }
        }

        private bool RequiresCollectionNormalization()
        {
            if (this.Definition.Cardinality != ImportCardinality.ZeroOrMore)
            {   // If we're not looking at a collection import, then don't 
                // 'normalize' the collection.

                return false;
            }

            if (this._member.CanWrite && this.ImportType.IsAssignableCollectionType)
            {   // If we can simply replace the entire value of the property/field, then 
                // we don't need to 'normalize' the collection.

                return false;
            }

            return true;
        }

        private void SetSingleMemberValue(object instance, object value)
        {
            EnsureWritable();

            try
            {
                this._member.SetValue(instance, value);
            }
            catch (TargetInvocationException exception)
            {   // Member threw an exception. Avoid letting this 
                // leak out as a 'raw' unhandled exception, instead,
                // we'll add some context and rethrow.
                throw new ComposablePartException(
                    String.Format(CultureInfo.CurrentCulture,
                        SR.ReflectionModel_ImportThrewException,
                        this._member.GetDisplayName()),
                    Definition.ToElement(),
                    exception.InnerException);
            }
            catch (TargetParameterCountException exception)
            {
                // Exception was a TargetParameterCountException this occurs when we try to set an Indexer that has an Import
                // this is not supported in MEF currently.  Ideally we would validate against it, however, we already shipped
                // so we will turn it into a ComposablePartException instead, that they should already be prepared for
                throw new ComposablePartException(
                    String.Format(CultureInfo.CurrentCulture,
                        SR.ImportNotValidOnIndexers,
                        this._member.GetDisplayName()),
                    Definition.ToElement(),
                    exception.InnerException);
            }
        }

        private void EnsureWritable()
        {
            if (!this._member.CanWrite)
            {   // Property does not have a setter, or 
                // field is marked as read-only.

                throw new ComposablePartException(
                    String.Format(CultureInfo.CurrentCulture,
                        SR.ReflectionModel_ImportNotWritable,
                        this._member.GetDisplayName()),
                        Definition.ToElement());
            }
        }

        private void SetCollectionMemberValue(object instance, IEnumerable values)
        {
            Assumes.NotNull(values);

            ICollection<object> collection = null;
            Type itemType = CollectionServices.GetCollectionElementType(this.ImportType.ActualType);
            if (itemType != null)
            {
                collection = GetNormalizedCollection(itemType, instance);
            }

            EnsureCollectionIsWritable(collection);
            PopulateCollection(collection, values);
        }

        private ICollection<object> GetNormalizedCollection(Type itemType, object instance)
        {
            Assumes.NotNull(itemType);

            object collectionObject = null;

            if (this._member.CanRead)
            {
                try
                {
                    collectionObject = this._member.GetValue(instance);
                }
                catch (TargetInvocationException exception)
                {
                    throw new ComposablePartException(
                        String.Format(CultureInfo.CurrentCulture,
                            SR.ReflectionModel_ImportCollectionGetThrewException,
                            this._member.GetDisplayName()),
                        this.Definition.ToElement(),
                        exception.InnerException);
                }
            }

            if (collectionObject == null)
            {
                ConstructorInfo constructor = this.ImportType.ActualType.GetConstructor(Type.EmptyTypes);

                // If it contains a default public constructor create a new instance.
                if (constructor != null)
                {
                    try
                    {
                        collectionObject = constructor.SafeInvoke();
                    }
                    catch (TargetInvocationException exception)
                    {
                        throw new ComposablePartException(
                            String.Format(CultureInfo.CurrentCulture,
                                SR.ReflectionModel_ImportCollectionConstructionThrewException,
                                this._member.GetDisplayName(),
                                this.ImportType.ActualType.FullName),
                            this.Definition.ToElement(),
                            exception.InnerException);
                    }

                    SetSingleMemberValue(instance, collectionObject);
                }
            }

            if (collectionObject == null)
            {
                throw new ComposablePartException(
                    String.Format(CultureInfo.CurrentCulture,
                        SR.ReflectionModel_ImportCollectionNull,
                        this._member.GetDisplayName()),
                    this.Definition.ToElement());
            }

            return CollectionServices.GetCollectionWrapper(itemType, collectionObject);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void EnsureCollectionIsWritable(ICollection<object> collection)
        {
            bool isReadOnly = true;
                
            try
            {
                if (collection != null)
                {
                    isReadOnly = collection.IsReadOnly;
                }
            }
            catch (Exception exception)
            {
                throw new ComposablePartException(
                    String.Format(CultureInfo.CurrentCulture,
                        SR.ReflectionModel_ImportCollectionIsReadOnlyThrewException,
                        this._member.GetDisplayName(),
                        collection.GetType().FullName),
                    this.Definition.ToElement(),
                    exception);
            }

            if (isReadOnly)
            {
                throw new ComposablePartException(
                    String.Format(CultureInfo.CurrentCulture,
                        SR.ReflectionModel_ImportCollectionNotWritable,
                        this._member.GetDisplayName()),
                    this.Definition.ToElement());
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void PopulateCollection(ICollection<object> collection, IEnumerable values)
        {
            Assumes.NotNull(collection, values);

            try
            {
                collection.Clear();
            }
            catch (Exception exception)
            {
                throw new ComposablePartException(
                    String.Format(CultureInfo.CurrentCulture,
                        SR.ReflectionModel_ImportCollectionClearThrewException,
                        this._member.GetDisplayName(),
                        collection.GetType().FullName),
                    this.Definition.ToElement(),
                    exception);
            }

            foreach (object value in values)
            {
                try
                {
                    collection.Add(value);
                }
                catch (Exception exception)
                {
                    throw new ComposablePartException(
                        String.Format(CultureInfo.CurrentCulture,
                            SR.ReflectionModel_ImportCollectionAddThrewException,
                            this._member.GetDisplayName(),
                            collection.GetType().FullName),
                        this.Definition.ToElement(),
                        exception);
                }
            }
        }
    }
}
