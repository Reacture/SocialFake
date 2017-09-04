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
        IHandles<DisplayNamesChanged>
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
                    DisplayNamesJson = JsonConvert.SerializeObject(new DisplayNames())
                };

                db.Users.Add(user);
                db.Correlations.Add(new Correlation{ Id = envelope.MessageId });

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
                IQueryable<User> query = from u in db.Users
                                         where u.Id == domainEvent.SourceId
                                         select u;

                User user = await query.SingleOrDefaultAsync(cancellationToken);

                if (user == null)
                {
                    throw new InvalidOperationException($"Could not find user entity with id '{domainEvent.SourceId}'.");
                }

                user.DisplayNamesJson = JsonConvert.SerializeObject(new DisplayNames
                {
                    FirstName = domainEvent.FirstName,
                    MiddleName = domainEvent.MiddleName,
                    LastName = domainEvent.LastName
                });

                db.Correlations.Add(new Correlation { Id = envelope.MessageId });

                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
