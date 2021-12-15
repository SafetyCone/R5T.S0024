using System;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0105;
using R5T.D0108.I001;


namespace R5T.S0024
{
    /// <summary>
    /// Utility operation to open all files in Notepad++ for the file-based extension method base repository.
    /// </summary>
    public class O900_OpenAllEmbRepositoryFiles : T0020.IOperation
    {
        private IExtensionMethodBaseRepositoryFilePathsProvider ExtensionMethodBaseRepositoryFilePathsProvider { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }


        public O900_OpenAllEmbRepositoryFiles(
            IExtensionMethodBaseRepositoryFilePathsProvider extensionMethodBaseRepositoryFilePathsProvider,
            INotepadPlusPlusOperator notepadPlusPlusOperator)
        {
            this.ExtensionMethodBaseRepositoryFilePathsProvider = extensionMethodBaseRepositoryFilePathsProvider;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
        }

        public async Task Run()
        {
            var (Task1Result, Task2Result, Task3Result, Task4Result, Task5Result) = await TaskHelper.WhenAll(
                this.ExtensionMethodBaseRepositoryFilePathsProvider.GetDuplicateExtensionMethodBaseNamesTextFilePath(),
                this.ExtensionMethodBaseRepositoryFilePathsProvider.GetExtensionMethodBaseNameSelectionsTextFilePath(),
                this.ExtensionMethodBaseRepositoryFilePathsProvider.GetExtensionMethodBasesListingJsonFilePath(),
                this.ExtensionMethodBaseRepositoryFilePathsProvider.GetIgnoredExtensionMethodBaseNamesTextFilePath(),
                this.ExtensionMethodBaseRepositoryFilePathsProvider.GetToProjectMappingsJsonFilePath());

            var openingAllFilePaths = new[]
            {
                Task1Result,
                Task2Result,
                Task3Result,
                Task4Result,
                Task5Result
            }
            .Select(filePath => this.NotepadPlusPlusOperator.OpenFilePath(filePath))
            ;

            await Task.WhenAll(openingAllFilePaths);
        }
    }
}
