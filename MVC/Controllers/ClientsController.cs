using Microsoft.AspNetCore.Mvc;
using InfraStructure.Context;
using HighSens.Domain;

namespace MVC.Controllers
{
    public class ClientsController : Controller
    {
        private readonly DBContext _db;
        public ClientsController(DBContext db) => _db = db;

        public IActionResult Index()
        {
            var clients = _db.Clients.ToList();
            return View(clients);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Client client)
        {
            if (!ModelState.IsValid) return View(client);
            _db.Clients.Add(client);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
