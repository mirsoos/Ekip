using System.Globalization;
using Ekip.Api.Grpc.User;
using Ekip.Application.Features.User.Commands.RegisterUser;
using Ekip.Application.Features.User.Queries.GetUserProfile;
using Ekip.Domain.Enums.User;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace Ekip.Api.gRPC.Services;

public sealed class UserGrpcService(ISender sender) : UserGrpc.UserGrpcBase
{
    public override async Task<RegisterUserGrpcResponse> RegisterUser(
        RegisterUserGrpcRequest request,
        ServerCallContext context)
    {
        var idempotencyKey = Guid.TryParse(request.IdempotencyKey, out var parsedKey)
            ? parsedKey
            : Guid.Empty;

        var birthDate = DateOnly.TryParseExact(
            request.BirthDate,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedDate)
            ? parsedDate
            : default;

        Guid? cityId = null;
        if (request.HasCityId && !string.IsNullOrWhiteSpace(request.CityId))
        {
            cityId = Guid.TryParse(request.CityId, out var parsedCityId)
                ? parsedCityId
                : Guid.Empty;
        }

        var command = new RegisterUserCommand(
            IdempotencyKey: idempotencyKey,
            Username: request.Username,
            Password: request.Password,
            FirstName: request.FirstName,
            LastName: request.LastName,
            UserGender: (Gender)request.UserGender,
            BirthDate: birthDate,
            Email: request.HasEmail ? request.Email : null,
            PhoneNumber: request.HasPhoneNumber ? request.PhoneNumber : null,
            CityId: cityId,
            ReferralCode: request.HasReferralCode ? request.ReferralCode : null);

        var result = await sender.Send(command, context.CancellationToken);
        var userId = result.GetValueOrThrowRpc();

        return new RegisterUserGrpcResponse
        {
            UserId = userId.ToString("D")
        };
    }

    public override async Task<UserProfileGrpcResponse> GetUserProfile(
        GetUserProfileGrpcRequest request,
        ServerCallContext context)
    {
        var userId = Guid.TryParse(request.UserId, out var parsedUserId)
            ? parsedUserId
            : Guid.Empty;

        var result = await sender.Send(new GetUserProfileQuery(userId), context.CancellationToken);
        var dto = result.GetValueOrThrowRpc();

        var response = new UserProfileGrpcResponse
        {
            UserId = dto.UserId.ToString("D"),
            Username = dto.Username,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserGender = (int)dto.UserGender,
            BirthDate = dto.BirthDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            IsFaceVerified = dto.IsFaceVerified,
            IsNationalIdVerified = dto.IsNationalIdVerified,
            IsSearchable = dto.IsSearchable,
            ShowContacts = dto.ShowContacts,
            ShowLastSeen = dto.ShowLastSeen,
            TotalXp = dto.TotalXp,
            CurrentLevel = dto.CurrentLevel,
            NextLevelXp = dto.NextLevelXp,
            PublicScore = (double)dto.PublicScore,
            RatingsCount = dto.RatingsCount,
            CreatedAtUtc = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.CreatedAtUtc, DateTimeKind.Utc))
        };

        if (dto.CityId.HasValue)
            response.CityId = dto.CityId.Value.ToString("D");

        if (dto.AvatarAssetId.HasValue)
            response.AvatarAssetId = dto.AvatarAssetId.Value.ToString("D");

        if (dto.BannerAssetId.HasValue)
            response.BannerAssetId = dto.BannerAssetId.Value.ToString("D");

        if (dto.Bio is not null)
            response.Bio = dto.Bio;

        if (dto.JobTitle is not null)
            response.JobTitle = dto.JobTitle;

        if (dto.MbtiTypes.HasValue)
            response.MbtiTypes = (int)dto.MbtiTypes.Value;

        if (dto.MoodWidget is not null)
        {
            var mood = new MoodWidgetGrpcDto
            {
                CreatedAtUtc = Timestamp.FromDateTime(
                    DateTime.SpecifyKind(dto.MoodWidget.CreatedAtUtc, DateTimeKind.Utc))
            };

            if (dto.MoodWidget.MoodEmoji is not null)
                mood.MoodEmoji = dto.MoodWidget.MoodEmoji;

            if (dto.MoodWidget.MoodDescription is not null)
                mood.MoodDescription = dto.MoodWidget.MoodDescription;

            if (dto.MoodWidget.MoodColor is not null)
                mood.MoodColor = dto.MoodWidget.MoodColor;

            response.MoodWidget = mood;
        }

        if (dto.LastSeenAtUtc.HasValue)
        {
            response.LastSeenAtUtc = Timestamp.FromDateTime(
                DateTime.SpecifyKind(dto.LastSeenAtUtc.Value, DateTimeKind.Utc));
        }

        foreach (var link in dto.SocialLinks)
        {
            response.SocialLinks.Add(new SocialLinkGrpcDto
            {
                Platform = link.Platform,
                Url = link.Url,
                LinkedAtUtc = Timestamp.FromDateTime(DateTime.SpecifyKind(link.LinkedAtUtc, DateTimeKind.Utc))
            });
        }

        response.Interests.AddRange(dto.Interests);

        return response;
    }
}