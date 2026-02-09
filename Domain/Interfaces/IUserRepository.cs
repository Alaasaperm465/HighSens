using HighSens.Domain;
using HighSens.Domain;
using HighSens.Domain.Interfaces;

namespace Frozen_Warehouse.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> FindByUserNameAsync(string userName);
    }
}