using System.Collections.Generic;
using System.Threading.Tasks;
using HighSens.Domain;
using HighSens.Application.Interfaces.IRepositories;
using InfraStructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InfraStructure.Repos
{
    public class ProductRepository : HighSens.Application.Interfaces.IRepositories.IProductRepository
    {
        private readonly DBContext _db;
        public ProductRepository(DBContext db) { _db = db; }

        public async Task AddAsync(Product product)
        {
            await _db.Products.AddAsync(product);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _db.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _db.Products.FindAsync(id);
        }

        public async Task<bool> AnyStockAsync(int productId)
        {
            return await _db.Stocks.AnyAsync(s => s.ProductId == productId);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Products.AnyAsync(p => p.Id == id && p.IsActive);
        }
    }
}
