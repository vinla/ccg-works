using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CcgWorks.Lambda
{
    public interface IConfigureServices
    {
        void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration);
    }
}