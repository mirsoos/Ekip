using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using Xunit;

namespace Ekip.ArchitectureTests;

public sealed class LayerDependencyTests : ArchitectureTestBase
{
    [Fact]
    public void Domain_Should_Not_Have_Any_External_Dependencies()
    {
        var forbidden = DomainAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name!)
            .Where(name =>
                !name.Equals("System", StringComparison.Ordinal)
                && !name.StartsWith("System.", StringComparison.Ordinal)
                && !name.Equals("netstandard", StringComparison.Ordinal)
                && !name.Equals("mscorlib", StringComparison.Ordinal))
            .ToList();

        Assert.Empty(forbidden);
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Layers()
    {
        ArchRuleDefinition.Types()
            .That().Are(DomainLayer)
            .Should().NotDependOnAny(ApplicationLayer)
            .AndShould().NotDependOnAny(PersistenceLayer)
            .AndShould().NotDependOnAny(ApiLayer)
            .Check(EkipArchitecture);
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Persistence_Or_Api_Or_EfCore()
    {
        ArchRuleDefinition.Types()
            .That().Are(ApplicationLayer)
            .Should().NotDependOnAny(PersistenceLayer)
            .AndShould().NotDependOnAny(ApiLayer)
            .Check(EkipArchitecture);

        var referenced = ApplicationAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name!)
            .ToList();

        Assert.DoesNotContain(referenced, name =>
            name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("Npgsql", StringComparison.OrdinalIgnoreCase)
            || name.Equals("Dapper", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("Ekip.Persistence", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("Ekip.Infrastructure", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("Ekip.Api", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Persistence_Should_Not_Depend_On_Api()
    {
        ArchRuleDefinition.Types()
            .That().Are(PersistenceLayer)
            .Should().NotDependOnAny(ApiLayer)
            .Check(EkipArchitecture);
    }
}