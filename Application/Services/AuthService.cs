using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Domain.Entities;
using OrganizaDinDin.Domain.Interfaces;

namespace OrganizaDinDin.Application.Services
{
    public class AuthService(IUsuarioRepository usuarioRepository) : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Usuario?> AuthenticateAsync(string email, string senha)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email.Trim());

            if (usuario == null || !usuario.Ativo || !BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
                return null;

            return usuario;
        }

        public async Task<Usuario> RegisterAsync(string nome, string email, string senha)
        {
            email = email.Trim();

            if (await _usuarioRepository.GetByEmailAsync(email) != null)
                throw new InvalidOperationException("Já existe um usuário com este email");

            return await _usuarioRepository.CreateAsync(new Usuario
            {
                Nome = nome.Trim(),
                Email = email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
                Ativo = true
            });
        }

        public Task<List<Usuario>> GetAllUsuariosAsync() => _usuarioRepository.GetAllAsync();

        public Task UpdateUsuarioRoleAsync(string id, string role) => AtualizarAsync(id, u => u.Role = role);

        public Task ToggleUsuarioAtivoAsync(string id) => AtualizarAsync(id, u => u.Ativo = !u.Ativo);

        public Task ResetSenhaAsync(string id, string novaSenha) => AtualizarAsync(id, u => u.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha));

        private async Task AtualizarAsync(string id, Action<Usuario> alteracao)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuário não encontrado");

            alteracao(usuario);
            await _usuarioRepository.UpdateAsync(usuario);
        }
    }
}
