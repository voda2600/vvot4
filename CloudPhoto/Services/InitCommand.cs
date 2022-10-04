using Amazon.S3.Model;
using CloudPhoto.Helpers;
using CloudPhoto.Settings;

namespace CloudPhoto.Services
{

    public class InitCommand : ConsoleAppBase
    {
        private static readonly string InitPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config", "cloudphoto", "cloudphotorc");
        
        [Command("init")]
        public async Task Handle()
        {
            var cloudSettings = GetCloudSettings();
            SaveCloudSettings(cloudSettings);
            await CreateBucket(cloudSettings);
        }

        private VvotSettings GetCloudSettings()
        {
            var cloudSettings = new VvotSettings
            {
                BaseUrl = @"https://storage.yandexcloud.net"
            };
            
            Console.WriteLine("Добро пожаловать в cloudphoto!");

            Console.Write("Пожалуйста, введите aws_access_key_id: ");
            cloudSettings.AwsAccessKeyId = Console.ReadLine() ?? throw new ArgumentException();

            Console.Write("Теперь введите aws_secret_access_key: ");
            cloudSettings.AwsSecretAccessKey = Console.ReadLine() ?? throw new ArgumentException();

            Console.Write("Введите бакет: ");
            cloudSettings.Bucket = Console.ReadLine() ?? throw new ArgumentException();

            return cloudSettings;
        }

        private void SaveCloudSettings(VvotSettings vvotSettings)
        {
            EnsureDirectoryExists(InitPath);
            using var streamWriter = new StreamWriter(InitPath);
            streamWriter.WriteLine("[DEFAULT]");
            streamWriter.WriteLine($"bucket = {vvotSettings.Bucket}");
            streamWriter.WriteLine($"aws_access_key_id = {vvotSettings.AwsAccessKeyId}");
            streamWriter.WriteLine($"aws_secret_access_key = {vvotSettings.AwsSecretAccessKey}");
            streamWriter.WriteLine("region = ru-central1");
            streamWriter.WriteLine($"endpoint_url = {vvotSettings.BaseUrl}");
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            var fi = new FileInfo(filePath);
            if (!fi.Directory!.Exists) Directory.CreateDirectory(fi.DirectoryName!);
        }

        private async Task CreateBucket(VvotSettings vvotSettings)
        {
            using var client = AwsHelper.CreateClient(vvotSettings);
            var bucket = vvotSettings.Bucket;
            var doesBucketExist = await client.DoesS3BucketExistAsync(bucket);
            if (!doesBucketExist)
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucket
                };

                await client.PutBucketAsync(request);
            }
        }
    }
}
