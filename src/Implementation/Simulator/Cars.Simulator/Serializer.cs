// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator
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