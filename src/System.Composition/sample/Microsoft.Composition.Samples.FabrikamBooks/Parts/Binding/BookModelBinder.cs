using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Composition.Demos.Web.Mvc;
using FabrikamBooks.Entities;
using FabrikamBooks.Parts.Data;

namespace FabrikamBooks.Parts.Binding
{
    [ExportModelBinder(typeof(Book))]
    public class BookModelBinder : IModelBinder
    {
        IDbContext _dbContext;

        public BookModelBinder(IDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException("dbContext");
            _dbContext = dbContext;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var idParameterName = bindingContext.ModelName + "Id";
            var idAsString = controllerContext.HttpContext.Request.Params[idParameterName];
            var id = int.Parse(idAsString);
            return _dbContext.Set<Book>().Find(id);
        }
    }
}