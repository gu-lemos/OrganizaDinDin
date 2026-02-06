using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Application.Services
{
    public class AuthService(IUsuarioRepository usuarioRepository) : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Usuario?> AuthenticateAsync(string email, string senha)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);

            if (usuario == null || !usuario.Ativo)
                return null;

            if (!VerifyPassword(senha, usuario.SenhaHash))
                return null;

            return usuario;
        }

        public async Task<Usuario> RegisterAsync(string nome, string email, string senha)
        {
            var usuarioExistente = await _usuarioRepository.GetByEmailAsync(email);

            if (usuarioExistente != null)
                throw new InvalidOperationException("Usuário já existe com este email");

            var usuario = new Usuario
            {
                Nome = nome,
                Email = email,
                SenhaHash = HashPassword(senha),
                Ativo = true
            };

            return await _usuarioRepository.CreateAsync(usuario);
        }

        public string HashPassword(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public bool VerifyPassword(string senha, string senhaHash)
        {
            return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
        }

        public async Task<List<Usuario>> GetAllUsuariosAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task UpdateUsuarioRoleAsync(string id, string role)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuário não encontrado");

            usuario.Role = role;
            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task ToggleUsuarioAtivoAsync(string id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuário não encontrado");

            usuario.Ativo = !usuario.Ativo;
            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task ResetSenhaAsync(string id, string novaSenha)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Usuário não encontrado");

            usuario.SenhaHash = HashPassword(novaSenha);
            await _usuarioRepository.UpdateAsync(usuario);
        }
    }
}
