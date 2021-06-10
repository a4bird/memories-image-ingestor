using System;

namespace Memories.Image.Ingestor.Lambda.InboundMessages
{
    public class AccountCreated
    {
        public Guid AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }
}
