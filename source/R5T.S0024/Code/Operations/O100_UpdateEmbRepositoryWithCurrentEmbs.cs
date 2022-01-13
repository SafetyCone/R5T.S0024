using System;
using System.Threading.Tasks;

using R5T.T0020;


namespace R5T.S0024
{
    [OperationMarker]
    public class O100_UpdateEmbRepositoryWithCurrentEmbs : IActionOperation
    {
        private O001_AnalyzeAllCurrentEmbs O001_AnalyzeAllCurrentEmbs { get; }
        private O002_BackupFileBasedRepositoryFiles O002_BackupFileBasedRepositoryFiles { get; }
        private O003_PerformRequiredHumanActions O003_PerformRequiredHumanActions { get; }
        private O004_UpdateEmbRepository O004_UpdateEmbRepository { get; }
        private O005_UpdateEmbToProjectMappings O005_UpdateEmbToProjectMappings { get; }


        public O100_UpdateEmbRepositoryWithCurrentEmbs(
            O001_AnalyzeAllCurrentEmbs o001_AnalyzeAllCurrentEmbs,
            O002_BackupFileBasedRepositoryFiles o002_BackupFileBasedRepositoryFiles,
            O003_PerformRequiredHumanActions o003_PerformRequiredHumanActions,
            O004_UpdateEmbRepository o004_UpdateEmbRepository,
            O005_UpdateEmbToProjectMappings o005_UpdateEmbToProjectMappings)
        {
            this.O001_AnalyzeAllCurrentEmbs = o001_AnalyzeAllCurrentEmbs;
            this.O002_BackupFileBasedRepositoryFiles = o002_BackupFileBasedRepositoryFiles;
            this.O003_PerformRequiredHumanActions = o003_PerformRequiredHumanActions;
            this.O004_UpdateEmbRepository = o004_UpdateEmbRepository;
            this.O005_UpdateEmbToProjectMappings = o005_UpdateEmbToProjectMappings;
        }

        public async Task Run()
        {
            await this.O001_AnalyzeAllCurrentEmbs.Run();
            await this.O002_BackupFileBasedRepositoryFiles.Run();
            await this.O003_PerformRequiredHumanActions.Run();
            await this.O004_UpdateEmbRepository.Run();
            await this.O005_UpdateEmbToProjectMappings.Run();
        }
    }
}
