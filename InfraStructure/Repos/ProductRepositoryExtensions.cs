using System.Threading.Tasks;
using HighSens.Domain;
using InfraStructure.Context;
using Microsoft.EntityFrameworkCore;
using HighSens.Application.Interfaces.IRepositories;

namespace InfraStructure.Repos
{
    public static class ProductRepositoryExtensions
    {
        public static async Task<Product?> GetByNameAsync(this IProductRepository repo, string name)
        {
            // If the implementation is the concrete ProductRepository in this assembly, call its internal method
            if (repo is ProductRepository pr)
            {
                return await pr.GetByNameInternalAsync(name);
            }
            // Otherwise, no-op - could be implemented by other repo implementations
            return null;
        }
    }
}