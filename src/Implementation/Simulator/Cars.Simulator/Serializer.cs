namespace Microsoft.Practices.DataPipeline.Cars.Simulator
{
    using System.Text;

    using Newtonsoft.Json;

    public static class Serializer
    {
        public static byte[] ToJsonUTF8(object msg)
        {
            var json = JsonConvert.SerializeObject(msg);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}