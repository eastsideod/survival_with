using System.Collections.Concurrent;

namespace AspNetCoreStudy.Service;

public class SessionService
{
    private ILogger<SessionService> logger;

    // TODO(chj0816): redis로 변경
    private ConcurrentDictionary<string, string> sessionIdMap = new();

    public SessionService(ILogger<SessionService> logger)
    {
        this.logger = logger;
    }

    /// <returns>
    /// newSessionId
    /// </returns>
    public string Upsert(string accountId)
    {
        string sessionId = Guid.NewGuid().ToString();
        this.sessionIdMap[accountId] = sessionId;
        return sessionId;
    }

    public void Remove(string accountId)
    {
        this.sessionIdMap.TryRemove(accountId, out _);
    }

    public bool Verify(string accountId, string sessionId)
    {
        if (this.sessionIdMap.TryGetValue(accountId,
                                          out string cachedSessionId) == false)
        {
            return false;
        }

        return cachedSessionId == sessionId;
    }
}
