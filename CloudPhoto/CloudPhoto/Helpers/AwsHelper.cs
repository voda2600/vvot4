using Amazon.Runtime;
using Amazon.S3;
using CloudPhoto.Settings;

namespace CloudPhoto.Helpers
{
    public static class AwsHelper
    {
        public static IAmazonS3 CreateClient(VvotSettings settings)
        {
            var config = new AmazonS3Config
            {
                ServiceURL = settings.BaseUrl,
            };

            var credentials = new BasicAWSCredentials(settings.AwsAccessKeyId, settings.AwsSecretAccessKey);

            return new AmazonS3Client(credentials, config);
        }
    }
}
