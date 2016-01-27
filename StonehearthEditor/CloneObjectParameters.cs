using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor
{
   public class CloneObjectParameters
   {
      private Dictionary<string, string> mStringReplacements = new Dictionary<string, string>();
      public void AddStringReplacement(string original, string replacement)
      {
         mStringReplacements[original] = replacement;
      }

      public string TransformParameter(string param)
      {
         string newString = param;
         foreach(KeyValuePair<string, string> replacement in mStringReplacements)
         {
            newString = newString.Replace(replacement.Key, replacement.Value);
         }
         return newString;
      }

      public bool IsDependency(string dependencyName)
      {
         foreach (string originalName in mStringReplacements.Keys)
         {
            if (dependencyName.Contains(originalName))
            {
               return true;
            }
         }
         return false;
      }
   }
}
