using System.Security.Cryptography;
using System.Text;
using Ekip.Application.Interfaces;
using Konscious.Security.Cryptography;

namespace Ekip.Infrastructure.Security.Password;

internal sealed class Argon2PasswordHasher : IPasswordHasher
{
    private const int SaltSizeBytes = 16;
    private const int HashSizeBytes = 32;
    private const int MemorySizeKiB = 19_456; // OWASP Argon2id baseline (~19 MiB)
    private const int Iterations = 2;
    private const int DegreeOfParallelism = 1;

    private static readonly SemaphoreSlim ConcurrencyLimiter = new(
        Math.Max(2, Environment.ProcessorCount),
        Math.Max(2, Environment.ProcessorCount));

    public async Task<string> HashAsync(string password, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        var hash = await ComputeHashAsync(
            password,
            salt,
            MemorySizeKiB,
            Iterations,
            DegreeOfParallelism,
            HashSizeBytes,
            cancellationToken);

        return $"$argon2id$v=19$m={MemorySizeKiB},t={Iterations},p={DegreeOfParallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public async Task<bool> VerifyAsync(
        string password,
        string passwordHash,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        if (!TryParsePhcHash(passwordHash, out var memoryKiB, out var iterations, out var parallelism, out var salt, out var expectedHash))
            return false;

        var actualHash = await ComputeHashAsync(
            password,
            salt,
            memoryKiB,
            iterations,
            parallelism,
            expectedHash.Length,
            cancellationToken);

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }

    private static async Task<byte[]> ComputeHashAsync(
        string password,
        byte[] salt,
        int memorySizeKiB,
        int iterations,
        int degreeOfParallelism,
        int outputLengthBytes,
        CancellationToken cancellationToken)
    {
        await ConcurrencyLimiter.WaitAsync(cancellationToken);
        try
        {
            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
                {
                    Salt = salt,
                    MemorySize = memorySizeKiB,
                    Iterations = iterations,
                    DegreeOfParallelism = degreeOfParallelism
                };

                return argon2.GetBytes(outputLengthBytes);
            }, cancellationToken);
        }
        finally
        {
            ConcurrencyLimiter.Release();
        }
    }

    private static bool TryParsePhcHash(
        string phcHash,
        out int memoryKiB,
        out int iterations,
        out int parallelism,
        out byte[] salt,
        out byte[] hash)
    {
        memoryKiB = 0;
        iterations = 0;
        parallelism = 0;
        salt = [];
        hash = [];

        // Format: $argon2id$v=19$m=19456,t=2,p=1$<salt>$<hash>
        var parts = phcHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 5 || parts[0] != "argon2id" || parts[1] != "v=19")
            return false;

        foreach (var param in parts[2].Split(','))
        {
            if (param.StartsWith("m=", StringComparison.Ordinal) && int.TryParse(param[2..], out var m))
                memoryKiB = m;
            else if (param.StartsWith("t=", StringComparison.Ordinal) && int.TryParse(param[2..], out var t))
                iterations = t;
            else if (param.StartsWith("p=", StringComparison.Ordinal) && int.TryParse(param[2..], out var p))
                parallelism = p;
        }

        if (memoryKiB <= 0 || iterations <= 0 || parallelism <= 0)
            return false;

        try
        {
            salt = Convert.FromBase64String(parts[3]);
            hash = Convert.FromBase64String(parts[4]);
            return salt.Length > 0 && hash.Length > 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}