using HighSens.Application.DTOs.Section;
using HighSens.Application.Interfaces.IServices;
using HighSens.Domain.Interfaces;
using System.Linq;

namespace HighSens.Application.Services
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _sectionRepo;

        public SectionService(ISectionRepository sectionRepo)
        {
            _sectionRepo = sectionRepo;
        }

        public async Task<IEnumerable<SectionDTO>> GetAllAsync()
        {
            var sections = await _sectionRepo.GetAllAsync();
            return sections.Select(s => new SectionDTO { Id = s.Id, Name = s.Name });
        }
    }
}
