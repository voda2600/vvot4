using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Helpers;
using CloudPhoto.Models;
using CloudPhoto.Settings;

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
            var bucket = await _amazonS3.ListObjectsV2Async(new ListObjectsV2Request()
            {
                BucketName = _vvotSettings.Bucket,
            });

            await _amazonS3.PutACLAsync(new PutACLRequest()
            {
                BucketName = _vvotSettings.Bucket,
                CannedACL = S3CannedACL.PublicRead,
            });

            await _amazonS3.PutBucketWebsiteAsync(new PutBucketWebsiteRequest()
            {
                BucketName = _vvotSettings.Bucket,
                WebsiteConfiguration = new WebsiteConfiguration()
                {
                    ErrorDocument = "error.html",
                    IndexDocumentSuffix = "index.html"
                }
            });

            var albumList = bucket.S3Objects
                .Select(x => Path.GetDirectoryName(x.Key))
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct();
            var albumHtmlList = new List<IndexModelView>();

            var i = 1;
            foreach (var album in albumList)
            {
                var objects = await _amazonS3.ListObjectsV2Async(new ListObjectsV2Request()
                {
                    BucketName = _vvotSettings.Bucket,
                    Prefix = album + "/",
                });
                var model = new List<AlbumModelView>();

                foreach (var obj in objects.S3Objects)
                {
                    var expiryUrlRequest = new GetPreSignedUrlRequest()
                    {
                        BucketName = _vvotSettings.Bucket,
                        Key = obj.Key,
                        Expires = DateTime.Now.AddDays(1),
                    };
                    model.Add(new AlbumModelView()
                    {
                        Title = obj.Key.Replace(album + "/", ""),
                        Url = _amazonS3.GetPreSignedURL(expiryUrlRequest),
                    });
                }

                var albumHtml = await RenderViewHelper.GetHtmlFromRazor("Views/Templates/AlbumModelView.cshtml", model);
                await using (var stream = RenderViewHelper.GenerateStreamFromString(albumHtml))
                {
                    await _amazonS3.PutObjectAsync(new PutObjectRequest()
                    {
                        BucketName = _vvotSettings.Bucket,
                        InputStream = stream,
                        Key = $"album{i}.html",
                    });
                }

                albumHtmlList.Add(new IndexModelView()
                {
                    Url = $"album{i}.html",
                    Name = album
                });
                i++;
            }

            var indexHtml = await RenderViewHelper.GetHtmlFromRazor("Views/Templates/Index.cshtml", albumHtmlList);
            await using (var stream = RenderViewHelper.GenerateStreamFromString(indexHtml))
            {
                await _amazonS3.PutObjectAsync(new PutObjectRequest()
                {
                    BucketName = _vvotSettings.Bucket,
                    InputStream = stream,
                    Key = $"index.html",
                });
            }

            var errorHtml = await RenderViewHelper.GetHtmlFromRazor("Views/Templates/Error.cshtml");
            await using (var stream = RenderViewHelper.GenerateStreamFromString(errorHtml))
            {
                await _amazonS3.PutObjectAsync(new PutObjectRequest()
                {
                    BucketName = _vvotSettings.Bucket,
                    InputStream = stream,
                    Key = $"error.html",
                });
            }

            Console.WriteLine($"https://{_vvotSettings.Bucket}.website.yandexcloud.net/");
        }
    }
}