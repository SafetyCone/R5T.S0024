using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.Magyar;
using R5T.Magyar.IO;

using R5T.D0084.D001;
using R5T.D0101;
using R5T.D0105;
using R5T.D0108;
using R5T.D0110;

using R5T.S0024.Library;


namespace R5T.S0024
{
    public class O001_AnalyzeAllCurrentEmbs : T0020.IOperation
    {
        private IAllProjectDirectoryPathsProvider AllProjectDirectoryPathsProvider { get; }
        private IExtensionMethodBaseRepository ExtensionMethodBaseRepository { get; }
        private INotepadPlusPlusOperator NotepadPlusPlusOperator { get; }
        private IProjectRepository ProjectRepository { get; }
        private ISummaryFilePathProvider SummaryFilePathProvider { get; }


        public O001_AnalyzeAllCurrentEmbs(
            IAllProjectDirectoryPathsProvider allProjectDirectoryPathsProvider,
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            INotepadPlusPlusOperator notepadPlusPlusOperator,
            IProjectRepository projectRepository,
            ISummaryFilePathProvider summaryFilePathProvider)
        {
            this.AllProjectDirectoryPathsProvider = allProjectDirectoryPathsProvider;
            this.ExtensionMethodBaseRepository = extensionMethodBaseRepository;
            this.NotepadPlusPlusOperator = notepadPlusPlusOperator;
            this.ProjectRepository = projectRepository;
            this.SummaryFilePathProvider = summaryFilePathProvider;
        }

        public async Task Run()
        {
            var (repositoryExtensionMethodBases, currentExtensionMethodBases) = await Instances.Operation.GetRepositoryAndCurrentExtensionMethodBases(
                this.AllProjectDirectoryPathsProvider,
                this.ExtensionMethodBaseRepository);

            // Fill in identities for the current extension method bases, using identies from corresponding repository extension method bases if available, or generating new identies if not.
            // This is required for evaluating existing to-project mappings below.
            currentExtensionMethodBases.FillCurrentIdentities(repositoryExtensionMethodBases);

            // Analysis.
            // New and departed.
            var (newExtensionMethodBases, departedExtensionMethodBases) = Instances.Operation.GetNewAndDepartedByNameAndFilePath(
                repositoryExtensionMethodBases,
                currentExtensionMethodBases);

            // Duplicate type names.
            var ignoredNamespacedTypeNames = await this.ExtensionMethodBaseRepository.GetAllIgnoredExtensionMethodBaseNamespacedTypeNames();

            var duplicateTypeNameSets = Instances.Operation.GetUnignoredDuplicateNameSets(
                currentExtensionMethodBases,
                ignoredNamespacedTypeNames,
                Instances.NamespacedTypeName.GetTypeName);

            // To project mappings.
            var existingToProjectMappings = await this.ExtensionMethodBaseRepository.GetAllToProjectMappings();

            var projects = await this.ProjectRepository.GetAllProjects();

            var (newToProjectMappings, oldToProjectMappings) = Instances.Operation.GetToProjectMappingChanges(
                currentExtensionMethodBases,
                projects,
                existingToProjectMappings);

            // Summarize changes.
            var summaryFilePath = await this.SummaryFilePathProvider.GetSummaryFilePath();

            using (var textFile = FileHelper.WriteTextFile(summaryFilePath))
            {
                // New extension method bases.
                var newExtensionMethodBasesCount = newExtensionMethodBases.Length;

                textFile.WriteLine($"New extension method bases ({newExtensionMethodBasesCount}):");
                textFile.WriteLine();

                if (newExtensionMethodBases.None())
                {
                    textFile.WriteLine("<none>");
                }
                else
                {
                    var typeNamedPairs = newExtensionMethodBases
                        .Select(x => (TypeName: Instances.NamespacedTypeName.GetTypeName(x.NamespacedTypeName), ExtensionMethodBase: x))
                        .OrderAlphabetically(x => x.TypeName)
                        ;

                    foreach (var (TypeName, ExtensionMethodBase) in typeNamedPairs)
                    {
                        textFile.WriteLine($"{TypeName}: {ExtensionMethodBase.NamespacedTypeName}");
                    }
                }

                // Departed extension method bases.
                var departedExtensionMethodBasesCount = departedExtensionMethodBases.Length;

                textFile.WriteLine();
                textFile.WriteLine($"Departed extension method bases ({departedExtensionMethodBasesCount}):");
                textFile.WriteLine();

                if (departedExtensionMethodBases.None())
                {
                    textFile.WriteLine("<none>");
                }
                else
                {
                    var typeNamedPairs = departedExtensionMethodBases
                        .Select(x => (TypeName: Instances.NamespacedTypeName.GetTypeName(x.NamespacedTypeName), ExtensionMethodBase: x))
                        .OrderAlphabetically(x => x.TypeName)
                        ;

                    foreach (var (TypeName, ExtensionMethodBase) in typeNamedPairs)
                    {
                        textFile.WriteLine($"{TypeName}: {ExtensionMethodBase.NamespacedTypeName}");
                    }
                }

                // Duplicate extension method bases
                var duplicateCount = duplicateTypeNameSets.Count;

                textFile.WriteLine();
                textFile.WriteLine($"New duplicate extension method base type names ({duplicateCount}):");
                textFile.WriteLine();

                if (duplicateTypeNameSets.None())
                {
                    textFile.WriteLine("<none>");
                }
                else
                {
                    foreach (var embTypeName in duplicateTypeNameSets.Keys.OrderAlphabetically())
                    {
                        textFile.WriteLine($"{embTypeName}:");

                        var set = duplicateTypeNameSets[embTypeName];
                        foreach (var emb in set)
                        {
                            textFile.WriteLine(emb.NamespacedTypeName);
                        }

                        textFile.WriteLine();
                    }
                }

                // New extension method to project mappings.
                var projectsByIdentity = projects.ToDictionaryByIdentity();
                var extensionMethodBasesByIdentity = currentExtensionMethodBases.ToDictionaryByIdentity();

                var newMappingsCount = newToProjectMappings.Length;

                textFile.WriteLine();
                textFile.WriteLine($"New extension method base-to-project mappings ({newMappingsCount}):");
                textFile.WriteLine();

                if (newToProjectMappings.None())
                {
                    textFile.WriteLine("<none>");
                }
                else
                {
                    foreach (var mapping in newToProjectMappings)
                    {
                        var extensionMethodBase = extensionMethodBasesByIdentity[mapping.ExtensionMethodBaseIdentity];
                        var project = projectsByIdentity[mapping.ProjectIdentity];

                        textFile.WriteLine($"{extensionMethodBase.NamespacedTypeName}: {project.Name}");
                        textFile.WriteLine($"{extensionMethodBase.CodeFilePath}: {project.FilePath}");
                        textFile.WriteLine();
                    }
                }

                // Old extension method to project mappings.
                var oldMappingsCount = oldToProjectMappings.Length;

                textFile.WriteLine();
                textFile.WriteLine($"Old extension method base-to-project mappings ({oldMappingsCount}):");
                textFile.WriteLine();

                if (oldToProjectMappings.None())
                {
                    textFile.WriteLine("<none>");
                }
                else
                {
                    foreach (var mapping in oldToProjectMappings)
                    {
                        var extensionMethodBase = extensionMethodBasesByIdentity[mapping.ExtensionMethodBaseIdentity];
                        var project = projectsByIdentity[mapping.ProjectIdentity];

                        textFile.WriteLine($"{extensionMethodBase.NamespacedTypeName}: {project.Name}");
                        textFile.WriteLine($"{extensionMethodBase.CodeFilePath}: {project.FilePath}");
                        textFile.WriteLine();
                    }
                }
            }

            // Show the summary in Notepad++ to be immediately helpful.
            await this.NotepadPlusPlusOperator.OpenFilePath(summaryFilePath);

            //// Output file paths.
            //var allEmbNamespacedTypeNamesTextFilePath = @"C:\Temp\Extension Method Base, Current Namespaced Type Names-All.txt";
            //var duplicateEmbNamespacedTypeNamesTextFilePath = @"C:\Temp\Extension Method Base, Current Namespaced Type Names-Duplicates.txt";
            //var duplicateEmbInformationByNamespacedTypeNameTextFilePath = @"C:\Temp\Extension Method Base, Current Namespaced Type Names-Duplicates with Information.txt";

            //var allEmbTypeNamesTextFilePath = @"C:\Temp\Extension Method Base, Current Type Names-All.txt";
            //var duplicateEmbTypeNamesTextFilePath = @"C:\Temp\Extension Method Base, Current Type Names-Duplicates.txt";
            //var duplicateEmbInformationByTypeNameTextFilePath = @"C:\Temp\Extension Method Base, Current Type Names-Duplicates with Information.txt";

            //// List all extension method base namespaced type names.
            //var allEmbNamespacedTypeNamesInOrder = currentExtensionMethodBases.GetAllDistinctNamesInAlphabeticalOrder();

            //FileHelper.WriteAllLinesSynchronous(
            //    allEmbNamespacedTypeNamesTextFilePath,
            //    allEmbNamespacedTypeNamesInOrder);

            //// List duplicate extension method base namespaced type names.
            //var duplicateEmbNamespacedTypeNamesInOrder = currentExtensionMethodBases.GetDuplicateNamesInAlphabeticalOrder();

            //FileHelper.WriteAllLinesSynchronous(
            //    duplicateEmbNamespacedTypeNamesTextFilePath,
            //    duplicateEmbNamespacedTypeNamesInOrder);

            //// List duplicate extension method base namespaced type names with their information (identity and code file path).
            //var duplicateEmbInformationByEmbNamespacedTypeName = currentExtensionMethodBases.GetInformationByNameForDuplicateNames();

            //duplicateEmbInformationByEmbNamespacedTypeName.WriteToFileInAlphabeticalOrder(duplicateEmbInformationByNamespacedTypeNameTextFilePath);

            //// 
            //var allCurrentEmbsWithTypeNames = currentExtensionMethodBases
            //    .Select(xEmb => xEmb.ToNamedIdentifiedFilePathed(
            //        xNamespacedTypeName => Instances.NamespacedTypeName.GetTypeName(xNamespacedTypeName)));

            //// List all extension method base type names.
            //var allEmbTypeNamesInOrder = allCurrentEmbsWithTypeNames.GetAllDistinctNamesInAlphabeticalOrder();

            //FileHelper.WriteAllLinesSynchronous(
            //    allEmbTypeNamesTextFilePath,
            //    allEmbTypeNamesInOrder);

            //// List duplicate extension method base type names.
            //var duplicateEmbTypeNamesInOrder = allCurrentEmbsWithTypeNames.GetDuplicateNamesInAlphabeticalOrder();

            //FileHelper.WriteAllLinesSynchronous(
            //    duplicateEmbTypeNamesTextFilePath,
            //    duplicateEmbTypeNamesInOrder);

            //// List duplicate extension method base type names with their information (identity and code file path).
            //var duplicateEmbInformationByEmbTypeName = allCurrentEmbsWithTypeNames.GetInformationByNameForDuplicateNames();

            //duplicateEmbInformationByEmbTypeName.WriteToFileInAlphabeticalOrder(duplicateEmbInformationByTypeNameTextFilePath);
        }
    }
}
