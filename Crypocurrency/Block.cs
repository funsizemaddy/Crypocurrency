using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using EllipticCurve;

namespace Crypocurrency
{
    class Block
    {
        public int Index { get; set; }
        public string Prevhash { get; set; }
        public string TimeStamp { get; set; }
     
        public string Hash { get; set; }

        public int Nonce { get; set; }
        public List<Transaction> Transactions { get; set; }

        //Contructor
        public Block(int index, string timestamp, List<Transaction> transactions, string previousHash = "")
        {
            this.Index = index;
            this.TimeStamp = timestamp;
            this.Transactions = transactions;
            this.Prevhash = previousHash;
            this.Hash = CalculateHash();
            this.Nonce = 0;


        }
        public string CalculateHash()
        {
            string blockData = this.Index + this.Prevhash + this.TimeStamp + this.Transactions.ToString() + this.Nonce;
            byte[] blockBytes = Encoding.ASCII.GetBytes(blockData);
            byte[] hashBytes = SHA256.Create().ComputeHash(blockBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        public void Mine(int difficulty)
        {
            while (this.Hash.Substring(0,difficulty) != new string ('0', difficulty))
            {
                this.Nonce++;
                this.Hash = this.CalculateHash();
                Console.WriteLine("Mining " + this.Hash);
            }
            Console.WriteLine("Block has been mined: " + this.Hash);
        }

    }
}
