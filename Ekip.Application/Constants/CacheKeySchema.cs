
namespace Ekip.Application.Constants
{
    public static class CacheKeySchema
    {
        public const string RequestRef = "req:";
        public const string UserRef = "uid:";
        public const string ChatRoomRef = "rom:";
        public const string UserRequests = "urq:";
        public const string UserAssignments = "asg:";
        public const string UserEkips = "ekp:";
        public const string UserPendingAssignment = "pnd:";

        public static string RequestKey(Guid requestRef) => $"{RequestRef}{requestRef}";
        public static string ProfileKey(Guid userRef) => $"{UserRef}{userRef}";
        public static string ChatRoomKey(Guid chatroomRef) => $"{ChatRoomRef}{chatroomRef}";
        public static string UserRequestsKey(Guid userRef) => $"{UserRequests}{userRef}";
        public static string UserAssignmentsKey(Guid userRef) => $"{UserAssignments}{userRef}";
        public static string UserEkipsKey(Guid userRef) => $"{UserEkips}{userRef}";
        public static string UserPendingAssignmentKey(Guid userRef) => $"{UserPendingAssignment}{userRef}";
    }
}
