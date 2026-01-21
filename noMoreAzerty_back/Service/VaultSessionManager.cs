using System.Collections.Concurrent;

namespace noMoreAzerty_back.Services
{
    /// <summary>
    /// Gestionnaire de sessions pour l'architecture Split-Knowledge
    /// Stocke KEY_STORAGE (jamais KEY_DERIVATION) pour chaque utilisateur/coffre
    /// </summary>
    public class VaultSessionManager
    {
        // Instance unique (Singleton) thread-safe
        private static readonly Lazy<VaultSessionManager> _instance =
            new Lazy<VaultSessionManager>(() => new VaultSessionManager());

        public static VaultSessionManager Instance => _instance.Value;

        // Stocke les sessions par utilisateur et par coffre
        // Clé : (UserId, VaultId)
        // Valeur : Session avec KEY_STORAGE, timestamp et IP
        private readonly ConcurrentDictionary<(Guid userId, Guid vaultId), VaultSession> _sessions
            = new ConcurrentDictionary<(Guid, Guid), VaultSession>();

        private VaultSessionManager() { }

        /// <summary>
        /// Crée ou met à jour une session avec KEY_STORAGE
        /// </summary>
        public void StoreKeyStorage(Guid userId, Guid vaultId, string keyStorage, string ip)
        {
            _sessions[(userId, vaultId)] = new VaultSession
            {
                KeyStorage = keyStorage,
                LastAccessed = DateTime.UtcNow,
                IpAddress = ip
            };
        }

        /// <summary>
        /// Récupère KEY_STORAGE si session valide
        /// </summary>
        public string? GetKeyStorage(Guid userId, Guid vaultId, string ip, TimeSpan maxAge)
        {
            if (!_sessions.TryGetValue((userId, vaultId), out var session))
                return null;

            // Vérifier IP et expiration
            if (session.IpAddress != ip)
                return null;

            if ((DateTime.UtcNow - session.LastAccessed) > maxAge)
            {
                // Session expirée, la supprimer
                _sessions.TryRemove((userId, vaultId), out _);
                return null;
            }

            // Mettre à jour le timestamp (renouveler la session)
            session.LastAccessed = DateTime.UtcNow;

            return session.KeyStorage;
        }

        /// <summary>
        /// Vérifie si une session valide existe (sans retourner KEY_STORAGE)
        /// </summary>
        public bool HasValidSession(Guid userId, Guid vaultId, string ip, TimeSpan maxAge)
        {
            return GetKeyStorage(userId, vaultId, ip, maxAge) != null;
        }

        /// <summary>
        /// Supprime une session (lock vault)
        /// </summary>
        public void RemoveSession(Guid userId, Guid vaultId)
        {
            _sessions.TryRemove((userId, vaultId), out _);
        }

        /// <summary>
        /// Nettoie les sessions expirées (à appeler périodiquement)
        /// </summary>
        public void CleanupExpiredSessions(TimeSpan maxAge)
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _sessions
                .Where(kvp => (now - kvp.Value.LastAccessed) > maxAge)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _sessions.TryRemove(key, out _);
            }
        }
    }

    /// <summary>
    /// Représente une session de coffre avec KEY_STORAGE
    /// </summary>
    public class VaultSession
    {
        public string KeyStorage { get; set; } = null!;
        public DateTime LastAccessed { get; set; }
        public string IpAddress { get; set; } = null!;
    }
}