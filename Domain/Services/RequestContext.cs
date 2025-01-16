namespace Domain.Services;

public class RequestContext
{
    private readonly Dictionary<string, string> _headers = new();

    public void AddHeader(string key, string value)
    {
        _headers[key] = value;
    }

    public Guid GetUserId()
    {
        return FindUserId() ?? throw new KeyNotFoundException();
    }

    public string GetUserName()
    {
        return FindUserName() ?? throw new KeyNotFoundException();
    }

    private Guid? FindUserId()
    {
        if (_headers.TryGetValue(RequestContextKeys.UserId, out var header)) return Guid.Parse(header);

        return null;
    }

    private string? FindUserName()
    {
        return _headers.GetValueOrDefault(RequestContextKeys.UserName);
    }
}