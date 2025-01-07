namespace Domain.Services;

public interface IUserService
{
    Task<Guid> CreateUserAsync(string name);
    
    Task<Guid> SetUserName(Guid userId, string name);
    
    Task<bool> FindUser(Guid userId);
}