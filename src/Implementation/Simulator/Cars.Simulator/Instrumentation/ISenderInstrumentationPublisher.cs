// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator.Instrumentation
{
    using System;

    public interface ISenderInstrumentationPublisher
    {
        void MessageSendRequested();
        void MessageSendCompleted(long length, TimeSpan elapsed);
    }
}