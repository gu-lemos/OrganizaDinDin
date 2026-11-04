using System.Reflection;
using OrganizaDinDin.Domain.Enums;

namespace OrganizaDinDin.Helpers
{
    public static class TipoGastoHelper
    {
        public static EAreaGasto GetArea(this ETipoGasto tipo)
        {
            var field = typeof(ETipoGasto).GetField(tipo.ToString());
            return field?.GetCustomAttribute<AreaGastoAttribute>()?.Area ?? EAreaGasto.Apartamento;
        }

        public static List<ETipoGasto> GetTipos(EAreaGasto area)
        {
            return [.. Enum.GetValues<ETipoGasto>().Where(t => t.GetArea() == area)];
        }

        public static string GetCorClasse(this ETipoGasto tipo)
        {
            var indice = GetTipos(tipo.GetArea()).IndexOf(tipo);
            return $"tipo-cor-{Math.Max(0, indice) % 8}";
        }
    }
}
