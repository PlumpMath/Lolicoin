using System;

namespace Lolicoin.BlockChain
{
    [Serializable]
    public class BlockData
    {
        public string Description { get; set; }
        public int Amount         { get; set; }
    }
}