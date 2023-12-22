using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Configuration;
using CloudPhoto.Options;


namespace CloudPhoto.CommandHandlers;

public class ListCommandHandler
{
    public async Task HandleAsync(IAmazonS3 s3Client, ListOptions options, ConfigFile config)
    {
        if (options.Album is null)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = config.Bucket,
                Delimiter = "/"
            };

            var directories = new List<string>();
            var response = await s3Client.ListObjectsV2Async(request);
            directories.AddRange(response.CommonPrefixes);

            if (directories.Count == 0)
            {
                Console.Error.WriteLine($"Error: There is no albums.");
            }
            else
            {
                Console.WriteLine("Albums: ");
            }

            for (var i = 0; i < directories.Count; i++)
            {
                Console.WriteLine($"{i}. {directories[i]}");
            }
        }
        else
        {
            var request = new ListObjectsRequest
            {
                BucketName = config.Bucket,
                Prefix = $"{options.Album}/"
            };

            var names = new List<string>();
            await foreach (var obj in s3Client.Paginators.ListObjects(request).S3Objects)
            {
                var keyParts = obj.Key.Split("/");
                var name = keyParts.Length > 1 ? keyParts[1] : null;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    names.Add(name);
                }
            }

            if (names.Count == 0)
            {
                Console.Error.WriteLine($"Error: There is no photos in album.");
            }
            else
            {
                Console.WriteLine($"Files in album {options.Album}: ");
            }

            for (var i = 0; i < names.Count; i++)
            {
                Console.WriteLine($"{i}. {names[i]}");
            }
        }
    }
}
