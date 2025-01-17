using System.Linq.Expressions;

namespace Chirp.Core;

// Repository pattern from Remondo:
// https://web.archive.org/web/20150404154203/https://www.remondo.net/repository-pattern-example-csharp/

public interface IReactionRepository<Reaction>
{
    Task InsertReaction(Reaction entity);
    Task DeleteReaction(Reaction entity);
    Task UpdateReaction(Reaction entity);
    IQueryable<Reaction> SearchFor(Expression<Func<Reaction, bool>> predicate);
    Task<int> GetCheepsUpvoteCountsFromCheepID(int id);
    Task<int> GetCheepsDownvoteCountsFromCheepID(int id);
    Task ReactToCheep(ReactionDTO reactionDTO);
    Task<bool> checkUserReacted(int userid, int cheepid);
    Task<string> checkUserReactionType(int userid, int cheepid);
    Task deleteAllUserReactions(int userid);
    Task<List<Reaction>> GetReactionByUsersId(int userid);
}