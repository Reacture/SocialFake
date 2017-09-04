using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Khala.Messaging;
using Newtonsoft.Json;
using SocialFake.Identity.Events;

namespace SocialFake.Facade.ReadModel
{
    public class ReadModelGenerator :
        InterfaceAwareHandler,
        IHandles<UserCreated>,
        IHandles<DisplayNamesChanged>,
        IHandles<BioChanged>
    {
        private readonly Func<SocialFakeDbContext> _dbContextFactory;

        public ReadModelGenerator(Func<SocialFakeDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task Handle(Envelope<UserCreated> envelope, CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            UserCreated domainEvent = envelope.Message;

            using (SocialFakeDbContext db = _dbContextFactory.Invoke())
            {
                var user = new User
                {
                    Id = domainEvent.SourceId,
                    Username = domainEvent.Username,
                    DisplayNamesJson = JsonConvert.SerializeObject(new DisplayNames()),
                    Bio = string.Empty
                };

                db.Users.Add(user);

                if (envelope.CorrelationId.HasValue)
                {
                    db.Correlations.Add(new Correlation { Id = envelope.CorrelationId.Value });
                }

                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task Handle(Envelope<DisplayNamesChanged> envelope, CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            DisplayNamesChanged domainEvent = envelope.Message;

            using (SocialFakeDbContext db = _dbContextFactory.Invoke())
            {
                User user = await GetUser(domainEvent.SourceId, db, cancellationToken);

                user.DisplayNamesJson = JsonConvert.SerializeObject(new DisplayNames
                {
                    FirstName = domainEvent.FirstName,
                    MiddleName = domainEvent.MiddleName,
                    LastName = domainEvent.LastName
                });

                if (envelope.CorrelationId.HasValue)
                {
                    db.Correlations.Add(new Correlation { Id = envelope.CorrelationId.Value });
                }

                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task Handle(Envelope<BioChanged> envelope, CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            BioChanged domainEvent = envelope.Message;

            using (SocialFakeDbContext db = _dbContextFactory.Invoke())
            {
                User user = await GetUser(domainEvent.SourceId, db, cancellationToken);

                user.Bio = domainEvent.Bio;

                if (envelope.CorrelationId.HasValue)
                {
                    db.Correlations.Add(new Correlation { Id = envelope.CorrelationId.Value });
                }

                await db.SaveChangesAsync(cancellationToken);
            }
        }

        private static async Task<User> GetUser(Guid userId, SocialFakeDbContext db, CancellationToken cancellationToken)
        {
            IQueryable<User> query = from u in db.Users
                                     where u.Id == userId
                                     select u;
            User user = await query.SingleOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"Could not find user entity with id '{userId}'.");
            }

            return user;
        }
    }
}
