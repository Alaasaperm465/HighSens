using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using InfraStructure.Context;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;

public class QueryClientInSectionModel : PageModel
{
    private readonly DBContext _db;
    public QueryClientInSectionModel(DBContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int? SectionId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ClientId { get; set; }

    public List<SelectListItem> Sections { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Clients { get; set; } = new List<SelectListItem>();

    public ClientSectionResult Result { get; set; }
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

        if (!SectionId.HasValue || !ClientId.HasValue) return;

        var sectionExists = await _db.Sections.AnyAsync(s => s.Id == SectionId.Value);
        var clientExists = await _db.Clients.AnyAsync(c => c.Id == ClientId.Value);

        if (!sectionExists || !clientExists)
        {
            ModelState.AddModelError(string.Empty, "????? ?? ?????? ??? ???????.");
            return;
        }

        var inboundCartons = await (from d in _db.InboundDetails
                                    join i in _db.Inbounds on d.InboundId equals i.Id
                                    where d.SectionId == SectionId.Value && i.ClientId == ClientId.Value
                                    select (int?)d.Cartons).SumAsync() ?? 0;

        var inboundPallets = await (from d in _db.InboundDetails
                                    join i in _db.Inbounds on d.InboundId equals i.Id
                                    where d.SectionId == SectionId.Value && i.ClientId == ClientId.Value
                                    select (int?)d.Pallets).SumAsync() ?? 0;

        var outboundCartons = await (from d in _db.OutboundDetails
                                     join o in _db.Outbounds on d.OutboundId equals o.Id
                                     where d.SectionId == SectionId.Value && o.ClientId == ClientId.Value
                                     select (int?)d.Cartons).SumAsync() ?? 0;

        var outboundPallets = await (from d in _db.OutboundDetails
                                     join o in _db.Outbounds on d.OutboundId equals o.Id
                                     where d.SectionId == SectionId.Value && o.ClientId == ClientId.Value
                                     select (int?)d.Pallets).SumAsync() ?? 0;

        Result = new ClientSectionResult
        {
            ClientId = ClientId.Value,
            SectionId = SectionId.Value,
            TotalCartons = inboundCartons - outboundCartons,
            TotalPallets = inboundPallets - outboundPallets
        };

        HasResult = true;
    }

    public class ClientSectionResult
    {
        public int ClientId { get; set; }
        public int SectionId { get; set; }
        public int TotalCartons { get; set; }
        public int TotalPallets { get; set; }
    }
}