namespace OrganizaDinDin.Domain.Entities
{
    using Google.Cloud.Firestore;

    [FirestoreData]
    public class CofrinhoMeta
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public long Valor { get; set; }
    }
}
