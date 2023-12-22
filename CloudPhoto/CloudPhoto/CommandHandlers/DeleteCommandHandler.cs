using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Configuration;
using CloudPhoto.Options;

namespace CloudPhoto.CommandHandlers;

public class DeleteCommandHandler
{
    public async Task HandleAsync(IAmazonS3 s3Client, DeleteOptions options, ConfigFile config)
    {
        if (options.Photo is null)
        {
            var listObjectsV2Request = new ListObjectsV2Request
            {
                BucketName = config.Bucket,
                Prefix = $"{options.Album}/"
            };


            ListObjectsV2Response listObjectsV2Response;
            var count = 0;
            do
            {
                listObjectsV2Response = await s3Client.ListObjectsV2Async(listObjectsV2Request);
                var deleteObjectsRequest = new DeleteObjectsRequest
                {
                    BucketName = config.Bucket,
                    Objects = listObjectsV2Response.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
                };

                count += listObjectsV2Response.S3Objects.Count;

                await s3Client.DeleteObjectsAsync(deleteObjectsRequest);
                listObjectsV2Request.ContinuationToken = listObjectsV2Response.NextContinuationToken;
            } while (listObjectsV2Response.IsTruncated);

            if (count == 0)
            {
                Console.Error.WriteLine($"Error: There is no album with name {options.Album}.");
            }
            else
            {
                Console.WriteLine($"Album {options.Album} and files deleted. Count: {count}");
            }
        }
        else
        {
            try
            {
                var key = $"{options.Album}/{options.Photo}";
                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = config.Bucket,
                    Key = key
                };

                var _ = await s3Client.GetObjectAsync(getObjectRequest);

                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = config.Bucket,
                    Key = key
                };

                await s3Client.DeleteObjectAsync(deleteObjectRequest);

                Console.WriteLine($"File {options.Photo} deleted.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
