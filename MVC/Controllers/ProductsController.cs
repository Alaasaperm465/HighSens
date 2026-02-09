using Microsoft.AspNetCore.Mvc;
using InfraStructure.Context;
using HighSens.Domain;

namespace MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DBContext _db;
        public ProductsController(DBContext db) => _db = db;

        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid) return View(product);
            _db.Products.Add(product);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
