namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator
{
    using System.Threading.Tasks;

    public interface IMessageSender
    {
        Task<bool> SendAsync(string partitionKey, object msg);
    }
}