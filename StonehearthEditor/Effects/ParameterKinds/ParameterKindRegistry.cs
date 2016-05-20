using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects.ParameterKinds
{
   public sealed class ParameterKindOption
   {
      public string Kind { get; private set; }
      public Dimension Dimension { get; private set; }
      public bool TimeVarying { get; private set; }

      private readonly MethodInfo fromJsonMethod;

      private readonly Type type;

      public ParameterKindOption(Type type)
      {
         this.type = type;

         if (type.IsAbstract)
         {
            throw new Exception("Can't be abstract");
         }

         if (!type.IsSubclassOf(typeof(ParameterKind)))
         {
            throw new Exception("Must be subclass");
         }

         var matches = type.GetCustomAttributes(typeof(ParameterKindAttribute), false);
         if (matches.Length != 1)
         {
            throw new Exception("Must have exactly one ParmaterKindAttribute");
         }

         var attr = (ParameterKindAttribute)matches[0];

         this.Dimension = attr.Dimension;
         this.TimeVarying = attr.TimeVarying;
         this.Kind = attr.Kind;

         this.fromJsonMethod = type.GetMethod("FromJson", BindingFlags.Static | BindingFlags.Public);
         if (fromJsonMethod == null)
         {
            throw new Exception("Missing FromJson");
         }
         var fromJsonParams = fromJsonMethod.GetParameters();
         if (fromJsonParams.Length != 1 || fromJsonParams[0].ParameterType != typeof(JToken))
         {
            throw new Exception("FromJson parameters incorrect");
         }

         var ctor = type.GetConstructor(Type.EmptyTypes);
         if (ctor == null)
         {
            throw new Exception("Missing parameterless constructor");
         }
      }

      public ParameterKind FromJson(JToken obj)
      {
         return (ParameterKind)fromJsonMethod.Invoke(null, new[] { obj });
      }

      public ParameterKind Create()
      {
         return (ParameterKind)Activator.CreateInstance(this.type);
      }
   }

   public static class ParameterKindRegistry
   {
      private static readonly List<ParameterKindOption> options = new[]
      {
         typeof(ConstantScalarParameterKind),
         typeof(RandomBetweenScalarParameterKind),

         typeof(CurveScalarParameterKind),
         typeof(RandomBetweenCurvesScalarParameterKind),

         typeof(ConstantRgbaParameterKind),
      }
         .Select(type => new ParameterKindOption(type))
         .ToList();

      public static List<ParameterKindOption> GetOptions(Dimension dimension, bool timeVarying)
      {
         return options.Where(opt =>
         {
            if (opt.Dimension != dimension)
            {
               return false;
            }

            // If we are looking for time-varying options, constant ones count as well. But
            // if we want constant, then providing time-varying options isn't helpful
            if (!timeVarying && opt.TimeVarying)
            {
               return false;
            }

            return true;
         })
         .OrderBy(o => o.Kind)
         .ToList();
      }

      public static ParameterKindOption Get(string kind, Dimension dimension)
      {
         return options.Single(o => o.Kind == kind && o.Dimension == dimension);
      }
   }
}
