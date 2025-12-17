using System.Collections.Concurrent;

namespace noMoreAzerty_back.Services
{
    public class VaultSessionManager
    {
        // Singleton thread-safe
        private static readonly Lazy<VaultSessionManager> _instance =
            new Lazy<VaultSessionManager>(() => new VaultSessionManager());

        public static VaultSessionManager Instance => _instance.Value;

        // Clé : (UserId, VaultId), Valeur : (DateTime lastOpened, string IP)
        private readonly ConcurrentDictionary<(Guid userId, Guid vaultId), (DateTime lastOpened, string ip)> _sessions
            = new ConcurrentDictionary<(Guid, Guid), (DateTime, string)>();

        private VaultSessionManager() { }

        public bool HasRecentSession(Guid userId, Guid vaultId, string ip, TimeSpan maxAge)
        {
            if (_sessions.TryGetValue((userId, vaultId), out var session))
            {
                if (session.ip == ip && (DateTime.UtcNow - session.lastOpened) <= maxAge)
                    return true;
            }
            return false;
        }

        public void UpdateSession(Guid userId, Guid vaultId, string ip)
        {
            _sessions[(userId, vaultId)] = (DateTime.UtcNow, ip);
        }
    }
}
