using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FabrikamBooks.Entities;
using FabrikamBooks.Models;
using FabrikamBooks.Parts.Data;
using FabrikamBooks.Parts.Tracing;

namespace FabrikamBooks.Controllers
{
    public class HomeController : Controller
    {
        IDbContext _dbContext;
        ILogger _logger;

        public HomeController(IDbContext dbContext, ILogger logger)
        {
            if (dbContext == null) throw new ArgumentNullException("dbContext");
            if (logger == null) throw new ArgumentNullException("logger");

            _dbContext = dbContext;
            _logger = logger;
        }

        public ActionResult Index()
        {
            _logger.Write("Executing the Index() action method.");

            var books = _dbContext.Set<Book>()
                .OrderBy(b => b.Title)
                .Select(b => new BookModel { Id = b.Id, Title = b.Title })
                .ToArray();

            return View(books);
        }

        [HttpPost, SaveChanges]
        public ActionResult Add(string title)
        {
            var book = new Book { Title = title };
            _dbContext.Set<Book>().Add(book);
            return RedirectToAction("Index");
        }

        [HttpPost, SaveChanges]
        public ActionResult Remove(Book bookToRemove)
        {
            _dbContext.Set<Book>().Remove(bookToRemove);
            return RedirectToAction("Index");
        }
    }
}
