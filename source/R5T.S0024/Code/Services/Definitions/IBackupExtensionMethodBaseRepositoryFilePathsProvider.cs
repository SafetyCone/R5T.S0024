using System;
using System.Threading.Tasks;

using R5T.T0064;


namespace R5T.S0024
{
    [ServiceDefinitionMarker]
    public interface IBackupExtensionMethodBaseRepositoryFilePathsProvider : IServiceImplementation
    {
        Task<string> GetExtensionMethodBasesListingJsonFilePath();
        Task<string> GetExtensionMethodBaseNameSelectionsTextFilePath();
        Task<string> GetIgnoredExtensionMethodBaseNamesTextFilePath();
        Task<string> GetDuplicateExtensionMethodBaseNamesTextFilePath();
        Task<string> GetToProjectMappingsJsonFilePath();
    }
}
