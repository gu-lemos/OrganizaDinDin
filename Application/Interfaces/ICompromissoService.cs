using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Application.Interfaces
{
    public interface ICompromissoService
    {
        Task<Compromisso> CreateAsync(CompromissoDto dto);
        Task<Compromisso> UpdateAsync(CompromissoDto dto);
        Task DeleteAsync(string id);
    }
}
