using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects
{
   public abstract class Property
   {
      /// <summary>
      /// Converts an object to its json representation.
      /// </summary>
      /// <param name="value">The object to convert.</param>
      /// <returns>The json representation, or null to skip.</returns>
      public abstract JToken ToJson(PropertyValue value);
      /// <summary>
      /// Converts json into its value representation.
      /// </summary>
      /// <param name="json">The json representation or null if missing.</param>
      /// <returns></returns>
      public abstract PropertyValue FromJson(JToken json);
      /// <summary>
      /// Gets the name of the property.
      /// </summary>
      public abstract string Name { get; }
   }
}
