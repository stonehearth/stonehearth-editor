using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects.ParameterKinds
{
   public abstract class ParameterKind
   {
      public abstract JToken ToJson();

      public abstract bool IsValid { get; }
   }
}
