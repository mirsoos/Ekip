using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using Ekip.Api.gRPC;
using Ekip.Application.Contracts;
using Ekip.Domain.Base;
using Ekip.Persistence.Context;
using Assembly = System.Reflection.Assembly;

namespace Ekip.ArchitectureTests;

public abstract class ArchitectureTestBase
{
    protected static readonly Assembly DomainAssembly = typeof(BaseEntity).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
    protected static readonly Assembly PersistenceAssembly = typeof(EkipDbContext).Assembly;
    protected static readonly Assembly ApiAssembly = typeof(GrpcExceptionInterceptor).Assembly;

    protected static readonly Architecture EkipArchitecture = new ArchLoader()
        .LoadAssemblies(
            DomainAssembly,
            ApplicationAssembly,
            PersistenceAssembly,
            ApiAssembly)
        .Build();

    protected static readonly IObjectProvider<IType> DomainLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(DomainAssembly).As("Domain Layer");

    protected static readonly IObjectProvider<IType> ApplicationLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(ApplicationAssembly).As("Application Layer");

    protected static readonly IObjectProvider<IType> PersistenceLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(PersistenceAssembly).As("Persistence Layer");

    protected static readonly IObjectProvider<IType> ApiLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(ApiAssembly).As("Api Layer");
}