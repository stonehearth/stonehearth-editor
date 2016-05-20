using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects
{
   public abstract class PropertyValue
   {
      public abstract bool IsMissing { get; }

      public abstract bool IsValid();
   }
}
