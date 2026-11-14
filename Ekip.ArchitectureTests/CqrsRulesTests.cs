using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using Ekip.Application.Contracts;
using Ekip.Application.Interfaces;
using FluentValidation;
using MediatR;
using Xunit;

namespace Ekip.ArchitectureTests;

public sealed class CqrsRulesTests : ArchitectureTestBase
{
    [Fact]
    public void Command_And_Query_Handlers_Should_Be_Sealed()
    {
        ArchRuleDefinition.Classes()
            .That().Are(ApplicationLayer)
            .And().ImplementInterface(typeof(IRequestHandler<,>))
            .Should().BeSealed()
            .WithoutRequiringPositiveResults()
            .Check(EkipArchitecture);
    }

    [Fact]
    public void Validators_Should_Be_Sealed()
    {
        ArchRuleDefinition.Classes()
            .That().Are(ApplicationLayer)
            .And().ImplementInterface(typeof(IValidator<>))
            .Should().BeSealed()
            .WithoutRequiringPositiveResults()
            .Check(EkipArchitecture);
    }

    [Fact]
    public void QueryHandlers_Should_Not_Depend_On_UnitOfWork()
    {
        var unitOfWorkType = ArchRuleDefinition.Types()
            .That().AreAssignableTo(typeof(IUnitOfWork))
            .As("IUnitOfWork");

        ArchRuleDefinition.Classes()
            .That().Are(ApplicationLayer)
            .And().ImplementInterface(typeof(IQueryHandler<,>))
            .Should().NotDependOnAny(unitOfWorkType)
            .WithoutRequiringPositiveResults()
            .Check(EkipArchitecture);
    }

    [Fact]
    public void All_Requests_Must_Use_Application_Command_Or_Query_Contracts()
    {
        var invalidRequests = ApplicationAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IBaseRequest).IsAssignableFrom(t)
                        && !typeof(IBaseCommand).IsAssignableFrom(t)
                        && !typeof(IBaseQuery).IsAssignableFrom(t))
            .Select(t => t.FullName)
            .ToList();

        Assert.Empty(invalidRequests);
    }
}