using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CloudPhoto.Settings;

namespace CloudPhoto.Services
{
    public class DownloadCommand : ConsoleAppBase
    {
        private readonly VvotSettings _vvotSettings;
        private readonly IAmazonS3 _amazonS3;

        public DownloadCommand(VvotSettings vvotSettings, IAmazonS3 amazonS3)
        {
            _vvotSettings = vvotSettings;
            _amazonS3 = amazonS3;
        }

        [Command("download")]
        public async Task Download([Option("album")] string album, [Option("path")] string? path = null)
        {
            path ??= Environment.CurrentDirectory;
            var prefix = album + "/";
            var objects = await _amazonS3.ListObjectsV2Async(new ListObjectsV2Request()
            {
                BucketName = _vvotSettings.Bucket,
                Prefix = prefix,
            });

            if (!objects.S3Objects.Any())
            {
                throw new ApplicationException("Directory not found");
            }

            using var utility = new TransferUtility(_amazonS3);

            foreach (var obj in objects.S3Objects)
            {
                var fileName = obj.Key.Replace(prefix, "");
                await utility.DownloadAsync(Path.Combine(path, fileName), _vvotSettings.Bucket, obj.Key);
            }
        }
    }
}
