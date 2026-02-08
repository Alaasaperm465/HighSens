using HighSens.Application.Interfaces.IServices;
using HighSens.Application.DTOs.Stock;
using HighSens.Domain;
using Frozen_Warehouse.Domain.Interfaces;
using HighSens.Domain.Interfaces;

namespace Frozen_Warehouse.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepo;

        public StockService(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }
        public async Task<StockResponse> GetStockAsync(int clientId, int productId, int sectionId)
        {
            var stock = await _stockRepo.FindAsync(clientId, productId, sectionId);

            if (stock == null)
            {
                return new StockResponse
                {
                    ClientId = clientId,
                    ProductId = productId,
                    SectionId = sectionId,
                    Cartons = 0,
                    Pallets = 0
                };
            }

            return new StockResponse
            {
                ClientId = stock.ClientId,
                ProductId = stock.ProductId,
                SectionId = stock.SectionId,
                Cartons = stock.Cartons,
                Pallets = stock.Pallets
            };
        }
        //public async Task<StockResponse> GetStockAsync(Guid clientId, Guid productId, Guid sectionId)
        public async Task<decimal> GetStockQuantityAsync(int clientId, int productId, int sectionId)
        {
            var stock = await _stockRepo.FindAsync(clientId, productId, sectionId);
            if (stock == null) return 0;
            return stock.Cartons;
        }
    }
}