using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Usuario?> AuthenticateAsync(string email, string senha);
        Task<Usuario> RegisterAsync(string nome, string email, string senha);
        string HashPassword(string senha);
        bool VerifyPassword(string senha, string senhaHash);
        Task<List<Usuario>> GetAllUsuariosAsync();
        Task UpdateUsuarioRoleAsync(string id, string role);
        Task ToggleUsuarioAtivoAsync(string id);
        Task ResetSenhaAsync(string id, string novaSenha);
    }
}
