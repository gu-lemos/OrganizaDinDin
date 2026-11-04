using Microsoft.AspNetCore.Mvc;

namespace OrganizaDinDin.Controllers
{
    public abstract class AppController : Controller
    {
        private const string MensagemErroInesperado = "Não foi possível concluir a operação. Tente novamente.";

        protected Task<IActionResult> ExecutarAsync(Func<Task> acao) => ExecutarAsync<object?>(async () =>
        {
            await acao();
            return null;
        });

        protected async Task<IActionResult> ExecutarAsync<T>(Func<Task<T>> acao)
        {
            try
            {
                var data = await acao();
                return Json(new { success = true, data });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AppController>>();
                logger.LogError(ex, "Erro ao executar {Caminho}", HttpContext.Request.Path);
                return Json(new { success = false, message = MensagemErroInesperado });
            }
        }
    }
}
