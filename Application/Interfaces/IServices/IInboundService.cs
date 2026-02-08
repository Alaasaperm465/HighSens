using HighSens.Application.DTOs.Inbound;
using HighSens.Domain;

namespace HighSens.Application.Interfaces.IServices
{
    public interface IInboundService
    {
        Task<int> CreateInboundAsync(CreateInboundRequest request);
        Task<IEnumerable<Inbound>> GetAllInboundsAsync();
        Task<IEnumerable<Inbound>> GetDailyInboundReportAsync();
        Task<IEnumerable<Inbound>> GetInboundReportFromToAsync(DateTime startDate, DateTime endDate);
        Task UpdateInboundAsync(int id, UpdateInboundRequest request);
    }
}