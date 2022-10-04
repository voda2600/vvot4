using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Settings;

namespace CloudPhoto.Services
{
    public class ListCommand : ConsoleAppBase
    {
        private readonly VvotSettings _vvotSettings;
        private readonly IAmazonS3 _amazonS3;

        public ListCommand(VvotSettings vvotSettings, IAmazonS3 amazonS3)
        {
            _vvotSettings = vvotSettings;
            _amazonS3 = amazonS3;
        }

        [Command("list")]
        public async Task List([Option("album")] string? album = null)
        {
            var list = await _amazonS3.ListObjectsV2Async(new ListObjectsV2Request()
            {
                BucketName = _vvotSettings.Bucket,
                Prefix = album,
            });

            Func<string, string?> action = album is null ? Path.GetDirectoryName : Path.GetFileName;
            ListObjects(list, action);
        }

        private static void ListObjects(ListObjectsV2Response list, Func<string, string?> func)
        {
            var objects = list.S3Objects
                .Select(x => func(x.Key))
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct();

            foreach (var obj in objects)
            {
                Console.WriteLine(obj);
            }
        }
    }
}
