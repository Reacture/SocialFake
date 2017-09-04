using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SocialFake.Facade.ReadModel
{
    public class ReadModelFacade
    {
        private readonly Func<SocialFakeDbContext> _dbContextFactory;

        public ReadModelFacade(Func<SocialFakeDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<UserDto> FindUser(Guid userId)
        {
            using (SocialFakeDbContext db = _dbContextFactory.Invoke())
            {
                IQueryable<User> query = from u in db.Users
                                         where u.Id == userId
                                         select u;

                User entity = await query.SingleOrDefaultAsync();

                return entity?.AssembleDto();
            }
        }

        public async Task<bool> CorrelationExists(Guid correlationId)
        {
            using (SocialFakeDbContext db = _dbContextFactory.Invoke())
            {
                IQueryable<Correlation> query = from c in db.Correlations
                                                where c.Id == correlationId
                                                select c;

                return await query.AnyAsync();
            }
        }
    }
}
