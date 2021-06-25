using Memories.Image.Remover.Lambda.Common.Enums;

namespace Memories.Image.Remover.Lambda.Common.Types
{
    public class AbstractResult
    {
        public Error Error { get; set; }

        public ResultStatus Status { get; set; }

        public bool IsSuccess => Status == ResultStatus.Success;
    }
}
