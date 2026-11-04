using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Filters;
using OrganizaDinDin.Application.Interfaces;

namespace OrganizaDinDin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController(IAuthService authService) : AppController
    {
        private readonly IAuthService _authService = authService;

        private string? UsuarioAtualId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IActionResult> Index()
        {
            ViewBag.CurrentUserId = UsuarioAtualId;
            return View(await _authService.GetAllUsuariosAsync());
        }

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> AlterarRole([FromBody] AlterarRoleDto dto) => ExecutarAsync(() =>
        {
            GarantirOutroUsuario(dto.Id, "Você não pode alterar a sua própria permissão");
            return _authService.UpdateUsuarioRoleAsync(dto.Id, dto.Role);
        });

        [HttpPost]
        public Task<IActionResult> ToggleAtivo([FromBody] string id) => ExecutarAsync(() =>
        {
            GarantirOutroUsuario(id, "Você não pode desativar a si mesmo");
            return _authService.ToggleUsuarioAtivoAsync(id);
        });

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> ResetSenha([FromBody] ResetSenhaDto dto) => ExecutarAsync(() =>
        {
            GarantirOutroUsuario(dto.Id, "Você não pode redefinir a sua própria senha por aqui");
            return _authService.ResetSenhaAsync(dto.Id, dto.NovaSenha);
        });

        private void GarantirOutroUsuario(string id, string mensagem)
        {
            if (id == UsuarioAtualId)
                throw new InvalidOperationException(mensagem);
        }
    }
}
