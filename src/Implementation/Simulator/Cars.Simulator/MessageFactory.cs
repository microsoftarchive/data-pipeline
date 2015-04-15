// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator
{
    using System;

    using Microsoft.Practices.DataPipeline.Cars.Messages;

    public static class MessageFactory
    {
        public static UpdateLocationMessage LocationFactory(Random random)
        {
            return new UpdateLocationMessage
                       {
                           TimeStamp = DateTime.UtcNow.Ticks,
                           Latitude = random.Next(90),
                           Longitude = random.Next(180),
                           Heading = random.Next(360),
                           Altitude = random.Next(13000),
                           Speed = (byte)random.Next(120)
                       };
        }

        public static UpdateEngineNotificationMessage EngineNotificationFactory(Random random)
        {
            return new UpdateEngineNotificationMessage
            {
                TimeStamp = DateTime.UtcNow.Ticks,
                Latitude = random.Next(90),
                Longitude = random.Next(180),
                Heading = random.Next(360),
                Altitude = random.Next(13000),
                Speed = (byte)random.Next(120),
                EngineStatus = (byte)random.Next(1)
            };
        }
    }
}