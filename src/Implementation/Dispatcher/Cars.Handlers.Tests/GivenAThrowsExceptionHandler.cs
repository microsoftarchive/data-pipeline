namespace Microsoft.Practices.DataPipeline.Cars.Handlers.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Cars.Handlers;
    using Microsoft.Practices.DataPipeline.Dispatcher;
    using Microsoft.Practices.DataPipeline.Tests;

    using Xunit;

    public class GivenAThrowsExceptionHandler
    {
        [Fact]
        [Trait("Running time", "Short")]
        [Trait("Kind", "Unit")]
        public async Task WhenHandlingAnything_ThenThrowsWellKnownException()
        {
            var handler = new ThrowsExceptionHandler();

            var body = new byte[] { };
            var headers = new Dictionary<string, string>();
            var context = new ProcessingContext("", "", "");

            await AssertExt.ThrowsAsync<JustForTestingException>(
                () => handler.ExecuteAsync(context, body, headers)
                );
        }
    }
}