using System.Text;
using System.Text.Json;

namespace Client
{
    public class Bencode
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
                Console.WriteLine(JsonSerializer.Serialize(Decode(param)));
            }
        }
        public static object Decode(string input)
        {
            switch (input[0])
            {
                case var c when Char.IsDigit(c):
                    return DecodeString(input);
                case 'i':
                    return DecodeInt(input);
                case 'l':
                    return DecodeList(input);
                case 'd':
                    return DecodeDictionary(input);
                default:
                    throw new InvalidOperationException($"invalid encoded value: {input}");
            }
        }
        private static string DecodeString(string input)
        {
            int colonIndex = input.IndexOf(":");
            if (colonIndex == -1)
            {
                throw new InvalidOperationException($"Invalid encoded string: {input}");
            }
            int lenght = int.Parse(input[..colonIndex]);
            return input.Substring(colonIndex + 1, lenght);
        }
        private static long DecodeInt(string input)
        {
            int endIndex = input.IndexOf('e', 1);
            if (endIndex == -1)
            {
                throw new InvalidOperationException(
                    $"Invalid encoded integer: {input}");
            }
            string number = input[1..endIndex];
            return long.Parse(number);
        }
        private static object[] DecodeList(string input)
        {
            List<object> elements = new();
            input = input[1..^1];
            while (input.Length > 0 && input[0] != 'e')
            {
                object element = Decode(input);
                elements.Add(element);
                string encodedElement = Encode(element);
                input = input[encodedElement.Length..];
            }
            return elements.ToArray();
        }
        private static Dictionary<string, object> DecodeDictionary(string input)
        {
            input = input[1..];
            var result = new Dictionary<string, object>();
            while (input.Length > 0 && input[0] != 'e')
            {
                var key = Decode(input);
                input = input[Encode(key).Length..];
                var value = Decode(input);
                input = input[Encode(value).Length..];
                if (key is string keyStr)
                {
                    result[keyStr] = value;
                }
                else
                {
                    throw new Exception("Dictionary key must be a string");
                }
            }
            return result;
        }
        private static string EncodeNonPrimitiveType(object input)
        {
            if (input is object[] inputArray)
            {
                return $"l{string.Join("", inputArray.Select(x => Encode(x)))}e";
            }
            else if (input is Dictionary<string, object> inputDictionary)
            {
                return $"d{string.Join("", inputDictionary.Values.Select(x => Encode(x)))}e";
            }
            else
            {
                throw new Exception($"Unknown type: {input.GetType().FullName}");
            }
        }
        public static string Encode(object input)
        {
            return input switch
            {
                long n => $"i{n}e",
                string s => $"{s.Length}:{s}",
                object[] arr => $"l{string.Join("", arr.Select(Encode))}e",
                object obj => EncodeNonPrimitiveType(input),
                _ => throw new Exception($"Unknown type: {input.GetType().FullName}")
            };
        }
    }
};