// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.Contracts;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CompositionBatch
    {
        private object _lock = new object();
        private bool _copyNeededForAdd;
        private bool _copyNeededForRemove;
        private List<ComposablePart> _partsToAdd;
        private ReadOnlyCollection<ComposablePart> _readOnlyPartsToAdd;
        private List<ComposablePart> _partsToRemove;
        private ReadOnlyCollection<ComposablePart> _readOnlyPartsToRemove;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionBatch"/> class.
        /// </summary>
        public CompositionBatch() : 
            this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionBatch"/> class.
        /// </summary>
        /// <param name="partsToAdd">The parts to add.</param>
        /// <param name="partsToRemove">The parts to remove.</param>
        public CompositionBatch(IEnumerable<ComposablePart> partsToAdd, IEnumerable<ComposablePart> partsToRemove)
        {
            this._partsToAdd = new List<ComposablePart>();
            if (partsToAdd != null)
            {
                foreach (var part in partsToAdd)
                {
                    if (part == null)
                    {
                        throw ExceptionBuilder.CreateContainsNullElement("partsToAdd");
                    }
                    this._partsToAdd.Add(part);
                }
            }
            this._readOnlyPartsToAdd = this._partsToAdd.AsReadOnly();

            this._partsToRemove = new List<ComposablePart>();
            if (partsToRemove != null)
            {
                foreach (var part in partsToRemove)
                {
                    if (part == null)
                    {
                        throw ExceptionBuilder.CreateContainsNullElement("partsToRemove");
                    }
                    this._partsToRemove.Add(part);
                }
            }
            this._readOnlyPartsToRemove = this._partsToRemove.AsReadOnly();
        }

        /// <summary>
        /// Returns the collection of parts that will be added.
        /// </summary>
        /// <value>The parts to be added.</value>
        public ReadOnlyCollection<ComposablePart> PartsToAdd
        {
            get
            {
                Contract.Ensures(Contract.Result<ReadOnlyCollection<ComposablePart>>() != null);

                lock (this._lock)
                {
                    this._copyNeededForAdd = true;
                    return this._readOnlyPartsToAdd;
                }
            }
        }

        /// <summary>
        /// Returns the collection of parts that will be removed.
        /// </summary>
        /// <value>The parts to be removed.</value>
        public ReadOnlyCollection<ComposablePart> PartsToRemove
        {
            get
            {
                Contract.Ensures(Contract.Result <ReadOnlyCollection<ComposablePart>>() != null);

                lock (this._lock)
                {
                    this._copyNeededForRemove = true;
                    return this._readOnlyPartsToRemove;
                }
            }
        }

        /// <summary>
        ///     Adds the specified part to the <see cref="CompositionBatch"/>.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="part"/> is <see langword="null"/>.
        /// </exception>
        public void AddPart(ComposablePart part)
        {
            Requires.NotNull(part, "part");
            lock (this._lock)
            {
                if (this._copyNeededForAdd)
                {
                    this._partsToAdd = new List<ComposablePart>(this._partsToAdd);
                    this._readOnlyPartsToAdd = this._partsToAdd.AsReadOnly();
                    this._copyNeededForAdd = false;
                }
                this._partsToAdd.Add(part);
            }
        }

        /// <summary>
        ///     Removes the specified part from the <see cref="CompositionBatch"/>.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="part"/> is <see langword="null"/>.
        /// </exception>
        public void RemovePart(ComposablePart part)
        {
            Requires.NotNull(part, "part");
            lock (this._lock)
            {
                if (this._copyNeededForRemove)
                {
                    this._partsToRemove = new List<ComposablePart>(this._partsToRemove);
                    this._readOnlyPartsToRemove = this._partsToRemove.AsReadOnly();
                    this._copyNeededForRemove = false;
                }
                this._partsToRemove.Add(part);
            }
        }

        /// <summary>
        ///     Adds the specified export to the <see cref="CompositionBatch"/>.
        /// </summary>
        /// <param name="export">
        ///     The <see cref="Export"/> to add to the <see cref="CompositionBatch"/>.
        /// </param>
        /// <returns>
        ///     A <see cref="ComposablePart"/> that can be used remove the <see cref="Export"/>
        ///     from the <see cref="CompositionBatch"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="export"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// </remarks>
        public ComposablePart AddExport(Export export)
        {
            Requires.NotNull(export, "export");
            Contract.Ensures(Contract.Result<ComposablePart>() != null);

            ComposablePart part = new SingleExportComposablePart(export);

            this.AddPart(part);

            return part;
        }
    }
}
