using System;
using System.Threading.Tasks;

using R5T.D0105;
using R5T.D0108.I001;
using R5T.D0110;


namespace R5T.S0024
{
    public class O003a_PromptForRequiredHumanActions
    {
        private IExtensionMethodBaseRepositoryFilePathsProvider ExtensionMethodBaseRepositoryFilePathsProvider { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }
        private ISummaryFilePathProvider SummaryFilePathProvider { get; }


        public O003a_PromptForRequiredHumanActions(
            INotepadPlusPlusOperator notepadPlusPlusOperator,
            IExtensionMethodBaseRepositoryFilePathsProvider extensionMethodBaseRepositoryFilePathsProvider,
            ISummaryFilePathProvider summaryFilePathProvider)
        {
            this.ExtensionMethodBaseRepositoryFilePathsProvider = extensionMethodBaseRepositoryFilePathsProvider;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
            this.SummaryFilePathProvider = summaryFilePathProvider;
        }

        public async Task Run(HumanActionsRequired humanActionsRequired)
        {
            var (summaryFilePath, ignoredProjectNamesTextFilePath) = await TaskHelper.WhenAll(
                this.SummaryFilePathProvider.GetSummaryFilePath(),
                this.ExtensionMethodBaseRepositoryFilePathsProvider.GetIgnoredExtensionMethodBaseNamesTextFilePath());

            await this.NotepadPlusPlusOperator.OpenFilePath(summaryFilePath);

            Console.WriteLine($"Review the summary file (which should be open in Notepad++):\n{summaryFilePath}\n");

            // * Review list of new projects.
            if (humanActionsRequired.ReviewNewExtensionMethodBases)
            {
                Console.WriteLine("*) Review the list of new extension method bases.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("*) No new extension method bases to review.\n");
            }

            // * Review list of departed projects.
            if (humanActionsRequired.ReviewDepartedExtensionMethodBases)
            {
                Console.WriteLine("*) Review the list of departed extension method bases.\nNote: departed extension method base names will be removed from the lists of ignored and selected extension method base names.");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("*) No departed extension method bases to review.\n");
            }

            // * Choose among duplicates in list of new duplicate projects by modifying the duplicate selections file: <duplicate selections file path>.
            if (humanActionsRequired.ReviewUnignoredDuplicateExtensionMethodBaseTypeNames)
            {
                // * For any new projects that should be ignored, add to the ignored projects file: <ignored projects file path>.
                await this.NotepadPlusPlusOperator.OpenFilePath(ignoredProjectNamesTextFilePath);

                Console.WriteLine($"*) Review the list of new duplicate extension method base names. Add all unwanted duplicates for each name and add to the ignored extension method base names file:\n{ignoredProjectNamesTextFilePath}\n");
                Console.WriteLine("Press enter when finished...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("*) No new duplicate extension method base names to choose among.");
            }

            if (humanActionsRequired.ReviewNewToProjectMappings)
            {
                Console.WriteLine("*) Review the list of new to-project mappings for extension method bases.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("*) No new to-project mappings for extension method bases.");
            }

            if (humanActionsRequired.ReviewOldToProjectMappings)
            {
                Console.WriteLine("*) Review the list of old to-project mappings for extension method bases.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("*) No old to-project mappings for extension method bases.");
            }
        }
    }
}
