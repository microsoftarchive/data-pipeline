namespace Microsoft.Practices.DataPipeline.Cars.Simulator
{
    using System.Threading.Tasks;

    public interface IMessageSender
    {
        Task<bool> SendAsync(string partitionKey, object msg);
    }
}