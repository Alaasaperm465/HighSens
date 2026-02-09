using HighSens.Domain;

namespace HighSens.Domain.Interfaces
{
    public interface IProductStockRepository
    {
        Task<ProductStock?> FindAsync(int clientId, int productId);
        Task AddAsync(ProductStock stock);
        void Update(ProductStock stock);
    }
}