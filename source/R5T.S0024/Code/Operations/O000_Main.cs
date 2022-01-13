using System;
using System.Threading.Tasks;

using R5T.T0020;


namespace R5T.S0024
{
    [OperationMarker]
    public class O000_Main : IActionOperation
    {
        private O100_UpdateEmbRepositoryWithCurrentEmbs O100_UpdateEmbRepositoryWithCurrentEmbs { get; }


        public O000_Main(
            O100_UpdateEmbRepositoryWithCurrentEmbs o100_UpdateEmbRepositoryWithCurrentEmbs)
        {
            this.O100_UpdateEmbRepositoryWithCurrentEmbs = o100_UpdateEmbRepositoryWithCurrentEmbs;
        }

        public async Task Run()
        {
            await this.O100_UpdateEmbRepositoryWithCurrentEmbs.Run();
        }
    }
}
