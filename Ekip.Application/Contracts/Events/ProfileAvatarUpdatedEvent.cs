
namespace Ekip.Application.Contracts.Events
{
    record class ProfileAvatarUpdatedEvent
    {
        public Guid ProfileRef { get; set; }
        public string AvatarUrl { get; set; }
    }
}
