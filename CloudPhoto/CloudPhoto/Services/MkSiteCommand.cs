using Amazon.S3;
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
        public void MkSite()
        {
        }
    }
}
