using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;


namespace Crypocurrency
{
    class Program
    {
        static void Main(string[] args)
        {
            PrivateKey key1 = new PrivateKey();
            PublicKey wallet1 = key1.publicKey();

            PrivateKey key2 =  new PrivateKey();
            PublicKey wallet2 = key2.publicKey();

            BlockChain doublecoin = new BlockChain(2,100);

            Console.WriteLine("Start the miner.");
            doublecoin.MinePendingTransactions(wallet1);
            Console.WriteLine(
                "\nBalance of wallet1 is $" + 
                doublecoin.GetBalanceofWallet(wallet1).ToString());

            Transaction tx1 = new Transaction(wallet1, wallet2, 10);
            tx1.SignTransaction(key1);
            doublecoin.addPendingTransaction(tx1);

            Console.WriteLine("Start the miner.");
            doublecoin.MinePendingTransactions(wallet2);
            Console.WriteLine(
                "\nBalance of wallet1 is $" +
                doublecoin.GetBalanceofWallet(wallet1).ToString());
            Console.WriteLine(
                "\nBalance of wallet2 is $" +
                doublecoin.GetBalanceofWallet(wallet2).ToString());


            string blockJSON = JsonConvert.SerializeObject(doublecoin, Formatting.Indented);
            Console.WriteLine(blockJSON);

            if (doublecoin.IsChainValid())
            {
                Console.WriteLine("BlockChain is valid");
            }
            else
            {
                Console.WriteLine("BlockChian is NOT valid");
            }
        }
    }
    
    
}
