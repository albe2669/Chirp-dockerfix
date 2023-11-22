using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface IUserRepository<User>
{
    void InsertUser(User entity);
    void DeleteUser(User entity);
    IQueryable<User> SearchFor(Expression<Func<User, bool>> predicate);

    Task<User?> GetUserById(int id);

    Task<User?> GetUserByName(string name);

    Task<User?> GetUserByEmail(string email);

    Task<int> GetUserIDByName(string name);

    Task CreateUser(string name, string? email = null);

    //follower
    Task FollowUser(FollowDTO followDTO);
    Task<bool> IsFollowing(int followerId, int followingId);
    Task UnfollowUser(FollowDTO unfollowDTO);
    //Task<List<User>> GetFollowedUsers(int userId);
}