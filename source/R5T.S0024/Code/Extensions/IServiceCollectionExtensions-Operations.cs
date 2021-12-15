using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.D0084.D001;
using R5T.D0096;
using R5T.D0101;
using R5T.D0105;
using R5T.D0108;
using R5T.D0108.I001;
using R5T.D0110;
using R5T.T0063;


namespace R5T.S0024
{
    public static partial class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="O100_UpdateEmbRepositoryWithCurrentEmbs"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO100_UpdateEmbRepositoryWithCurrentEmbs(this IServiceCollection services,
            IServiceAction<O001_AnalyzeAllCurrentEmbs> o001_AnalyzeAllCurrentEmbsAction,
            IServiceAction<O002_BackupFileBasedRepositoryFiles> o002_BackupFileBasedRepositoryFilesAction,
            IServiceAction<O003_PerformRequiredHumanActions> o003_PerformRequiredHumanActionsAction,
            IServiceAction<O004_UpdateEmbRepository> o004_UpdateEmbRepositoryAction,
            IServiceAction<O005_UpdateEmbToProjectMappings> o005_UpdateEmbToProjectMappingsAction)
        {
            services
                .Run(o001_AnalyzeAllCurrentEmbsAction)
                .Run(o002_BackupFileBasedRepositoryFilesAction)
                .Run(o003_PerformRequiredHumanActionsAction)
                .Run(o004_UpdateEmbRepositoryAction)
                .Run(o005_UpdateEmbToProjectMappingsAction)
                .AddSingleton<O100_UpdateEmbRepositoryWithCurrentEmbs>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="O005_UpdateEmbToProjectMappings"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO005_UpdateEmbToProjectMappings(this IServiceCollection services,
            IServiceAction<IExtensionMethodBaseRepository> extensionMethodBaseRepositoryAction,
            IServiceAction<IProjectRepository> projectRepositoryAction)
        {
            services
                .Run(extensionMethodBaseRepositoryAction)
                .Run(projectRepositoryAction)
                .AddSingleton<O005_UpdateEmbToProjectMappings>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="O004_UpdateEmbRepository"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO004_UpdateEmbRepository(this IServiceCollection services,
            IServiceAction<IAllProjectDirectoryPathsProvider> allProjectDirectoryPathsProviderAction,
            IServiceAction<IAllExtensionMethodBasesListingFilePathProvider> allExtensionMethodBasesListingFilePathProviderAction,
            IServiceAction<IExtensionMethodBaseRepository> extensionMethodBaseRepositoryAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction)
        {
            services
                .Run(allProjectDirectoryPathsProviderAction)
                .Run(allExtensionMethodBasesListingFilePathProviderAction)
                .Run(extensionMethodBaseRepositoryAction)
                .Run(notepadPlusPlusOperatorAction)
                .AddSingleton<O004_UpdateEmbRepository>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="O900_OpenAllEmbRepositoryFiles"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO900_OpenAllEmbRepositoryFiles(this IServiceCollection services,
            IServiceAction<IExtensionMethodBaseRepositoryFilePathsProvider> extensionMethodBaseRepositoryFilePathsProviderAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction)
        {
            services
                .Run(extensionMethodBaseRepositoryFilePathsProviderAction)
                .Run(notepadPlusPlusOperatorAction)
                .AddSingleton<O900_OpenAllEmbRepositoryFiles>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="O003a_PromptForRequiredHumanActions"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO003a_PromptForRequiredHumanActions(this IServiceCollection services,
            IServiceAction<IExtensionMethodBaseRepositoryFilePathsProvider> extensionMethodBaseRepositoryFilePathsProviderAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction,
            IServiceAction<ISummaryFilePathProvider> summaryFilePathProviderAction)
        {
            services
                .Run(extensionMethodBaseRepositoryFilePathsProviderAction)
                .Run(notepadPlusPlusOperatorAction)
                .Run(summaryFilePathProviderAction)
                .AddSingleton<O003a_PromptForRequiredHumanActions>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="O003_PerformRequiredHumanActions"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO003_PerformRequiredHumanActions(this IServiceCollection services,
            IServiceAction<IAllProjectDirectoryPathsProvider> allProjectDirectoryPathsProviderAction,
            IServiceAction<IExtensionMethodBaseRepository> extensionMethodBaseRepositoryAction,
            IServiceAction<IProjectRepository> projectRepositoryAction,
            IServiceAction<O003a_PromptForRequiredHumanActions> o003a_PromptForRequiredHumanActionsAction)
        {
            services
                .Run(allProjectDirectoryPathsProviderAction)
                .Run(extensionMethodBaseRepositoryAction)
                .Run(projectRepositoryAction)
                .Run(o003a_PromptForRequiredHumanActionsAction)
                .AddSingleton<O003_PerformRequiredHumanActions>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="O002_BackupFileBasedRepositoryFiles"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO002_BackupFileBasedRepositoryFiles(this IServiceCollection services,
            IServiceAction<IBackupExtensionMethodBaseRepositoryFilePathsProvider> backupExtensionMethodBaseRepositoryFilePathsProviderAction,
            IServiceAction<IHumanOutput> humanOutputAction,
            IServiceAction<IExtensionMethodBaseRepositoryFilePathsProvider> extensionMethodBaseRepositoryFilePathsProviderAction)
        {
            services
                .Run(backupExtensionMethodBaseRepositoryFilePathsProviderAction)
                .Run(humanOutputAction)
                .Run(extensionMethodBaseRepositoryFilePathsProviderAction)
                .AddSingleton<O002_BackupFileBasedRepositoryFiles>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="O001_AnalyzeAllCurrentEmbs"/> operation as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddO001_AnalyzeAllCurrentEmbs(this IServiceCollection services,
            IServiceAction<IAllProjectDirectoryPathsProvider> allProjectDirectoryPathsProviderAction,
            IServiceAction<IExtensionMethodBaseRepository> extensionMethodBaseRepositoryAction,
            IServiceAction<INotepadPlusPlusOperator> notepadPlusPlusOperatorAction,
            IServiceAction<IProjectRepository> projectRepositoryAction,
            IServiceAction<ISummaryFilePathProvider> summaryFilePathProviderAction)
        {
            services
                .Run(allProjectDirectoryPathsProviderAction)
                .Run(extensionMethodBaseRepositoryAction)
                .Run(notepadPlusPlusOperatorAction)
                .Run(summaryFilePathProviderAction)
                .Run(projectRepositoryAction)
                .AddSingleton<O001_AnalyzeAllCurrentEmbs>();

            return services;
        }
    }
}
