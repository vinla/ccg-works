using System;
using CcgWorks.SimpleDbStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CcgWorks.Lambda.UploadCards
{
     public class Configuration : IConfigureServices
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var awsOptions = configuration.GetAWSOptions();            
            services.AddSimpleDbStore(awsOptions);
            services.AddS3ImageStore(awsOptions, configuration.GetValue<String>(""), configuration.GetValue<String>(""));
        }
    }   
}