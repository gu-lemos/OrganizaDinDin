using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OrganizaDinDin.Application.Interfaces;
using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Filters;

namespace OrganizaDinDin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService = authService;

        public async Task<IActionResult> Index()
        {
            var usuarios = await _authService.GetAllUsuariosAsync();
            ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(usuarios);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> AlterarRole([FromBody] AlterarRoleDto dto)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (dto.Id == currentUserId)
                    return Json(new { success = false, message = "Você não pode alterar sua própria role" });

                await _authService.UpdateUsuarioRoleAsync(dto.Id, dto.Role);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAtivo([FromBody] string id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (id == currentUserId)
                    return Json(new { success = false, message = "Você não pode desativar a si mesmo" });

                await _authService.ToggleUsuarioAtivoAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> ResetSenha([FromBody] ResetSenhaDto dto)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (dto.Id == currentUserId)
                    return Json(new { success = false, message = "Você não pode resetar sua própria senha por aqui" });

                await _authService.ResetSenhaAsync(dto.Id, dto.NovaSenha);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
