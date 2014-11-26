using System;
using System.Collections.Generic;
using System.Composition;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FabrikamBooks.Parts.Data;

namespace FabrikamBooks.Parts.Data
{
    public class SaveChangesAttribute : ActionFilterAttribute
    {
        [Import]
        public IDbContext DbContext { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception == null)
                DbContext.SaveChanges();
        }
    }
}
