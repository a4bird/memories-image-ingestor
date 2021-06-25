using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Memories.Image.Remover.Lambda.Common.Types;
using Memories.Image.Remover.Lambda.Results;

namespace Memories.Image.Remover.Lambda.Services
{
    public interface ICloudStorage
    {
        Task<Result<ObjectMetadataResult>> GetObjectMetadata(string bucketName, string objectKey);
    };

    public class CloudStorage: ICloudStorage
    {
        private readonly IAmazonS3 _s3Client;

        public CloudStorage(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }
        public async Task<Result<ObjectMetadataResult>> GetObjectMetadata(string bucketName, string objectKey)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                var objMetadata = await _s3Client.GetObjectMetadataAsync(request);
                var objMetadataReult = new ObjectMetadataResult();
                foreach (var propertyInfo in typeof(ObjectMetadataResult).GetProperties())
                {
                    var key = $"x-amz-meta-{propertyInfo.Name.ToLower()}";
                    var propertyValue = Convert.ChangeType(objMetadata.Metadata[key], propertyInfo.PropertyType);
                    propertyInfo.SetValue(objMetadataReult, propertyValue);
                }

                return Result<ObjectMetadataResult>.Ok(objMetadataReult);


            }
            catch (AmazonS3Exception e)
            {
                return Result<ObjectMetadataResult>.Fail(new Error
                {
                    Message = e.Message
                });
            }
        }
    }
}
