using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using R5T.D0088;
using R5T.D0090;


namespace R5T.S0024
{
    class Program : ProgramAsAServiceBase
    {
        #region Static
        
        static async Task Main()
        {
            //OverridableProcessStartTimeProvider.Override("20211208-104059");

            await Instances.Host.NewBuilder()
                .UseProgramAsAService<Program, T0075.IHostBuilder>()
                .UseHostStartup<HostStartup, T0075.IHostBuilder>(Instances.ServiceAction.AddStartupAction())
                .Build()
                .SerializeConfigurationAudit()
                .SerializeServiceCollectionAudit()
                .RunAsync();
        }

        #endregion
        
        
        public Program(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        
        protected override Task ServiceMain(CancellationToken stoppingToken)
        {
            return this.RunOperation();
        }
        
        private async Task RunOperation()
        {
            await this.ServiceProvider.Run<O000_Main>();

            //await this.ServiceProvider.Run<O900_OpenAllEmbRepositoryFiles>();

            //await this.ServiceProvider.Run<O100_UpdateEmbRepositoryWithCurrentEmbs>();

            //await this.ServiceProvider.Run<O009_UpdateRepositoryWithSelectedEmbs>();
            //await this.ServiceProvider.Run<O008_MapEmbsToProjects>();
            //await this.ServiceProvider.Run<O006_UpdateRepositoryWithAllEmbs>();
            //await this.ServiceProvider.Run<O004_UpdateEmbRepository>();
            //await this.ServiceProvider.Run<O003_PerformRequiredHumanActions>();
            //await this.ServiceProvider.Run<O002_BackupFileBasedRepositoryFiles>();
            //await this.ServiceProvider.Run<O001_AnalyzeAllCurrentEmbs>();
        }
        
        //private async Task RunMethod()
        //{
        
        //}
    }
}