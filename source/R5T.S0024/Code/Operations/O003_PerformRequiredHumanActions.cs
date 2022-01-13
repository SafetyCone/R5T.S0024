using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0084.D001;
using R5T.D0101;
using R5T.D0108;
using R5T.T0020;
using R5T.T0100;

using R5T.S0024.Library;


namespace R5T.S0024
{
    [OperationMarker]
    public class O003_PerformRequiredHumanActions : IActionOperation
    {
        #region Static

        private static async Task<
            (HumanActionsRequired humanActionsRequired,
            Dictionary<string, ExtensionMethodBase[]> unignoredDuplicateTypeNameSets,
            string[] ignoredNamespacedTypeNames)>
        DetermineWhatHumanActionsAreRequired(
            IEnumerable<ExtensionMethodBase> newExtensionMethodBases,
            IEnumerable<ExtensionMethodBase> departedExtensionMethodBases,
            ExtensionMethodBase[] currentExtensionMethodBases,
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            ExtensionMethodBaseToProjectMapping[] newToProjectMappings,
            ExtensionMethodBaseToProjectMapping[] oldToProjectMappings)
        {
            var ignoredNamespacedTypeNames = await extensionMethodBaseRepository.GetAllIgnoredExtensionMethodBaseNamespacedTypeNames();

            var (humanActionsRequired, unignoredDuplicateTypeNameSets) = O003_PerformRequiredHumanActions.DetermineWhatHumanActionsAreRequired(
                newExtensionMethodBases,
                departedExtensionMethodBases,
                currentExtensionMethodBases,
                ignoredNamespacedTypeNames);

            humanActionsRequired.ReviewNewToProjectMappings = newToProjectMappings.Any();
            humanActionsRequired.ReviewOldToProjectMappings = oldToProjectMappings.Any();

            return (humanActionsRequired, unignoredDuplicateTypeNameSets, ignoredNamespacedTypeNames);
        }

        private static 
            (HumanActionsRequired humanActionsRequired,
            Dictionary<string, ExtensionMethodBase[]> unignoredDuplicateTypeNameSets)
        DetermineWhatHumanActionsAreRequired(
            IEnumerable<ExtensionMethodBase> newExtensionMethodBases,
            IEnumerable<ExtensionMethodBase> departedExtensionMethodBases,
            ExtensionMethodBase[] currentExtensionMethodBases,
            string[] ignoredNamespacedTypeNames)
        {
            var unignoredDuplicateTypeNameSets = Instances.Operation.GetUnignoredDuplicateNameSets(
                currentExtensionMethodBases,
                ignoredNamespacedTypeNames,
                Instances.NamespacedTypeName.GetTypeName);

            var humanActionsRequired = O003_PerformRequiredHumanActions.DetermineWhatHumanActionsAreRequired(
                newExtensionMethodBases,
                departedExtensionMethodBases,
                unignoredDuplicateTypeNameSets);

            return (humanActionsRequired, unignoredDuplicateTypeNameSets);
        }

        private static HumanActionsRequired DetermineWhatHumanActionsAreRequired(
            IEnumerable<ExtensionMethodBase> newExtensionMethodBases,
            IEnumerable<ExtensionMethodBase> departedExtensionMethodBases,
            Dictionary<string, ExtensionMethodBase[]> unignoredDuplicateTypeNameSets)
        {
            // Are there any human actions required?
            // Are there any new extension method bases?
            var anyNewProjects = newExtensionMethodBases.Any();

            // Are there any departed projects?
            var anyDepartedProjects = departedExtensionMethodBases.Any();

            // Are there any unignored duplicate extension method base type names?
            var anyUnignoredDuplicateTypeNames = unignoredDuplicateTypeNameSets.Any();

            var humanActionsRequired = new HumanActionsRequired
            {
                ReviewDepartedExtensionMethodBases = anyDepartedProjects,
                ReviewNewExtensionMethodBases = anyNewProjects,
                ReviewUnignoredDuplicateExtensionMethodBaseTypeNames = anyUnignoredDuplicateTypeNames,
            };

            return humanActionsRequired;
        }

        #endregion


        private IAllProjectDirectoryPathsProvider AllProjectDirectoryPathsProvider { get; }
        private IExtensionMethodBaseRepository ExtensionMethodBaseRepository { get; }
        private IProjectRepository ProjectRepository { get; }
        private O003a_PromptForRequiredHumanActions O003A_PromptForRequiredHumanActions { get; }


        public O003_PerformRequiredHumanActions(
            IAllProjectDirectoryPathsProvider allProjectDirectoryPathsProvider,
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            IProjectRepository projectRepository,
            O003a_PromptForRequiredHumanActions o003A_PromptForRequiredHumanActions)
        {
            this.AllProjectDirectoryPathsProvider = allProjectDirectoryPathsProvider;
            this.ExtensionMethodBaseRepository = extensionMethodBaseRepository;
            this.ProjectRepository = projectRepository;
            this.O003A_PromptForRequiredHumanActions = o003A_PromptForRequiredHumanActions;
        }

        public async Task Run()
        {
            // Repository and current state of the local file system.
            var (repositoryExtensionMethodBases, currentExtensionMethodBases) = await Instances.Operation.GetRepositoryAndCurrentExtensionMethodBases(
                this.AllProjectDirectoryPathsProvider,
                this.ExtensionMethodBaseRepository);

            // New and departed.
            var (newExtensionMethodBases, departedExtensionMethodBases) = Instances.Operation.GetNewAndDepartedByNameAndFilePath(
                repositoryExtensionMethodBases,
                currentExtensionMethodBases);

            // This is required for evaluating existing to-project mappings below.
            // To project mappings.
            currentExtensionMethodBases.FillIdentitiesFromSourceOrSetNew(repositoryExtensionMethodBases);

            var existingToProjectMappings = await this.ExtensionMethodBaseRepository.GetAllToProjectMappings();

            var projects = await this.ProjectRepository.GetAllProjects();

            var (newToProjectMappings, oldToProjectMappings) = Instances.Operation.GetToProjectMappingChanges(
                currentExtensionMethodBases,
                projects,
                existingToProjectMappings);

            // Determine what human actions are required.
            var (humanActionsRequired, _, _) = await O003_PerformRequiredHumanActions.DetermineWhatHumanActionsAreRequired(
                newExtensionMethodBases,
                departedExtensionMethodBases,
                currentExtensionMethodBases,
                this.ExtensionMethodBaseRepository,
                newToProjectMappings,
                oldToProjectMappings);

            var anyHumanActionsRequired = humanActionsRequired.Any();
            if(anyHumanActionsRequired)
            {
                Console.WriteLine("Human actions are required before updating the project repository.\n");

                // Prompt for human actions.
                await this.O003A_PromptForRequiredHumanActions.Run(humanActionsRequired);

                // Now redetermine what human actions are required and reprompt, until all mandatory human actions are finished.
                while (true)
                {
                    var (humanActionsRequired2, _, _) = await O003_PerformRequiredHumanActions.DetermineWhatHumanActionsAreRequired(
                        newExtensionMethodBases,
                        departedExtensionMethodBases,
                        currentExtensionMethodBases,
                        this.ExtensionMethodBaseRepository,
                        newToProjectMappings,
                        oldToProjectMappings);

                    // Unset any new/departed review since that has already been done once.
                    humanActionsRequired2.UnsetNonMandatory();

                    var anyMandatoryHumanActionsRequired = humanActionsRequired2.AnyMandatory();
                    if(!anyMandatoryHumanActionsRequired)
                    {
                        break;
                    }

                    Console.WriteLine("MANDATORY human actions are required before updating the project repository.\n");

                    // Else prompt.
                    await this.O003A_PromptForRequiredHumanActions.Run(humanActionsRequired);
                }
            }
            else
            {
                Console.WriteLine("No human actions are required before updating the extension method base repository.\n");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }
    }
}
