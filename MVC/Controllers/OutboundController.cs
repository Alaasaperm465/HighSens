using Microsoft.AspNetCore.Mvc;
using InfraStructure.Context;
using HighSens.Domain;

namespace MVC.Controllers
{
    public class OutboundController : Controller
    {
        private readonly DBContext _db;
        public OutboundController(DBContext db) => _db = db;

        public IActionResult Index()
        {
            // simple list of outbounds
            var outbounds = _db.Outbounds.ToList();
            return View(outbounds);
        }

        public IActionResult Create()
        {
            ViewBag.Clients = _db.Clients.ToList();
            ViewBag.Products = _db.Products.ToList();
            ViewBag.Sections = _db.Sections.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(int clientId, int productId, int sectionId, int cartons, int pallets)
        {
            var stock = _db.Stocks.FirstOrDefault(s => s.ClientId == clientId && s.ProductId == productId && s.SectionId == sectionId);
            if (stock == null || stock.Cartons < cartons || stock.Pallets < pallets)
            {
                TempData["Error"] = "Insufficient stock";
                return RedirectToAction("Create");
            }

            // reduce stock
            stock.Cartons -= cartons;
            stock.Pallets -= pallets;
            _db.Outbounds.Add(new Outbound { ClientId = clientId, CreatedAt = DateTime.UtcNow });
            _db.SaveChanges();
            TempData["Success"] = "Outbound created successfully";
            return RedirectToAction("Index");
        }
    }
}