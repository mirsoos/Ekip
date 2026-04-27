using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;
using Ekip.Infrastructure.Persistence.PostgreSql.Contexts;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class UserEkipUpdaterService : IUserEkipUpdaterService
    {
        private readonly PostgresDbContext _postgresDb;
        public UserEkipUpdaterService(PostgresDbContext postgresDb)
        {
            _postgresDb = postgresDb;
        }
        public Task AddAcceptedMemberAsync(Guid requestRef, EkipMember member, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task AddEkipAsync(UserEkipReadModel ekip, CancellationToken cancellationToken)
        {
            await _postgresDb.userEkipReads.AddAsync(ekip,cancellationToken);
            await _postgresDb.SaveChangesAsync(cancellationToken);
        }

        public Task AddPendingAssignmentAsync(Guid requestRef, PendingAssignmentInfo pending, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEkipAsync(Guid requestRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAcceptedMemberAsync(Guid requestRef, Guid profileRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemovePendingAssignmentAsync(Guid requestRef, Guid assignmentRef, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCurrentMembersCountAsync(Guid requestRef, int currentCount, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStatusAsync(Guid requestRef, RequestStatus newStatus, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
