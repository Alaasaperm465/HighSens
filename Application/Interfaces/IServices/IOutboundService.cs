using HighSens.Application.DTOs.Outbound;

namespace HighSens.Application.Interfaces.IServices
{
    public interface IOutboundService
    {
        Task<int> CreateOutboundAsync(CreateOutboundRequest request);
    }
}