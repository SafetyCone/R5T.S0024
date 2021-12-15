using System;

using R5T.D0084.D001;
using R5T.D0096;
using R5T.D0101;
using R5T.D0105;
using R5T.D0108;
using R5T.D0108.I001;
using R5T.D0110;
using R5T.T0062;
using R5T.T0063;


namespace R5T.S0024
{
    public static partial class IServiceActionExtensions
    {
        /// <summary>
        /// Adds the <see cref="O100_UpdateEmbRepositoryWithCurrentEmbs"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O100_UpdateEmbRepositoryWithCurrentEmbs> AddO100_UpdateEmbRepositoryWithCurrentEmbsAction(this IServiceAction _,
            IServiceAction<O001_AnalyzeAllCurrentEmbs> o001_AnalyzeAllCurrentEmbsAction,
            IServiceAction<O002_BackupFileBasedRepositoryFiles> o002_BackupFileBasedRepositoryFilesAction,
            IServiceAction<O003_PerformRequiredHumanActions> o003_PerformRequiredHumanActionsAction,
            IServiceAction<O004_UpdateEmbRepository> o004_UpdateEmbRepositoryAction,
            IServiceAction<O005_UpdateEmbToProjectMappings> o005_UpdateEmbToProjectMappingsAction)
        {
            var serviceAction = _.New<O100_UpdateEmbRepositoryWithCurrentEmbs>(services => services.AddO100_UpdateEmbRepositoryWithCurrentEmbs(
                o001_AnalyzeAllCurrentEmbsAction,
                o002_BackupFileBasedRepositoryFilesAction,
                o003_PerformRequiredHumanActionsAction,
                o004_UpdateEmbRepositoryAction,
                o005_UpdateEmbToProjectMappingsAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="O005_UpdateEmbToProjectMappings"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O005_UpdateEmbToProjectMappings> AddO005_UpdateEmbToProjectMappingsAction(this IServiceAction _,
            IServiceAction<IExtensionMethodBaseRepository> extensionMethodBaseRepositoryAction,
            IServiceAction<IProjectRepository> projectRepositoryAction)
        {
            var serviceAction = _.New<O005_UpdateEmbToProjectMappings>(services => services.AddO005_UpdateEmbToProjectMappings(
                extensionMethodBaseRepositoryAction,
                projectRepositoryAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="O900_OpenAllEmbRepositoryFiles"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O900_OpenAllEmbRepositoryFiles> AddO900_OpenAllEmbRepositoryFilesAction(this IServiceAction _,
            IServiceAction<IExtensionMethodBaseRepositoryFilePathsProvider> extensionMethodBaseRepositoryFilePathsProviderAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction)
        {
            var serviceAction = _.New<O900_OpenAllEmbRepositoryFiles>(services => services.AddO900_OpenAllEmbRepositoryFiles(
                extensionMethodBaseRepositoryFilePathsProviderAction,
                notepadPlusPlusOperatorAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="O004_UpdateEmbRepository"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O004_UpdateEmbRepository> AddO004_UpdateEmbRepositoryAction(this IServiceAction _,
            IServiceAction<IAllProjectDirectoryPathsProvider> allProjectDirectoryPathsProviderAction,
            IServiceAction<IAllExtensionMethodBasesListingFilePathProvider> allExtensionMethodBasesListingFilePathProviderAction,
            IServiceAction<IExtensionMethodBaseRepository> extensionMethodBaseRepositoryAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction)
        {
            var serviceAction = _.New<O004_UpdateEmbRepository>(services => services.AddO004_UpdateEmbRepository(
                allProjectDirectoryPathsProviderAction,
                allExtensionMethodBasesListingFilePathProviderAction,
                extensionMethodBaseRepositoryAction,
                notepadPlusPlusOperatorAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="O003a_PromptForRequiredHumanActions"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O003a_PromptForRequiredHumanActions> AddO003a_PromptForRequiredHumanActionsAction(this IServiceAction _,
            IServiceAction<IExtensionMethodBaseRepositoryFilePathsProvider> extensionMethodBaseRepositoryFilePathsProviderAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction,
            IServiceAction<ISummaryFilePathProvider> summaryFilePathProviderAction)
        {
            var serviceAction = _.New<O003a_PromptForRequiredHumanActions>(services => services.AddO003a_PromptForRequiredHumanActions(
                extensionMethodBaseRepositoryFilePathsProviderAction,
                notepadPlusPlusOperatorAction,
                summaryFilePathProviderAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="O003_PerformRequiredHumanActions"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O003_PerformRequiredHumanActions> AddO003_PerformRequiredHumanActionsAction(this IServiceAction _,
            IServiceAction<IAllProjectDirectoryPathsProvider> allProjectDirectoryPathsProviderAction,
            IServiceAction<IExtensionMethodBaseRepository> extensionMethodBaseRepositoryAction,
            IServiceAction<IProjectRepository> projectRepositoryAction,
            IServiceAction<O003a_PromptForRequiredHumanActions> o003a_PromptForRequiredHumanActionsAction)
        {
            var serviceAction = _.New<O003_PerformRequiredHumanActions>(services => services.AddO003_PerformRequiredHumanActions(
                allProjectDirectoryPathsProviderAction,
                extensionMethodBaseRepositoryAction,
                projectRepositoryAction,
                o003a_PromptForRequiredHumanActionsAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="O002_BackupFileBasedRepositoryFiles"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O002_BackupFileBasedRepositoryFiles> AddO002_BackupFileBasedRepositoryFilesAction(this IServiceAction _,
            IServiceAction<IBackupExtensionMethodBaseRepositoryFilePathsProvider> backupExtensionMethodBaseRepositoryFilePathsProviderAction,
            IServiceAction<IHumanOutput> humanOutputAction,
            IServiceAction<IExtensionMethodBaseRepositoryFilePathsProvider> extensionMethodBaseRepositoryFilePathsProviderAction)
        {
            var serviceAction = _.New<O002_BackupFileBasedRepositoryFiles>(services => services.AddO002_BackupFileBasedRepositoryFiles(
                backupExtensionMethodBaseRepositoryFilePathsProviderAction,
                humanOutputAction,
                extensionMethodBaseRepositoryFilePathsProviderAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="O001_AnalyzeAllCurrentEmbs"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<O001_AnalyzeAllCurrentEmbs> AddO001_AnalyzeAllCurrentEmbsAction(this IServiceAction _,
            IServiceAction<IAllProjectDirectoryPathsProvider> allProjectDirectoryPathsProviderAction,
            IServiceAction<IExtensionMethodBaseRepository> extensionMethodBaseRepositoryAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction,
            IServiceAction<IProjectRepository> projectRepositoryAction,
            IServiceAction<ISummaryFilePathProvider> summaryFilePathProviderAction)
        {
            var serviceAction = _.New<O001_AnalyzeAllCurrentEmbs>(services => services.AddO001_AnalyzeAllCurrentEmbs(
                allProjectDirectoryPathsProviderAction,
                extensionMethodBaseRepositoryAction,
                notepadPlusPlusOperatorAction,
                projectRepositoryAction,
                summaryFilePathProviderAction));

            return serviceAction;
        }
    }
}
