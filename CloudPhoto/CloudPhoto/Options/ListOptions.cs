using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPhoto.Options;

[Verb("list", HelpText = "")]
public class ListOptions
{
    [Option("album", HelpText = "Cloud Storage album name")]
    public string Album { get; set; }
}
