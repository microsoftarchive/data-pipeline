namespace Microsoft.Practices.DataPipeline.Cars.Messages
{
    public class UpdateEngineNotificationMessage
    {
        /// <summary>
        /// The observation timestamp (device), UTC offset, stored as ticks 
        /// </summary>
        public long TimeStamp { get; set; }

        /// <summary>
        /// Latitude in degrees
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Longitude in degrees
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// Heading in degrees
        /// </summary>
        public float Heading { get; set; }

        /// <summary>
        /// Altitude in metres
        /// </summary>
        public int Altitude { get; set; }

        /// <summary>
        /// Speed in km/h
        /// </summary>
        public byte Speed { get; set; }

        /// <summary>
        /// Engine On/Off status
        /// </summary>
        public byte EngineStatus { get; set; }
    }
}