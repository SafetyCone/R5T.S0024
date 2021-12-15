using System;
using System.Threading.Tasks;

using R5T.D0101;
using R5T.D0108;


namespace R5T.S0024
{
    public class O005_UpdateEmbToProjectMappings : T0020.IOperation
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
