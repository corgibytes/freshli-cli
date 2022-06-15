using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public class SupportedDependencyManagers
{
    private readonly string _dependencyManager;

    private const string ConstantComposer = "composer";
    private const string ConstantBundler = "bundler";
    private const string ConstantCarton = "carton";
    private const string ConstantNuGet = "nuget";
    private const string ConstantPip = "pip";

    public static SupportedDependencyManagers FromString(string dependencyManager)
    {
        return dependencyManager switch
        {
            ConstantComposer => new SupportedDependencyManagers(ConstantComposer),
            ConstantBundler => new SupportedDependencyManagers(ConstantBundler),
            ConstantCarton => new SupportedDependencyManagers(ConstantCarton),
            ConstantNuGet => new SupportedDependencyManagers(ConstantNuGet),
            ConstantPip => new SupportedDependencyManagers(ConstantPip),
            _ => throw new ArgumentException($"Invalid dependency manager given '{dependencyManager}'"),
        };
    }

    public bool Equals(SupportedDependencyManagers other)
    {
        return _dependencyManager == other._dependencyManager;
    }

    public static SupportedDependencyManagers Composer()
    {
        return new SupportedDependencyManagers(ConstantComposer);
    }

    public static SupportedDependencyManagers Bundler()
    {
        return new SupportedDependencyManagers(ConstantBundler);
    }

    public static SupportedDependencyManagers Carton()
    {
        return new SupportedDependencyManagers(ConstantCarton);
    }

    public static SupportedDependencyManagers NuGet()
    {
        return new SupportedDependencyManagers(ConstantNuGet);
    }

    public static SupportedDependencyManagers Pip()
    {
        return new SupportedDependencyManagers(ConstantPip);
    }

    public string DependencyManager()
    {
        return _dependencyManager;
    }

    private SupportedDependencyManagers(string dependencyManager)
    {
        _dependencyManager = dependencyManager;
    }
}
