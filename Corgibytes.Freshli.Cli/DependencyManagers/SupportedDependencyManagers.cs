using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers
{
    public class SupportedDependencyManagers
    {
        private readonly string _dependencyManager;

        private const string _Composer = "composer";
        private const string _Bundler = "bundler";
        private const string _Carton = "carton";
        private const string _NuGet = "nuget";
        private const string _Pip = "pip";

        public static SupportedDependencyManagers FromString(string dependencyManager)
        {
            switch (dependencyManager)
            {
                case SupportedDependencyManagers._Composer:
                    return new SupportedDependencyManagers(_Composer);
                case SupportedDependencyManagers._Bundler:
                    return new SupportedDependencyManagers(_Bundler);
                case SupportedDependencyManagers._Carton:
                    return new SupportedDependencyManagers(_Carton);
                case SupportedDependencyManagers._NuGet:
                    return new SupportedDependencyManagers(_NuGet);
                case SupportedDependencyManagers._Pip:
                    return new SupportedDependencyManagers(_Pip);
                default:
                    throw new ArgumentException($"Invalid dependency manager given '{dependencyManager}'");
            }
        }

        public bool Equals(SupportedDependencyManagers other)
        {
            return this._dependencyManager == other._dependencyManager;
        }

        public static SupportedDependencyManagers Composer()
        {
            return new SupportedDependencyManagers(_Composer);
        }

        public static SupportedDependencyManagers Bundler()
        {
            return new SupportedDependencyManagers(_Bundler);
        }

        public static SupportedDependencyManagers Carton()
        {
            return new SupportedDependencyManagers(_Carton);
        }

        public static SupportedDependencyManagers NuGet()
        {
            return new SupportedDependencyManagers(_NuGet);
        }

        public static SupportedDependencyManagers Pip()
        {
            return new SupportedDependencyManagers(_Pip);
        }

        private SupportedDependencyManagers(string dependencyManager)
        {
            _dependencyManager = dependencyManager;
        }
    }
}
