using System;
using System.IO;

namespace Corgibytes.Freshli.Cli.CommandOptions
{
    public class ScanCommandOptions : CommandOptions
    {
        public override CommandOptionType Type => CommandOptionType.Scan;
        
        public string Path { get ; set; }

    }
}
