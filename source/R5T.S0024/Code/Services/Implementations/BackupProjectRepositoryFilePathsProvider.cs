using System;
using System.Threading.Tasks;

using R5T.D0048;
using R5T.T0064;


namespace R5T.S0024
{
    [ServiceImplementationMarker]
    public class BackupProjectRepositoryFilePathsProvider : IBackupExtensionMethodBaseRepositoryFilePathsProvider, IServiceImplementation
    {
        private IOutputFilePathProvider OutputFilePathProvider { get; }


        public BackupProjectRepositoryFilePathsProvider(
            IOutputFilePathProvider outputFilePathProvider)
        {
            this.OutputFilePathProvider = outputFilePathProvider;
        }

        public async Task<string> GetDuplicateExtensionMethodBaseNamesTextFilePath()
        {
            var output = await this.OutputFilePathProvider.GetOutputFilePath("Extension Method Bases-Duplicate Name Selections-Backup.txt");
            return output;
        }

        public async Task<string> GetExtensionMethodBaseNameSelectionsTextFilePath()
        {
            var output = await this.OutputFilePathProvider.GetOutputFilePath("Extension Method Bases-Selected-Backup.txt");
            return output;
        }

        public async Task<string> GetExtensionMethodBasesListingJsonFilePath()
        {
            var output = await this.OutputFilePathProvider.GetOutputFilePath("Extension Method Bases-All-Backup.json");
            return output;
        }

        public async Task<string> GetIgnoredExtensionMethodBaseNamesTextFilePath()
        {
            var output = await this.OutputFilePathProvider.GetOutputFilePath("Extension Method Bases-Ignored Names-Backup.txt");
            return output;
        }

        public async Task<string> GetToProjectMappingsJsonFilePath()
        {
            var output = await this.OutputFilePathProvider.GetOutputFilePath("Extension Method Bases-To Project Mappings-Backup.json");
            return output;
        }
    }
}
