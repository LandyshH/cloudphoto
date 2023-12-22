using CommandLine;

namespace CloudPhoto.Options;

[Verb("list", HelpText = "Album and photo list")]
public class ListOptions
{
    [Option("album", HelpText = "Cloud Storage album name")]
    public string Album { get; set; }
}
