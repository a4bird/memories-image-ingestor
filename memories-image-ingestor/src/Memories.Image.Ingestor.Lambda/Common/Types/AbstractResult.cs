using Memories.Image.Ingestor.Lambda.Common.Enums;

namespace Memories.Image.Ingestor.Lambda.Common.Types
{
    public class AbstractResult
    {
        public Error Error { get; set; }

        public ResultStatus Status { get; set; }

        public bool IsSuccess => Status == ResultStatus.Success;
    }
}
