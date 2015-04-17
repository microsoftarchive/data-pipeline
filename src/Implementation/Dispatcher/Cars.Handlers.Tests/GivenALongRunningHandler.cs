// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Handlers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline.Cars.Handlers;
    using Microsoft.Practices.DataPipeline.Dispatcher;
    using Microsoft.Practices.DataPipeline.Tests;

    using Xunit;

    public class GivenALongRunningHandler
    {
        [Fact]
        [Trait("Running time", "Long")]
        [Trait("Category", "Unit")]
        public async Task WhenHandlingAnything_ThenDelaysTheSpecifiedDuration()
        {
            var duration = TimeSpan.FromSeconds(2);

            var handler = new LongRunningHandler(duration);

            var context = new ProcessingContext("", "", "");
            var body = new byte[] { };
            var headers = new Dictionary<string, string>();

            await AssertExt.TaskRanForAtLeast(() => handler.ExecuteAsync(context, body, headers), duration);
        }
    }
}