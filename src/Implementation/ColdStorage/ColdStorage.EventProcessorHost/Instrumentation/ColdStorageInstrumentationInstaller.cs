// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Practices.DataPipeline.ColdStorage.Instrumentation
{
    using System.ComponentModel;
    using System.Configuration.Install;

    [RunInstaller(true)]
    public partial class ColdStorageInstrumentationInstaller : Installer
    {
        public ColdStorageInstrumentationInstaller()
        {
            this.Installers.Add(new ColdStorageInstrumentationManager(false, false).GetInstaller());
        }
    }
}