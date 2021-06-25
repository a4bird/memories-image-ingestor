namespace Memories.Image.Remover.Lambda
{
    public static class Constants
    {
        public static string ApplicationName => "memories-image-ingestor-lambda";
        public static string BucketName => "my-memories-bucket";
        public static class EnvironmentName
        {
            public static readonly string Local = "Local";
        }
    }
}
