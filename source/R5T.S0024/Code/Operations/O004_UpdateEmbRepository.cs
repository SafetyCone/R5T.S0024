using System;
using System.Linq;
using System.Threading.Tasks;

using R5T.Magyar.IO;

using R5T.D0084.D001;
using R5T.D0105;
using R5T.D0108;


namespace R5T.S0024
{
    public class O004_UpdateEmbRepository : T0020.IOperation
    {
        private IAllProjectDirectoryPathsProvider AllProjectDirectoryPathsProvider { get; }
        private IAllExtensionMethodBasesListingFilePathProvider AllExtensionMethodBasesListingFilePathProvider { get; }
        private IExtensionMethodBaseRepository ExtensionMethodBaseRepository { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }


        public O004_UpdateEmbRepository(
            IAllProjectDirectoryPathsProvider allProjectDirectoryPathsProvider,
            IAllExtensionMethodBasesListingFilePathProvider allExtensionMethodBasesListingFilePathProvider,
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            INotepadPlusPlusOperator notepadPlusPlusOperator)
        {
            this.AllProjectDirectoryPathsProvider = allProjectDirectoryPathsProvider;
            this.AllExtensionMethodBasesListingFilePathProvider = allExtensionMethodBasesListingFilePathProvider;
            this.ExtensionMethodBaseRepository = extensionMethodBaseRepository;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
        }

        public async Task Run()
        {
            var (repositoryExtensionMethodBases, currentExtensionMethodBases) = await Instances.Operation.GetRepositoryAndCurrentExtensionMethodBases(
                this.AllProjectDirectoryPathsProvider,
                this.ExtensionMethodBaseRepository);

            // Analysis.
            // New and departed.
            var (newExtensionMethodBases, departedExtensionMethodBases) = Instances.Operation.GetNewAndDepartedByNameAndFilePath(
                repositoryExtensionMethodBases,
                currentExtensionMethodBases);

            // Modify the project repository to match the current set of projects.
            await this.ExtensionMethodBaseRepository.DeleteExtensionMethodBases(departedExtensionMethodBases);

            await this.ExtensionMethodBaseRepository.AddExtensionMethodBases(newExtensionMethodBases);

            // With all current projects now in the repository:
            // Update the name selections using the project list, ignored names list, and duplicate name selections.
            await this.ExtensionMethodBaseRepository.UpdateExtensionMethodBaseNameSelections();

            // Then update the list of all project names (including any duplicates).
            // Get all projects again, now with new projects added and departed projects removed.
            var allExtensionMethodBases = await this.ExtensionMethodBaseRepository.GetAllExtensionMethodBases();

            var allExtensionMethodBaseNamesInOrder = allExtensionMethodBases
                .Select(xProject =>
                {
                    var typeName = Instances.NamespacedTypeName.GetTypeName(xProject.NamespacedTypeName);

                    var output = $"{typeName}: {xProject.NamespacedTypeName}";
                    return output;
                })
                .OrderAlphabetically()
                ;

            var allExtensionMethodBaseNamesListingFilePath = await this.AllExtensionMethodBasesListingFilePathProvider.GetAllExtensionMethodBaseNamesListingFileName();

            FileHelper.WriteAllLinesSynchronous(
                allExtensionMethodBaseNamesListingFilePath,
                allExtensionMethodBaseNamesInOrder);

            // Show the file in Notepad++.
            await this.NotepadPlusPlusOperator.OpenFilePath(allExtensionMethodBaseNamesListingFilePath);
        }
    }
}
