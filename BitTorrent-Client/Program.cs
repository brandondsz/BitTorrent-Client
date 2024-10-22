using Client;
using System.Text;
using System.Text.Json;

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
            else if (command == "info")
            {
                var filePath = param;
                var encodedValueInBytes = File.ReadAllBytes(filePath);
                var encodedValue = Encoding.ASCII.GetString(encodedValueInBytes);
                var decodedValue = Bencode.DecodeElement(encodedValue, 0, out _);
                var serializedValue = JsonSerializer.Serialize(decodedValue);
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                TorrentFile torrentFile = JsonSerializer.Deserialize<TorrentFile>(serializedValue, jsonSerializerOptions)!;
                Console.WriteLine($"Tracker URL: {torrentFile.Announce}");
                Console.WriteLine($"Length: {torrentFile.Info.Length}");
            }
            else
            {
                throw new InvalidOperationException($"Invalid command: {command}");
            }
        }

        //Torrent file format - dictionary
        //announce:
        //    URL to a "tracker", which is a central server that keeps track of peers participating in the sharing of a torrent.
        //info:
        //    A dictionary with keys:
        //        length: size of the file in bytes, for single-file torrents
        //        name: suggested name to save the file / directory as
        //        piece length: number of bytes in each piece
        //        pieces: concatenated SHA-1 hashes of each piece

        record TorrentFile(string Announce, TorrentFileInfo Info);
        record TorrentFileInfo(int Length);



    }
}
