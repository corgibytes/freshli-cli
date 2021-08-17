using System.IO;

namespace Corgibytes.Freshli.Cli.CommandOptions
{
    public class ScanCommandOptions : CommandOptions
    {
        public override CommandOptionType Type => CommandOptionType.Scan;

        public DirectoryInfo Path { get; set; }

        public ScanCommandOptions()
        {
            this.Path = new DirectoryInfo(Directory.GetCurrentDirectory());
        }
    }
}
