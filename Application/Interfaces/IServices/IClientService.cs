using HighSens.Application.DTOs.Client;

namespace HighSens.Application.Interfaces.IServices
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllAsync();
        Task<ClientDto?> GetByIdAsync(int id);
        Task<ClientDto> CreateAsync(CreateClientRequest request);
        Task<ClientDto?> UpdateAsync(int id, UpdateClientRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
