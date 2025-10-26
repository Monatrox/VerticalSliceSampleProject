using System.Reflection;
using NetArchTest.Rules;
using Shouldly;

namespace VerticalSlice.Tests.Architecture;

public class ArchitectureTests
{
    private static readonly string[] Modules = ["Todo", "Weather"];
    private static readonly string[] RestrictedLayers = ["Application", "Domain", "Infrastructure"];

    [Theory]
    [MemberData(nameof(GetModuleDependencyTestCases))]
    public void ModuleToModuleDependencies_ShouldOnlyBeViaPublicLibraries(string assemblyUnderTest, string forbiddenAssembly)
    {
        var result = Types.InAssembly(Assembly.Load(assemblyUnderTest))
            .That()
            .ResideInNamespace(assemblyUnderTest)
            .ShouldNot()
            .HaveDependencyOn(forbiddenAssembly)
            .GetResult();

        var failingTypes = String.Join(", ", (result.FailingTypes ?? []).Select(type => type.FullName));

        result.IsSuccessful
            .ShouldBeTrue($"{assemblyUnderTest} should not have a dependency on {forbiddenAssembly}. Failing types: {failingTypes}");
    }

    #region [Helpers]
    
    public static IEnumerable<object[]> GetModuleDependencyTestCases()
    {
        return from module in Modules
            from layer in RestrictedLayers
            let assemblyUnderTest = $"{module}.{layer}"
            from otherModule in Modules.Where(m => m != module)
            from restrictedLayer in RestrictedLayers
            select new object[] { assemblyUnderTest, $"{otherModule}.{restrictedLayer}" };
    }
    
    #endregion
}