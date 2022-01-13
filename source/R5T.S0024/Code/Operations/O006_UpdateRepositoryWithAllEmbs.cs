using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0084.D001;
using R5T.D0105;
using R5T.D0108;
using R5T.D0110;
using R5T.T0020;
using R5T.T0100;


namespace R5T.S0024
{
    /// <summary>
    /// Updates the extension method base repository with ALL current (i.e. in the local filesystem) extension method base types.
    /// </summary>
    [OperationMarker]
    public class O006_UpdateRepositoryWithAllEmbs : IActionOperation
    {
        #region Static

        public static HumanActionsRequired01 DetermineRequiredHumanActions(
            IList<ExtensionMethodBase> newExtensionMethodBases,
            IList<ExtensionMethodBase> departedExtensionMethodBases)
        {
            var anyNew = newExtensionMethodBases.Any();
            var anyDeparted = departedExtensionMethodBases.Any();

            var output = new HumanActionsRequired01
            {
                ReviewDepartedExtensionMethodBases = anyDeparted,
                ReviewNewExtensionMethodBases = anyNew,
            };

            return output;
        }

        #endregion

        private IAllProjectDirectoryPathsProvider AllProjectDirectoryPathsProvider { get; }
        private IExtensionMethodBaseRepository ExtensionMethodBaseRepository { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }
        private ISummaryFilePathProvider SummaryFilePathProvider { get; }
        private O002_BackupFileBasedRepositoryFiles O002_BackupFileBasedRepositoryFiles { get; }
        private O007_WriteOutAllEmbs O007_WriteOutAllEmbs { get; }


        public O006_UpdateRepositoryWithAllEmbs(
            IAllProjectDirectoryPathsProvider allProjectDirectoryPathsProvider,
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            INotepadPlusPlusOperator notepadPlusPlusOperator,
            ISummaryFilePathProvider summaryFilePathProvider,
            O002_BackupFileBasedRepositoryFiles o002_BackupFileBasedRepositoryFiles,
            O007_WriteOutAllEmbs o007_WriteOutAllEmbs)
        {
            this.AllProjectDirectoryPathsProvider = allProjectDirectoryPathsProvider;
            this.ExtensionMethodBaseRepository = extensionMethodBaseRepository;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
            this.SummaryFilePathProvider = summaryFilePathProvider;
            this.O002_BackupFileBasedRepositoryFiles = o002_BackupFileBasedRepositoryFiles;
            this.O007_WriteOutAllEmbs = o007_WriteOutAllEmbs;
        }

        public async Task Run()
        {
            // Backup.
            await this.O002_BackupFileBasedRepositoryFiles.Run();

            // Get all current extension method bases (in the file system).
            // Get all repository extension method bases (previously existing projects).
            var (repositoryExtensionMethodBases, currentExtensionMethodBases) = await Instances.Operation.GetRepositoryAndCurrentExtensionMethodBases(
                this.AllProjectDirectoryPathsProvider,
                this.ExtensionMethodBaseRepository);

            // Analysis.
            // Determine new and departed.
            var (newExtensionMethodBases, departedExtensionMethodBases) = Instances.Operation.GetNewAndDepartedByNameAndFilePath(
                repositoryExtensionMethodBases,
                currentExtensionMethodBases);

            // Write summary to file.
            var summaryFilePath = await this.SummaryFilePathProvider.GetSummaryFilePath();

            // Use a scope so that file is flushed by the time it's needed.
            using (var summaryFile = FileHelper.WriteTextFile(summaryFilePath))
            {
                Instances.Operation.WriteNewAndDepartedEmbsSummaryFile(
                    summaryFile,
                    newExtensionMethodBases,
                    departedExtensionMethodBases);
            }

            // Now prompt for required human actions.
            // Determine required human actions.
            var humanActionsRequired = O006_UpdateRepositoryWithAllEmbs.DetermineRequiredHumanActions(
                newExtensionMethodBases,
                departedExtensionMethodBases);

            var anyHumanActionsRequired = humanActionsRequired.Any();
            if (anyHumanActionsRequired)
            {
                Console.WriteLine("Human actions are required before updating the list of all extension method bases in the repository.\n");

                // Prompt for required human actions.
                await this.PromptForHumanActions(
                    summaryFilePath,
                    humanActionsRequired);

                // Repeatedly prompt for mandatory required human actions until they are complete.
                // Note: while no required human actions are actually mandatory for this process, this code shows the desired methodology as practice.
                while (true)
                {
                    // Recalculate analysis data (same data in this case, no recalculation necessary).

                    // Determine required human actions.
                    humanActionsRequired = O006_UpdateRepositoryWithAllEmbs.DetermineRequiredHumanActions(
                        newExtensionMethodBases,
                        departedExtensionMethodBases);

                    // Only remaining mandatory human actions prevent progress.
                    var anyMandatoryHumanActionsRequired = humanActionsRequired.AnyMandatory();
                    if (!anyMandatoryHumanActionsRequired)
                    {
                        break;
                    }

                    // Prompt for mandatory human actions only.
                    humanActionsRequired.UnsetNonMandatory();

                    Console.WriteLine("MANDATORY human actions are required before updating the extension method base repository.\n");

                    await this.PromptForHumanActions(
                        summaryFilePath,
                        humanActionsRequired);
                }

                Console.WriteLine("All human actions required before updating the extension method base repository are complete.\n");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No human actions are required before updating the extension method base repository.\n");
                Console.WriteLine();
            }

            Console.WriteLine("The extension method base repository will now be updated.");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

            // Update the extension method base repository.
            // Deletions first.
            await this.ExtensionMethodBaseRepository.DeleteExtensionMethodBases(departedExtensionMethodBases);

            await this.ExtensionMethodBaseRepository.AddExtensionMethodBases(newExtensionMethodBases);

            // Now write out the file showing the results of the extension method base survey.
            await this.O007_WriteOutAllEmbs.Run();
        }

        private async Task PromptForHumanActions(
            string summaryFilePath,
            HumanActionsRequired01 humanActionsRequired)
        {
            await this.NotepadPlusPlusOperator.OpenFilePath(summaryFilePath);

            Console.WriteLine($"Review the summary file (which should be open in Notepad++):\n{summaryFilePath}\n");

            // * Review list of new extension method bases.
            if (humanActionsRequired.ReviewNewExtensionMethodBases)
            {
                Console.WriteLine("=> Review the list of new extension method bases.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No new extension method bases to review. (ok)\n");
            }

            // * Review list of departed extension method bases.
            if (humanActionsRequired.ReviewDepartedExtensionMethodBases)
            {
                Console.WriteLine("=> Review the list of departed extension method bases.\nNote: departed extension method base names will be removed from the lists of ignored and selected extension method base names.");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No departed extension method bases to review. (ok)\n");
            }
        }
    }
}
