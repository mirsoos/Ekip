using Ekip.Application.DTOs.Request;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Infrastructure.Persistence.PostgreSql.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class RequestReadRepository : IRequestReadRepository
    {
        private readonly PostgresDbContext _postgreDb;
        public RequestReadRepository(PostgresDbContext postgresDb)
        {
            _postgreDb = postgresDb;
        }
        public async Task<RequestReadModel> AddRequestAsync(RequestReadModel requestReadModel, CancellationToken cancellationToken)
        {
            await _postgreDb.RequestReads.AddAsync(requestReadModel, cancellationToken);
            await _postgreDb.SaveChangesAsync(cancellationToken);
            return requestReadModel;
        }

        public async Task<RequestDetailsDto?> GetRequestByIdAsync(Guid requestRef, CancellationToken cancellationToken)
        {

            var request = await _postgreDb.RequestReads
                .AsNoTracking()
                .Where(r => r.Id == requestRef)
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.Description,
                    s.RequiredMembers,
                    s.MaximumRequiredAssignmnets,
                    s.Status,
                    s.CreateDate,
                    s.RequestDateTime,
                    s.RequestForbidDateTime,
                    s.RequestType,
                    s.MemberType,
                    s.IsAutoAccept,
                    s.IsRepeatable,
                    s.RepeatType,

                    TagsString = s.Tags,
                    RequestFiltersJson = s.RequestFilters,

                    Creator = new RequestCreatorDto
                    {
                        ProfileId = s.CreatorRef,
                        FirstName = s.Creator.User.FirstName,
                        LastName = s.Creator.User.LastName,
                        UserName = s.Creator.User.UserName,
                        AvatarUrl = s.Creator.AvatarUrl
                    },

                    Members = s.Assignments.Select(m => new AssignmentMemberDto
                    {
                        ProfileRef = m.SenderProfile.Id,
                        UserName = m.SenderProfile.User.UserName,
                        AvatarUrl = m.SenderProfile.AvatarUrl,
                        Status = m.Status,
                        Description = m.Description
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (request == null) return null;

            return new RequestDetailsDto
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Creator = request.Creator,
                RequiredMembers = request.RequiredMembers,
                MaximumRequiredMembers = request.MaximumRequiredAssignmnets,
                Status = request.Status,
                Members = request.Members,
                RequestCreateDate = request.CreateDate,
                RequestDateTime = request.RequestDateTime,
                RequestForbidDateTime = request.RequestForbidDateTime,
                RequestType = request.RequestType,
                MemberType = request.MemberType,
                IsAutoAccept = request.IsAutoAccept,
                IsRepeatable = request.IsRepeatable,
                RepeatType = request.RepeatType,
                NextRepeatDate = request.RepeatType != RequestRepeatType.None ? NextRepeatDate(request.CreateDate, request.RepeatType.Value) : null,
                Tags = !string.IsNullOrEmpty(request.TagsString)
                    ? request.TagsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    : new string[0],
                RequestFilters = !string.IsNullOrEmpty(request.RequestFiltersJson)
                    ? JsonSerializer.Deserialize<List<RequestFilterDto>>(request.RequestFiltersJson)
                    : new List<RequestFilterDto>(),
                CanAssignRequest = CanAssign(request.Status, request.Members.Count(m => m.Status == AssignmentStatus.Accepted), request.MaximumRequiredAssignmnets ?? request.RequiredMembers, request.RequestForbidDateTime)
            };
        }

        bool CanAssign(RequestStatus status, int currentMemberCount, int capacity, DateTime forbidTime)
        {
            bool isOpen = status == RequestStatus.Open || status == RequestStatus.InProgress;
            bool hasSpace = currentMemberCount < capacity;
            bool isValidTime = DateTime.UtcNow < forbidTime;

            return isOpen & hasSpace & isValidTime;
        }

        DateTime? NextRepeatDate(DateTime createDate, RequestRepeatType repeatType)
        {
            var repeatDays = (int)repeatType;
            if (repeatDays <= 0) return null;

            DateTime nextDate = createDate;
            while (nextDate < DateTime.UtcNow)
            {
                nextDate = nextDate.AddDays(repeatDays);
            }

            return nextDate;
        }

        public async Task AddAssignmentAsync(RequestAssignmentReadModel assignment, CancellationToken cancellationToken)
        {
            await _postgreDb.RequestAssignmentReads.AddAsync(assignment, cancellationToken);
            await _postgreDb.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Guid requestRef, RequestStatus status, CancellationToken cancellationToken)
        {
            await _postgreDb.RequestReads.Where(x => x.Id == requestRef).ExecuteUpdateAsync(r=> r.SetProperty(x=>x.Status,status),cancellationToken);
        }
    }
}
