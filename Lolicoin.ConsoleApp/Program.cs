using Lolicoin.BlockChain;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Lolicoin.ConsoleApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            sw.Start();

            var lolicoin = new Blockchain();

            lolicoin.AddBlock(new Block(1, DateTime.Today.TimeOfDay, new BlockData { Amount = 4, Description = "My first 4 lolicoins" }));
            int firstBlockTime = sw.Elapsed.Milliseconds;

            lolicoin.AddBlock(new Block(2, DateTime.Today.AddDays(1).TimeOfDay, new BlockData { Amount = 10, Description = "Another 10 lolicoins" }));
            int secondBlockTime = sw.Elapsed.Milliseconds;

            sw.Stop();

            Debug.WriteLine($"First Block added after {firstBlockTime} ms");
            Debug.WriteLine($"Second Block added after {secondBlockTime} ms");

            Console.WriteLine(JsonConvert.SerializeObject(lolicoin, Formatting.Indented));
            Debug.WriteLine($"Is Blockchain valid? {lolicoin.IsChainValid()}");

            Debug.WriteLine("Tampering block data...");
            lolicoin.Chain[1].Data = new BlockData { Amount = 100 };
            Debug.WriteLine($"Is Blockchain valid? {lolicoin.IsChainValid()}");

            Debug.WriteLine("Tampering block hash...");
            lolicoin.Chain[1].Hash = lolicoin.Chain[1].CalculateHash();
            Debug.WriteLine($"Is Blockchain valid? {lolicoin.IsChainValid()}");

            Console.ReadLine();
        }
    }
}