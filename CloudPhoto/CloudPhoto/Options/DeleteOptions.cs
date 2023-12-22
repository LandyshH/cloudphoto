using CommandLine;

namespace CloudPhoto.Options;

[Verb("delete", HelpText = "Deleting Albums and Photos in Cloud Storage.")]
public class DeleteOptions
{
    [Option("album", Required = true, HelpText = "Cloud Storage album name")]
    public string Album { get; set; }

    [Option("photo", HelpText = "Cloud Storage photo name")]
    public string Photo { get; set; }
}
