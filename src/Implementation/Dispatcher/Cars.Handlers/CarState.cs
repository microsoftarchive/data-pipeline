namespace Microsoft.Practices.DataPipeline.Cars
{
    using System;

    public class CarState
    {
        public DateTime LastUpdatedUtc { get; set; }

        public long UpdateCount { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public float Heading { get; set; }

        public int Altitude { get; set; }

        public byte Speed { get; set; }
    }
}