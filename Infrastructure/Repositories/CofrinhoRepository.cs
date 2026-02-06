using Google.Cloud.Firestore;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Infrastructure.Repositories
{
    public class CofrinhoRepository(FirestoreDb firestoreDb) : ICofrinhoRepository
    {
        private readonly FirestoreDb _firestoreDb = firestoreDb;
        private const string CollectionName = "cofrinho";

        public async Task<List<CofrinhoTransacao>> GetAllAsync()
        {
            var collection = _firestoreDb.Collection(CollectionName);
            var snapshot = await collection.GetSnapshotAsync();
            return [.. snapshot.Documents.Select(doc => doc.ConvertTo<CofrinhoTransacao>()).OrderByDescending(doc => doc.Data)];
        }

        public async Task<CofrinhoTransacao> CreateAsync(CofrinhoTransacao transacao)
        {
            var collection = _firestoreDb.Collection(CollectionName);
            var docRef = await collection.AddAsync(transacao);
            transacao.Id = docRef.Id;
            return transacao;
        }
    }
}
