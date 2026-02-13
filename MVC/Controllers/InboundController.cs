using AutoMapper;
using HighSens.Application.DTOs.Inbound;
using HighSens.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.ViewModels.Inbound;

namespace MVC.Controllers
{
    public class InboundController : Controller
    {
        private readonly IInboundService _inboundService;
        private readonly HighSens.Application.Interfaces.IServices.IClientService _clientService;
        private readonly HighSens.Application.Interfaces.IServices.IProductService _productService;
        private readonly HighSens.Application.Interfaces.IServices.ISectionService _sectionService;
        private readonly IMapper _mapper;

        public InboundController(
            IInboundService inboundService,
            HighSens.Application.Interfaces.IServices.IClientService clientService,
            HighSens.Application.Interfaces.IServices.IProductService productService,
            HighSens.Application.Interfaces.IServices.ISectionService sectionService,
            IMapper mapper)
        {
            _inboundService = inboundService;
            _clientService = clientService;
            _productService = productService;
            _sectionService = sectionService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var inbounds = await _inboundService.GetAllInboundsAsync();
            var model = _mapper.Map<IEnumerable<InboundListVM>>(inbounds);
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var clients = await _clientService.GetAllAsync();
            var products = await _productService.GetAllAsync();
            var sections = await _sectionService.GetAllAsync();

            var vm = new InboundCreateVM
            {
                Clients = clients.Select(c => new SelectListItem(c.Name, c.Id.ToString())),
                Products = products.Select(p => new SelectListItem(p.Name, p.Id.ToString())),
                Sections = sections.Select(s => new SelectListItem(s.Name, s.Id.ToString())),
                Details = new List<InboundDetailVM> { new InboundDetailVM() }
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InboundCreateVM vm)
        {
            // existing server-side non-AJAX handling
            async Task PopulateLookups()
            {
                var clients = await _clientService.GetAllAsync();
                var products = await _productService.GetAllAsync();
                var sections = await _sectionService.GetAllAsync();

                vm.Clients = clients.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
                vm.Products = products.Select(p => new SelectListItem(p.Name, p.Id.ToString()));
                vm.Sections = sections.Select(s => new SelectListItem(s.Name, s.Id.ToString()));
            }

            if (!ModelState.IsValid)
            {
                await PopulateLookups();
                return View(vm);
            }

            var clientDto = await _clientService.GetByIdAsync(vm.ClientId);
            if (clientDto == null)
            {
                ModelState.AddModelError(nameof(vm.ClientId), "Selected client does not exist");
                await PopulateLookups();
                return View(vm);
            }

            var lines = new List<InboundLineRequest>();
            var allSections = (await _sectionService.GetAllAsync()).ToList();

            foreach (var line in vm.Details)
            {
                var productDto = await _productService.GetByIdAsync(line.ProductId);
                var sectionDto = allSections.FirstOrDefault(s => s.Id == line.SectionId);

                if (productDto == null)
                {
                    ModelState.AddModelError(string.Empty, $"Product not found for id {line.ProductId}");
                    break;
                }
                if (sectionDto == null)
                {
                    ModelState.AddModelError(string.Empty, $"Section not found for id {line.SectionId}");
                    break;
                }
                if (line.Cartons < 0 || line.Pallets < 0)
                {
                    ModelState.AddModelError(string.Empty, "Cartons and Pallets must be non-negative");
                    break;
                }

                lines.Add(new InboundLineRequest
                {
                    ProductName = productDto.Name,
                    SectionName = sectionDto.Name,
                    Cartons = line.Cartons,
                    Pallets = line.Pallets
                });
            }

            if (!ModelState.IsValid)
            {
                await PopulateLookups();
                return View(vm);
            }

            var request = new CreateInboundRequest
            {
                ClientName = clientDto.Name,
                Lines = lines
            };

            try
            {
                var id = await _inboundService.CreateInboundAsync(request);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateLookups();
                return View(vm);
            }
        }

        // New AJAX endpoint to handle JSON POST and return JSON result
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Inbound/CreateAjax")]
        public async Task<IActionResult> CreateAjax([FromBody] CreateInboundRequest request)
        {
            if (request == null) return BadRequest(new { success = false, error = "Request body is required" });

            try
            {
                var id = await _inboundService.CreateInboundAsync(request);
                return Ok(new { success = true, id = id, client = request.ClientName });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, error = "Server error" });
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var inbounds = await _inboundService.GetAllInboundsAsync();
            var inbound = inbounds.FirstOrDefault(i => i.Id == id);
            if (inbound == null) return NotFound();

            var vm = _mapper.Map<InboundDetailsVM>(inbound);
            return View(vm);
        }
    }
}