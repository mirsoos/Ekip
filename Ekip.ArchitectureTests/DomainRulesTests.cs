using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using Ekip.Domain.Base;
using Ekip.Domain.Results;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;

namespace Ekip.ArchitectureTests;

public sealed partial class DomainRulesTests : ArchitectureTestBase
{
    [GeneratedRegex(@"^[A-Z0-9_]+\.[A-Z0-9_]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ErrorCodePattern();

    [Fact]
    public void AppendOnly_Entities_Must_Be_AggregateRoots_And_Immutable()
    {
        ArchRuleDefinition.Classes()
            .That().Are(DomainLayer)
            .And().ImplementInterface(typeof(IAppendOnlyEntity))
            .Should().ImplementInterface(typeof(IAggregateRoot))
            .AndShould().NotImplementInterface(typeof(ISoftDeletable))
            .AndShould().NotImplementInterface(typeof(IAuditableEntity))
            .WithoutRequiringPositiveResults()
            .Check(EkipArchitecture);
    }

    [Fact]
    public void All_Domain_Errors_Must_Have_Unique_Valid_Codes()
    {
        var errorFields = DomainAssembly.GetTypes()
            .Where(t => t.IsClass)
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            .Where(f => typeof(Error).IsAssignableFrom(f.FieldType))
            .ToList();

        var seenCodes = new HashSet<string>(StringComparer.Ordinal);
        var invalidCodes = new List<string>();
        var duplicateCodes = new List<string>();

        foreach (var field in errorFields)
        {
            if (field.GetValue(null) is not Error error || error == Error.None)
                continue;

            if (!ErrorCodePattern().IsMatch(error.Code))
                invalidCodes.Add($"{field.DeclaringType?.Name}.{field.Name} => '{error.Code}'");

            if (!seenCodes.Add(error.Code))
                duplicateCodes.Add($"{field.DeclaringType?.Name}.{field.Name} => '{error.Code}'");
        }

        Assert.Empty(invalidCodes);
        Assert.Empty(duplicateCodes);
    }
}