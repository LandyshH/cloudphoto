using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Options;
using CloudPhoto.Services;

namespace CloudPhoto.CommandHandlers
{
    public class InitCommandHandler
    {
        public async Task HandleAsync(InitOptions options)
        {
            if (string.IsNullOrEmpty(options.AwsAccessKeyId))
            {
                Console.WriteLine("Enter AWS Access Key ID: ");
                options.AwsAccessKeyId = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(options.AwsSecretAccessKey))
            {
                Console.WriteLine("Enter AWS Secret Access Key: ");
                options.AwsSecretAccessKey = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(options.Bucket))
            {
                Console.WriteLine("Enter S3 Bucket Name: ");
                options.Bucket = Console.ReadLine();
            }

            FileManager.SaveConfigurationToIniFile(options);

            var s3Client = new AmazonS3Client(options.AwsAccessKeyId, options.AwsSecretAccessKey, new AmazonS3Config
            {
                ServiceURL = Constants.S3EndpointUrl,
                AuthenticationRegion = Constants.Region
            });

            var bucketName = options.Bucket;
            var isBucketExist = await IsBucketExistAsync(s3Client, bucketName);

            if (!isBucketExist)
            {
                var bucketCreated = await CreateBucketAsync(s3Client, bucketName);
                if(bucketCreated)
                {
                    Console.WriteLine($"Bucket {bucketName} created.");
                }
            }
            else
            {
                Console.WriteLine($"Bucket {bucketName} exists.");
            }
        }

        static async Task<bool> CreateBucketAsync(AmazonS3Client client, string bucketName)
        {
            try
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                };

                var response = await client.PutBucketAsync(request);
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                Console.Error.WriteLine($"Error: creating bucket: '{ex.Message}' {ex.ErrorCode}");
                return false;
            }
        }

        static async Task<bool> IsBucketExistAsync(AmazonS3Client s3Client, string bucketName)
        {
            var response = await s3Client.ListBucketsAsync();
            return response.Buckets.Any(b => b.BucketName == bucketName);
        }
    }
}
