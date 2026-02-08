using HighSens.Domain;

namespace HighSens.Domain.Interfaces
{
    public interface IStockRepository
    {
        Task<Stock?> FindAsync(int clientId, int productId, int sectionId);
        Task AddAsync(Stock stock);
        void Update(Stock stock);
    }
}