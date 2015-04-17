// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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