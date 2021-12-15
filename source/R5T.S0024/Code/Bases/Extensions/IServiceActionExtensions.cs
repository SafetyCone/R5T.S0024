using System;

using R5T.Quadia.D002;

using R5T.D0048;
using R5T.D0108.I001;
using R5T.T0062;
using R5T.T0063;


namespace R5T.S0024
{
    public static partial class IServiceActionExtensions
    {
        /// <summary>
        /// Adds the <see cref="AllExtensionMethodBasesListingFilePathProvider"/> implementation of <see cref="IAllExtensionMethodBasesListingFilePathProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IAllExtensionMethodBasesListingFilePathProvider> AddAllExtensionMethodBasesListingFilePathProviderAction(this IServiceAction _,
            IServiceAction<IOrganizationSharedDataDirectoryFilePathProvider> organizationSharedDataDirectoryFilePathProviderAction)
        {
            var serviceAction = _.New<IAllExtensionMethodBasesListingFilePathProvider>(services => services.AddAllExtensionMethodBasesListingFilePathProvider(
                organizationSharedDataDirectoryFilePathProviderAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="BackupProjectRepositoryFilePathsProvider"/> implementation of <see cref="IBackupExtensionMethodBaseRepositoryFilePathsProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IBackupExtensionMethodBaseRepositoryFilePathsProvider> AddBackupProjectRepositoryFilePathsProviderAction(this IServiceAction _,
            IServiceAction<IOutputFilePathProvider> outputFilePathProviderAction)
        {
            var serviceAction = _.New<IBackupExtensionMethodBaseRepositoryFilePathsProvider>(services => services.AddBackupProjectRepositoryFilePathsProvider(
                outputFilePathProviderAction));

            return serviceAction;
        }

        public static IServiceAction<HostStartup> AddStartupAction(this IServiceAction _)
        {
            var output = _.New<HostStartup>(services => services.AddHostStartup());

            return output;
        }
    }
}
