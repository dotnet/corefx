using System;
using System.Collections.Generic;
using System.Composition;
using Microsoft.Composition.Demos.Web.Mvc;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FabrikamBooks.Entities;
using FabrikamBooks.Parts.Tracing;

namespace FabrikamBooks.Parts.Data
{
    [Shared(Boundaries.DataConsistency)]
    public class FabrikamBooksDbContext : DbContext, IDbContext
    {
        readonly ILogger _logger;

        public FabrikamBooksDbContext(ILogger logger)
            : base("Books")
        {
            _logger = logger;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>();
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public new void SaveChanges()
        {
            _logger.Write("Saving changes...");
            base.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _logger.Write("Data context disposed.");
        }
    }
}
