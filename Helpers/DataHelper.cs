using System.Globalization;

namespace OrganizaDinDin.Helpers
{
    public static class DataHelper
    {
        public static DateTime ParseData(string valor)
        {
            if (!DateTime.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
                throw new InvalidOperationException("Data inválida");

            return DateTime.SpecifyKind(data, DateTimeKind.Utc);
        }

        public static DateTime InicioDoDiaUtc(DateTime data) => DateTime.SpecifyKind(data.Date, DateTimeKind.Utc);

        public static DateTime FimDoDiaUtc(DateTime data) => DateTime.SpecifyKind(data.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
    }
}
