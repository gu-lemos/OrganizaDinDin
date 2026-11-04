using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Domain.Interfaces
{
    public interface ICompromissoRepository
    {
        Task<List<Compromisso>> GetAllAsync();
        Task<Compromisso?> GetByIdAsync(string id);
        Task<Compromisso> CreateAsync(Compromisso compromisso);
        Task<Compromisso> UpdateAsync(Compromisso compromisso);
        Task DeleteAsync(string id);
    }
}
