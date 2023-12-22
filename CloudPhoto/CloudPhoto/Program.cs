using Amazon.S3;
using CloudPhoto;
using CloudPhoto.CommandHandlers;
using CloudPhoto.Options;
using CloudPhoto.Services;
using CommandLine;

var result = Parser.Default.ParseArguments<InitOptions, UploadOptions, DownloadOptions, ListOptions, MksiteOptions, DeleteOptions>(args);

await result.WithParsedAsync<InitOptions>(async (options) => await new InitCommandHandler().HandleAsync(options));

if(result.Value is not InitOptions)
{
    var configFileExists = FileManager.IsConfigExists();
    if (configFileExists)
    {
        var config = FileManager.ReadConfigurationIniFile();

        var s3Client = new AmazonS3Client(config.AwsAccessKeyId, config.AwsSecretAccessKey, new AmazonS3Config
        {
            ServiceURL = Constants.S3EndpointUrl,
            AuthenticationRegion = Constants.Region
        });

        await result
            .WithParsedAsync<UploadOptions>(async (options) => await new UploadCommandHandler().HandleAsync(s3Client, options, config));

        await result
           .WithParsedAsync<DownloadOptions>(async (options) => await new DownloadCommandHandler().HandleAsync(s3Client, options, config));

        await result
           .WithParsedAsync<ListOptions>(async (options) => await new ListCommandHandler().HandleAsync(s3Client, options, config));

        await result
           .WithParsedAsync<MksiteOptions>(async (options) => await new MksiteCommandHandler().HandleAsync(s3Client, options, config));

        await result
           .WithParsedAsync<DeleteOptions>(async (options) => await new DeleteCommandHandler().HandleAsync(s3Client, options, config));
    }
    else
    {
        Console.WriteLine("Please create config file with command 'cloudphoto init'");
    }

}
