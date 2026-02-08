using Frozen_Warehouse.Domain.Interfaces;
using HighSens.Domain;

namespace HighSens.Domain.Interfaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client?> FindByNameAsync(string name);
        Task<IEnumerable<Client>> GetAllAsync();
    }
}
