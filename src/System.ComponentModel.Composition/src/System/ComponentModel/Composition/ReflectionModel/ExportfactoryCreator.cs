// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal sealed partial class ExportFactoryCreator
    {
        private static readonly MethodInfo _createStronglyTypedExportFactoryOfT = typeof(ExportFactoryCreator).GetMethod("CreateStronglyTypedExportFactoryOfT", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo _createStronglyTypedExportFactoryOfTM = typeof(ExportFactoryCreator).GetMethod("CreateStronglyTypedExportFactoryOfTM", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private Type    _exportFactoryType;

        public ExportFactoryCreator(Type exportFactoryType)
        {
            Assumes.NotNull(exportFactoryType);

            this._exportFactoryType = exportFactoryType;
        }

        public Func<Export, object> CreateStronglyTypedExportFactoryFactory(Type exportType, Type metadataViewType)
        {
            MethodInfo genericMethod = null;
            if (metadataViewType == null)
            {
                 genericMethod = _createStronglyTypedExportFactoryOfT.MakeGenericMethod(exportType);
            }
            else
            {
                genericMethod = _createStronglyTypedExportFactoryOfTM.MakeGenericMethod(exportType, metadataViewType);
            }

            Assumes.NotNull(genericMethod);
            Func<Export, object> exportFactoryFactory = (Func<Export, object>)Delegate.CreateDelegate(typeof(Func<Export, object>), this, genericMethod);
            return (e) => exportFactoryFactory.Invoke(e);
        }

        private object CreateStronglyTypedExportFactoryOfT<T>(Export export)
        {
            Type[] typeArgs = { typeof(T) };
            Type constructed = this._exportFactoryType.MakeGenericType(typeArgs);

            var lifetimeContext = new LifetimeContext();

            Func<Tuple<T, Action>> exportLifetimeContextCreator = () => lifetimeContext.GetExportLifetimeContextFromExport<T>(export);
            object[] args = { exportLifetimeContextCreator };

            var instance = Activator.CreateInstance(constructed, args);

            return instance;
        }

        private object CreateStronglyTypedExportFactoryOfTM<T, M>(Export export)
        {
            Type[] typeArgs = { typeof(T), typeof(M) };
            Type constructed = this._exportFactoryType.MakeGenericType(typeArgs);

            var lifetimeContext = new LifetimeContext();

            Func<Tuple<T, Action>> exportLifetimeContextCreator = () => lifetimeContext.GetExportLifetimeContextFromExport<T>(export);
            var metadataView = AttributedModelServices.GetMetadataView<M>(export.Metadata);
            object[] args = { exportLifetimeContextCreator, metadataView };

            var instance =  Activator.CreateInstance(constructed, args);

            return instance;
        }

    }
}
