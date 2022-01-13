using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0105;
using R5T.D0108;
using R5T.T0020;
using R5T.T0100;
using R5T.T0100.X002;


namespace R5T.S0024
{
    [OperationMarker]
    public class O009_UpdateRepositoryWithSelectedEmbs : IActionOperation
    {
        #region Static

        private static async Task<(string[] ignoredNames, ExtensionMethodBaseNameSelection[] extensionMethodBaseNameSelections)> LoadAnalysisInputData(
            string temporaryDuplicateNamesTextFilePath,
            string temporaryIgnoredNamesTextFilePath)
        {
            var ignoredNamesHash = await Instances.IgnoredValuesOperator.LoadIgnoredValues(
                temporaryIgnoredNamesTextFilePath);

            var ignoredNames = ignoredNamesHash.OrderAlphabetically().ToArray();

            var duplicateNameSelections = await Instances.Operation.LoadDuplicateExtensionMethodBaseNameSelections(
                temporaryDuplicateNamesTextFilePath);

            return (ignoredNames, duplicateNameSelections);
        }

        private static HumanActionsRequired04 DetermineRequiredHumanActionsForDuplicateNames(
            IEnumerable<string> duplicateNames)
        {
            var anyDuplicateNames = duplicateNames.Any();

            var output = new HumanActionsRequired04
            {
                ReviewDuplicateExtensionMethodBaseNames = anyDuplicateNames,
            };

            return output;
        }

        public static HumanActionsRequired05 DetermineRequiredHumanActionsForSelectedNames(
            IEnumerable<(ExtensionMethodBaseNameSelection, string)> newSelectionsWithReasons,
            IEnumerable<(ExtensionMethodBaseNameSelection, string)> departedSelectionsWithReasons)
        {
            var anyNewSelections = newSelectionsWithReasons.Any();
            var anyDepartedSelections = departedSelectionsWithReasons.Any();

            var output = new HumanActionsRequired05
            {
                ReviewDepartedSelectedNames = anyDepartedSelections,
                ReviewNewSelectedNames = anyNewSelections,
            };

            return output;
        }

        #endregion


        private IExtensionMethodBaseRepository ExtensionMethodBaseRepository { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }
        private O002_BackupFileBasedRepositoryFiles O002_BackupFileBasedRepositoryFiles { get; }


        public O009_UpdateRepositoryWithSelectedEmbs(
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            INotepadPlusPlusOperator notepadPlusPlusOperator,
            O002_BackupFileBasedRepositoryFiles o002_BackupFileBasedRepositoryFiles)
        {
            this.ExtensionMethodBaseRepository = extensionMethodBaseRepository;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
            this.O002_BackupFileBasedRepositoryFiles = o002_BackupFileBasedRepositoryFiles;
        }

        public async Task Run()
        {
            // Backup.
            await this.O002_BackupFileBasedRepositoryFiles.Run();

            // Inputs.
            var temporaryDuplicateNamesTextFilePath = @"C:\Temp\EMBs-Duplicate Name Selections-Temp.txt";
            var temporaryIgnoredNamesTextFilePath = @"C:\Temp\EMBs-Ignored Names-Temp.txt";

            var unspecifiedDuplicatesSummaryFilePath = @"C:\Temp\Summary-Unspecified Duplicate EMB Names.txt";
            var selectedNameChangesSummaryFilePath = @"C:\Temp\Summary-Selected EMB Changes.txt";

            // Get all extension method bases.
            var extensionMethodBases = await this.ExtensionMethodBaseRepository.GetAllExtensionMethodBases();

            // Save ignored and duplicate data to temporary file locations that will be modified during the process.
            var repositoryDuplicateNameSelections = await this.ExtensionMethodBaseRepository.GetAllDuplicateExtensionMethodBaseNameSelections();
            var repositoryIgnoredNames = await this.ExtensionMethodBaseRepository.GetAllIgnoredExtensionMethodBaseNamespacedTypeNames();

            await Instances.Operation.SaveExtensionMethodBaseNameSelections(
                temporaryDuplicateNamesTextFilePath,
                repositoryDuplicateNameSelections);

            await Instances.IgnoredValuesOperator.SaveIgnoredValues(
                temporaryIgnoredNamesTextFilePath,
                repositoryIgnoredNames);

            // Load the ignored and duplicate values from the temporary file locations(to be sure we are really using that data).
            var (ignoredNames, duplicateNameSelections) = await O009_UpdateRepositoryWithSelectedEmbs.LoadAnalysisInputData(
                temporaryDuplicateNamesTextFilePath,
                temporaryIgnoredNamesTextFilePath);

            var (currentNameSelections, unspecifiedDuplicateNameSets) = Instances.Operation.GetSelectedNames(
                extensionMethodBases,
                ignoredNames,
                duplicateNameSelections);

            // Write summary of unspecified duplicates to a file.
            // Use a scope so that file is flushed by the time it's needed.
            using (var summaryFile = FileHelper.WriteTextFile(unspecifiedDuplicatesSummaryFilePath))
            {
                Instances.Operation.WriteUnspecifiedDuplicateNamesSummaryFile(
                    summaryFile,
                    unspecifiedDuplicateNameSets);
            }

            // Now prompt for required human actions.
            // Determine required human actions.
            var humanActionsRequired = O009_UpdateRepositoryWithSelectedEmbs.DetermineRequiredHumanActionsForDuplicateNames(
                unspecifiedDuplicateNameSets.Keys);

            var anyHumanActionsRequired = humanActionsRequired.Any();
            if (anyHumanActionsRequired)
            {
                Console.WriteLine("Human actions are required before determining selected extension method bases.\n");

                // Prompt for required human actions.
                await this.PromptForHumanActionsOnDuplicateNames(
                    unspecifiedDuplicatesSummaryFilePath,
                    temporaryIgnoredNamesTextFilePath,
                    temporaryDuplicateNamesTextFilePath,
                    humanActionsRequired);

                // Repeatedly prompt for mandatory required human actions until they are complete.
                // Note: while no required human actions are actually mandatory for this process, this code shows the desired methodology as practice.
                // Perform analysis loop to demand that all duplicate project names are either ignored, or specified.
                while (true)
                {
                    // Reload possibly modified analysis input data.
                    (ignoredNames, duplicateNameSelections) = await O009_UpdateRepositoryWithSelectedEmbs.LoadAnalysisInputData(
                        temporaryDuplicateNamesTextFilePath,
                        temporaryIgnoredNamesTextFilePath);

                    // Recalculate analysis data (same data in this case, no recalculation necessary).
                    (currentNameSelections, unspecifiedDuplicateNameSets) = Instances.Operation.GetSelectedNames(
                        extensionMethodBases,
                        ignoredNames,
                        duplicateNameSelections);

                    // Determine required human actions.
                    humanActionsRequired = O009_UpdateRepositoryWithSelectedEmbs.DetermineRequiredHumanActionsForDuplicateNames(
                        unspecifiedDuplicateNameSets.Keys);

                    // Only remaining mandatory human actions prevent progress.
                    var anyMandatoryHumanActionsRequired = humanActionsRequired.AnyMandatory();
                    if (!anyMandatoryHumanActionsRequired)
                    {
                        break;
                    }

                    // Prompt for mandatory human actions only.
                    humanActionsRequired.UnsetNonMandatory();

                    Console.WriteLine("MANDATORY human actions are required before determining selected extension method bases.\n");

                    await this.PromptForHumanActionsOnDuplicateNames(
                        unspecifiedDuplicatesSummaryFilePath,
                        temporaryIgnoredNamesTextFilePath,
                        temporaryDuplicateNamesTextFilePath,
                        humanActionsRequired);
                }

                Console.WriteLine("All human actions required to before determining selected extension method bases are complete.\n");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No human actions are required before determining selected extension method bases.\n");
                Console.WriteLine();
            }

            Console.WriteLine("Selected extension method bases can now be determined.");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

            // Use the name selections from above, and load the existing (repository) name selections.
            var repositoryNameSelections = await this.ExtensionMethodBaseRepository.GetAllExtensionMethodBaseNameSelections();

            var (_, _, newNameSelectionsWithReasons, departedNameSelectionsWithReasons)
                = Instances.Operation.GetNewAndDepartedNameSelectionsWithReasons(
                extensionMethodBases,
                repositoryDuplicateNameSelections,
                repositoryNameSelections,
                currentNameSelections,
                repositoryIgnoredNames,
                ignoredNames);

            // Write out the reasons for the selected project changes.
            using (var summaryFile = FileHelper.WriteTextFile(selectedNameChangesSummaryFilePath))
            {
                Instances.Operation.WriteSelectedNameChangesSummaryFile(
                    summaryFile,
                    newNameSelectionsWithReasons,
                    departedNameSelectionsWithReasons,
                    extensionMethodBases);
            }

            var humanActionsRequiredForSelectedNames = O009_UpdateRepositoryWithSelectedEmbs.DetermineRequiredHumanActionsForSelectedNames(
                newNameSelectionsWithReasons,
                departedNameSelectionsWithReasons);

            var anyHumanActionsRequiredForSelectedNames = humanActionsRequiredForSelectedNames.Any();
            if (anyHumanActionsRequiredForSelectedNames)
            {
                Console.WriteLine("Human actions are required before updating the list of selected extension method bases in the extension method base repository.\n");

                // Prompt for required human actions.
                await this.PromptForHumanActionsOnSelectedNames(
                    selectedNameChangesSummaryFilePath,
                    humanActionsRequiredForSelectedNames);

                // Repeatedly prompt for mandatory required human actions until they are complete.
                // Note: while no required human actions are actually mandatory for this process, this code shows the desired methodology as practice.
                while (true)
                {
                    // Recalculate analysis data (same data in this case, no recalculation necessary).

                    // Determine required human actions.
                    humanActionsRequiredForSelectedNames = O009_UpdateRepositoryWithSelectedEmbs.DetermineRequiredHumanActionsForSelectedNames(
                        newNameSelectionsWithReasons,
                        departedNameSelectionsWithReasons);

                    // Only remaining mandatory human actions prevent progress.
                    var anyMandatoryHumanActionsRequired = humanActionsRequiredForSelectedNames.AnyMandatory();
                    if (!anyMandatoryHumanActionsRequired)
                    {
                        break;
                    }

                    // Prompt for mandatory human actions only.
                    humanActionsRequiredForSelectedNames.UnsetNonMandatory();

                    Console.WriteLine("MANDATORY human actions are required before updating the extension method base repository.\n");

                    await this.PromptForHumanActionsOnSelectedNames(
                        selectedNameChangesSummaryFilePath,
                        humanActionsRequiredForSelectedNames);
                }

                Console.WriteLine("All human actions required before updating the extension method bases repository are complete.\n");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No human actions are required before updating the extension method bases repository.\n");
                Console.WriteLine();
            }

            Console.WriteLine("The extension method bases repository will now be updated.");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

            // Update the project repository.
            // Save temporary data back to the repository, ignored and duplicates.
            await this.ExtensionMethodBaseRepository.ClearAllIgnoredExtensionMethodBaseNamespacedTypeNames();
            await this.ExtensionMethodBaseRepository.AddIgnoredExtensionMethodBaseNamespacedTypeNames(ignoredNames);

            await this.ExtensionMethodBaseRepository.ClearAllDuplicateExtensionMethodBaseNameSelections();
            await this.ExtensionMethodBaseRepository.AddDuplicateExtensionMethodBaseNameSelections(duplicateNameSelections);

            // Just clear and add all selected names.
            await this.ExtensionMethodBaseRepository.ClearAllExtensionMethodBaseNameSelections();
            await this.ExtensionMethodBaseRepository.AddExtensionMethodBaseNameSelections(currentNameSelections);
        }

        private async Task PromptForHumanActionsOnSelectedNames(
            string summaryFilePath,
            HumanActionsRequired05 humanActionsRequired)
        {
            await this.NotepadPlusPlusOperator.OpenFilePath(summaryFilePath);

            Console.WriteLine($"Review the summary file (which should be open in Notepad++):\n{summaryFilePath}\n");

            // * New selected project names.
            if (humanActionsRequired.ReviewNewSelectedNames)
            {
                Console.WriteLine($"=> Review the list of new selected extension method base names.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No new selected extension method base names. (ok)\n");
            }
            Console.WriteLine();

            // * Departed selected project names.
            if (humanActionsRequired.ReviewDepartedSelectedNames)
            {
                Console.WriteLine($"=> Review the list of departed selected extension method base names.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No departed selected extension method base names. (ok)\n");
            }
            Console.WriteLine();
        }

        private async Task PromptForHumanActionsOnDuplicateNames(
            string summaryFilePath,
            string ignoredProjectNamesFilePath,
            string duplicateProjectNamesFilePath,
            HumanActionsRequired04 humanActionsRequired)
        {
            await this.NotepadPlusPlusOperator.OpenFilePath(ignoredProjectNamesFilePath);
            await this.NotepadPlusPlusOperator.OpenFilePath(duplicateProjectNamesFilePath);
            await this.NotepadPlusPlusOperator.OpenFilePath(summaryFilePath);

            Console.WriteLine($"Review the summary file (which should be open in Notepad++):\n{summaryFilePath}\n");

            // * New projects.
            if (humanActionsRequired.ReviewDuplicateExtensionMethodBaseNames)
            {
                Console.WriteLine($"=> Review the list of duplicate extension method base names.\n\nSpecify which identity should be assigned to each duplicate extension method base namespaced type name in the duplicate name selections file, or add names that should be ignored to the ignored extension method base names file.\n\nDuplicates:\n{duplicateProjectNamesFilePath}\nIgnored:\n{ignoredProjectNamesFilePath}\n\n(These files should also be open in Notepad++.)\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No duplicate project names to select from or ignore. (ok)\n");
            }
            Console.WriteLine();
        }
    }
}
