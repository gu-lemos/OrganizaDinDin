using Google.Cloud.Firestore;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Infrastructure.Repositories
{
    public class CofrinhoRepository(FirestoreDb firestoreDb) : ICofrinhoRepository
    {
        private readonly FirestoreDb _firestoreDb = firestoreDb;
        private const string CollectionName = "cofrinho";
        private const string MetaCollectionName = "cofrinho_config";
        private const string MetaDocumentId = "meta";

        public async Task<List<CofrinhoTransacao>> GetAllAsync()
        {
            var collection = _firestoreDb.Collection(CollectionName);
            var snapshot = await collection.GetSnapshotAsync();
            return [.. snapshot.Documents.Select(doc => doc.ConvertTo<CofrinhoTransacao>()).OrderByDescending(doc => doc.Data)];
        }

        public async Task<List<CofrinhoTransacao>> GetFilteredAsync(List<int>? tipos, string? usuarioId, DateTime? dataInicio, DateTime? dataFim)
        {
            Query query = _firestoreDb.Collection(CollectionName);

            if (tipos != null && tipos.Count != 0)
                query = query.WhereIn("Tipo", tipos);

            if (dataInicio.HasValue)
            {
                var dataInicioUtc = DateTime.SpecifyKind(dataInicio.Value.Date, DateTimeKind.Utc);
                query = query.WhereGreaterThanOrEqualTo("DataTimestamp", Timestamp.FromDateTime(dataInicioUtc));
            }

            if (dataFim.HasValue)
            {
                var dataFimUtc = DateTime.SpecifyKind(dataFim.Value.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc);
                query = query.WhereLessThanOrEqualTo("DataTimestamp", Timestamp.FromDateTime(dataFimUtc));
            }

            var snapshot = await query.GetSnapshotAsync();
            var transacoes = snapshot.Documents.Select(doc => doc.ConvertTo<CofrinhoTransacao>()).ToList();

            if (!string.IsNullOrEmpty(usuarioId))
                transacoes = transacoes.Where(t => t.UsuarioId == usuarioId).ToList();

            return [.. transacoes.OrderByDescending(t => t.Data)];
        }

        public async Task<CofrinhoTransacao> CreateAsync(CofrinhoTransacao transacao)
        {
            var collection = _firestoreDb.Collection(CollectionName);
            var docRef = await collection.AddAsync(transacao);
            transacao.Id = docRef.Id;
            return transacao;
        }

        public async Task<CofrinhoTransacao> UpdateAsync(string id, CofrinhoTransacao transacao)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.SetAsync(transacao);
            transacao.Id = id;
            return transacao;
        }

        public async Task DeleteAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }

        public async Task<long?> GetMetaAsync()
        {
            var docRef = _firestoreDb.Collection(MetaCollectionName).Document(MetaDocumentId);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return null;
            var meta = snapshot.ConvertTo<CofrinhoMeta>();
            return meta.Valor;
        }

        public async Task SetMetaAsync(long valor)
        {
            var docRef = _firestoreDb.Collection(MetaCollectionName).Document(MetaDocumentId);
            var meta = new CofrinhoMeta { Valor = valor };
            await docRef.SetAsync(meta);
        }
    }
}
