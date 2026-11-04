using Google.Cloud.Firestore;
using OrganizaDinDin.Domain.Entities;
using OrganizaDinDin.Domain.Interfaces;

namespace OrganizaDinDin.Infrastructure.Repositories
{
    public class CompromissoRepository(FirestoreDb firestoreDb) : ICompromissoRepository
    {
        private readonly FirestoreDb _firestoreDb = firestoreDb;
        private const string CollectionName = "compromissos";

        public async Task<List<Compromisso>> GetAllAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return [.. snapshot.Documents.Select(doc => doc.ConvertTo<Compromisso>()).OrderBy(c => c.Descricao)];
        }

        public async Task<Compromisso?> GetByIdAsync(string id)
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).Document(id).GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Compromisso>() : null;
        }

        public async Task<Compromisso> CreateAsync(Compromisso compromisso)
        {
            var docRef = await _firestoreDb.Collection(CollectionName).AddAsync(compromisso);
            compromisso.Id = docRef.Id;
            return compromisso;
        }

        public async Task<Compromisso> UpdateAsync(Compromisso compromisso)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(compromisso.Id);
            await docRef.SetAsync(compromisso, SetOptions.Overwrite);
            return compromisso;
        }

        public async Task DeleteAsync(string id)
        {
            await _firestoreDb.Collection(CollectionName).Document(id).DeleteAsync();
        }
    }
}
