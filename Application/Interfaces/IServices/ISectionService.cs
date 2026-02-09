using HighSens.Application.DTOs.Section;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HighSens.Application.Interfaces.IServices
{
    public interface ISectionService
    {
        Task<IEnumerable<SectionDTO>> GetAllAsync();
    }

}
