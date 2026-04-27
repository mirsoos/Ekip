using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Infrastructure.Persistence.PostgreSql.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class UserEkipReadRepository : IUserEkipReadRepository
    {
        private readonly PostgresDbContext _postgresDb;
        public UserEkipReadRepository(PostgresDbContext postgresDb)
        {
            _postgresDb = postgresDb;
        }
        public async Task<List<MyEkipDto>> GetEkipByProfileIdAsync(Guid profileRef, CancellationToken cancellationToken)
        {
            return await _postgresDb.userEkipReads.Where(x => x.CreatorRef == profileRef
            || x.AcceptedMembers.Any(a => a.ProfileRef == profileRef)
            || x.PendingAssignments.Any(p => p.ApplicantId == profileRef)
            ).Select(s => new MyEkipDto
            {
                PendingAssignments = s.PendingAssignments,
                AcceptedMembers = s.AcceptedMembers,
                CreateDate = s.CreateDate,
                CreatorAvatar = s.CreatorAvatar,
                CreatorName = s.CreatorName,
                CreatorRef = s.CreatorRef,
                CurrentMembersCount = s.CurrentMembersCount,
                Description = s.Description,
                IsAutoAccept = s.IsAutoAccept,
                IsCreator = s.CreatorRef == profileRef,
                IsDeleted = s.IsDeleted,
                IsRepeatable = s.IsRepeatable,
                MaximumAge = s.MaximumAge,
                MaximumRequiredMembers = s.MaximumRequiredMembers,
                MemberType = s.MemberType,
                MinimumAge = s.MinimumAge,
                MinimumScore = s.MinimumScore,
                MyAssignmentStatus = s.CreatorRef == profileRef ? AssignmentStatus.Accepted :
                s.AcceptedMembers.Any(a=>a.ProfileRef == profileRef) ? AssignmentStatus.Accepted : 
                s.PendingAssignments.Any(p=>p.ApplicantId == profileRef) ? AssignmentStatus.Pending : null,
                Status = s.Status,
                RepeatType = s.RepeatType,
                RequestDateTime = s.RequestDateTime,
                RequestForbidDateTime = s.RequestForbidDateTime,
                RequestRef = s.RequestRef,
                RequestType = s.RequestType,
                RequiredLevel = s.RequiredLevel,
                RequiredMembers = s.RequiredMembers,
                Tags = s.Tags != null ? string.Join(",", s.Tags) : null,
                TargetGender = s.TargetGender,
                Title = s.EkipTitle
            }).ToListAsync();
        }
    }
}
