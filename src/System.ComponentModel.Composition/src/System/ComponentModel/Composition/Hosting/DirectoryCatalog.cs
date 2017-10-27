// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Diagnostics;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using IOPath = System.IO.Path;

namespace System.ComponentModel.Composition.Hosting
{
    [DebuggerTypeProxy(typeof(DirectoryCatalogDebuggerProxy))]
    public partial class DirectoryCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged, ICompositionElement
    {
        private readonly Lock _thisLock = new Lock();
        private readonly ICompositionElement _definitionOrigin = null;
        private ComposablePartCatalogCollection _catalogCollection;
        private Dictionary<string, AssemblyCatalog> _assemblyCatalogs;
        private volatile bool _isDisposed = false;
        private string _path;
        private string _fullPath;
        private string _searchPattern;
        private ReadOnlyCollection<string> _loadedFiles;

#if FEATURE_REFLECTIONCONTEXT
        private readonly ReflectionContext _reflectionContext = null;
#endif //FEATURE_REFLECTIONCONTEXT

        /// <summary>
        ///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the *.dll files 
        ///     in the given directory path.
        ///
        ///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
        ///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
        /// </summary>
        /// <param name="path">
        ///     Path to the directory to scan for assemblies to add to the catalog.
        ///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="path"/> is a zero-length string, contains only white space, or 
        ///     contains one or more implementation-specific invalid characters.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
        ///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
        ///     be less than 260 characters. 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission. 
        /// </exception>
        public DirectoryCatalog(string path)
            : this(path, "*.dll")
        {
        }

#if FEATURE_REFLECTIONCONTEXT
        /// <summary>
        ///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the *.dll files 
        ///     in the given directory path.
        ///
        ///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
        ///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
        /// </summary>
        /// <param name="path">
        ///     Path to the directory to scan for assemblies to add to the catalog.
        ///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
        /// </param>
        /// <param name="reflectionContext">
        ///     The <see cref="ReflectionContext"/> a context used by the catalog when 
        ///     interpreting the types to inject attributes into the type definition.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="path"/> is a zero-length string, contains only white space, or 
        ///     contains one or more implementation-specific invalid characters.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path"/> is <see langword="null"/> or
        ///     <paramref name="reflectionContext"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
        ///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
        ///     be less than 260 characters. 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission. 
        /// </exception>
        public DirectoryCatalog(string path, ReflectionContext reflectionContext)
            : this(path, "*.dll", reflectionContext)
        {
        }
#endif //FEATURE_REFLECTIONCONTEXT

        /// <summary>
        ///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the *.dll files 
        ///     in the given directory path.
        ///
        ///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
        ///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
        /// </summary>
        /// <param name="path">
        ///     Path to the directory to scan for assemblies to add to the catalog.
        ///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
        /// </param>
        /// <param name="definitionOrigin">
        ///     The <see cref="ICompositionElement"/> CompositionElement used by Diagnostics to identify the source for parts.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="path"/> is a zero-length string, contains only white space, or 
        ///     contains one or more implementation-specific invalid characters.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path"/> is <see langword="null"/> or
        ///     <paramref name="definitionOrigin"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
        ///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
        ///     be less than 260 characters. 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission. 
        /// </exception>
        public DirectoryCatalog(string path, ICompositionElement definitionOrigin) 
            : this(path, "*.dll", definitionOrigin)
        {
        }

#if FEATURE_REFLECTIONCONTEXT
        /// <summary>
        ///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the given searchPattern 
        ///     over the files in the given directory path.
        ///     
        ///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
        ///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
        /// </summary>
        /// <param name="path">
        ///     Path to the directory to scan for assemblies to add to the catalog.
        ///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
        /// </param>
        /// <param name="reflectionContext">
        ///     The <see cref="ReflectionContext"/> a context used by the catalog when 
        ///     interpreting the types to inject attributes into the type definition.
        /// </param>
        /// <param name="definitionOrigin">
        ///     The <see cref="ICompositionElement"/> CompositionElement used by Diagnostics to identify the source for parts.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="path"/> is a zero-length string, contains only white space 
        ///     does not contain a valid pattern. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path"/> is <see langword="null"/> or
        ///     <paramref name="reflectionContext"/> is <see langword="null"/> or
        ///     <paramref name="definitionOrigin"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
        ///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
        ///     be less than 260 characters. 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission. 
        /// </exception>
        public DirectoryCatalog(string path, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
            : this(path, "*.dll", reflectionContext, definitionOrigin)
        {
        }
#endif //FEATURE_REFLECTIONCONTEXT

        /// <summary>
        ///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the *.dll files 
        ///     in the given directory path.
        ///
        ///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
        ///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
        /// </summary>
        /// <param name="path">
        ///     Path to the directory to scan for assemblies to add to the catalog.
        ///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
        /// </param>
        /// <param name="searchPattern">
        ///     Any valid searchPattern that <see cref="Directory.GetFiles(string, string)"/> will accept.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="path"/> is a zero-length string, contains only white space, or 
        ///     contains one or more implementation-specific invalid characters. Or <paramref name="searchPattern"/> 
        ///     does not contain a valid pattern. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
        ///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
        ///     be less than 260 characters. 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission. 
        /// </exception>
        public DirectoryCatalog(string path, string searchPattern)
        {
            Requires.NotNullOrEmpty(path, "path");
            Requires.NotNullOrEmpty(searchPattern, "searchPattern");

            this._definitionOrigin = this;
            this.Initialize(path, searchPattern);
        }

        /// <summary>
        ///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the *.dll files 
        ///     in the given directory path.
        ///
        ///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
        ///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
        /// </summary>
        /// <param name="path">
        ///     Path to the directory to scan for assemblies to add to the catalog.
        ///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
        /// </param>
        /// <param name="definitionOrigin">
        ///     The <see cref="ICompositionElement"/> CompositionElement used by Diagnostics to identify the source for parts.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="path"/> is a zero-length string, contains only white space, or 
        ///     contains one or more implementation-specific invalid characters. Or <paramref name="searchPattern"/> 
        ///     does not contain a valid pattern. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path"/> is <see langword="null"/>.
        ///     <paramref name="definitionOrigin"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
        ///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
        ///     be less than 260 characters. 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission. 
        /// </exception>
        public DirectoryCatalog(string path, string searchPattern, ICompositionElement definitionOrigin)
        {
            Requires.NotNullOrEmpty(path, "path");
            Requires.NotNullOrEmpty(searchPattern, "searchPattern");
            Requires.NotNull(definitionOrigin, "definitionOrigin");

            this._definitionOrigin = definitionOrigin;
            this.Initialize(path, searchPattern);
        }

#if FEATURE_REFLECTIONCONTEXT
        /// <summary>
        ///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the given searchPattern 
        ///     over the files in the given directory path.
        ///     
        ///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
        ///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
        /// </summary>
        /// <param name="path">
        ///     Path to the directory to scan for assemblies to add to the catalog.
        ///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
        /// </param>
        /// <param name="searchPattern">
        ///     Any valid searchPattern that <see cref="Directory.GetFiles(string, string)"/> will accept.
        /// </param>
        /// <param name="reflectionContext">
        ///     The <see cref="ReflectionContext"/> a context used by the catalog when 
        ///     interpreting the types to inject attributes into the type definition.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="path"/> is a zero-length string, contains only white space, or 
        ///     contains one or more implementation-specific invalid characters. Or <paramref name="searchPattern"/> 
        ///     does not contain a valid pattern. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path"/> is <see langword="null"/> 
        ///     or <paramref name="searchPattern"/> is <see langword="null"/>.
        ///     or <paramref name="reflectionContext"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
        ///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
        ///     be less than 260 characters. 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission. 
        /// </exception>
        public DirectoryCatalog(string path, string searchPattern, ReflectionContext reflectionContext)
        {
            Requires.NotNullOrEmpty(path, "path");
            Requires.NotNullOrEmpty(searchPattern, "searchPattern");
            Requires.NotNull(reflectionContext, "reflectionContext");

            this._reflectionContext = reflectionContext;
            this._definitionOrigin = this;
            this.Initialize(path, searchPattern);
        }

        /// <summary>
        ///     Creates a catalog of <see cref="ComposablePartDefinition"/>s based on all the given searchPattern 
        ///     over the files in the given directory path.
        ///     
        ///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
        ///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
        /// </summary>
        /// <param name="path">
        ///     Path to the directory to scan for assemblies to add to the catalog.
        ///     The path needs to be absolute or relative to <see cref="AppDomain.BaseDirectory"/>
        /// </param>
        /// <param name="searchPattern">
        ///     Any valid searchPattern that <see cref="Directory.GetFiles(string, string)"/> will accept.
        /// </param>
        /// <param name="reflectionContext">
        ///     The <see cref="ReflectionContext"/> a context used by the catalog when 
        ///     interpreting the types to inject attributes into the type definition.
        /// </param>
        /// <param name="definitionOrigin">
        ///     The <see cref="ICompositionElement"/> CompositionElement used by Diagnostics to identify the source for parts.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="path"/> is a zero-length string, contains only white space, or 
        ///     contains one or more implementation-specific invalid characters. Or <paramref name="searchPattern"/> 
        ///     does not contain a valid pattern. 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path"/> is <see langword="null"/> 
        ///     or <paramref name="searchPattern"/> is <see langword="null"/>.
        ///     or <paramref name="reflectionContext"/> is <see langword="null"/>.
        ///     or <paramref name="definitionOrigin"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     The specified <paramref name="path"/> is invalid (for example, it is on an unmapped drive). 
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified <paramref name="path"/>, file name, or both exceed the system-defined maximum length. 
        ///     For example, on Windows-based platforms, paths must be less than 248 characters and file names must 
        ///     be less than 260 characters. 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission. 
        /// </exception>
        public DirectoryCatalog(string path, string searchPattern, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
        {
            Requires.NotNullOrEmpty(path, "path");
            Requires.NotNullOrEmpty(searchPattern, "searchPattern");
            Requires.NotNull(reflectionContext, "reflectionContext");
            Requires.NotNull(definitionOrigin, "definitionOrigin");

            this._reflectionContext = reflectionContext;
            this._definitionOrigin = definitionOrigin;
            this.Initialize(path, searchPattern);
        }
#endif //FEATURE_REFLECTIONCONTEXT

        /// <summary>
        ///     Translated absolute path of the path passed into the constructor of <see cref="DirectoryCatalog"/>. 
        /// </summary>
        public string FullPath
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                
                return this._fullPath;
            }
        }

        /// <summary>
        ///     Set of files that have currently been loaded into the catalog.
        /// </summary>
        public ReadOnlyCollection<string> LoadedFiles
        {
            get
            {
                Contract.Ensures(Contract.Result<ReadOnlyCollection<string>>() != null);

                using (new ReadLock(this._thisLock))
                {
                    return this._loadedFiles;
                }
            }
        }

        /// <summary>
        ///     Path passed into the constructor of <see cref="DirectoryCatalog"/>.
        /// </summary>
        public string Path
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                
                return this._path;
            }
        }

        /// <summary>
        ///   SearchPattern passed into the constructor of <see cref="DirectoryCatalog"/>, or the default *.dll.
        /// </summary>
        public string SearchPattern
        {
            get
            {
                return this._searchPattern;
            }
        }

        /// <summary>
        /// Notify when the contents of the Catalog has changed.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        /// <summary>
        /// Notify when the contents of the Catalog has changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (!this._isDisposed)
                    {
                        bool disposeLock = false;
                        ComposablePartCatalogCollection catalogs = null;

                        try
                        {
                            using (new WriteLock(this._thisLock))
                            {
                                if (!this._isDisposed)
                                {
                                    disposeLock = true;
                                    catalogs = this._catalogCollection;
                                    this._catalogCollection = null;
                                    this._assemblyCatalogs = null;
                                    this._isDisposed = true;
                                }
                            }
                        }
                        finally
                        {
                            if (catalogs != null)
                            {
                                catalogs.Dispose();
                            }

                            if (disposeLock)
                            {
                                this._thisLock.Dispose();
                            }
                        }
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            return this._catalogCollection.SelectMany(catalog => catalog as IEnumerable<ComposablePartDefinition>).GetEnumerator();
        }

        /// <summary>
        ///     Returns the export definitions that match the constraint defined by the specified definition.
        /// </summary>
        /// <param name="definition">
        ///     The <see cref="ImportDefinition"/> that defines the conditions of the 
        ///     <see cref="ExportDefinition"/> objects to return.
        /// </param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}"/> of <see cref="Tuple{T1, T2}"/> containing the 
        ///     <see cref="ExportDefinition"/> objects and their associated 
        ///     <see cref="ComposablePartDefinition"/> for objects that match the constraint defined 
        ///     by <paramref name="definition"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="definition"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="DirectoryCatalog"/> has been disposed of.
        /// </exception>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            this.ThrowIfDisposed();

            Requires.NotNull(definition, "definition");

            return this._catalogCollection.SelectMany(catalog => catalog.GetExports(definition));
        }

        /// <summary>
        ///     Raises the <see cref="INotifyComposablePartCatalogChanged.Changed"/> event.
        /// </summary>
        /// <param name="e">
        ///     An <see cref="ComposablePartCatalogChangeEventArgs"/> containing the data for the event.
        /// </param>
        protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
        {
            EventHandler<ComposablePartCatalogChangeEventArgs> changedEvent = this.Changed;
            if (changedEvent != null)
            {
                changedEvent(this, e);
            }
        }

        /// <summary>
        ///     Raises the <see cref="INotifyComposablePartCatalogChanged.Changing"/> event.
        /// </summary>
        /// <param name="e">
        ///     An <see cref="ComposablePartCatalogChangeEventArgs"/> containing the data for the event.
        /// </param>
        protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
        {
            EventHandler<ComposablePartCatalogChangeEventArgs> changingEvent = this.Changing;
            if (changingEvent != null)
            {
                changingEvent(this, e);
            }
        }

        /// <summary>
        ///     Refreshes the <see cref="ComposablePartDefinition"/>s with the latest files in the directory that match
        ///     the searchPattern. If any files have been added they will be added to the catalog and if any files were
        ///     removed they will be removed from the catalog. For files that have been removed keep in mind that the 
        ///     assembly cannot be unloaded from the process so <see cref="ComposablePartDefinition"/>s for those files
        ///     will simply be removed from the catalog.
        /// 
        ///     Possible exceptions that can be thrown are any that <see cref="Directory.GetFiles(string, string)"/> or 
        ///     <see cref="Assembly.Load(AssemblyName)"/> can throw.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">
        ///     The specified path has been removed since object construction.
        /// </exception>
        public void Refresh()
        {
            this.ThrowIfDisposed();
            Assumes.NotNull(this._loadedFiles);

            List<Tuple<string, AssemblyCatalog>> catalogsToAdd;
            List<Tuple<string, AssemblyCatalog>> catalogsToRemove;
            ComposablePartDefinition[] addedDefinitions;
            ComposablePartDefinition[] removedDefinitions;
            object changeReferenceObject;
            string[] afterFiles;
            string[] beforeFiles;

            while (true)
            {
                afterFiles = this.GetFiles();

                using (new ReadLock(this._thisLock))
                {
                    changeReferenceObject = this._loadedFiles;
                    beforeFiles = this._loadedFiles.ToArray();
                }

                this.DiffChanges(beforeFiles, afterFiles, out catalogsToAdd, out catalogsToRemove);

                // Don't go any further if there's no work to do
                if (catalogsToAdd.Count == 0 && catalogsToRemove.Count == 0)
                {
                    return;
                }

                // Notify listeners to give them a preview before completeting the changes
                addedDefinitions = catalogsToAdd
                    .SelectMany(cat => cat.Item2 as IEnumerable<ComposablePartDefinition>)
                    .ToArray<ComposablePartDefinition>();

                removedDefinitions = catalogsToRemove
                    .SelectMany(cat => cat.Item2 as IEnumerable<ComposablePartDefinition>)
                    .ToArray<ComposablePartDefinition>();

                using (var atomicComposition = new AtomicComposition())
                {
                    var changingArgs = new ComposablePartCatalogChangeEventArgs(addedDefinitions, removedDefinitions, atomicComposition);
                    this.OnChanging(changingArgs);

                    // if the change went through then write the catalog changes
                    using (new WriteLock(this._thisLock))
                    {
                        if (changeReferenceObject != this._loadedFiles)
                        {
                            // Someone updated the list while we were diffing so we need to try the diff again
                            continue;
                        }

                        foreach (var catalogToAdd in catalogsToAdd)
                        {
                            this._assemblyCatalogs.Add(catalogToAdd.Item1, catalogToAdd.Item2);
                            this._catalogCollection.Add(catalogToAdd.Item2);
                        }

                        foreach (var catalogToRemove in catalogsToRemove)
                        {
                            this._assemblyCatalogs.Remove(catalogToRemove.Item1);
                            this._catalogCollection.Remove(catalogToRemove.Item2);
                        }

                        this._loadedFiles = afterFiles.ToReadOnlyCollection();

                        // Lastly complete any changes added to the atomicComposition during the change event
                        atomicComposition.Complete();

                        // Break out of the while(true)
                        break;
                    } // WriteLock
                } // AtomicComposition
            }   // while (true)

            var changedArgs = new ComposablePartCatalogChangeEventArgs(addedDefinitions, removedDefinitions, null);
            this.OnChanged(changedArgs);
        }

        /// <summary>
        ///     Returns a string representation of the directory catalog.
        /// </summary>
        /// <returns>
        ///     A <see cref="String"/> containing the string representation of the <see cref="DirectoryCatalog"/>.
        /// </returns>
        public override string ToString()
        {
            return GetDisplayName();
        }

        private AssemblyCatalog CreateAssemblyCatalogGuarded(string assemblyFilePath)
        {
            Exception exception = null;

            try
            {
#if FEATURE_REFLECTIONCONTEXT
                return (this._reflectionContext != null)
                    ? new AssemblyCatalog(assemblyFilePath, this._reflectionContext, this)
                    : new AssemblyCatalog(assemblyFilePath, this);
#else
                return new AssemblyCatalog(assemblyFilePath, this);
#endif //FEATURE_REFLECTIONCONTEXT
            }
            catch (FileNotFoundException ex)
            {   // Files should always exists but don't blow up here if they don't
                exception = ex;
            }
            catch (FileLoadException ex)
            {   // File was found but could not be loaded
                exception = ex;
            }
            catch (BadImageFormatException ex)
            {   // Dlls that contain native code are not loaded, but do not invalidate the Directory
                exception = ex;
            }
            catch (ReflectionTypeLoadException ex)
            {   // Dlls that have missing Managed dependencies are not loaded, but do not invalidate the Directory 
                exception = ex;
            }

            CompositionTrace.AssemblyLoadFailed(this, assemblyFilePath, exception);

            return null;
        }

        private void DiffChanges(string[] beforeFiles, string[] afterFiles,
            out List<Tuple<string, AssemblyCatalog>> catalogsToAdd,
            out List<Tuple<string, AssemblyCatalog>> catalogsToRemove)
        {
            catalogsToAdd = new List<Tuple<string, AssemblyCatalog>>();
            catalogsToRemove = new List<Tuple<string, AssemblyCatalog>>();

            IEnumerable<string> filesToAdd = afterFiles.Except(beforeFiles);
            foreach (string file in filesToAdd)
            {
                AssemblyCatalog catalog = CreateAssemblyCatalogGuarded(file);

                if (catalog != null)
                {
                    catalogsToAdd.Add(new Tuple<string, AssemblyCatalog>(file, catalog));
                }
            }

            IEnumerable<string> filesToRemove = beforeFiles.Except(afterFiles);
            using (new ReadLock(this._thisLock))
            {
                foreach (string file in filesToRemove)
                {
                    AssemblyCatalog catalog;
                    if (this._assemblyCatalogs.TryGetValue(file, out catalog))
                    {
                        catalogsToRemove.Add(new Tuple<string, AssemblyCatalog>(file, catalog));
                    }
                }
            }
        }

        private string GetDisplayName()
        {
            return string.Format(CultureInfo.CurrentCulture,
                                "{0} (Path=\"{1}\")",   // NOLOC
                                this.GetType().Name,
                                this._path);
        }

        private string[] GetFiles()
        {
            string[] files = Directory.GetFiles(this._fullPath, this._searchPattern);
            return Array.ConvertAll<string, string>(files, (file) => file.ToUpperInvariant());
        }

        private static string GetFullPath(string path)
        {
            if (!IOPath.IsPathRooted(path) && AppDomain.CurrentDomain.BaseDirectory != null)
            {
                path = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            return IOPath.GetFullPath(path).ToUpperInvariant();
        }

        private void Initialize(string path, string searchPattern)
        {
            this._path = path;
            this._fullPath = GetFullPath(path);
            this._searchPattern = searchPattern;
            this._assemblyCatalogs = new Dictionary<string, AssemblyCatalog>();
            this._catalogCollection = new ComposablePartCatalogCollection(null, null, null);

            this._loadedFiles = GetFiles().ToReadOnlyCollection();

            foreach (string file in this._loadedFiles)
            {
                AssemblyCatalog assemblyCatalog = null;
                assemblyCatalog = CreateAssemblyCatalogGuarded(file);

                if (assemblyCatalog != null)
                {
                    this._assemblyCatalogs.Add(file, assemblyCatalog);
                    this._catalogCollection.Add(assemblyCatalog);
                }
            }
        }

        [DebuggerStepThrough]
        [ContractArgumentValidator]
        [SuppressMessage("Microsoft.Contracts", "CC1053", Justification = "Suppressing warning because this validator has no public contract")]
        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }
       
        /// <summary>
        ///     Gets the display name of the directory catalog.
        /// </summary>
        /// <value>
        ///     A <see cref="String"/> containing a human-readable display name of the <see cref="DirectoryCatalog"/>.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        string ICompositionElement.DisplayName
        {
            get { return this.GetDisplayName(); }
        }

        /// <summary>
        ///     Gets the composition element from which the directory catalog originated.
        /// </summary>
        /// <value>
        ///     This property always returns <see langword="null"/>.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        ICompositionElement ICompositionElement.Origin
        {
            get { return null; }
        }
    }
}
