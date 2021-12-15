using System;
using System.Threading.Tasks;

using R5T.Quadia.D002;


namespace R5T.S0024
{
    public class AllExtensionMethodBasesListingFilePathProvider : IAllExtensionMethodBasesListingFilePathProvider
    {
        public const string FileName = "Extension Method Base Names-All.txt";


        private IOrganizationSharedDataDirectoryFilePathProvider OrganizationSharedDataDirectoryFilePathProvider { get; }


        public AllExtensionMethodBasesListingFilePathProvider(
            IOrganizationSharedDataDirectoryFilePathProvider organizationSharedDataDirectoryFilePathProvider)
        {
            this.OrganizationSharedDataDirectoryFilePathProvider = organizationSharedDataDirectoryFilePathProvider;
        }

        public async Task<string> GetAllExtensionMethodBaseNamesListingFileName()
        {
            var output = await this.OrganizationSharedDataDirectoryFilePathProvider.GetFilePath(AllExtensionMethodBasesListingFilePathProvider.FileName);
            return output;
        }
    }
}
