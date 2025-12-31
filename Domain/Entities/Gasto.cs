namespace OrganizaDinDin.Domain.Entities
{
    using Google.Cloud.Firestore;
    using OrganizaDinDin.Domain.Enums;

    [FirestoreData]
    public class Gasto
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public required string Descricao { get; set; }

        [FirestoreProperty]
        public required long Valor { get; set; }

        [FirestoreProperty]
        public Timestamp DataTimestamp { get; set; }

        [FirestoreProperty]
        public required ETipoGasto Tipo { get; set; }

        public virtual DateTime Data
        {
            get => DataTimestamp.ToDateTime();
            set => DataTimestamp = Timestamp.FromDateTime(value.ToUniversalTime());
        }
    }

}
