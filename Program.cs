using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Trustpilot;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication
{
  public class Program
  {
    public const string searchText = "poultry outwits ants";
    public const string easyMd5 = "e4820b45d2277f3844eac66c903e84be";
    public const string mediumMd5 = "23170acc097c24edb98fc5488ab033fe";
    public const string hardMd5 = "665e5bcb0c20062fe8abaaf4628bb154";

    public static void Main(string[] args)
    {
      if (args.Length != 2) Console.WriteLine("usage: dotnet run min_word_length max_word_count");
      var minWordLength = int.Parse(args[0]);
      var maxWordCount = int.Parse(args[1]);
      var map = new CharacterCountMap(searchText);
      var wordlist = LoadWordlist(minWordLength, map);
      //foreach(var anagram in Anagrams.Find(wordlist, map, maxWordCount))
      Parallel.ForEach(wordlist, x =>
      {
        Console.WriteLine("Processing subtree for {0}", x);
        foreach(var anagram in Anagrams.Find(
          wordlist.Where(y => y!=x), 
          map.Subtract(x),
          maxWordCount - 1))
        {
          var final = x + " " + anagram;
          Console.WriteLine(final);
          CheckForMatch(final);
        }
      });
    }

    public static void CheckForMatch(string anagram){
      var md5 = MD5.Create();
      var hash = GetMd5Hash(md5, anagram);
      CheckAgainst(hash, easyMd5, "easy", anagram);
      CheckAgainst(hash, mediumMd5, "medium", anagram);
      CheckAgainst(hash, hardMd5, "hard", anagram);

    }

    public static void CheckAgainst(string actual, string expected, string label, string anagram)
    {
      if(actual == expected)
      {
        Console.WriteLine("********** MATCH ({0}): {1} ***********", label, anagram);
        File.WriteAllText(label, anagram);
      }
    }

    static string GetMd5Hash(MD5 md5Hash, string input)
    {

      // Convert the input string to a byte array and compute the hash.
      byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

      // Create a new Stringbuilder to collect the bytes
      // and create a string.
      StringBuilder sBuilder = new StringBuilder();

      // Loop through each byte of the hashed data 
      // and format each one as a hexadecimal string.
      for (int i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }

      // Return the hexadecimal string.
      return sBuilder.ToString();
    }

    public static IEnumerable<string> LoadWordlist(int minWordLength, CharacterCountMap map)
    {
      var lines = File.ReadAllLines("wordlist2").Distinct();
      Console.WriteLine("Original wordlist has {0} unique elements.", lines.Count());
      var filtered = lines
        .Where(x => x.Length >= minWordLength)
        .Where(x => map.Subtract(x) != null);
      Console.WriteLine(
        "There are {0} elements >= {1} letters long, that are contained in the map.",
        filtered.Count(),
        minWordLength);
      return filtered;
    }
  }
}
