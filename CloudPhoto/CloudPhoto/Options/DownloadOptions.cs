using CommandLine;

namespace CloudPhoto.Options;

[Verb("download", HelpText = "Uploading photos from cloud storage")]
public class DownloadOptions
{
    [Option("album", Required = true, HelpText = "Cloud Storage album name")]
    public string Album { get; set; }

    private string _path = default!;

    [Option("path", Default = "", HelpText = "Directory path (default: current directory)")]
    public string Path
    {
        get => _path;
        set => _path = value.Trim('\"').Trim('\'');
    }
}
