using Ekip.Application.Contracts.Events;
using Ekip.Application.Interfaces;
using Ekip.Domain.Entities.ReadModels;
using MassTransit;

namespace Ekip.Application.Features.Request.Consumers
{
    public class UserEkipCreatedUpdaterConsumer : IConsumer<UserEkipCreatedUpdaterEvent>
    {
        private readonly IUserEkipUpdaterService _ekipUpdater;
        public UserEkipCreatedUpdaterConsumer(IUserEkipUpdaterService ekipUpdater)
        {
            _ekipUpdater = ekipUpdater;
        }
        public async Task Consume(ConsumeContext<UserEkipCreatedUpdaterEvent> context)
        {
            var message = context.Message;

            var toPostgre = new UserEkipReadModel
            {
                AcceptedMembers = message.AcceptedMembers,
                CreateDate = message.CreateDate,
                CreatorAvatar= message.CreatorAvatar,
                CreatorName= message.CreatorName,
                CreatorRef= message.CreatorRef,
                CurrentMembersCount= message.CurrentMembersCount,
                Description= message.Description,
                EkipTitle= message.EkipTitle,
                IsAutoAccept= message.IsAutoAccept,
                IsDeleted= message.IsDeleted,
                IsRepeatable= message.IsRepeatable,
                LastUpdated= message.LastUpdated,
                MaximumAge = message.MaximumAge,
                MinimumAge = message.MinimumAge,
                MaximumRequiredMembers= message.MaximumRequiredMembers,
                MemberType= message.MemberType,
                MinimumScore= message.MinimumScore,
                PendingAssignments= message.PendingAssignments,
                RepeatType= message.RepeatType,
                RequestDateTime= message.RequestDateTime,
                RequestForbidDateTime= message.RequestForbidDateTime,
                RequestRef= message.RequestRef,
                RequestType= message.RequestType,
                RequiredLevel= message.RequiredLevel,
                RequiredMembers= message.RequiredMembers,
                Status= message.Status,
                Tags = message.Tags,
                TargetGender = message.TargetGender
            };

            await _ekipUpdater.AddEkipAsync(toPostgre,context.CancellationToken);
        }
    }
}
