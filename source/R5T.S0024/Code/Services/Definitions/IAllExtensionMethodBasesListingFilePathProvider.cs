using System;
using System.Threading.Tasks;using R5T.T0064;


namespace R5T.S0024
{[ServiceDefinitionMarker]
    public interface IAllExtensionMethodBasesListingFilePathProvider:IServiceDefinition
    {
        Task<string> GetAllExtensionMethodBaseNamesListingFileName();
    }
}
