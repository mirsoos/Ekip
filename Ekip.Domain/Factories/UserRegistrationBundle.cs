using Ekip.Domain.Entities.Gamification;
using Ekip.Domain.Entities.User;

namespace Ekip.Domain.Factories;

public readonly record struct UserRegistrationBundle(
    UserAccount Account,
    UserProfile Profile,
    UserGamificationSummary Summary);