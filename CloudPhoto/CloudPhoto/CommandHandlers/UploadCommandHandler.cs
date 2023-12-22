using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Configuration;
using CloudPhoto.Options;
using CloudPhoto.Services;
using CommandLine;
using static System.Net.Mime.MediaTypeNames;

namespace CloudPhoto.CommandHandlers
{
    public class UploadCommandHandler
    {
        public async Task HandleAsync(IAmazonS3 s3Client, UploadOptions options, ConfigFile config)
        {
            try
            {
                var path = FileManager.GetFullPath(options.Path);
                if (!Directory.Exists(path))
                {
                    Console.Error.WriteLine($"Error: Photos directory '{path}' not found.");
                    return;
                }

                var tasks = new DirectoryInfo(path)
                    .GetFiles("*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => f.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                        || f.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                    .Select(async image => await UploadPhotoAsync(s3Client, options.Album, image, config.Bucket))
                    .ToArray();

                await Task.WhenAll(tasks);

                Console.WriteLine("Upload command executed succesfully.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task UploadPhotoAsync(IAmazonS3 s3Client, string album, FileSystemInfo image, string bucketName)
        {
            var request = new PutObjectRequest
            {
                Key = $"{album}/{image.Name}",
                BucketName = bucketName,
                FilePath = image.FullName
            };

            await s3Client.PutObjectAsync(request);
            Console.WriteLine($"Uploading {image.Name} to album {album} in bucket {bucketName}");
        }
    }
}