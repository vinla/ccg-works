using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using CcgWorks.AwsStore;
using CcgWorks.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddS3ImageStore(this IServiceCollection services, AWSOptions awsOptions, string bucketName, string distributionUrl)
        {
            services.Configure<ImageStoreOptions>(opts => {
                opts.BucketName = bucketName;
                opts.DistributionUrl = distributionUrl;
            });
            services.AddAWSService<IAmazonS3>(awsOptions);
            services.AddScoped<IImageStore, ImageStore>();
            return services;
        }
    }
}