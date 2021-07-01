using CommandLine;
using System;
using System.IO;

namespace Corgibytes.Freshli.Cli.Options
{
    [Verb("scan", HelpText = "Scan command help text.")]

    public class ScanOptions : Option
    {
        public override OptionType Type => OptionType.scan;

        private string path = String.Empty;

        [Value(0, MetaName = "path", Required = true, HelpText = "Repository Path.")]
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
