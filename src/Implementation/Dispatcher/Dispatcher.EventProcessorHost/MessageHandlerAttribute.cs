// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Dispatcher
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MessageHandlerAttribute : Attribute
    {
        public string MessageType { get; set; }

        public int Version { get; set; }
    }
}