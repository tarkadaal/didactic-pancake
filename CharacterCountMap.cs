using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
namespace Trustpilot
{
  public class CharacterCountMap : Dictionary<char, int>
  {
    public CharacterCountMap(CharacterCountMap that) : base(that) { }
    public CharacterCountMap() : base() { }

    public CharacterCountMap(string text) : base()
    {
      foreach (var c in text.Replace(" ", ""))
      {
        var sane = c;
        if(sane == '\'') continue;
        if(sane > 'z') sane = Sanitize(sane);
        if (!ContainsKey(sane)) Add(c, 0);
        this[sane]++;
      }
    }

    public bool AreEqual(CharacterCountMap that)
    {
      if (this.Keys.Count != that.Keys.Count) return false;
      foreach (var k in this.Keys)
      {
        if (!that.ContainsKey(k)) return false;
        if (this[k] != that[k]) return false;
      }
      return true;
    }

    public CharacterCountMap Subtract(string word)
    {
      var that = new CharacterCountMap(this);
      foreach(var c in word)
      {
        if(!that.ContainsKey(c) || that[c] == 0) return null;
        that[c]--;
      }
      return that;
    }

    public bool IsEmpty()
    {
      return Values.Sum() == 0;
    }

    private char Sanitize(char input)
    {
      var inputString = input.ToString();
      foreach(var c in Enumerable.Range('A', 'Z' - 'A' + 1).Select(c => (char)c))
      {
        if (String.Compare(inputString, c.ToString(), StringComparison.CurrentCulture) == 0)
        {
          return c;
        }
      }
      return input;
    }
  }
}