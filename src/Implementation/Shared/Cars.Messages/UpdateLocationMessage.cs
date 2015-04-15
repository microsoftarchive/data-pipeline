// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Messages
{
    public class UpdateLocationMessage
    {
        // The observation timestamp (device), UTC offset, stored as ticks 
        public long TimeStamp { get; set; }
 
        // Latitude in degrees
        public float Latitude { get; set; }

        // Longitude in degrees
        public float Longitude { get; set; }

        // Heading in degrees
        public float Heading { get; set; }

        // Altitude in metres
        public int Altitude { get; set; }

        // Speed in km/h
        public byte Speed { get; set; }
    }
}
