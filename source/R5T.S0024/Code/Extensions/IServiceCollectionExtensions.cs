using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.Quadia.D002;

using R5T.D0048;
using R5T.D0088.I0002;
using R5T.D0108.I001;
using R5T.T0063;


namespace R5T.S0024
{
    public static partial class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="AllExtensionMethodBasesListingFilePathProvider"/> implementation of <see cref="IAllExtensionMethodBasesListingFilePathProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddAllExtensionMethodBasesListingFilePathProvider(this IServiceCollection services,
            IServiceAction<IOrganizationSharedDataDirectoryFilePathProvider> organizationSharedDataDirectoryFilePathProviderAction)
        {
            services
                .Run(organizationSharedDataDirectoryFilePathProviderAction)
                .AddSingleton<IAllExtensionMethodBasesListingFilePathProvider, AllExtensionMethodBasesListingFilePathProvider>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="BackupProjectRepositoryFilePathsProvider"/> implementation of <see cref="IBackupExtensionMethodBaseRepositoryFilePathsProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddBackupProjectRepositoryFilePathsProvider(this IServiceCollection services,
            IServiceAction<IOutputFilePathProvider> outputFilePathProviderAction)
        {
            services
                .Run(outputFilePathProviderAction)
                .AddSingleton<IBackupExtensionMethodBaseRepositoryFilePathsProvider, BackupProjectRepositoryFilePathsProvider>();

            return services;
        }

        public static IServiceCollection AddHostStartup(this IServiceCollection services)
        {
            var dependencyServiceActions = new DependencyServiceActionAggregation();

            services.AddHostStartup<HostStartup>(dependencyServiceActions)
                // Add services required by HostStartup, but not by HostStartupBase.
                ;

            return services;
        }
    }
}
