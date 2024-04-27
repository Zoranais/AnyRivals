using AnyRivals.Application.Common.Interfaces.Handlers;
using FluentAssertions;
using NetArchTest.Rules;

namespace AnyRivals.Tests.ArchitectureTests;
public class CqrsTests
{
    private readonly Type CommandHandlerInterface = typeof(ICommandHandler<,>);
    private readonly Type QueryHandlerInterface = typeof(IQueryHandler<,>);
    private readonly Type DomainEventInterface = typeof(IDomainEventHandler<>);

    private readonly Type CommandInterface = typeof(ICommand<>);
    private readonly Type QueryInterface = typeof(IQuery<>);

    [Test]
    public void Handlers_Should_ImplementCorrespondingInterfaces()
    {
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .ImplementInterface(CommandHandlerInterface)
            .Or()
            .ImplementInterface(QueryHandlerInterface)
            .Or()
            .ImplementInterface(DomainEventInterface)
            .GetResult();

        result.FailingTypes.Should().BeNull();
    }

    [Test]
    public void Commands_Should_ImplementCorrespondingInterface()
    {
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Command")
            .Should()
            .ImplementInterface(CommandInterface)
            .GetResult();

        result.FailingTypes.Should().BeNull();
    }

    [Test]
    public void Queries_Should_ImplementCorrespondingInterface()
    {
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Query")
            .Should()
            .ImplementInterface(QueryInterface)
            .GetResult();

        result.FailingTypes.Should().BeNull();
    }
}
