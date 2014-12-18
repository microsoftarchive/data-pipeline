namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    public struct MessageRegistration
    {
        public string Type { get; set; }

        public int Version { get; set; }

        public override string ToString()
        {
            return string.Format(
                "[Type: {0}, Version: {1}]",
                Type, 
                Version);
        }
    }
}