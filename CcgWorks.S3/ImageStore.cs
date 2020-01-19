using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using CcgWorks.Core;
using Microsoft.Extensions.Options;

namespace CcgWorks.AwsStore
{
    public class ImageStore : IImageStore
    {
        private readonly ImageStoreOptions _options;
        private readonly IAmazonS3 _s3Service;

        public ImageStore(IAmazonS3 s3Service, IOptions<ImageStoreOptions> options)
        {
            _s3Service = s3Service ?? throw new ArgumentNullException(nameof(s3Service));
            _options = options.Value;
        }

        public async Task<string> Add(string associatedObjectType, Guid associatedObjectId, int version, string imageType, byte[] data)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key =  GetKey(associatedObjectType, associatedObjectId),
                InputStream = new MemoryStream(data),
                CannedACL = S3CannedACL.PublicRead
            };

            putRequest.Metadata.Add("ImageType", imageType);
            putRequest.Metadata.Add("Version", version.ToString());

            var response = await _s3Service.PutObjectAsync(putRequest);

            if(response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Failed to store image");
            }

            return _options.DistributionUrl + putRequest.Key;
        }
        
        public async Task<byte[]> Get(string associatedObjectType, Guid associatedObjectId, int version)
        {
            var imageKey = GetKey(associatedObjectType, associatedObjectId);

            var getRequest = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = imageKey,
            };

            try
            {
                var response = await _s3Service.GetObjectAsync(getRequest);
                if (response.Metadata["x-amz-meta-version"] == version.ToString())
                {                
                    using (var binaryReader = new BinaryReader(response.ResponseStream))
                    {
                        return binaryReader.ReadBytes((int)response.ContentLength);                            
                    }
                }
            }
            catch(AmazonS3Exception ex)
            {
                if(ex.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return null;
        }        

        private string GetKey(string type, Guid id)
        {
            return $"{type}/{id}";
        }
    }

    public class ImageStoreOptions
    {
        public string BucketName { get; set; }
        public string DistributionUrl { get; set; }
    }
}