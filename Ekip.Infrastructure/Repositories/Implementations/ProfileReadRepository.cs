using Ekip.Application.DTOs.User;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using Ekip.Domain.Enums.Identity.Enums;
using Ekip.Infrastructure.Persistence.PostgreSql.Contexts;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

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
                Age = s.User.Age,
            }).FirstOrDefaultAsync(cancellationToken);

            return profile;
        }

        public async Task<ProfileDto?> GetProfileDetailsByIdAsync(Guid profileRef, CancellationToken cancellationToken)
        {
            return await _postgreDb.ProfileReads
                .AsNoTracking()
                .Where(x => x.Id == profileRef)
                .Select(s => new ProfileDto
                {
                    ProfileRef = s.Id,
                    FirstName = s.User.FirstName,
                    LastName = s.User.LastName,
                    UserName = s.User.UserName,
                    Email = s.User.Email,
                    AvatarUrl = s.AvatarUrl,
                    Bio = s.Bio,
                    Age = s.User.Age,
                    Gender = s.User.Gender,
                    Experience = s.Experience,
                    Score = s.Score
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateAvatarAsync(Guid profileRef, string avatarUrl, CancellationToken cancellationToken)
        {
            var profile = await _postgreDb.ProfileReads.Where(x => x.Id == profileRef).ExecuteUpdateAsync(setters => 
            setters.SetProperty(p=>p.AvatarUrl, avatarUrl),cancellationToken);
        }

        public async Task UpdateFaceVerificationStatusAsync(Guid profileRef, VerificationLevel verificationLevel, CancellationToken cancellationToken)
        {
            var validStates = new[] { VerificationLevel.None, VerificationLevel.rejected };

            await _postgreDb.ProfileReads
                .Where(x => x.Id == profileRef && validStates.Contains(x.VerificationLevel))
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.VerificationLevel, verificationLevel),cancellationToken);
        }
    }
}
