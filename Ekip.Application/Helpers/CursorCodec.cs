using System.Buffers.Text;
using System.Text.Json;

namespace Ekip.Application.Helpers;

public static class CursorCodec
{
    private const int MaxCompositeCursorLength = 512;

    public static string EncodeId(Guid id) => id.ToString("D");

    public static bool TryDecodeId(string? cursor, out Guid? id)
    {
        id = null;

        if (string.IsNullOrWhiteSpace(cursor))
            return true;

        if (Guid.TryParseExact(cursor, "D", out var parsed) && parsed != Guid.Empty)
        {
            id = parsed;
            return true;
        }

        return false;
    }

    public static string EncodeComposite<T>(T value) where T : struct =>
        Base64Url.EncodeToString(JsonSerializer.SerializeToUtf8Bytes(value));

    public static bool TryDecodeComposite<T>(string? cursor, out T? value) where T : struct
    {
        value = null;

        if (string.IsNullOrWhiteSpace(cursor))
            return true;

        if (cursor.Length > MaxCompositeCursorLength)
            return false;

        Span<byte> buffer = stackalloc byte[Base64Url.GetMaxDecodedLength(cursor.Length)];
        if (!Base64Url.TryDecodeFromChars(cursor, buffer, out var bytesWritten))
            return false;

        try
        {
            value = JsonSerializer.Deserialize<T?>(buffer[..bytesWritten]);
            return value.HasValue;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}