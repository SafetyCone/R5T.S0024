using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using R5T.T0097;
using R5T.T0098;
using R5T.T0100;


namespace R5T.S0024
{
    public static class IOperationExtensions
    {
        public static void WriteUnspecifiedDuplicateNamesSummaryFile(this IOperation _,
            StreamWriter output,
            Dictionary<string, ExtensionMethodBase[]> embsByName)
        {
            var duplicateNamesCount = embsByName.Count;

            output.WriteLine($"Duplicate extension method base namespaced type names ({duplicateNamesCount}):");
            output.WriteLine();

            if (embsByName.None())
            {
                output.WriteLine("<none> (ok)");
            }
            else
            {
                foreach (var pair in embsByName)
                {
                    output.WriteLine($"{pair.Key}:");

                    foreach (var emb in pair.Value)
                    {
                        output.WriteLine($"{emb.NamespacedTypeName}| {emb.Identity} ({emb.CodeFilePath})");
                    }
                    output.WriteLine();
                }
            }
            output.WriteLine("\n***\n");
        }

        public static void WriteNewAndDepartedToProjectMappingsSummaryFile(this IOperation _,
            TextWriter output,
            Dictionary<Guid, ExtensionMethodBase> extensionMethodBasesByIdentity,
            Dictionary<Guid, Project> projectsByIdentity,
            ExtensionMethodBaseToProjectMapping[] newToProjectMappings,
            ExtensionMethodBaseToProjectMapping[] departedToProjectMappings)
        {
            var newMappingsCount = newToProjectMappings.Length;

            output.WriteLine($"New extension method base-to-project mappings ({newMappingsCount}):");
            output.WriteLine();

            if (newToProjectMappings.None())
            {
                output.WriteLine("<none> (ok)");
            }
            else
            {
                foreach (var mapping in newToProjectMappings)
                {
                    var extensionMethodBase = extensionMethodBasesByIdentity[mapping.ExtensionMethodBaseIdentity];
                    var project = projectsByIdentity[mapping.ProjectIdentity];

                    output.WriteLine($"{extensionMethodBase.NamespacedTypeName}: {project.Name}");
                    output.WriteLine($"{extensionMethodBase.CodeFilePath}: {project.FilePath}");
                    output.WriteLine();
                }
            }

            // Old extension method to project mappings.
            var departedMappingsCount = departedToProjectMappings.Length;

            output.WriteLine($"Departed extension method base-to-project mappings ({departedMappingsCount}):");
            output.WriteLine();

            if (departedToProjectMappings.None())
            {
                output.WriteLine("<none> (ok)");
            }
            else
            {
                foreach (var mapping in departedToProjectMappings)
                {
                    var extensionMethodBase = extensionMethodBasesByIdentity[mapping.ExtensionMethodBaseIdentity];
                    var project = projectsByIdentity[mapping.ProjectIdentity];

                    output.WriteLine($"{extensionMethodBase.NamespacedTypeName}: {project.Name}");
                    output.WriteLine($"{extensionMethodBase.CodeFilePath}: {project.FilePath}");
                    output.WriteLine();
                }
            }
        }

        public static void WriteUnmappableToProjectSummaryFile(this IOperation _,
            TextWriter output,
            IList<(ExtensionMethodBase, string)> unmappableExtensionMethodBasesWithReasons)
        {
            var unmappableEmbCount = unmappableExtensionMethodBasesWithReasons.Count;

            output.WriteLine($"Unmappable extension method bases ({unmappableEmbCount}):");
            output.WriteLine();

            if (unmappableExtensionMethodBasesWithReasons.None())
            {
                output.WriteLine("<none> (good)");
            }
            else
            {
                var embGroupsByReason = unmappableExtensionMethodBasesWithReasons
                    .GroupBy(x => x.Item2)
                    .OrderAlphabetically(xGroup => xGroup.Key)
                    ;

                foreach (var group in embGroupsByReason)
                {
                    output.WriteLine($"# {group.Key}");

                    foreach (var embAndReason in group)
                    {
                        var typeName = Instances.NamespacedTypeName.GetTypeName(embAndReason.Item1.NamespacedTypeName);
                        
                        output.WriteLine($"{typeName}: {embAndReason.Item1.NamespacedTypeName}");
                    }
                }
            }
            output.WriteLine("\n***\n");
        }

        public static void WriteNewAndDepartedEmbsSummaryFile(this IOperation _,
            TextWriter output,
            IList<ExtensionMethodBase> newExtensionMethodBases,
            IList<ExtensionMethodBase> departedExtensionMethodBases)
        {
            // New extension method bases.
            var newExtensionMethodBasesCount = newExtensionMethodBases.Count;

            output.WriteLine($"New extension method bases ({newExtensionMethodBasesCount}):");
            output.WriteLine();

            if (newExtensionMethodBases.None())
            {
                output.WriteLine("<none> (ok)");
            }
            else
            {
                var typeNamedPairs = newExtensionMethodBases
                    .Select(x => (TypeName: Instances.NamespacedTypeName.GetTypeName(x.NamespacedTypeName), ExtensionMethodBase: x))
                    .OrderAlphabetically(x => x.TypeName)
                    ;

                foreach (var (TypeName, ExtensionMethodBase) in typeNamedPairs)
                {
                    output.WriteLine($"{TypeName}: {ExtensionMethodBase.NamespacedTypeName}");
                }
            }
            output.WriteLine("\n***\n");

            // Departed extension method bases.
            var departedExtensionMethodBasesCount = departedExtensionMethodBases.Count;

            output.WriteLine();
            output.WriteLine($"Departed extension method bases ({departedExtensionMethodBasesCount}):");
            output.WriteLine();

            if (departedExtensionMethodBases.None())
            {
                output.WriteLine("<none> (ok)");
            }
            else
            {
                var typeNamedPairs = departedExtensionMethodBases
                    .Select(x => (TypeName: Instances.NamespacedTypeName.GetTypeName(x.NamespacedTypeName), ExtensionMethodBase: x))
                    .OrderAlphabetically(x => x.TypeName)
                    ;

                foreach (var (TypeName, ExtensionMethodBase) in typeNamedPairs)
                {
                    output.WriteLine($"{TypeName}: {ExtensionMethodBase.NamespacedTypeName}");
                }
            }
            output.WriteLine("\n***\n");
        }
    }
}
