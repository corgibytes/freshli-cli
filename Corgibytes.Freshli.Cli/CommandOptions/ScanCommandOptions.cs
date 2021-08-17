using System.IO;

namespace Corgibytes.Freshli.Cli.CommandOptions
{
    public class ScanCommandOptions : CommandOptions
    {
        public override CommandOptionType Type => CommandOptionType.Scan;

        private string path = string.Empty;
        
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = File.Exists(value) ? System.IO.Path.GetFullPath(path) : value;
            }
        }
    }
}
