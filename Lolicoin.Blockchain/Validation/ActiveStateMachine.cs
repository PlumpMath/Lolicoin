using Lolicoin.StateMachine;

namespace Lolicoin.BlockChain.Validation
{
    public class ActiveStateMachine
    {
        private readonly Block[] _chain;

        public ActiveStateMachine(Block[] chain)
        {
            _chain = chain;
        }

        public bool Start()
        {
            var _state = new State("End", null, null, null);

            for (int i = 1; i < _chain.Length; i++)
            {
                var currentBlock  = _chain[i];
                var previousBlock = _chain[i - 1];

                // TODO: Implement state machine to validate blockchain

                if (currentBlock.Hash != currentBlock.CalculateHash())
                    return false;

                if (currentBlock.PreviousHash != previousBlock.Hash)
                    return false;
            }

            return true;
        }
    }
}