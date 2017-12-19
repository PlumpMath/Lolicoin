namespace Lolicoin.BlockChain.Validation
{
    public class ValidationStateMachine
    {
        public bool IsChainValid(Block[] chain) =>
            new ActiveStateMachine(chain).Start();
    }
}