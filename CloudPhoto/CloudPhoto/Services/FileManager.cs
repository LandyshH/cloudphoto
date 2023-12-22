using CloudPhoto.Options;
using IniParser.Model;
using IniParser;
using CloudPhoto.Configuration;

namespace CloudPhoto.Services;

public static class FileManager
{
    public static void SaveConfigurationToIniFile(InitOptions options)
    {
        var directoryPath = Path.GetDirectoryName(GetIniFilePath());
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var filePath = GetIniFilePath();
        Console.WriteLine($"Creating config file in {filePath}");

        var parser = new FileIniDataParser();
        var config = new IniData();

        config.Global["bucket"] = options.Bucket;
        config.Global["aws_access_key_id"] = options.AwsAccessKeyId;
        config.Global["aws_secret_access_key"] = options.AwsSecretAccessKey;
        config.Global["region"] = Constants.Region;
        config.Global["endpoint_url"] = Constants.S3EndpointUrl;

        parser.WriteFile(filePath, config);
    }

    public static ConfigFile ReadConfigurationIniFile()
    {
        var filePath = GetIniFilePath();
        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(filePath);

        var options = new ConfigFile
        {
            Bucket = data.Global["bucket"],
            AwsAccessKeyId = data.Global["aws_access_key_id"],
            AwsSecretAccessKey = data.Global["aws_secret_access_key"],
        };

        return options;
    }

    public static bool IsConfigExists()
    {
        var filePath = GetIniFilePath();
        return File.Exists(filePath);
    }

    public static string GetIniFilePath()
    {
        var homeCatalog = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var filePath = Path.Combine(homeCatalog, ".config", "cloudphoto", "cloudphotorc", "config.ini");
        return filePath;
    }

    public static string GetFullPath(string path)
    {
        return Path.IsPathFullyQualified(path)
            ? path
            : Path.Combine(Environment.CurrentDirectory, path);
    }
}
