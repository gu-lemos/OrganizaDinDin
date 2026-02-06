namespace OrganizaDinDin.Domain.Entities
{
    using Google.Cloud.Firestore;
    using OrganizaDinDin.Domain.Enums;

    [FirestoreData]
    public class CofrinhoTransacao
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public required long Valor { get; set; }

        [FirestoreProperty]
        public Timestamp DataTimestamp { get; set; }

        [FirestoreProperty]
        public required string UsuarioId { get; set; }

        [FirestoreProperty]
        public required ETipoTransacaoCofrinho Tipo { get; set; }

        [FirestoreProperty]
        public string? Motivo { get; set; }

        public virtual DateTime Data
        {
            get => DataTimestamp.ToDateTime();
            set => DataTimestamp = Timestamp.FromDateTime(value.ToUniversalTime());
        }
    }
}
