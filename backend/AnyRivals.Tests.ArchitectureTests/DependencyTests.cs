using FluentAssertions;
using NetArchTest.Rules;

namespace AnyRivals.Tests.ArchitectureTests;

public class DependencyTests
{
    [Test]
    public void Domain_ShouldNot_ReferenceOtherProjects()
    {
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        string[] projects = 
        [
            Constants.Application,
            Constants.Infrastructure,
            Constants.Web,
        ];

        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(projects)
            .GetResult();


        result.IsSuccessful.Should().BeTrue();
    }

    [Test]
    public void Application_ShouldNot_ReferenceOtherProjects()
    {
        var assembly = typeof(Application.AssemblyReference).Assembly;

        string[] projects =
        [
            Constants.Infrastructure,
            Constants.Web,
        ];

        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(projects)
            .GetResult();


        result.IsSuccessful.Should().BeTrue();
    }

    [Test]
    public void Infrastructure_ShouldNot_ReferenceOtherProjects()
    {
        var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

        string[] projects =
        [
            Constants.Web,
        ];

        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(projects)
            .GetResult();


        result.IsSuccessful.Should().BeTrue();
    }
}