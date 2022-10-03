using CloudPhoto.Extensions;
using CloudPhoto.Helpers;
using CloudPhoto.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CloudPhoto
{
    public static class Startup
    {
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var configuration = context.Configuration;
            services.AddSingleton(configuration.GetSettings<AppSettings>("AppSettings"));
            
            var cloudSettings = VvotSettings.FromIni( Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config", "cloudphoto", "cloudphotorc"));
            
            services.AddSingleton(cloudSettings);

            var client = AwsHelper.CreateClient(cloudSettings);
            services.AddSingleton(client);
        }
    }
}
