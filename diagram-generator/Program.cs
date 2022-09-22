using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

MSBuildLocator.RegisterDefaults();
var workspace = MSBuildWorkspace.Create();

var projectFilePath = Path.Combine("Corgibytes.Freshli.Cli", "Corgibytes.Freshli.Cli.csproj");
while (!File.Exists(projectFilePath))
{
    // check the parent directory to see if the file is accessible from there
    var nextProjectFilePath = Path.Combine("..", projectFilePath);

    // stop walking up the directory tree if we've reached the top
    if (Path.GetFullPath(projectFilePath) == Path.GetFullPath(nextProjectFilePath))
    {
        Console.WriteLine("Coulndn't find the project file for Corgibytes.Freshli.Cli");
        return -1;
    }

    projectFilePath = nextProjectFilePath;
}

var project = await workspace.OpenProjectAsync(projectFilePath);

var compilation = await project.GetCompilationAsync();

Console.WriteLine("flowchart TD;");

foreach (var tree in compilation!.SyntaxTrees)
{
    var root = await tree.GetRootAsync();
    var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
    foreach (var classNode in classes)
    {
        var classModel = compilation.GetSemanticModel(classNode.SyntaxTree);
        var classSymbol = classModel.GetDeclaredSymbol(classNode);

        if (classSymbol!.HasInterfaceNamed("IApplicationActivity") || classSymbol!.HasInterfaceNamed("IApplicationEvent"))
        {
            List<ITypeSymbol> targetTypeSymbols = new();

            var objectCreations = classNode.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var objectCreation in objectCreations)
            {
                if (classModel.GetSymbolInfo(objectCreation.Type).Symbol is INamedTypeSymbol objectTypeSymbol)
                {
                    if (objectTypeSymbol.HasInterfaceNamed("IApplicationActivity") ||
                        objectTypeSymbol.HasInterfaceNamed("IApplicationEvent"))
                    {
                        targetTypeSymbols.Add(objectTypeSymbol);
                    }
                }
            }

            var implicitObjectCreations = classNode.DescendantNodes().OfType<ImplicitObjectCreationExpressionSyntax>();
            foreach (var objectCreation in implicitObjectCreations)
            {
                var operation = classModel.GetOperation(objectCreation);
                if (operation!.Type != null)
                {
                    if (operation.Type.HasInterfaceNamed("IApplicationActivity") ||
                        operation.Type.HasInterfaceNamed("IApplicationEvent"))
                    {
                        targetTypeSymbols.Add(operation.Type);
                    }
                }
            }

            if (classSymbol!.BaseType != null)
            {
                foreach (var typeParameterSymbol in classSymbol.BaseType.TypeArguments)
                {
                    if (typeParameterSymbol.HasInterfaceNamed("IApplicationActivity") ||
                        typeParameterSymbol.HasInterfaceNamed("IApplicationEvent"))
                    {
                        targetTypeSymbols.Add(typeParameterSymbol);
                    }
                }

                if (classSymbol.BaseType.HasInterfaceNamed("IApplicationActivity") ||
                    classSymbol.BaseType.HasInterfaceNamed("IApplicationEvent"))
                {
                    Console.WriteLine($"    {classSymbol.Name} -.-> {classSymbol.BaseType.Name}");
                }
            }

            targetTypeSymbols = targetTypeSymbols.Distinct().ToList();

            foreach (var targetClassSymbol in targetTypeSymbols)
            {
                Console.WriteLine($"    {classSymbol.Name} --> {targetClassSymbol.Name}");
            }

            if (targetTypeSymbols.Count == 0)
            {
                Console.WriteLine($"    {classSymbol.Name}");
            }
        }
    }
}

return 0;

public static class TypeSymbolExtensions
{
    public static bool HasInterfaceNamed(this ITypeSymbol symbol, string interfaceName)
    {
        var interfaceNames = symbol.AllInterfaces.Select(i => i.Name);
        return interfaceNames.Contains(interfaceName);
    }
}





