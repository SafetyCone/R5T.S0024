using System;
using System.Threading.Tasks;

using R5T.D0101;
using R5T.D0108;
using R5T.T0020;


namespace R5T.S0024
{
    [OperationMarker]
    public class O005_UpdateEmbToProjectMappings : IActionOperation
    {
        private IExtensionMethodBaseRepository ExtensionMethodBaseRepository { get; }
        private IProjectRepository ProjectRepository { get; }


        public O005_UpdateEmbToProjectMappings(
            IExtensionMethodBaseRepository extensionMethodBaseRepository,
            IProjectRepository projectRepository)
        {
            this.ExtensionMethodBaseRepository = extensionMethodBaseRepository;
            this.ProjectRepository = projectRepository;
        }

        public async Task Run()
        {
            // Get changes.
            var (newToProjectMappings, oldToProjectMappings) = await Instances.Operation.GetToProjectMappingChanges(
                this.ExtensionMethodBaseRepository,
                this.ProjectRepository);

            // Now make changes.
            await this.ExtensionMethodBaseRepository.DeleteToProjectMappings(oldToProjectMappings);

            await this.ExtensionMethodBaseRepository.AddToProjectMappings(newToProjectMappings);
        }
    }
}
