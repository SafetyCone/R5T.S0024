using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using R5T.Magyar;
using R5T.Ostrogothia.Rivet;

using R5T.A0003;
using R5T.D0048.Default;
using R5T.D0081.I001;
using R5T.D0084.I0001;
using R5T.D0088.I0002;
using R5T.D0101.I0001;
using R5T.D0101.I001;
using R5T.D0105.I001;
using R5T.D0108.I0001;
using R5T.D0108.I001;
using R5T.D0110.I001;
using R5T.D0110.I002;
using R5T.T0063;

using IProvidedServiceActionAggregation = R5T.D0088.I0002.IProvidedServiceActionAggregation;
using IRequiredServiceActionAggregation = R5T.D0088.I0002.IRequiredServiceActionAggregation;
using ServicesPlatformRequiredServiceActionAggregation = R5T.A0003.RequiredServiceActionAggregation;


namespace R5T.S0024
{
    public class HostStartup : HostStartupBase
    {
        public override Task ConfigureConfiguration(IConfigurationBuilder configurationBuilder)
        {
            // Do nothing.

            return Task.CompletedTask;
        }

        protected override Task ConfigureServices(IServiceCollection services, IProvidedServiceActionAggregation providedServicesAggregation)
        {
            // Inputs.
            var executionSynchronicityProviderAction = Instances.ServiceAction.AddConstructorBasedExecutionSynchronicityProviderAction(Synchronicity.Synchronous);
            var organizationProviderAction = Instances.ServiceAction.AddOrganizationProviderAction(); // Rivet organization.
            var rootOutputDirectoryPathProviderAction = Instances.ServiceAction.AddConstructorBasedRootOutputDirectoryPathProviderAction(@"C:\Temp\Output");

            // Services platform.
            var servicesPlatformRequiredServiceActionAggregation = new ServicesPlatformRequiredServiceActionAggregation
            {
                ConfigurationAction = providedServicesAggregation.ConfigurationAction,
                ExecutionSynchronicityProviderAction = executionSynchronicityProviderAction,
                LoggerAction = providedServicesAggregation.LoggerAction,
                LoggerFactoryAction = providedServicesAggregation.LoggerFactoryAction,
                MachineMessageOutputSinkProviderActions = EnumerableHelper.Empty<IServiceAction<D0099.D002.IMachineMessageOutputSinkProvider>>(),
                MachineMessageTypeJsonSerializationHandlerActions = EnumerableHelper.Empty<IServiceAction<D0098.IMachineMessageTypeJsonSerializationHandler>>(),
                OrganizationProviderAction = organizationProviderAction,
                RootOutputDirectoryPathProviderAction = rootOutputDirectoryPathProviderAction,
            };

            var servicesPlatform = Instances.ServiceAction.AddProvidedServiceActionAggregation(
                servicesPlatformRequiredServiceActionAggregation);

            // Utility services.
            var notepadPlusPlusExecutableFilePathProviderAction = Instances.ServiceAction.AddHardCodedNotepadPlusPlusExecutableFilePathProviderAction();
            var notepadPlusPlusOperatorAction = Instances.ServiceAction.AddNotepadPlusPlusOperatorAction(
                servicesPlatform.BaseCommandLineOperatorAction,
                notepadPlusPlusExecutableFilePathProviderAction);

            // Core competencies.
            var allExtensionMethodBasesListingFilePathProviderAction = Instances.ServiceAction.AddAllExtensionMethodBasesListingFilePathProviderAction(
                servicesPlatform.OrganizationSharedDataDirectoryFilePathProviderAction);

            var summaryFileNameProviderAction = Instances.ServiceAction.AddConstructorBasedSummaryFileNameProviderAction("Summary-Current Extension Method Bases Analysis.txt");
            var summaryFilePathProviderAction = Instances.ServiceAction.AddSummaryFilePathProviderAction(
                servicesPlatform.OutputFilePathProviderAction,
                summaryFileNameProviderAction);

            // Extension method base repository.
            var extensionMethodBaseRepositoryFilePathsProviderAction = Instances.ServiceAction.AddHardCodedExtensionMethodBaseRepositoryFilePathsProviderAction();
            var backupProjectRepositoryFilePathsProviderAction = Instances.ServiceAction.AddBackupProjectRepositoryFilePathsProviderAction(
                servicesPlatform.OutputFilePathProviderAction);

            var fileBasedExtensionMethodBaseRepositoryAction = Instances.ServiceAction.AddFileBasedExtensionMethodBaseRepositoryAction(
                extensionMethodBaseRepositoryFilePathsProviderAction);

            var extensionMethodBaseRepositoryAction = Instances.ServiceAction.ForwardToIExtensionMethodBaseRepositoryAction(
                fileBasedExtensionMethodBaseRepositoryAction);

            // Project repository.
            var projectRepositoryFilePathsProviderAction = Instances.ServiceAction.AddHardCodedProjectRepositoryFilePathsProviderAction();

            var fileBasedProjectRepositoryAction = Instances.ServiceAction.AddFileBasedProjectRepositoryAction(
                projectRepositoryFilePathsProviderAction);

            var projectRepositoryAction = Instances.ServiceAction.ForwardFileBasedProjectRepositoryToProjectRepositoryAction(
                fileBasedProjectRepositoryAction);

            // Project file paths.
            var allProjectDirectoryPathsProviderAction = Instances.ServiceAction.AddAllProjectDirectoryPathsProviderAction(
                projectRepositoryAction,
                servicesPlatform.StringlyTypedPathOperatorAction);

            // Operations.
            // Level 01.
            var o001_AnalyzeAllCurrentEmbsAction = Instances.ServiceAction.AddO001_AnalyzeAllCurrentEmbsAction(
                allProjectDirectoryPathsProviderAction,
                extensionMethodBaseRepositoryAction,
                notepadPlusPlusOperatorAction,
                projectRepositoryAction,
                summaryFilePathProviderAction);
            var o002_BackupFileBasedRepositoryFilesAction = Instances.ServiceAction.AddO002_BackupFileBasedRepositoryFilesAction(
                backupProjectRepositoryFilePathsProviderAction,
                servicesPlatform.HumanOutputAction,
                extensionMethodBaseRepositoryFilePathsProviderAction);
            var o003a_PromptForRequiredHumanActionsAction = Instances.ServiceAction.AddO003a_PromptForRequiredHumanActionsAction(
                extensionMethodBaseRepositoryFilePathsProviderAction,
                notepadPlusPlusOperatorAction,
                summaryFilePathProviderAction);
            var o004_UpdateEmbRepositoryAction = Instances.ServiceAction.AddO004_UpdateEmbRepositoryAction(
                allProjectDirectoryPathsProviderAction,
                allExtensionMethodBasesListingFilePathProviderAction,
                extensionMethodBaseRepositoryAction,
                notepadPlusPlusOperatorAction);
            var o005_UpdateEmbToProjectMappingsAction = Instances.ServiceAction.AddO005_UpdateEmbToProjectMappingsAction(
                extensionMethodBaseRepositoryAction,
                projectRepositoryAction);
            var o007_WriteOutAllEmbsAction = Instances.ServiceAction.AddO007_WriteOutAllEmbsAction(
                allExtensionMethodBasesListingFilePathProviderAction,
                extensionMethodBaseRepositoryAction,
                notepadPlusPlusOperatorAction);

            // Level 02.
            var o003_PerformRequiredHumanActionsAction = Instances.ServiceAction.AddO003_PerformRequiredHumanActionsAction(
                allProjectDirectoryPathsProviderAction,
                extensionMethodBaseRepositoryAction,
                projectRepositoryAction,
                o003a_PromptForRequiredHumanActionsAction);
            var o006_UpdateRepositoryWithAllEmbsAction = Instances.ServiceAction.AddO006_UpdateRepositoryWithAllEmbsAction(
                allProjectDirectoryPathsProviderAction,
                extensionMethodBaseRepositoryAction,
                notepadPlusPlusOperatorAction,
                summaryFilePathProviderAction,
                o002_BackupFileBasedRepositoryFilesAction,
                o007_WriteOutAllEmbsAction);
            var o008_MapEmbsToProjectsAction = Instances.ServiceAction.AddO008_MapEmbsToProjectsAction(
                extensionMethodBaseRepositoryAction,
                notepadPlusPlusOperatorAction,
                projectRepositoryAction,
                o002_BackupFileBasedRepositoryFilesAction);
            var o009_UpdateRepositoryWithSelectedEmbsAction = Instances.ServiceAction.AddO009_UpdateRepositoryWithSelectedEmbsAction(
                extensionMethodBaseRepositoryAction,
                notepadPlusPlusOperatorAction,
                o002_BackupFileBasedRepositoryFilesAction);

            var o100_UpdateEmbRepositoryWithCurrentEmbsAction = Instances.ServiceAction.AddO100_UpdateEmbRepositoryWithCurrentEmbsAction(
                o001_AnalyzeAllCurrentEmbsAction,
                o002_BackupFileBasedRepositoryFilesAction,
                o003_PerformRequiredHumanActionsAction,
                o004_UpdateEmbRepositoryAction,
                o005_UpdateEmbToProjectMappingsAction);

            var o900_OpenAllEmbRepositoryFilesAction = Instances.ServiceAction.AddO900_OpenAllEmbRepositoryFilesAction(
                extensionMethodBaseRepositoryFilePathsProviderAction,
                notepadPlusPlusOperatorAction);

            // Level 03.
            var o000_MainAction = Instances.ServiceAction.AddO000_MainAction(
                o100_UpdateEmbRepositoryWithCurrentEmbsAction);

            // Run.
            services
                .Run(servicesPlatform.ConfigurationAuditSerializerAction)
                .Run(servicesPlatform.ServiceCollectionAuditSerializerAction)
                // Operations.
                .Run(o000_MainAction)
                .Run(o001_AnalyzeAllCurrentEmbsAction)
                .Run(o002_BackupFileBasedRepositoryFilesAction)
                .Run(o003_PerformRequiredHumanActionsAction)
                .Run(o006_UpdateRepositoryWithAllEmbsAction)
                .Run(o008_MapEmbsToProjectsAction)
                .Run(o009_UpdateRepositoryWithSelectedEmbsAction)
                .Run(o100_UpdateEmbRepositoryWithCurrentEmbsAction)
                .Run(o900_OpenAllEmbRepositoryFilesAction)
                ;

            return Task.CompletedTask;
        }

        protected override Task FillRequiredServiceActions(IRequiredServiceActionAggregation requiredServiceActions)
        {
            // Do nothing since none are required.

            return Task.CompletedTask;
        }
    }
}
