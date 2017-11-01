// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ExportingMember
    {
        private readonly ExportDefinition _definition;
        private readonly ReflectionMember _member;
        private object _cachedValue = null;
        private volatile bool _isValueCached = false;

        public ExportingMember(ExportDefinition definition, ReflectionMember member)
        {
            Assumes.NotNull(definition, member);

            this._definition = definition;
            this._member = member;
        }

        public bool RequiresInstance
        {
            get { return _member.RequiresInstance; }
        }

        public ExportDefinition Definition
        {
            get { return _definition; }
        }

        public object GetExportedValue(object instance, object @lock)
        {
            this.EnsureReadable();

            if (!this._isValueCached)
            {
                object exportedValue;
                try
                {
                    exportedValue = this._member.GetValue(instance);
                }
                catch (TargetInvocationException exception)
                {   // Member threw an exception. Avoid letting this 
                    // leak out as a 'raw' unhandled exception, instead,
                    // we'll add some context and rethrow.

                    throw new ComposablePartException(
                        String.Format(CultureInfo.CurrentCulture,
                            SR.ReflectionModel_ExportThrewException,
                            this._member.GetDisplayName()),
                        Definition.ToElement(),
                        exception.InnerException);
                }
                catch (TargetParameterCountException exception)
                {
                    // Exception was a TargetParameterCountException this occurs when we try to get an Indexer that has an Export
                    // this is not supported in MEF currently.  Ideally we would validate against it, however, we already shipped
                    // so we will turn it into a ComposablePartException instead, that they should already be prepared for
                    throw new ComposablePartException(
                        String.Format(CultureInfo.CurrentCulture,
                        SR.ExportNotValidOnIndexers,
                        this._member.GetDisplayName()),
                        Definition.ToElement(),
                        exception.InnerException);
                }

                lock (@lock)
                {
                    if (!this._isValueCached)
                    {
                        this._cachedValue = exportedValue;
                        Thread.MemoryBarrier();

                        this._isValueCached = true;
                    }
                }
            }

            return this._cachedValue;
        }

        private void EnsureReadable()
        {
            if (!this._member.CanRead)
            {   // Property does not have a getter

                throw new ComposablePartException(
                    String.Format(CultureInfo.CurrentCulture, 
                        SR.ReflectionModel_ExportNotReadable,
                        this._member.GetDisplayName()),
                    Definition.ToElement());
            }
        }
    }
}
