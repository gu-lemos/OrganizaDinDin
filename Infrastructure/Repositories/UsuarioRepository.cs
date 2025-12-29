using Google.Cloud.Firestore;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "usuarios";

        public UsuarioRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            var collection = _firestoreDb.Collection(CollectionName);
            var query = collection.WhereEqualTo("Email", email);
            var snapshot = await query.GetSnapshotAsync();

            if (!snapshot.Any())
                return null;

            return snapshot.Documents.First().ConvertTo<Usuario>();
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            var collection = _firestoreDb.Collection(CollectionName);
            var docRef = await collection.AddAsync(usuario);
            usuario.Id = docRef.Id;
            return usuario;
        }
    }
}
