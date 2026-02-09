using HighSens.Domain;

namespace HighSens.Domain.Interfaces
{
    public interface ISectionRepository : IRepository<Section>
    {
        Task<Section?> GetByNameAsync(string name);
        Task<IEnumerable<Section>> GetAllAsync();
    }
}
