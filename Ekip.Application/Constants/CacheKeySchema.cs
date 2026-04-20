
namespace Ekip.Application.Constants
{
    public static class CacheKeySchema
    {
        public const string RequestRef = "req:";
        public const string ProfileRef = "prf:";
        public const string ChatRoomRef = "rom:";
        public const string UserRequests = "urq:";
        public const string UserAssignments = "asg:";
        public const string UserEkips = "ekp:";
        public const string UserPendingAssignment = "pnd:";

        public static string RequestKey(Guid requestRef) => $"{RequestRef}{requestRef}";
        public static string ProfileKey(Guid profileRef) => $"{ProfileRef}{profileRef}";
        public static string ChatRoomKey(Guid chatroomRef) => $"{ChatRoomRef}{chatroomRef}";
        public static string UserRequestsKey(Guid userRef) => $"{UserRequests}{userRef}";
        public static string UserAssignmentsKey(Guid profileRef) => $"{UserAssignments}{profileRef}";
        public static string UserEkipsKey(Guid profileRef) => $"{UserEkips}{profileRef}";
        public static string UserPendingAssignmentKey(Guid profileRef) => $"{UserPendingAssignment}{profileRef}";
    }
}
