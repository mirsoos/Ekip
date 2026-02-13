using Ekip.Application.DTOs.Request;
using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Enums.Requests.Enums;
using Ekip.Infrastructure.Persistence.PostgreSql.Contexts;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ekip.Infrastructure.Repositories.Implementations
{
    public class ProfileReadRepository : IProfileReadRepository
    {
        private readonly PostgresDbContext _postgreDb;
        public ProfileReadRepository(PostgresDbContext postgresDb)
        {
            _postgreDb = postgresDb;
        }
        public async Task AddProfileAsync(ProfileReadModel profile, CancellationToken cancellationToken)
        {
            await _postgreDb.ProfileReads.AddAsync(profile,cancellationToken);
            await _postgreDb.SaveChangesAsync(cancellationToken);
        }

        public async Task<ProfileDto> GetProfileByIdAsync(Guid profileRef, CancellationToken cancellationToken)
        {
            var profile = await _postgreDb.ProfileReads.AsNoTracking().Where(x=>x.Id == profileRef).Select(s=> new ProfileDto
            {
                FirstName = s.User.FirstName,
                LastName = s.User.LastName,
                Gender = s.User.Gender,
                Experience = s.Experience,
                Score = s.Score,
                UserName = s.User.UserName,
                Email = s.User.Email,
                AvatarUrl = s.AvatarUrl,
                JoinRequests = new List<JoinRequestDto>(),
                MyEkips = new List<MyEkipDto>(),
                Requests = new List<NewRequestDto>(),
                Age = s.User.Age,

            }).FirstOrDefaultAsync(cancellationToken);

            return profile;
        }

        public async Task<ProfileDto?> GetProfileDetailsByIdAsync(Guid profileRef, CancellationToken cancellationToken)
        {
            var rawData = await _postgreDb.ProfileReads
                .AsNoTracking()
                .Where(x => x.Id == profileRef)
                .Select(s => new
                {
                    User = s.User,
                    s.Experience,
                    s.Score,
                    s.AvatarUrl,

                    MyCreatedRequests = s.Requests.Select(r => new
                    {
                        r.Id,
                        r.Title,
                        r.Description,
                        r.CreatorRef,
                        r.IsAutoAccept,
                        r.IsRepeatable,
                        r.MaximumRequiredAssignmnets,
                        r.MemberType,
                        r.RepeatType,
                        r.CreateDate,
                        r.RequestDateTime,
                        r.RequiredMembers,
                        r.RequestType,
                        r.RequestForbidDateTime,
                        TagsString = r.Tags,
                        FiltersJson = r.RequestFilters
                    }).ToList(),

                    MyAssignments = s.RequestAssignments.Select(a => new
                    {
                        RequestRef = a.RequestRef,
                        RequestTitle = a.Requests.Title,
                        MyStatus = a.Status,
                        RequestType = a.Requests.RequestType,
                        CreatorName = a.Requests.Creator.User.FirstName + " " + a.Requests.Creator.User.LastName,
                        StartDateTime = a.Requests.RequestDateTime,

                        EkipMembers = a.Requests.Assignments.Select(Ekip => new
                        {
                            ProfileId = Ekip.SenderProfile.Id,
                            FullName = Ekip.SenderProfile.User.FirstName + " " + Ekip.SenderProfile.User.LastName,
                            AvatarUrl = Ekip.SenderProfile.AvatarUrl,
                            Status = Ekip.Status
                        }).ToList()

                    }).ToList()

                }).FirstOrDefaultAsync(cancellationToken);

            if (rawData == null) return null;

            return new ProfileDto
            {
                FirstName = rawData.User.FirstName,
                LastName = rawData.User.LastName,
                Gender = rawData.User.Gender,
                Experience = rawData.Experience,
                Score = rawData.Score,
                UserName = rawData.User.UserName,
                Email = rawData.User.Email,
                AvatarUrl = rawData.AvatarUrl,

                Requests = rawData.MyCreatedRequests.Select(r => new NewRequestDto
                {
                    RequestRef = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Creator = r.CreatorRef,
                    IsAutoAccept = r.IsAutoAccept,
                    IsRepeatable = r.IsRepeatable,
                    MaximumRequiredAssignmnets = r.MaximumRequiredAssignmnets,
                    MemberType = r.MemberType,
                    RepeatType = r.RepeatType,
                    RequestCreateDateTime = r.CreateDate,
                    RequestDateTime = r.RequestDateTime,
                    RequiredMembers = r.RequiredMembers,
                    RequestType = r.RequestType,
                    RequestForbidDateTime = r.RequestForbidDateTime,
                    Tags = !string.IsNullOrEmpty(r.TagsString) ? r.TagsString.Split(',', StringSplitOptions.RemoveEmptyEntries) : new string[0],
                    RequestFilters = !string.IsNullOrEmpty(r.FiltersJson) ? JsonSerializer.Deserialize<RequestFilterDto[]>(r.FiltersJson) : new RequestFilterDto[0],
                }).ToList(),

                JoinRequests = rawData.MyAssignments
                    .Where(a => a.MyStatus == AssignmentStatus.Pending)
                    .Select(a => new JoinRequestDto
                    {
                        RequestRef = a.RequestRef,
                        
                    }).ToList(),

                MyEkips = rawData.MyAssignments
                    .Where(a => a.MyStatus == AssignmentStatus.Accepted)
                    .Select(a => new MyEkipDto
                    {
                        RequestRef = a.RequestRef,
                        Title = a.RequestTitle,
                        Status = a.MyStatus.ToString(),
                        Creator = a.CreatorName,
                        StartEventDateTime = a.StartDateTime,

                        Members = a.EkipMembers.Select(tm => new AssignmentMemberDto
                        {
                            ProfileRef = tm.ProfileId,
                            FullName = tm.FullName,
                            AvatarUrl = tm.AvatarUrl,
                            Status = tm.Status
                        }).ToList()
                    }).ToList()
            };
        }

        public async Task UpdateAvatarAsync(Guid profileRef, string avatarUrl, CancellationToken cancellationToken)
        {
            var profile = await _postgreDb.ProfileReads.Where(x => x.Id == profileRef).ExecuteUpdateAsync(setters => 
            setters.SetProperty(p=>p.AvatarUrl, avatarUrl),cancellationToken);
        }
    }
}
