using HighSens.Domain.Interfaces;
using HighSens.Domain;

namespace HighSens.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByNameAsync(string name);
    }
}
