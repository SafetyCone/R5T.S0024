using System;
using System.Threading.Tasks;


namespace R5T.S0024
{
    public interface IAllExtensionMethodBasesListingFilePathProvider
    {
        Task<string> GetAllExtensionMethodBaseNamesListingFileName();
    }
}
