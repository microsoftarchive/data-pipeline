// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Simulator.Tests
{
    using Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator;
    using Microsoft.Practices.DataPipeline.Cars.Messages;

    using Xunit;

    public class WhenSendingMessages
    {
        [Fact]
        [Trait("Running time", "Short")]
        public void MessageTypeWillBeShortTypeName()
        {
            // This assumes that the event consumers are 
            // making the same choice.
            var msg = new UpdateLocationMessage();
            var expected = msg.GetType().Name;
            var actual = MessageSender.DetermineTypeFromMessage(msg);

            Assert.Equal(expected, actual.Item1);
        }

        [Fact]
        [Trait("Running time", "Short")]
        public void MessageTypeVersionAlwaysReturns1()
        {
            // TODO: The final system should not be hard coded
            var msg = new UpdateLocationMessage();

            var actual = MessageSender.DetermineTypeFromMessage(msg);

            Assert.Equal(1, actual.Item2);
        }
    }
}