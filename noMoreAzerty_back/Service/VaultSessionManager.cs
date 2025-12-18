using System.Collections.Concurrent;

namespace noMoreAzerty_back.Services
{
    public class VaultSessionManager
    {
        // Instance unique (Singleton) thread-safe
        private static readonly Lazy<VaultSessionManager> _instance =
            new Lazy<VaultSessionManager>(() => new VaultSessionManager());

        public static VaultSessionManager Instance => _instance.Value;

        // Stocke les sessions récentes par utilisateur et par coffre
        // Clé : (UserId, VaultId)
        // Valeur : (Date de dernière ouverture, IP)
        private readonly ConcurrentDictionary<(Guid userId, Guid vaultId), (DateTime lastOpened, string ip)> _sessions
            = new ConcurrentDictionary<(Guid, Guid), (DateTime, string)>();

        private VaultSessionManager() { }

        // Vérifie s'il existe une session récente valide pour ce coffre
        public bool HasRecentSession(Guid userId, Guid vaultId, string ip, TimeSpan maxAge)
        {
            // Tente de récupérer la session existante
            if (_sessions.TryGetValue((userId, vaultId), out var session))
            {
                // Vérifie que l'IP correspond et que la session n'est pas expirée
                if (session.ip == ip && (DateTime.UtcNow - session.lastOpened) <= maxAge)
                    return true;
            }
            return false;
        }

        // Met à jour ou crée une session pour ce coffre
        public void UpdateSession(Guid userId, Guid vaultId, string ip)
        {
            _sessions[(userId, vaultId)] = (DateTime.UtcNow, ip);
        }
    }
}
