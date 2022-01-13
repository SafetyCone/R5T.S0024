using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0101;
using R5T.D0105;
using R5T.D0108;
using R5T.T0020;
using R5T.T0100;


namespace R5T.S0024
{
    /// <summary>
    /// * Unmappable extension method bases.
    /// * New to-project mappings (with reasons).
    /// * Departed to-project mappings (with reasons).
    /// </summary>
    [OperationMarker]
    public class O008_MapEmbsToProjects : IActionOperation
    {
        #region Static

        public static HumanActionsRequired02 DetermineRequiredHumanActionsForEmbsUnmappableToSingleProject(
            IEnumerable<(ExtensionMethodBase, string)> unmappableEmbsWithReasons)
        {
            var anyUnmappable = unmappableEmbsWithReasons.Any();

            var output = new HumanActionsRequired02
            {
                ReviewExtensionMethodBasesUnmappableToSingleProject = anyUnmappable,
            };

            return output;
        }

        public static HumanActionsRequired03 DetermineRequiredHumanActionsForToProjectMappings(
            IEnumerable<ExtensionMethodBaseToProjectMapping> newToProjectMappings,
            IEnumerable<ExtensionMethodBaseToProjectMapping> departedToProjectMappings)
        {
            var anyNew = newToProjectMappings.Any();
            var anyDeparted = departedToProjectMappings.Any();

            var output = new HumanActionsRequired03
            {
                ReviewDepartedToProjectMappings = anyDeparted,
                ReviewNewToProjectMappings = anyNew,
            };

            return output;
        }

        #endregion


        private IExtensionMethodBaseRepository ExtensionMethodBaseRepository { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }
        private IProjectRepository ProjectRepository { get; }
        private O002_BackupFileBasedRepositoryFiles O002_BackupFileBasedRepositoryFiles { get; }


        public O008_MapEmbsToProjects(
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            INotepadPlusPlusOperator notepadPlusPlusOperator,
            IProjectRepository projectRepository,
            O002_BackupFileBasedRepositoryFiles o002_BackupFileBasedRepositoryFiles)
        {
            this.ExtensionMethodBaseRepository = extensionMethodBaseRepository;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
            this.ProjectRepository = projectRepository;
            this.O002_BackupFileBasedRepositoryFiles = o002_BackupFileBasedRepositoryFiles;
        }

        public async Task Run()
        {
            // Inputs.
            var unmappableExtensionMethodBasesSummaryFilePath = @"C:\Temp\Unmappable Extension Method Bases-Summary.txt";
            var newAndDepartedToProjectMappingsSummaryFilePath = @"C:\Temp\New and Departed Extension Method Bases-Summary.txt";

            // Backup.
            await this.O002_BackupFileBasedRepositoryFiles.Run();

            // Analysis - Unmappable extension method bases.
            // Get input data.
            var extensionMethodBases = await this.ExtensionMethodBaseRepository.GetAllExtensionMethodBases();
            var projects = await this.ProjectRepository.GetAllProjects();
            var repositoryToProjectMappings = await this.ExtensionMethodBaseRepository.GetAllToProjectMappings();

            // Determine new, departed, and unmappable.
            var (newToProjectMappings, departedToProjectMappings, unmappableExtensionMethodBasesWithReasons) = Instances.Operation.DetermineToProjectMappingChanges(
                extensionMethodBases,
                projects,
                repositoryToProjectMappings);

            // Write summary to file.
            // Use a scope so that file is flushed by the time it's needed.
            using (var summaryFile = FileHelper.WriteTextFile(unmappableExtensionMethodBasesSummaryFilePath))
            {
                Instances.Operation.WriteUnmappableToProjectSummaryFile(
                    summaryFile,
                    unmappableExtensionMethodBasesWithReasons);
            }

            // Now prompt for required human actions.
            // Determine required human actions.
            var humanActionsRequiredForUnmappable = O008_MapEmbsToProjects.DetermineRequiredHumanActionsForEmbsUnmappableToSingleProject(
                unmappableExtensionMethodBasesWithReasons);

            var anyHumanActionsRequired = humanActionsRequiredForUnmappable.Any();
            if (anyHumanActionsRequired)
            {
                Console.WriteLine("Human actions are required to map extension method bases to projects.\n");

                // Prompt for required human actions.
                await this.PromptForEmbUnmappableToProjectHumanActions(
                    unmappableExtensionMethodBasesSummaryFilePath,
                    humanActionsRequiredForUnmappable);

                // Repeatedly prompt for mandatory required human actions until they are complete.
                // Note: while no required human actions are actually mandatory for this process, this code shows the desired methodology as practice.
                while (true)
                {
                    // Recalculate analysis data.
                    extensionMethodBases = await this.ExtensionMethodBaseRepository.GetAllExtensionMethodBases();
                    projects = await this.ProjectRepository.GetAllProjects();
                    repositoryToProjectMappings = await this.ExtensionMethodBaseRepository.GetAllToProjectMappings();

                    // Determine new, departed, and unmappable.
                    (newToProjectMappings, departedToProjectMappings, unmappableExtensionMethodBasesWithReasons) = Instances.Operation.DetermineToProjectMappingChanges(
                        extensionMethodBases,
                        projects,
                        repositoryToProjectMappings);

                    // Determine required human actions.
                    humanActionsRequiredForUnmappable = O008_MapEmbsToProjects.DetermineRequiredHumanActionsForEmbsUnmappableToSingleProject(
                        unmappableExtensionMethodBasesWithReasons);

                    // Only remaining mandatory human actions prevent progress.
                    var anyMandatoryHumanActionsRequired = humanActionsRequiredForUnmappable.AnyMandatory();
                    if (!anyMandatoryHumanActionsRequired)
                    {
                        break;
                    }

                    // Prompt for mandatory human actions only.
                    humanActionsRequiredForUnmappable.UnsetNonMandatory();

                    Console.WriteLine("MANDATORY human actions are required to map extension method bases projects.\n");

                    await this.PromptForEmbUnmappableToProjectHumanActions(
                        unmappableExtensionMethodBasesSummaryFilePath,
                        humanActionsRequiredForUnmappable);
                }

                Console.WriteLine("All human actions required to map extension method bases to a single project are complete.\n");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No human actions are required to map extension method bases to a single project.\n");
                Console.WriteLine();
            }

            Console.WriteLine("The extension method bases will now be mapped to projects.");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

            // Now analyze new and departed extension method bases.
            var projectsByIdentity = projects.ToDictionaryByIdentity();
            var extensionMethodBasesByIdentity = extensionMethodBases.ToDictionaryByIdentity();

            // Write summary to file.
            // Use a scope so that file is flushed by the time it's needed.
            using (var summaryFile = FileHelper.WriteTextFile(newAndDepartedToProjectMappingsSummaryFilePath))
            {
                Instances.Operation.WriteNewAndDepartedToProjectMappingsSummaryFile(
                    summaryFile,
                    extensionMethodBasesByIdentity,
                    projectsByIdentity,
                    newToProjectMappings,
                    departedToProjectMappings);
            }

            // Now prompt for required human actions.
            // Determine required human actions.
            var humanActionsRequiredForToProjectMappings = O008_MapEmbsToProjects.DetermineRequiredHumanActionsForToProjectMappings(
                newToProjectMappings,
                departedToProjectMappings);

            var anyHumanActionsRequiredForToProjectMappings = humanActionsRequiredForToProjectMappings.Any();
            if (anyHumanActionsRequiredForToProjectMappings)
            {
                Console.WriteLine("Human actions are required before updating extension method base-to-project mappings in the repository.\n");

                // Prompt for required human actions.
                await this.PromptForToProjectMappingChangesHumanActions(
                    newAndDepartedToProjectMappingsSummaryFilePath,
                    humanActionsRequiredForToProjectMappings);

                // Repeatedly prompt for mandatory required human actions until they are complete.
                // Note: while no required human actions are actually mandatory for this process, this code shows the desired methodology as practice.
                while (true)
                {
                    // Recalculate analysis data (same data in this case, no recalculation necessary).

                    // Determine required human actions.
                    humanActionsRequiredForToProjectMappings = O008_MapEmbsToProjects.DetermineRequiredHumanActionsForToProjectMappings(
                        newToProjectMappings,
                        departedToProjectMappings);

                    // Only remaining mandatory human actions prevent progress.
                    var anyMandatoryHumanActionsRequired = humanActionsRequiredForToProjectMappings.AnyMandatory();
                    if (!anyMandatoryHumanActionsRequired)
                    {
                        break;
                    }

                    // Prompt for mandatory human actions only.
                    humanActionsRequiredForToProjectMappings.UnsetNonMandatory();

                    Console.WriteLine("MANDATORY human actions are required before updating the extension method base repository.\n");

                    await this.PromptForToProjectMappingChangesHumanActions(
                        newAndDepartedToProjectMappingsSummaryFilePath,
                        humanActionsRequiredForToProjectMappings);
                }

                Console.WriteLine("All human actions required before updating extension method base-to-project mappings in the repository are complete.\n");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No human actions are required before updating the extension method base-to-project mappings in the repository.\n");
                Console.WriteLine();
            }

            Console.WriteLine("The extension method base repository will now be updated with extension method base-to-project mappings.");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

            // Update the extension method base repository.
            // Deletions first.
            await this.ExtensionMethodBaseRepository.DeleteToProjectMappings(departedToProjectMappings);

            await this.ExtensionMethodBaseRepository.AddToProjectMappings(newToProjectMappings);
        }

        private async Task PromptForToProjectMappingChangesHumanActions(
            string summaryFilePath,
            HumanActionsRequired03 humanActionsRequired)
        {
            await this.NotepadPlusPlusOperator.OpenFilePath(summaryFilePath);

            Console.WriteLine($"Review the summary file (which should be open in Notepad++):\n{summaryFilePath}\n");

            if (humanActionsRequired.ReviewNewToProjectMappings)
            {
                Console.WriteLine("=> Review the list of new to-project mappings for extension method bases.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No new to-project mappings for extension method bases. (ok)");
            }

            if (humanActionsRequired.ReviewDepartedToProjectMappings)
            {
                Console.WriteLine("=> Review the list of old to-project mappings for extension method bases.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No old to-project mappings for extension method bases. (ok)");
            }
        }

        private async Task PromptForEmbUnmappableToProjectHumanActions(
            string summaryFilePath,
            HumanActionsRequired02 humanActionsRequired)
        {
            await this.NotepadPlusPlusOperator.OpenFilePath(summaryFilePath);

            Console.WriteLine($"Review the summary file (which should be open in Notepad++):\n{summaryFilePath}\n");

            // * Review list of extension method bases unmappable to single projects.
            if (humanActionsRequired.ReviewExtensionMethodBasesUnmappableToSingleProject)
            {
                Console.WriteLine("=> Review the list of extension method bases that could not be mapped to a single project.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No extension method bases unmappable to project to review. (ok)\n");
            }
        }
    }
}
