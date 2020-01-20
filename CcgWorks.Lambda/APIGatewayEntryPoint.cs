using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CcgWorks.Lambda
{
    public abstract class APIGatewayEntryPoint<THandler, TConfigure>
        where THandler : class, IRequestHandler
        where TConfigure : IConfigureServices, new()
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly Lazy<IServiceProvider> _serviceProvider;

        protected APIGatewayEntryPoint(IConfigurationBuilder configurationBuilder)
        {
            Configuration = BuildConfiguration(configurationBuilder);
            _serviceCollection = new ServiceCollection();
            _serviceCollection
                .AddSingleton<HttpClient>()
                .AddSingleton<IConfiguration>(Configuration)
                .AddScoped<IRequestHandler, THandler>();

            var configurator = new TConfigure();
            configurator.ConfigureServices(_serviceCollection, Configuration);
            _serviceProvider = new Lazy<IServiceProvider>(() => _serviceCollection.BuildServiceProvider());
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider Services => _serviceProvider.Value;

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public APIGatewayProxyResponse HandleRequest(APIGatewayProxyRequest request, ILambdaContext context)
        {
            using (var scope = Services.CreateScope())
            {
                var requestHandler = scope.ServiceProvider.GetService<IRequestHandler>();
                var response = requestHandler.HandleRequest(request, context).GetAwaiter().GetResult();
                response.EnsureHeader("Access-Control-Allow-Origin", "*");
                return response;
            }
        }

        public void ConfigureAdditionalServices(Action<IServiceCollection> configureServices)
        {
            if (_serviceProvider.IsValueCreated)
                throw new InvalidOperationException("Cannot register additional service once service provider has been built");
            configureServices(_serviceCollection);
        }

        private IConfigurationRoot BuildConfiguration(IConfigurationBuilder configurationBuilder)
        {
            return configurationBuilder
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
