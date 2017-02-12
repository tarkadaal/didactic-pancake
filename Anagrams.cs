using System;
using System.Collections.Generic;
using System.Linq;

namespace Trustpilot
{
  public class Anagrams
  {
    public static IEnumerable<string> Find(IEnumerable<string> wordlist, CharacterCountMap map, int maxWordCount)
    {
      foreach(var word in wordlist)
      {
        var wordmap = map.Subtract(word);
        if(wordmap != null)
        {
          if(wordmap.IsEmpty())
          {
            yield return word;
          }
          else if (maxWordCount > 0)
          {
            var nextWordlist = wordlist.Where(x => x != word);
            foreach(var s in Find(nextWordlist, wordmap, maxWordCount-1))
            {
              yield return word + " " + s;
            }
          }
        }
      }
    }
  }
}