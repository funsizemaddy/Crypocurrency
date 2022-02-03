using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EllipticCurve;
using System.Security.Cryptography;

namespace Crypocurrency
{
    class BlockChain
    {
        public List<Block> Chain { get; set; }
        public int Difficulty { get; set; }
        public List<Transaction> pendingTransactions { get; set; }
        public decimal MiningReward { get; set; }


        public BlockChain(int difficulty,decimal miningreward)
        {
            this.Chain = new List<Block>();
            this.Chain.Add(CreateGenesisBlock());
            this.Difficulty = difficulty;
            this.MiningReward = miningreward;
            this.pendingTransactions = new List<Transaction>();
        }

        public Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyyMMddHHssffff"), new List<Transaction>());
        }

        public Block GetLastestBlock()
        {
            return this.Chain.Last();

        }

        public void AddBlock(Block newBlock)
        {
            newBlock.Prevhash = this.GetLastestBlock().Hash;
            newBlock.Hash = newBlock.CalculateHash();
            this.Chain.Add(newBlock);
        }

        public void addPendingTransaction (Transaction transaction)
        {
            if (transaction.FromAddress is null || transaction.ToAddress is null)
            {
                throw new Exception("Transaction must include a to and from address.");
            }
            if (transaction.Amount > this.GetBalanceofWallet(transaction.FromAddress))
            {
                throw new Exception("There must be sufficient money in the wallet");
            }
            if (transaction.IsValid() == false)
            {
                throw new Exception("Cannot add an invalid transaction to a block.");
            }
            this.pendingTransactions.Add(transaction);
        }
        public decimal GetBalanceofWallet(PublicKey address)
        {
            decimal balance = 0;

            string addressDER = BitConverter.ToString(address.toDer()).Replace("-", "");
            

            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if (!(transaction.FromAddress is null))
                    {
                        string fromDER = BitConverter.ToString(transaction.FromAddress.toDer()).Replace("-", "");
                        
                        if (fromDER == addressDER)
                        {
                            balance -= transaction.Amount;
                        }
                        string toDER = BitConverter.ToString(transaction.ToAddress.toDer()).Replace("-", "");
                        if (toDER == addressDER)
                        {
                            balance += transaction.Amount;
                        }
                    }
                }
            }
            return balance;
        }

        public void MinePendingTransactions(PublicKey miningRewardWallet)
        {
            Transaction rewardTx = new Transaction(null, miningRewardWallet, MiningReward);
            this.pendingTransactions.Add(rewardTx);

            Block newBlock = new Block(GetLastestBlock().Index + 1, DateTime.Now.ToString("yyyyMMddHHssffff"), this.pendingTransactions, GetLastestBlock().Hash);
            newBlock.Mine(this.Difficulty);

            Console.WriteLine("Block successfully mined");
            this.Chain.Add(newBlock);
            this.pendingTransactions = new List<Transaction>();
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < this.Chain.Count; i++)
            {
                Block currentBlock = this.Chain[i];
                Block previousBlock = this.Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.Prevhash != previousBlock.Hash)
                {
                    return false;
                }
            }

            return true;
        }
    }

}
