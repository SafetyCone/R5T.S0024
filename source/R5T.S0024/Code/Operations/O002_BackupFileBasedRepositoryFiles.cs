using System;
using System.Threading.Tasks;

using R5T.D0096;
using R5T.D0108.I001;
using R5T.T0020;


namespace R5T.S0024
{
    [OperationMarker]
    public class O002_BackupFileBasedRepositoryFiles : IActionOperation
    {
        private IBackupExtensionMethodBaseRepositoryFilePathsProvider BackupExtensionMethodBaseRepositoryFilePathsProvider { get; }
        private IHumanOutput HumanOutput { get; }
        private IExtensionMethodBaseRepositoryFilePathsProvider ExtensionMethodBaseRepositoryFilePathsProvider { get; }


        public O002_BackupFileBasedRepositoryFiles(
            IBackupExtensionMethodBaseRepositoryFilePathsProvider backupExtensionMethodBaseRepositoryFilePathsProvider,
            IHumanOutput humanOutput,
            IExtensionMethodBaseRepositoryFilePathsProvider extensionMethodBaseRepositoryFilePathsProvider)
        {
            this.BackupExtensionMethodBaseRepositoryFilePathsProvider = backupExtensionMethodBaseRepositoryFilePathsProvider;
            this.HumanOutput = humanOutput;
            this.ExtensionMethodBaseRepositoryFilePathsProvider = extensionMethodBaseRepositoryFilePathsProvider;
        }

        public async Task Run()
        {
            var (Task1Result, Task2Result, Task3Result, Task4Result, Task5Result) = await TaskHelper.WhenAll(
                TaskHelper.WhenAll(
                    this.ExtensionMethodBaseRepositoryFilePathsProvider.GetDuplicateExtensionMethodBaseNamesTextFilePath(),
                    this.BackupExtensionMethodBaseRepositoryFilePathsProvider.GetDuplicateExtensionMethodBaseNamesTextFilePath()),
                TaskHelper.WhenAll(
                    this.ExtensionMethodBaseRepositoryFilePathsProvider.GetExtensionMethodBaseNameSelectionsTextFilePath(),
                    this.BackupExtensionMethodBaseRepositoryFilePathsProvider.GetExtensionMethodBaseNameSelectionsTextFilePath()),
                TaskHelper.WhenAll(
                    this.ExtensionMethodBaseRepositoryFilePathsProvider.GetExtensionMethodBasesListingJsonFilePath(),
                    this.BackupExtensionMethodBaseRepositoryFilePathsProvider.GetExtensionMethodBasesListingJsonFilePath()),
                TaskHelper.WhenAll(
                    this.ExtensionMethodBaseRepositoryFilePathsProvider.GetIgnoredExtensionMethodBaseNamesTextFilePath(),
                    this.BackupExtensionMethodBaseRepositoryFilePathsProvider.GetIgnoredExtensionMethodBaseNamesTextFilePath()),
                TaskHelper.WhenAll(
                    this.ExtensionMethodBaseRepositoryFilePathsProvider.GetToProjectMappingsJsonFilePath(),
                    this.BackupExtensionMethodBaseRepositoryFilePathsProvider.GetToProjectMappingsJsonFilePath()));

            foreach (var fileBackupSourceDestinationPair in new[]
            {
                Task1Result,
                Task2Result,
                Task3Result,
                Task4Result,
                Task5Result,
            })
            {
                var sourceFilePath = fileBackupSourceDestinationPair.Task1Result;
                var destinationFilePath = fileBackupSourceDestinationPair.Task2Result;

                Instances.FileSystemOperator.CopyFile(sourceFilePath, destinationFilePath);

                this.HumanOutput.WriteLine($"File based project repository file back-up copy made:\nSource:\n{sourceFilePath}\nDestination:\n{destinationFilePath}\n");
            }
        }
    }
}
