using Microsoft.Extensions.DependencyInjection;
using Amazon.SimpleDB;
using CcgWorks.Core;
using Amazon.Extensions.NETCore.Setup;

namespace CcgWorks.SimpleDbStore
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddSimpleDbStore(this IServiceCollection services, AWSOptions awsOptions)
        {
            var simpleDb = services.AddAWSService<IAmazonSimpleDB>(awsOptions);
            services.AddScoped<IGameStore, GameStore>();
            services.AddScoped<ICardStore, CardStore>();
            services.AddScoped<IDeckStore, DeckStore>();
            return services;
        }
    }
}