using Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitTorrent_Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Parse arguments
            string command, param;
            if (args.Length < 2)
            {
                throw new InvalidOperationException("Usage: <command> <param>");
            }
            command = args[0];
            param = args[1];

            // Parse command and act accordingly
            if (command == "decode")
            {
                Console.WriteLine(JsonSerializer.Serialize(Bencode.DecodeElement(param, 0, out _)));
            }
        }
    }
}
