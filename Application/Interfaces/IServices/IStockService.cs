using HighSens.Application.DTOs.Stock;

namespace HighSens.Application.Interfaces.IServices
{
    public interface IStockService
    {
        Task<StockResponse> GetStockAsync(int clientId, int productId, int sectionId);
        Task<decimal> GetStockQuantityAsync(int clientId, int productId, int sectionId);
    }
}