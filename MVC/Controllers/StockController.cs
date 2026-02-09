using Microsoft.AspNetCore.Mvc;
using InfraStructure.Context;
using HighSens.Domain;
using Microsoft.EntityFrameworkCore;

namespace MVC.Controllers
{
    public class StockController : Controller
    {
        private readonly DBContext _db;
        public StockController(DBContext db) => _db = db;

        public async Task<IActionResult> Index(int? productId, int? sectionId)
        {
            var query = _db.Stocks.AsQueryable();
            if (productId.HasValue) query = query.Where(s => s.ProductId == productId.Value);
            if (sectionId.HasValue) query = query.Where(s => s.SectionId == sectionId.Value);
            var list = await query.Include(s => s.Product).Include(s => s.Section).ToListAsync();

            var products = await _db.Products.AsNoTracking().ToListAsync();
            var sections = await _db.Sections.AsNoTracking().ToListAsync();
            ViewBag.Products = products;
            ViewBag.Sections = sections;

            return View(list);
        }

        [HttpPost]
        public IActionResult Check(int clientId, int productId, int sectionId)
        {
            var stock = _db.Stocks.FirstOrDefault(s => s.ClientId == clientId && s.ProductId == productId && s.SectionId == sectionId);
            if (stock == null)
            {
                ViewBag.Message = "No stock found.";
                ViewBag.Cartons = 0;
                ViewBag.Pallets = 0;
            }
            else
            {
                ViewBag.Message = "Stock found.";
                ViewBag.Cartons = stock.Cartons;
                ViewBag.Pallets = stock.Pallets;
            }
            return View("Index");
        }
    }
}