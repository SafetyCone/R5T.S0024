using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0105;
using R5T.D0108;
using R5T.T0020;


namespace R5T.S0024
{
    /// <summary>
    /// Writes out the names of ALL extension method bases (even duplicates) to the <see cref="IAllExtensionMethodBasesListingFilePathProvider"/> file path.
    /// Then opens the file in Notepad++.
    /// </summary>
    [OperationMarker]
    public class O007_WriteOutAllEmbs : IActionOperation
    {
        private IAllExtensionMethodBasesListingFilePathProvider AllExtensionMethodBasesListingFilePathProvider { get; }
        private IExtensionMethodBaseRepository ExtensionMethodBaseRepository { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }


        public O007_WriteOutAllEmbs(
            IAllExtensionMethodBasesListingFilePathProvider allExtensionMethodBasesListingFilePathProvider,
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            INotepadPlusPlusOperator notepadPlusPlusOperator)
        {
            this.AllExtensionMethodBasesListingFilePathProvider = allExtensionMethodBasesListingFilePathProvider;
            this.ExtensionMethodBaseRepository = extensionMethodBaseRepository;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
        }

        public async Task Run()
        {
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

            await FileHelper.WriteAllLines(
                allExtensionMethodBaseNamesListingFilePath,
                allExtensionMethodBaseNamesInOrder);

            // Show the file in Notepad++.
            await this.NotepadPlusPlusOperator.OpenFilePath(allExtensionMethodBaseNamesListingFilePath);
        }
    }
}
