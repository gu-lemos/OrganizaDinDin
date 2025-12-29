namespace OrganizaDinDin.Domain.Entities
{
    using OrganizaDinDin.Enum;
    using Google.Cloud.Firestore;

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

        public DateTime Data
        {
            get => DataTimestamp.ToDateTime();
            set => DataTimestamp = Timestamp.FromDateTime(value.ToUniversalTime());
        }
    }

}
