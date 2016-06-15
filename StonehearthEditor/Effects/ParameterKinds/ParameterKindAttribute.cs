using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects.ParameterKinds
{
   [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
   public class ParameterKindAttribute : Attribute
   {
      public string Kind { get; private set; }

      public Dimension Dimension { get; private set; }

      public bool TimeVarying { get; private set; }

      public ParameterKindAttribute(string kind, Dimension dimension, bool timeVarying)
      {
         this.Kind = kind;
         this.Dimension = dimension;
         this.TimeVarying = timeVarying;
      }
   }
}
