using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Configuration;
using CloudPhoto.Options;

namespace CloudPhoto.CommandHandlers;

public class MksiteCommandHandler
{
    public async Task HandleAsync(IAmazonS3 s3Client, MksiteOptions options, ConfigFile config)
    {
        try
        {
            await SetBucketPolicyAsync(s3Client, config.Bucket);
            await SetBucketWebsiteConfigurationAsync(s3Client, config.Bucket, "index.html", "error.html");

            var listObjectsV2Request = new ListObjectsV2Request
            {
                BucketName = config.Bucket,
                Delimiter = "/"
            };

            var directories = new List<string>();
            var response = await s3Client.ListObjectsV2Async(listObjectsV2Request);
            directories.AddRange(response.CommonPrefixes);

            var albumNames = new List<string>();
            foreach (var directory in directories)
            {
                var albumName = Path.GetFileName(directory.TrimEnd('/'));
                albumNames.Add(albumName);

                var photosList = await GetPhotoUrlsFromAlbumAsync(s3Client, config.Bucket, directory);
                var albumHtml = await GenerateAlbumHtmlAsync(albumName, photosList);
                var albumKey = $"album{albumNames.Count}.html";

                await UploadHtmlToS3Async(s3Client, config.Bucket, albumKey, albumHtml);
            }

            var indexHtml = await GenerateIndexHtmlAsync(albumNames);
            var indexPath = Path.Combine($@"{AppContext.BaseDirectory}Pages\", "album.html");
            await UploadHtmlToS3Async(s3Client, config.Bucket, "index.html", indexHtml);

            var errorHtmlPath = Path.Combine($@"{AppContext.BaseDirectory}Pages\", "error.html");
            var errorHtml = await ReadHtmlAsync(errorHtmlPath);
            await UploadHtmlToS3Async(s3Client, config.Bucket, "error.html", errorHtml);

            Console.WriteLine($"Website URL: http://{config.Bucket}.website.yandexcloud.net/");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task<string> ReadHtmlAsync(string filePath)
    {
        try
        {
            using var reader = new StreamReader(filePath);
            return await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: reading error.html file: {ex.Message}");
            return string.Empty;
        }
    }

    private async Task<string> GenerateAlbumHtmlAsync(string albumName, List<string> photoUrls)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($@"<!doctype html>
    <html>
        <head>
            <meta charset=utf-8>
            <link rel=""stylesheet"" type=""text/css"" href=""https://cdnjs.cloudflare.com/ajax/libs/galleria/1.6.1/themes/classic/galleria.classic.min.css"" />
            <style>
                .galleria{{ width: 960px; height: 540px; background: #000 }}
            </style>
            <script src=""https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js""></script>
            <script src=""https://cdnjs.cloudflare.com/ajax/libs/galleria/1.6.1/galleria.min.js""></script>
            <script src=""https://cdnjs.cloudflare.com/ajax/libs/galleria/1.6.1/themes/classic/galleria.classic.min.js""></script>
        </head>
        <body>
            <div class=""galleria"">");

        foreach (var photoUrl in photoUrls)
        {
            sb.AppendLine($@"        <img src=""{photoUrl}"" data-title=""{Path.GetFileName(photoUrl)}"">");
        }

        sb.AppendLine($@"    </div>
    <p>Вернуться на <a href=""index.html"">главную страницу</a> фотоархива</p>
    <script>
            (function() {{
                Galleria.run('.galleria');
            }}());
    </script>
</body>
</html>");

        return sb.ToString();

    }

    private async Task<string> GenerateIndexHtmlAsync(List<string> albumNames)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($@"<!doctype html>
<html>
<head>
    <meta charset=utf-8>
    <title>Фотоархив</title>
</head>
<body>
    <h1>Фотоархив</h1>
    <ul>");

        for (int i = 0; i < albumNames.Count; i++)
        {
            sb.AppendLine($@"        <li><a href=""album{i + 1}.html"">{albumNames[i]}</a></li>");
        }

        sb.AppendLine($@"    </ul>
</body>
</html>");

        return sb.ToString();
    }

    private async Task UploadHtmlToS3Async(IAmazonS3 s3Client, string bucketName, string key, string htmlContent)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            ContentBody = htmlContent,
        };

        await s3Client.PutObjectAsync(putObjectRequest);
    }

    private async Task SetBucketPolicyAsync(IAmazonS3 s3Client, string bucketName)
    {
        var putAclRequest = new PutACLRequest
        {
            BucketName = bucketName,
            CannedACL = S3CannedACL.PublicRead
        };

        await s3Client.PutACLAsync(putAclRequest);
    }

    private async Task SetBucketWebsiteConfigurationAsync(IAmazonS3 s3Client, string bucketName, string indexDocument, string errorDocument)
    {
        var putBucketWebsiteRequest = new PutBucketWebsiteRequest
        {
            BucketName = bucketName,
            WebsiteConfiguration = new WebsiteConfiguration
            {
                IndexDocumentSuffix = indexDocument,
                ErrorDocument = errorDocument
            }
        };

        await s3Client.PutBucketWebsiteAsync(putBucketWebsiteRequest);
    }

    private async Task<List<string>> GetPhotoUrlsFromAlbumAsync(IAmazonS3 s3Client, string bucketName, string albumDirectory)
    {
        var listObjectsV2Request = new ListObjectsV2Request
        {
            BucketName = bucketName,
            Prefix = albumDirectory
        };

        var photoUrls = new List<string>();
        var response = await s3Client.ListObjectsV2Async(listObjectsV2Request);
        foreach (var s3Object in response.S3Objects)
        {
            photoUrls.Add(s3Object.Key);
        }

        return photoUrls;
    }

}
