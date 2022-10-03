using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Helpers;
using CloudPhoto.Models;
using CloudPhoto.Settings;
using static RazorEngine.Razor;

namespace CloudPhoto.Services
{
    public class MkSiteCommand : ConsoleAppBase
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly VvotSettings _vvotSettings;

        public MkSiteCommand(IAmazonS3 amazonS3, VvotSettings vvotSettings)
        {
            _amazonS3 = amazonS3;
            _vvotSettings = vvotSettings;
        }

        [Command("mksite")]
        public async Task MkSite()
        {
            var list = await _amazonS3.ListObjectsV2Async(new ListObjectsV2Request()
            {
                BucketName = _vvotSettings.Bucket,
            });
            var albumList = list.S3Objects
                           .Select(x => Path.GetDirectoryName(x.Key))
                           .Distinct();
            foreach (var album in albumList)
            {
                Console.WriteLine(album);
                
                var objects = await _amazonS3.ListObjectsV2Async(new ListObjectsV2Request()
                {
                    BucketName = _vvotSettings.Bucket,
                    Prefix = album + "/",
                });
                var model = new List<AlbumModel>();
                
                foreach (var obj in objects.S3Objects)
                {
                    var expiryUrlRequest = new GetPreSignedUrlRequest()
                    {
                        BucketName = _vvotSettings.Bucket,
                        Key = obj.Key,
                        Expires = DateTime.Now.AddDays(1),
                    };
                    model.Add(new AlbumModel()
                    {
                        Title = obj.Key,
                        Url = "хуй"//_amazonS3.GetPreSignedURL(expiryUrlRequest),
                    });
                }

                var adsfasf = RenderViewHelper.RenderPartialToString("~/wwwroot/tempalate/index.cshtml",null);
                Console.WriteLine(adsfasf);
            }
        }
    }
}
