
namespace Ekip.Application.Contracts.Events
{
    record class ProfileAvatarUpdatedEvent
    {
        public Guid UserRef { get; set; }
        public string AvatarUrl { get; set; }
    }
}
