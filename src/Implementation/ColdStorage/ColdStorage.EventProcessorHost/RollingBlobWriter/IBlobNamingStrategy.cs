namespace Microsoft.Practices.DataPipeline.ColdStorage.BlobWriter
{
    public interface IBlobNamingStrategy
    {
        string GetNamePrefix();
    }
}