using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using Ekip.Application.Contracts;
using Ekip.Application.Extensions;
using Ekip.Application.Interfaces;
using Ekip.Application.Interfaces.Repositories.User;
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
    public void QueryHandlers_Should_Not_Depend_On_Write_Repositories()
    {
        var writeRepositories = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching("^Ekip\\.Application\\.Interfaces\\.Repositories(\\..+)?$")
            .As("Write Repositories");

        ArchRuleDefinition.Classes()
            .That().Are(ApplicationLayer)
            .And().ImplementInterface(typeof(IQueryHandler<,>))
            .Should().NotDependOnAny(writeRepositories)
            .WithoutRequiringPositiveResults()
            .Check(EkipArchitecture);
    }

    [Fact]
    public void Validators_Should_Use_WithError_And_Not_Call_WithErrorCode_Or_WithMessage()
    {
        var domainErrorCatalogs = ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching("^Ekip\\.Domain\\.Errors(\\..+)?$")
            .As("Domain Error Catalogs");

        ArchRuleDefinition.Classes()
            .That().Are(ApplicationLayer)
            .And().ImplementInterface(typeof(IValidator<>))
            .Should().DependOnAny(typeof(FluentValidationExtensions))
            .AndShould().DependOnAny(domainErrorCatalogs)
            .WithoutRequiringPositiveResults()
            .Check(EkipArchitecture);

        var forbiddenMethods = ArchRuleDefinition.MethodMembers()
            .That().AreDeclaredIn(typeof(DefaultValidatorOptions))
            .And().HaveNameStartingWith("WithErrorCode")
            .Or().AreDeclaredIn(typeof(DefaultValidatorOptions))
            .And().HaveNameStartingWith("WithMessage")
            .As("Hardcoded WithErrorCode/WithMessage");

        ArchRuleDefinition.Classes()
            .That().Are(ApplicationLayer)
            .And().ImplementInterface(typeof(IValidator<>))
            .Should().NotCallAny(forbiddenMethods)
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

    [Fact]
    public void Only_Registration_Command_Handlers_Should_Call_AddAccount()
    {
        var addAccountMethod = ArchRuleDefinition.MethodMembers()
            .That().AreDeclaredIn(typeof(IUserRepository))
            .And().HaveName("AddAccount")
            .As("IUserRepository.AddAccount");

        ArchRuleDefinition.Classes()
            .That().Are(ApplicationLayer)
            .And().DoNotHaveNameMatching(".*Register.*Handler.*")
            .Should().NotCallAny(addAccountMethod)
            .WithoutRequiringPositiveResults()
            .Check(EkipArchitecture);
    }
}