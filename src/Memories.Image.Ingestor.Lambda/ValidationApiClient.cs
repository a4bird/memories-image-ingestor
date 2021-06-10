using System.Net.Http;
using System.Threading.Tasks;
using Memories.Image.Ingestor.Lambda.InboundMessages;

namespace Memories.Image.Ingestor.Lambda
{
    public class ValidationApiClient
    {
        private readonly HttpClient _client;
        
        public ValidationApiClient(HttpClient client) => _client = client;

        public async Task ValidateAccount(AccountCreated accountCreated)
        {
            var accountValidation = new AccountValidation
            {
                FirstName = accountCreated.FirstName,
                LastName = accountCreated.LastName,
                EmailAddress = accountCreated.EmailAddress
            };

            var response = await _client.PostAsJsonAsync($"accounts/{accountCreated.AccountId}/validate", accountValidation);
            response.EnsureSuccessStatusCode();
        }

        private class AccountValidation
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
        }
    }
}
