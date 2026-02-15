using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using InfraStructure.Context;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;

public class QueryClientProductInSectionModel : PageModel
{
    private readonly DBContext _db;
    public QueryClientProductInSectionModel(DBContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int? SectionId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ClientId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ProductId { get; set; }

    public List<SelectListItem> Sections { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();

    public ClientProductResult Result { get; set; }
    public bool HasResult { get; set; }

    public async Task OnGetAsync()
    {
        Sections = await _db.Sections
            .OrderBy(s => s.Name)
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
            .ToListAsync();

        Clients = await _db.Clients
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToListAsync();

        Products = await _db.Products
            .OrderBy(p => p.Name)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
            .ToListAsync();

        if (!SectionId.HasValue || !ClientId.HasValue || !ProductId.HasValue) return;

        var exists = await _db.Sections.AnyAsync(s => s.Id == SectionId.Value)
            && await _db.Clients.AnyAsync(c => c.Id == ClientId.Value)
            && await _db.Products.AnyAsync(p => p.Id == ProductId.Value);

        if (!exists)
        {
            ModelState.AddModelError(string.Empty, "????? ?? ?????? ?? ?????? ??? ???????.");
            return;
        }

        var inboundCartons = await (from d in _db.InboundDetails
                                    join i in _db.Inbounds on d.InboundId equals i.Id
                                    where d.SectionId == SectionId.Value
                                       && i.ClientId == ClientId.Value
                                       && d.ProductId == ProductId.Value
                                    select (int?)d.Cartons).SumAsync() ?? 0;

        var inboundPallets = await (from d in _db.InboundDetails
                                    join i in _db.Inbounds on d.InboundId equals i.Id
                                    where d.SectionId == SectionId.Value
                                       && i.ClientId == ClientId.Value
                                       && d.ProductId == ProductId.Value
                                    select (int?)d.Pallets).SumAsync() ?? 0;

        var outboundCartons = await (from d in _db.OutboundDetails
                                     join o in _db.Outbounds on d.OutboundId equals o.Id
                                     where d.SectionId == SectionId.Value
                                       && o.ClientId == ClientId.Value
                                       && d.ProductId == ProductId.Value
                                     select (int?)d.Cartons).SumAsync() ?? 0;

        var outboundPallets = await (from d in _db.OutboundDetails
                                     join o in _db.Outbounds on d.OutboundId equals o.Id
                                     where d.SectionId == SectionId.Value
                                       && o.ClientId == ClientId.Value
                                       && d.ProductId == ProductId.Value
                                     select (int?)d.Pallets).SumAsync() ?? 0;

        Result = new ClientProductResult
        {
            ClientId = ClientId.Value,
            ProductId = ProductId.Value,
            SectionId = SectionId.Value,
            Cartons = inboundCartons - outboundCartons,
            Pallets = inboundPallets - outboundPallets
        };

        HasResult = true;
    }

    public class ClientProductResult
    {
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public int SectionId { get; set; }
        public int Cartons { get; set; }
        public int Pallets { get; set; }
    }
}