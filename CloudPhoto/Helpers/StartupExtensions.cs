
using Microsoft.Extensions.Configuration;

namespace CloudPhoto.Helpers
{
    public static class StartupExtensions
    {
        public static T GetSettings<T>(this IConfiguration configuration, string name) where T : new()
        {
            var result = new T();
            configuration.GetSection(name).Bind(result);
            return result;
        }
    }
}
