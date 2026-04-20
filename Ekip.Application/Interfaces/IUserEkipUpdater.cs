using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Domain.ValueObjects;

namespace Ekip.Application.Interfaces
{
    public interface IUserEkipUpdater
    {
        Task AddOrUpdateEkipAsync(Guid requestRef , Guid creatorRef , string ekipTitle , RequestStatus status , RequestType requestType , DateTime createDate , DateTime eventDateTime , int requiredMembers , CancellationToken cancellationToken);

        Task UpdateCurrentMembersCountAsync(Guid requestRef , int currentCount , CancellationToken cancellationToken);

        Task UpdateStatusAsync(Guid requestRef , RequestStatus newStatus , CancellationToken cancellationToken);

        Task AddAcceptedMemberAsync(Guid requestRef , EkipMember member , CancellationToken cancellationToken);

        Task RemoveAcceptedMemberAsync(Guid requestRef , Guid profileRef , CancellationToken cancellationToken);

        Task AddPendingAssignmentAsync(Guid requestRef , PendingAssignmentInfo pending , CancellationToken cancellationToken);

        Task RemovePendingAssignmentAsync(Guid requestRef , Guid assignmentRef , CancellationToken cancellationToken);

        Task DeleteEkipAsync(Guid requestRef , CancellationToken cancellationToken);
    }
}