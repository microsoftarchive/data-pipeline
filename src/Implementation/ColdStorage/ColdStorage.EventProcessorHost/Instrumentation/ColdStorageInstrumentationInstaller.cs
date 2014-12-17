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