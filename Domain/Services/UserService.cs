namespace Domain.Services;

public class UserService : IUserService
{
    public async Task<Guid> CreateUserAsync(string name)
    {
        return Guid.NewGuid();
    }

    public Task<Guid> SetUserName(Guid userId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> FindUser(Guid userId)
    {
        return true;
    }
}