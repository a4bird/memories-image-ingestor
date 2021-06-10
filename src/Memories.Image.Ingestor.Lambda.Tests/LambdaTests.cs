using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Memories.Image.Ingestor.Lambda.InboundMessages;
using Memories.Image.Ingestor.Tests.Infrastructure;
using Xunit;

namespace Memories.Image.Ingestor.Tests
{
    public class LambdaTests
    {
        [Fact]
        public async Task GivenAnSqsMessage_ThenCallShouldBeMadeToValidationApi()
        {
            using var testContext = new TestContext();
            var sqsBody = new AccountCreated { AccountId = Guid.NewGuid() };
            await testContext.SystemUnderTest.ExecuteWithEvent(sqsBody, "AccountCreated");
            testContext.ValidationApi.HasBeenCalled.Should().BeTrue();
        }

        [Fact]
        public async Task GivenAnSqsMessage_WhenValidationApiCallFails_ThenExceptionShouldBeThrown()
        {
            using var testContext = new TestContext();
            testContext.ValidationApi.ApiCallShouldFail = true;
            var sqsBody = new AccountCreated { AccountId = Guid.NewGuid() };

            await Assert.ThrowsAsync<HttpRequestException>(async () => await testContext.SystemUnderTest.ExecuteWithEvent(sqsBody, "AccountCreated"));
        }
    }
}
