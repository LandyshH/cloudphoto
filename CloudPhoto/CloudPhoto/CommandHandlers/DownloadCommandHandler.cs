using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Configuration;
using CloudPhoto.Options;
using CloudPhoto.Services;
using System.Net;

namespace CloudPhoto.CommandHandlers;

public class DownloadCommandHandler
{
    public async Task HandleAsync(IAmazonS3 s3Client, DownloadOptions options, ConfigFile config)
    {
        var listObjectsRequest = new ListObjectsRequest
        {
            BucketName = config.Bucket,
            Prefix = $"{options.Album}/"
        };

        var path = FileManager.GetFullPath(options.Path);

        int i = 0;
        await foreach (var obj in s3Client.Paginators
                                          .ListObjects(listObjectsRequest).S3Objects)
        {
            await DownloadPhotoAsync(s3Client, path, i++, obj.Key, config.Bucket, options.Album);
        }

        if (i == 0)
        {
            Console.Error.WriteLine($"Error: There is no album with name '{options.Album}'");
        }
        else
        {
            Console.WriteLine("Download command executed succesfully.");
        }
    }

    private async Task DownloadPhotoAsync(IAmazonS3 s3Client, string path, int index, string key, string bucketName, string album)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await s3Client.GetObjectAsync(request);
        var name = response.Key.Split("/")[1];
        var pathToFile = Path.Combine(path, name);

        Console.WriteLine($"Downloading file {name} from album {album} bucket {bucketName}");

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine($"Error downloading {index} file. Status code: {response.HttpStatusCode}");
        }

        try
        {
            await using var fileStream = File.OpenWrite(pathToFile);
            await response.ResponseStream.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
