using Ekip.Domain.ValueObjects.User;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ekip.Persistence.Configurations.User;

public sealed class UsernameValueConverter()
    : ValueConverter<Username, string>(
        username => username.Value,
        value => Username.Rehydrate(value));

public sealed class EmailValueConverter()
    : ValueConverter<Email, string>(
        email => email.Value,
        value => Email.Rehydrate(value));

public sealed class PhoneNumberValueConverter()
    : ValueConverter<PhoneNumber, string>(
        phone => phone.Value,
        value => PhoneNumber.Rehydrate(value));