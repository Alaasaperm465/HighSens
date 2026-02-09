using HighSens.Domain;

namespace HighSens.Domain.Interfaces
{
    public interface ISectionStockRepository
    {
        Task<SectionStock?> FindAsync(int clientId, int sectionId);
        Task AddAsync(SectionStock stock);
        void Update(SectionStock stock);
    }
}