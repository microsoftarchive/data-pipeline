// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.ColdStorage
{
    public class ColdStorageEvent
    {
        public string MessageType { get; set; }
        public string Offset { get; set; }
        public string Payload { get; set; }
    }
}