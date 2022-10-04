using Amazon.S3;
using Amazon.S3.Model;
using CloudPhoto.Settings;

namespace CloudPhoto.Services
{
    public class UploadCommand : ConsoleAppBase
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly VvotSettings _vvotSettings;
        private readonly AppSettings _appSettings;

        public UploadCommand(IAmazonS3 amazonS3, VvotSettings vvotSettings, AppSettings appSettings)
        {
            _amazonS3 = amazonS3;
            _vvotSettings = vvotSettings;
            _appSettings = appSettings;
        }

        [Command("upload")]
        public async Task Upload(string album, [Option("path")] string? path = null)
        {
            path ??= Environment.CurrentDirectory;
            var files = Directory.GetFiles(path);
            if (!files.Any(file => _appSettings.FileExtensions.Contains(Path.GetExtension(file))))
            {
                throw new ApplicationException("No photos in directory");
            }

            foreach (var file in files)
            {
                if (!_appSettings.FileExtensions.Contains(Path.GetExtension(file))) continue;
                try
                {
                    await _amazonS3.PutObjectAsync(new PutObjectRequest()
                    {
                        BucketName = _vvotSettings.Bucket,
                        FilePath = file,
                        Key = $"{album}/{Path.GetFileName(file)}",
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine($"При отправке файла {Path.GetFileName(file)} произошла ошибка!");
                    Console.WriteLine($"Текст ошибки: {e.Message}");
                }
            }
        }
    }
}
