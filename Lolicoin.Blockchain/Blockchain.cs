using Lolicoin.BlockChain.Validation;
using System;
using System.Linq;

namespace Lolicoin.BlockChain
{
    [Serializable]
    public class Blockchain
    {
        public Blockchain()
        {
            Chain      = new Block[1] { CreateGenesisBlock() };
            Difficulty = 2;
        }

        public Block LatestBlock => Chain[Chain.Length - 1];
        public Block[] Chain  { get; set; }
        public int Difficulty { get; }

        private Block CreateGenesisBlock() =>
            new Block(0, DateTime.Today.TimeOfDay, new BlockData { Description = "Genesis Block" }, "0");

        public void AddBlock(Block newBlock)
        {
            newBlock.PreviousHash = LatestBlock.Hash;
            newBlock.Hash         = newBlock.MineCoin(Difficulty);
            Chain                 = Chain.Concat(new Block[1] { newBlock }).ToArray();
        }

        public bool IsChainValid() =>
            new ValidationStateMachine().IsChainValid(Chain);
    }
}