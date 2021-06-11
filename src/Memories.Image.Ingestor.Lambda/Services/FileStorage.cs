using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Memories.Image.Ingestor.Lambda.Common.Types;
using Microsoft.Extensions.Configuration;

namespace Memories.Image.Ingestor.Lambda.Services
{
    public interface IFileStorage
    {
        Task<Result> IsFileExists(string bucketName, string objectKey);
    };

    public class FileStorage: IFileStorage
    {
        private readonly IAmazonS3 _client;
        public FileStorage(IConfiguration Configuration)
        {
            var options = Configuration.GetAWSOptions();
            _client = options.CreateServiceClient<IAmazonS3>();
        }
        public async Task<Result> IsFileExists(string bucketName, string objectKey)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                await _client.GetObjectMetadataAsync(request);

                return Result.Ok;


            }
            catch (AmazonS3Exception e)
            {
                return Result.Fail(new Error
                {
                    Message = e.Message
                });
            }
        }
    }
}
