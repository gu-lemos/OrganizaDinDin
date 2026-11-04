using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizaDinDin.Application.DTOs;
using OrganizaDinDin.Application.Filters;
using OrganizaDinDin.Application.Interfaces;

namespace OrganizaDinDin.Controllers
{
    [Authorize(Policy = "UserAccess")]
    public class CompromissosController(ICompromissoService compromissoService) : AppController
    {
        private readonly ICompromissoService _compromissoService = compromissoService;

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> Create([FromBody] CompromissoDto dto) => ExecutarAsync(() => _compromissoService.CreateAsync(dto));

        [HttpPost]
        [ValidateModel]
        public Task<IActionResult> Update([FromBody] CompromissoDto dto) => ExecutarAsync(() => _compromissoService.UpdateAsync(dto));

        [HttpPost]
        public Task<IActionResult> Delete([FromBody] string id) => ExecutarAsync(() => _compromissoService.DeleteAsync(id));
    }
}
