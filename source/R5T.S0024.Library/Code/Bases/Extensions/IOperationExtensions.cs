using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.Magyar;

using R5T.D0084.D001;
using R5T.D0101;
using R5T.D0108;
using R5T.T0097;
using R5T.T0098;
using R5T.T0100;

using R5T.S0024.Library;

using Instances = R5T.S0024.Library.Instances;


namespace System
{
    public static class IOperationExtensions
    {
        public static async Task<(
            ExtensionMethodBaseToProjectMapping[] newToProjectMappings,
            ExtensionMethodBaseToProjectMapping[] oldToProjectMappings)>
        GetToProjectMappingChanges(this IOperation _,
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            IProjectRepository projectRepository)
        {
            var (extensionMethodBases, toProjectMappings, projects) = await TaskHelper.WhenAll(
                extensionMethodBaseRepository.GetAllExtensionMethodBases(),
                extensionMethodBaseRepository.GetAllToProjectMappings(),
                projectRepository.GetAllProjects());

            var output = _.GetToProjectMappingChanges(
                extensionMethodBases,
                projects,
                toProjectMappings);

            return output;
        }

        public static (
            ExtensionMethodBaseToProjectMapping[] newToProjectMappings,
            ExtensionMethodBaseToProjectMapping[] oldToProjectMappings)
        GetToProjectMappingChanges(this IOperation _,
            ExtensionMethodBase[] extensionMethodBases,
            Project[] projects,
            ExtensionMethodBaseToProjectMapping[] existingToProjectMappings)
        {
            // Verify inputs.
            extensionMethodBases.VerifyAllIdentitiesAreSet();
            projects.VerifyAllIdentitiesAreSet();

            var existingToProjectMappingsByEmbIdentity = existingToProjectMappings
                .ToDictionary(
                    x => x.ExtensionMethodBaseIdentity);

            var projectsByProjectIdentity = projects
                .ToDictionary(
                    x => x.Identity);

            var projectsByProjectDirectoryPath = projects.ToDictionaryByFilePathModified(
                Instances.ProjectPathsOperator.GetProjectDirectoryPath);

            var newToProjectMappings = new List<ExtensionMethodBaseToProjectMapping>();
            var oldToProjectMappings = new List<ExtensionMethodBaseToProjectMapping>(); // For existing mappings that are no longer valid, and should be removed.

            foreach (var extensionMethodBase in extensionMethodBases)
            {
                var hasMapping = existingToProjectMappingsByEmbIdentity.ContainsKey(extensionMethodBase.Identity);
                if (hasMapping)
                {
                    var existingMapping = existingToProjectMappingsByEmbIdentity[extensionMethodBase.Identity];

                    // If an extension method has a mapping, just test if that mapping is still valid (this saves significant computation time).
                    var projectExists = projectsByProjectIdentity.ContainsKey(existingMapping.ProjectIdentity);
                    if(projectExists)
                    {
                        var project = projectsByProjectIdentity[existingMapping.ProjectIdentity];

                        // If the project exists, is the extension method base still in the project's directory?
                        var extensionMethodBaseCodeFileIsInProjectDirectory = Instances.PathOperator.IsFileInDirectoryOrSubDirectoriesOfFileDirectory(
                            extensionMethodBase.CodeFilePath,
                            project.FilePath);

                        if (!extensionMethodBaseCodeFileIsInProjectDirectory)
                        {
                            // Remove the mapping.
                            oldToProjectMappings.Add(existingMapping);

                            // The add new mapping for the extension method base.
                            _.AddMappingToMappings(
                                extensionMethodBase,
                                projectsByProjectDirectoryPath,
                                newToProjectMappings);
                        }
                        // Else, do nothing. Mapping is still valid.
                    }
                    else
                    {
                        // Project no longer exists, definitely remove the mapping.
                        oldToProjectMappings.Add(existingMapping);
                    }
                }
                else
                {
                    // If an extension method base does not have a mapping, determine it's mapping to a single project.
                    _.AddMappingToMappings(
                        extensionMethodBase,
                        projectsByProjectDirectoryPath,
                        newToProjectMappings);
                }
            }

            return (newToProjectMappings.ToArray(), oldToProjectMappings.ToArray());
        }

        /// <summary>
        /// Maps an extension method base to a single project.
        /// Performs an exhaustive search of all projects (which is slow) to ensure the extension method base only maps to a single project.
        /// </summary>
        public static void AddMappingToMappings(this IOperation _,
            ExtensionMethodBase extensionMethodBase,
            Dictionary<string, Project> projectsByProjectDirectoryPath,
            List<ExtensionMethodBaseToProjectMapping> mappings
            )
        {
            var projectAlreadyFound = false;

            foreach (var projectDirectoryPath in projectsByProjectDirectoryPath.Keys)
            {
                var extensionMethodBaseCodeFileIsInProjectDirectory = Instances.PathOperator.IsFileInDirectoryOrSubDirectories(
                    extensionMethodBase.CodeFilePath,
                    projectDirectoryPath);

                if (extensionMethodBaseCodeFileIsInProjectDirectory)
                {
                    if (projectAlreadyFound)
                    {
                        // If it does, that means there are multiple projects with the same project directory, or there is a project in Instances.PathOperator.IsPathSubPathOfParentPath() or GetProjectDirectoryPath() method.
                        throw new Exception("Should not happen.");
                    }

                    var project = projectsByProjectDirectoryPath[projectDirectoryPath];

                    var toProjectMapping = new ExtensionMethodBaseToProjectMapping
                    {
                        ExtensionMethodBaseIdentity = extensionMethodBase.Identity,
                        ProjectIdentity = project.Identity
                    };

                    mappings.Add(toProjectMapping);

                    projectAlreadyFound = true;
                }
            }

            if (!projectAlreadyFound)
            {
                // If it does, the project index needs to be updated by running the project update script (R5T.S0023) again to collect any new projects.
                throw new Exception("Should not happen.");
            }
        }

        /// <summary>
        /// Uses existing mappings to speed up calculation (existing mappings are just checked for validity).
        /// </summary>
        public static (
            ExtensionMethodBaseToProjectMapping[] newToProjectMappings,
            ExtensionMethodBaseToProjectMapping[] oldToProjectMappings,
            (ExtensionMethodBase, string)[] unmappableExtensionMethodBasesWithReasons)
        DetermineToProjectMappingChanges(this IOperation _,
            ExtensionMethodBase[] extensionMethodBases,
            Project[] projects,
            ExtensionMethodBaseToProjectMapping[] existingToProjectMappings)
        {
            // Verify inputs.
            extensionMethodBases.VerifyAllIdentitiesAreSet();
            projects.VerifyAllIdentitiesAreSet();

            var existingToProjectMappingsByEmbIdentity = existingToProjectMappings
                .ToDictionary(
                    x => x.ExtensionMethodBaseIdentity);

            var projectsByIdentity = projects
                .ToDictionary(
                    x => x.Identity);

            var projectsByProjectDirectoryPath = projects.ToDictionaryByFilePathModified(
                Instances.ProjectPathsOperator.GetProjectDirectoryPath);

            var newToProjectMappings = new List<ExtensionMethodBaseToProjectMapping>();
            var oldToProjectMappings = new List<ExtensionMethodBaseToProjectMapping>(); // For existing mappings that are no longer valid, and should be removed.
            var unmappableExtensionMethodBasesWithReasons = new List<(ExtensionMethodBase, string)>();

            foreach (var extensionMethodBase in extensionMethodBases)
            {
                var hasMapping = existingToProjectMappingsByEmbIdentity.TryGetValue(extensionMethodBase.Identity, out var existingMapping);
                if (hasMapping)
                {
                    // If an extension method has a mapping, just test if that mapping is still valid (this saves significant computation time).
                    var isExistingMappingValid = _.IsExistingMappingValid(
                        extensionMethodBase,
                        projectsByIdentity,
                        existingMapping);

                    if(!isExistingMappingValid)
                    {
                        // Mark the existing mapping as old.
                        oldToProjectMappings.Add(existingMapping);

                        // Add the new mapping.
                        _.AddToProjectMapping(
                            extensionMethodBase,
                            projectsByProjectDirectoryPath,
                            newToProjectMappings,
                            unmappableExtensionMethodBasesWithReasons);
                    }
                }
                else
                {
                    // If an extension method base does not have a mapping, determine it's mapping and add it.
                    _.AddToProjectMapping(
                        extensionMethodBase,
                        projectsByProjectDirectoryPath,
                        newToProjectMappings,
                        unmappableExtensionMethodBasesWithReasons);
                }
            }

            return (
                newToProjectMappings.ToArray(),
                oldToProjectMappings.ToArray(),
                unmappableExtensionMethodBasesWithReasons.ToArray());
        }

        public static bool IsExistingMappingValid(this IOperation _,
            ExtensionMethodBase extensionMethodBase,
            Dictionary<Guid, Project> projectsByIdentity,
            ExtensionMethodBaseToProjectMapping existingMapping)
        {
            var output = true;

            var projectExists = projectsByIdentity.TryGetValue(existingMapping.ProjectIdentity, out var project);
            if (projectExists)
            {
                // If the project exists, is the extension method base still in the project's directory?
                var extensionMethodBaseCodeFileIsInProjectDirectory = Instances.PathOperator.IsFileInDirectoryOrSubDirectoriesOfFileDirectory(
                    extensionMethodBase.CodeFilePath,
                    project.FilePath);

                if (!extensionMethodBaseCodeFileIsInProjectDirectory)
                {
                    output = false;
                }
                // Else, do nothing. Mapping is still valid.
            }
            else
            {
                // Project no longer exists, definitely remove the mapping.
                output = false;
            }

            return output;
        }

        public static void AddToProjectMapping(this IOperation _,
            ExtensionMethodBase extensionMethodBase,
            Dictionary<string, Project> projectsByProjectDirectoryPath,
            IList<ExtensionMethodBaseToProjectMapping> newToProjectMappings,
            IList<(ExtensionMethodBase, string)> unmappableExtensionMethodBasesWithReasons)
        {
            var result = _.DetermineToProjectMapping(
                extensionMethodBase,
                projectsByProjectDirectoryPath,
                out var toProjectMapping);

            if(result == ToProjectMappingResult.Success)
            {
                newToProjectMappings.Add(toProjectMapping);
            }
            else
            {
                switch(result)
                {
                    case ToProjectMappingResult.MoreThanOneProjectFound:
                        unmappableExtensionMethodBasesWithReasons.Add((extensionMethodBase, "More than one project found."));
                        break;

                    case ToProjectMappingResult.NoProjectFound:
                        unmappableExtensionMethodBasesWithReasons.Add((extensionMethodBase, "No project found."));
                        break;

                    default:
                        throw EnumerationHelper.SwitchDefaultCaseException(result);
                }
            }
        }

        /// <summary>
        /// Maps an extension method base to a single project, guaranteeing that the extension method base maps to one, and only one project.
        /// Performs an exhaustive search of all projects (which is slow) to ensure the extension method base only maps to a single project.
        /// </summary>
        public static ToProjectMappingResult DetermineToProjectMapping(this IOperation _,
            ExtensionMethodBase extensionMethodBase,
            Dictionary<string, Project> projectsByProjectDirectoryPath,
            out ExtensionMethodBaseToProjectMapping toProjectMapping)
        {
            var projects = _.DetermineProjectsForExtensionMethodBase(
                extensionMethodBase,
                projectsByProjectDirectoryPath);

            // Should not happen. If it does, then somehow an extension method base was found in a file not covered by any project.
            if (projects.Length == 0)
            {
                toProjectMapping = default;

                return ToProjectMappingResult.NoProjectFound;
            }

            // By now, there is at least one project found. Create a mapping from the first by name in alphabetical order.
            var project = projects.OrderAlphabetically2(xProject => xProject.Name).First();

            toProjectMapping = new ExtensionMethodBaseToProjectMapping
            {
                ExtensionMethodBaseIdentity = extensionMethodBase.Identity,
                ProjectIdentity = project.Identity
            };

            // Should not happen.
            // If it does, that means there are multiple projects with the same project directory, or there is a project in Instances.PathOperator.IsPathSubPathOfParentPath() or GetProjectDirectoryPath() method.
            if (projects.Length > 1)
            {
                return ToProjectMappingResult.NoProjectFound;
            }

            return ToProjectMappingResult.Success;
        }

        /// <summary>
        /// Determines the projects that include an extension method base.
        /// </summary>
        public static Project[] DetermineProjectsForExtensionMethodBase(this IOperation _,
            ExtensionMethodBase extensionMethodBase,
            Dictionary<string, Project> projectsByProjectDirectoryPath)
        {
            var projects = new List<Project>();

            foreach (var projectDirectoryPath in projectsByProjectDirectoryPath.Keys)
            {
                var extensionMethodBaseCodeFileIsInProjectDirectory = Instances.PathOperator.IsFileInDirectoryOrSubDirectories(
                    extensionMethodBase.CodeFilePath,
                    projectDirectoryPath);

                if (extensionMethodBaseCodeFileIsInProjectDirectory)
                {
                    var project = projectsByProjectDirectoryPath[projectDirectoryPath];
                    projects.Add(project);
                }
            }

            return projects.ToArray();
        }

        public static string[] GetNewDuplicateTypeNames(this IOperation _,
            ExtensionMethodBase[] repositoryExtensionMethodBases,
            ExtensionMethodBase[] currentExtensionMethodBases,
            string[] repositoryIgnoredExtensionMethodBaseNamespacedTypeNames)
        {
            var (unignoredRepositoryExtensionMethodBases, unignoredCurrentExtensionMethodBases) = _.GetUnignoredExtensionMethodBases(
                repositoryExtensionMethodBases,
                currentExtensionMethodBases,
                repositoryIgnoredExtensionMethodBaseNamespacedTypeNames);

            // Determine any new duplicate extension method base type names (not namespaced type names).
            var repositoryDuplicateTypeNames = unignoredRepositoryExtensionMethodBases
                .Select(x => Instances.NamespacedTypeName.GetTypeName(x.NamespacedTypeName))
                .GetDuplicatesInAlphabeticalOrder();
            var currentDuplicateTypeNames = unignoredCurrentExtensionMethodBases
                .Select(x => Instances.NamespacedTypeName.GetTypeName(x.NamespacedTypeName))
                .GetDuplicatesInAlphabeticalOrder();

            var newDuplicateTypeNames = currentDuplicateTypeNames.Except(repositoryDuplicateTypeNames)
                .ToArray();

            return newDuplicateTypeNames;
        }

        public static string[] GetNewDuplicateNamespacedTypeNames(this IOperation _,
            ExtensionMethodBase[] repositoryExtensionMethodBases,
            ExtensionMethodBase[] currentExtensionMethodBases,
            string[] repositoryIgnoredExtensionMethodBaseNamespacedTypeNames)
        {
            // Determine any new duplicate project names.
            var repositoryDuplicateNamespacedTypeNames = repositoryExtensionMethodBases.GetDuplicateNamesInAlphabeticalOrder();
            var currentDuplicateNamespacedTypeNames = currentExtensionMethodBases.GetDuplicateNamesInAlphabeticalOrder();

            var newDuplicateNamespacedTypeNames = currentDuplicateNamespacedTypeNames.Except(repositoryDuplicateNamespacedTypeNames)
                .Except(repositoryIgnoredExtensionMethodBaseNamespacedTypeNames) // Ignored names are ignored, even if duplicates.
                .ToArray();

            return newDuplicateNamespacedTypeNames;
        }

        public static (
            IEnumerable<ExtensionMethodBase> unignoredRepositoryExtensionMethodBases,
            IEnumerable<ExtensionMethodBase> unignoredCurrentExtensionMethodBases)
        GetUnignoredExtensionMethodBases(this IOperation _,
            ExtensionMethodBase[] repositoryExtensionMethodBases,
            ExtensionMethodBase[] currentExtensionMethodBases,
            string[] repositoryIgnoredExtensionMethodBaseNamespacedTypeNames)
        {
            var ignoredNamespacedTypeNamesHash = new HashSet<string>(repositoryIgnoredExtensionMethodBaseNamespacedTypeNames);

            var unignoredRepositoryExtensionMethodBases = repositoryExtensionMethodBases
                .Where(x => !ignoredNamespacedTypeNamesHash.Contains(x.NamespacedTypeName))
                ;

            var unignoredCurrentExtensionMethodBases = currentExtensionMethodBases
                .Where(x => !ignoredNamespacedTypeNamesHash.Contains(x.NamespacedTypeName))
                ;

            return (unignoredRepositoryExtensionMethodBases, unignoredCurrentExtensionMethodBases);
        }

        public static async Task<(
            ExtensionMethodBase[] repositoryExtensionMethodBases,
            ExtensionMethodBase[] currentExtensionMethodBases)>
        GetRepositoryAndCurrentExtensionMethodBases(this IOperation _,
            IAllProjectDirectoryPathsProvider allProjectDirectoryPathsProvider,
            IExtensionMethodBaseRepository extensionMethodBaseRepository)
        {
            // Get all repository extension method bases.
            var repositoryExtensionMethodBases = await extensionMethodBaseRepository.GetAllExtensionMethodBases();

            // Verify distinct by (namespaced type name, code file path) pair.
            repositoryExtensionMethodBases.VerifyDistinctByNamedFilePathedData();

            // Get all current extension method bases from all current project directory paths.
            var currentExtensionMethodBases = await _.GetCurrentExtensionMethodBases(allProjectDirectoryPathsProvider);

            // Verify distinct by (namespaced type name, code file path) pair.
            currentExtensionMethodBases.VerifyDistinctByNamedFilePathedData();

            return (repositoryExtensionMethodBases, currentExtensionMethodBases);
        }

        public static async Task<ExtensionMethodBase[]> GetCurrentExtensionMethodBases(this IOperation _,
            IAllProjectDirectoryPathsProvider allProjectDirectoryPathsProvider)
        {
            var projectDirectoryPaths = await allProjectDirectoryPathsProvider.GetAllProjectDirectoryPaths();

            var currentExtensionMethodBases = new List<ExtensionMethodBase>();
            foreach (var projectDirectoryPath in projectDirectoryPaths)
            {
                var namespacedTypeNameCSharpCodeFilePaths = await Instances.ExtensionMethodBaseOperator.GetAllExtensionMethodBases(
                    projectDirectoryPath);

                foreach (var namespacedTypeNameCSharpCodeFilePath in namespacedTypeNameCSharpCodeFilePaths)
                {
                    var extensionMethodBase = new ExtensionMethodBase
                    {
                        // No identity, as that will be added (if needed) when adding to the repository, and comparisons will be done using a data value-based equality comparer.
                        NamespacedTypeName = namespacedTypeNameCSharpCodeFilePath.NamespacedTypeName,
                        CodeFilePath = namespacedTypeNameCSharpCodeFilePath.CodeFilePath,
                    };

                    currentExtensionMethodBases.Add(extensionMethodBase);
                }
            }

            var output = currentExtensionMethodBases.ToArray();
            return output;
        }
    }
}
