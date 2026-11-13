using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pgvector;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Ekip.Persistence.Extensions;

public static class PropertyBuilderExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static PropertyBuilder<float[]> HasPgVectorConversion(
        this PropertyBuilder<float[]> builder,
        int dimensions)
    {
        var converter = new ValueConverter<float[], Vector>(
            v => new Vector(v),
            v => v.ToArray());

        var comparer = new ValueComparer<float[]>(
            (l, r) => VectorsEqual(l, r),
            v => ComputeVectorHashCode(v),
            v => CloneVector(v));

        builder.HasColumnType($"vector({dimensions})");
        builder.HasConversion(converter, comparer);

        return builder;
    }

    private static bool VectorsEqual(float[]? left, float[]? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.AsSpan().SequenceEqual(right);
    }

    private static int ComputeVectorHashCode(float[] value)
    {
        if (value is null) return 0;
        var hash = new HashCode();
        hash.AddBytes(MemoryMarshal.AsBytes(value.AsSpan()));
        return hash.ToHashCode();
    }

    private static float[] CloneVector(float[] value) =>
        value is null ? [] : (float[])value.Clone();

    public static PropertyBuilder<T> HasJsonbConversion<T>(this PropertyBuilder<T> builder)
    {
        var converter = new ValueConverter<T, string>(
            v => Serialize(v),
            v => Deserialize<T>(v));

        var comparer = new ValueComparer<T>(
            (l, r) => AreEqual(l, r),
            v => ComputeHashCode(v),
            v => CreateSnapshot(v));

        builder.HasColumnType("jsonb");
        builder.HasConversion(converter, comparer);

        return builder;
    }

    private static string Serialize<T>(T value) =>
        JsonSerializer.Serialize(value, JsonOptions);

    private static T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, JsonOptions)!;

    private static bool AreEqual<T>(T? left, T? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        if (EqualityComparer<T>.Default.Equals(left, right)) return true;

        return string.Equals(
            JsonSerializer.Serialize(left, JsonOptions),
            JsonSerializer.Serialize(right, JsonOptions),
            StringComparison.Ordinal);
    }

    private static int ComputeHashCode<T>(T value)
    {
        if (value is null) return 0;
        return JsonSerializer.Serialize(value, JsonOptions).GetHashCode(StringComparison.Ordinal);
    }

    private static T CreateSnapshot<T>(T value)
    {
        if (value is null) return default!;
        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, JsonOptions), JsonOptions)!;
    }
}