namespace OrganizaDinDin.Domain.Enums
{
    public enum EAreaGasto
    {
        Apartamento,
        Casamento
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AreaGastoAttribute(EAreaGasto area) : Attribute
    {
        public EAreaGasto Area { get; } = area;
    }
}
