using Google.Cloud.Firestore;

namespace OrganizaDinDin.Firebase
{
    public static class FirebaseConfig
    {
        public static FirestoreDb CreateFirestoreDb(string projectId)
        {
            return FirestoreDb.Create(projectId);
        }
    }
}
