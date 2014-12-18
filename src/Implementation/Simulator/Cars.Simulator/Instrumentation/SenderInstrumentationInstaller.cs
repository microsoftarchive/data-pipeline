namespace Microsoft.Practices.DataPipeline.Cars.Dispatcher.Simulator.Instrumentation
{
    using System.ComponentModel;
    using System.Configuration.Install;

    [RunInstaller(true)]
    public partial class SenderInstrumentationInstaller : Installer
    {
        public SenderInstrumentationInstaller()
        {
            this.Installers.Add(new SenderInstrumentationManager(false, false).GetInstaller());
        }
    }
}