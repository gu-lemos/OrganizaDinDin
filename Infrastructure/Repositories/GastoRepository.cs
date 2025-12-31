using Google.Cloud.Firestore;
using OrganizaDinDin.Domain.Interfaces;
using OrganizaDinDin.Domain.Entities;

namespace OrganizaDinDin.Infrastructure.Repositories
{
    public class GastoRepository(FirestoreDb firestoreDb) : IGastoRepository
    {
        private readonly FirestoreDb _firestoreDb = firestoreDb;
        private const string CollectionName = "gastos";

        public async Task<List<Gasto>> GetAllAsync()
        {
            var collection = _firestoreDb.Collection(CollectionName);
            var snapshot = await collection.GetSnapshotAsync();
            return [.. snapshot.Documents.Select(doc => doc.ConvertTo<Gasto>()).OrderByDescending(doc => doc.Data)];
        }

        public async Task<List<Gasto>> GetFilteredAsync(string? descricao, int? tipo, DateTime? dataInicio, DateTime? dataFim)
        {
            Query query = _firestoreDb.Collection(CollectionName);

            if (tipo.HasValue)
            {
                query = query.WhereEqualTo("Tipo", tipo.Value);
            }

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
            var gastos = snapshot.Documents.Select(doc => doc.ConvertTo<Gasto>()).ToList();

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                gastos = [.. gastos.Where(g => g.Descricao.Contains(descricao, StringComparison.OrdinalIgnoreCase))];
            }

            return [.. gastos.OrderByDescending(doc => doc.Data)];
        }

        public async Task<Gasto?> GetByIdAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<Gasto>();
        }

        public async Task<Gasto> CreateAsync(Gasto gasto)
        {
            var collection = _firestoreDb.Collection(CollectionName);
            var docRef = await collection.AddAsync(gasto);
            gasto.Id = docRef.Id;
            return gasto;
        }

        public async Task<Gasto> UpdateAsync(Gasto gasto)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(gasto.Id);
            await docRef.SetAsync(gasto, SetOptions.Overwrite);
            return gasto;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var docRef = _firestoreDb.Collection(CollectionName).Document(id);
                await docRef.DeleteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
