using Google.Cloud.Firestore;

namespace OrganizaDinDin.Models
{
    [FirestoreData]
    public class Usuario
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public required string Email { get; set; }

        [FirestoreProperty]
        public required string SenhaHash { get; set; }

        [FirestoreProperty]
        public required string Nome { get; set; }

        [FirestoreProperty]
        public bool Ativo { get; set; } = true;

        [FirestoreProperty]
        public string Role { get; set; } = "User"; // User, Admin
    }
}
