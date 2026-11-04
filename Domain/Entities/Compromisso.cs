namespace OrganizaDinDin.Domain.Entities
{
    using Google.Cloud.Firestore;
    using OrganizaDinDin.Domain.Enums;

    [FirestoreData]
    public class Compromisso
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public required string Descricao { get; set; }

        [FirestoreProperty]
        public required long ValorTotal { get; set; }

        [FirestoreProperty]
        public int? TotalParcelas { get; set; }

        [FirestoreProperty]
        public required ETipoGasto Tipo { get; set; }

        [FirestoreProperty]
        public Timestamp? DataFimTimestamp { get; set; }

        public DateTime? DataFim
        {
            get => DataFimTimestamp?.ToDateTime();
            set => DataFimTimestamp = value.HasValue
                ? Timestamp.FromDateTime(DateTime.SpecifyKind(value.Value.Date, DateTimeKind.Utc))
                : null;
        }
    }
}
