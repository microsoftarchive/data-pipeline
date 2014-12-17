namespace Microsoft.Practices.DataPipeline.Processor.Instrumentation
{
    using System.ComponentModel;
    using System.Configuration.Install;

    [RunInstaller(true)]
    public partial class DispatcherInstrumentationInstaller : Installer
    {
        public DispatcherInstrumentationInstaller()
        {
            this.Installers.Add(new DispatcherInstrumentationManager(false).GetInstaller());
        }
    }
}
