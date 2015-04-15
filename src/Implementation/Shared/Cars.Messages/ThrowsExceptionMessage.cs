// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.Cars.Messages
{
    // This message represents a "malformed" message. The
    // implication is that the message producer created
    // a message that dispatcher routes to a handler, but
    // that handler is not able to successfully process the
    // message and it throws.
    // This behavior is explicitly modelled in the handler in
    // order to demonstrate how the dispatcher compensates for 
    // handlers that throw.
    public class ThrowsExceptionMessage { }
}