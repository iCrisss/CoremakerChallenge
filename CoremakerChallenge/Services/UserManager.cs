using CoremakerChallenge.Models;

namespace CoremakerChallenge.Services;

public interface IUserManager
{
    void Create(AppUser user);
    AppUser? GetUser(string email);
    bool IsValidPassword(AppUser user, string password);
}

public class UserManager : IUserManager
{
    private readonly List<AppUser> _users;

    public UserManager()
    {
        _users = new List<AppUser>();
    }

    public void Create(AppUser user) =>_users.Add(user);
    public AppUser? GetUser(string email) => _users.FirstOrDefault(u => u.Email == email);
    public bool IsValidPassword(AppUser user, string password) => user.Password == password;
}
