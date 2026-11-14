using System.Security.Cryptography;
using Ekip.Application.Interfaces;

namespace Ekip.Infrastructure.Security.ReferralCode;

internal sealed class ReferralCodeGenerator : IReferralCodeGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int CodeLength = 8;

    public string Generate() => RandomNumberGenerator.GetString(Alphabet, CodeLength);
}