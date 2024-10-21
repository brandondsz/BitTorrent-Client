using System.Text;
using System.Text.Json;

namespace Client
{
    public class Bencode
    {
        
        public static object DecodeElement(string encodedValue, int index, out int endIndex)
        {
            int _endIndex;
            object decodedValue = encodedValue[index] switch
            {
                //<length>:<contents>
                var target when char.IsDigit(target) =>
                    DecodeString(encodedValue, index, out _endIndex),
                //i<number>e
                'i' => DecodeInteger(encodedValue, index + 1, out _endIndex),
                //l<bencoded_elements>e
                'l' => DecodeList(encodedValue, index + 1, out _endIndex),
                //d<key1><value1>...<keyN><valueN>e
                'd' => DecodeDictionary(encodedValue, index + 1, out _endIndex),
                _ => throw new InvalidOperationException("Invalid encoded value: " + encodedValue)
            };
            endIndex = _endIndex;
            return decodedValue;
        }

        static string DecodeString(string encodedValue, int firstDigitDetected, out int endIndex)
        {
            var colonIndex = encodedValue.IndexOf(':', firstDigitDetected);
            var length =
                int.Parse(encodedValue[firstDigitDetected..colonIndex].ToString());
            var startIndex = colonIndex + 1;
            endIndex = startIndex + length;
            var strValue = encodedValue[startIndex..endIndex];
            return strValue;
        }


        static long DecodeInteger(string encodedValue, int startIndex, out int endIndex)
        {
            endIndex = encodedValue.IndexOf('e', startIndex);
            if (endIndex != -1)
            {
                var strValue = encodedValue[startIndex..endIndex];
                endIndex++;
                return long.Parse(strValue);
            }
            else
            {
                throw new InvalidOperationException("Invalid encoded value: " +
                                                    encodedValue);
            }
        }

        static List<object> DecodeList(string encodedValue, int startIndex, out int endIndex)
        {
            var result = new List<object>();
            var index = startIndex;
            while (index < encodedValue.Length)
            {
                if (encodedValue[index] == 'e')
                {
                    break;
                }
                var decodedValue =
                    DecodeElement(encodedValue, index, out int separatorIndex);
                result.Add(decodedValue);
                index = separatorIndex;
            }
            endIndex = index + 1;
            return result;
        }


        static Dictionary<string, object> DecodeDictionary(string encodedValue, int startIndex, out int endIndex)
        {
            var result = new Dictionary<string, object>();
            var index = startIndex;
            var count = 0;
            while (index < encodedValue.Length)
            {
                count++;
                if (encodedValue[index] == 'e')
                {
                    break;
                }
                var key = DecodeString(encodedValue, index, out int keyEndIndex);
                var value =
                    DecodeElement(encodedValue, keyEndIndex, out int separatorIndex);
                result.Add(key, value);
                index = separatorIndex;
            }
            endIndex = index + 1;
            return result;
        }
    }
};