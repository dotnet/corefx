using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FabrikamBooks.Parts.Data
{
    public interface IDbContext
    {
        IDbSet<T> Set<T>() where T : class;

        void SaveChanges();
    }
}