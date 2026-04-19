
namespace Ekip.Infrastructure.Persistence.Redis
{
    public static class KeySchema
    {
        public const string Presence = "prs:";
        public const string UserRooms = "urm:";

        public static string PresenceKey(Guid userId) => $"{Presence}{userId}";
        public static string UserRoomsKey(Guid userId) => $"{UserRooms}{userId}";
    }
}
