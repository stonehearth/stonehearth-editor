using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using ChildSchemaValidationError = NJsonSchema.Validation.ChildSchemaValidationError;
using NJSValidationError = NJsonSchema.Validation.ValidationError;
using ValidationErrorKind = NJsonSchema.Validation.ValidationErrorKind;

namespace StonehearthEditor
{
    // Tools to perform high level tasks on JsonSchema4s.
    internal static class JsonSchemaTools
    {
        private static readonly TaskFactory taskFactory = new TaskFactory(
            CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static JsonSchema4 ParseSchema(string schemaJson, string pathForReferenceResolution)
        {
            return taskFactory.StartNew(() => JsonSchema4.FromJsonAsync(schemaJson, pathForReferenceResolution)).Unwrap().GetAwaiter().GetResult();
        }

        public static JsonSchema4 Clone(JsonSchema4 original)
        {
            return ParseSchema(original.ToJson(), original.DocumentPath);
        }

        public class AnnotatedSchema
        {
            public JsonSchema4 Schema;
            public string Name;
            public string Title;
            public string Description;
            public bool IsRequired;
        }

        internal enum ValidationResult
        {
            Valid,
            InvalidJson,
            DoesNotSatisfySchema,
        }

        internal struct ValidationError
        {
            public int LineNumber;
            public string Message;

            public ValidationError(int lineNumber, string message)
            {
                LineNumber = lineNumber;
                Message = message;
            }
        }

        public static Tuple<ValidationResult, Dictionary<int, string>> Validate(JsonSchema4 schema, string jsonToValidate)
        {
            // Find errors.
            ValidationResult result = ValidationResult.Valid;
            Dictionary<int, string> errors = new Dictionary<int, string>();
            try
            {
                if (schema == null)
                {
                    // No schema. Just make sure the JSON is valid.
                    JToken.Parse(jsonToValidate);
                }
                else
                {
                    // Validate based on the schema.
                    var rawErrors = schema.Validate(jsonToValidate);
                    if (rawErrors.Count > 0)
                    {
                        result = ValidationResult.DoesNotSatisfySchema;
                        foreach (var rawError in schema.Validate(jsonToValidate))
                        {
                            foreach (var error in GenerateErrorMessages(rawError))
                            {
                                if (error.LineNumber <= 0)
                                {
                                    continue;
                                }

                                var zeroBasedLine = error.LineNumber - 1;
                                if (errors.ContainsKey(zeroBasedLine))
                                {
                                    errors[zeroBasedLine] += '\n' + error.Message;
                                }
                                else
                                {
                                    errors[zeroBasedLine] = error.Message;
                                }
                            }
                        }
                    }
                }
            }
            catch (JsonReaderException exception)
            {
                result = ValidationResult.InvalidJson;
                errors[exception.LineNumber - 1] = exception.Message;
            }

            return new Tuple<ValidationResult, Dictionary<int, string>>(result, errors);
        }

        private static IEnumerable<ValidationError> GenerateErrorMessages(NJSValidationError error)
        {
            switch (error.Kind)
            {
                case ValidationErrorKind.AdditionalPropertiesNotValid:
                case ValidationErrorKind.AdditionalItemNotValid:
                case ValidationErrorKind.NotAllOf:
                    // The child error gives all the relevant info for these. The parent error is just confusing.
                    break;
                case ValidationErrorKind.PatternMismatch:
                    yield return new ValidationError(error.LineNumber, string.Format("The value of '{0}' must match the regex pattern: {1}", error.Property, error.Schema.Pattern));
                    break;
                case ValidationErrorKind.NotInEnumeration:
                    if (error.Schema.Enumeration.Count == 1)
                    {
                        yield return new ValidationError(error.LineNumber, string.Format("The value of '{0}' must be {1}", error.Property, error.Schema.Enumeration.First().ToString()));
                    }
                    else
                    {
                        string choices = "";
                        foreach (var choice in error.Schema.Enumeration)
                        {
                            if (choices.Length > 0)
                            {
                                choices += ", ";
                            }

                            choices += '"';
                            choices += choice == null ? "null" : choice.ToString();
                            choices += '"';
                        }

                        yield return new ValidationError(error.LineNumber, string.Format("The value of '{0}' must be one of: {1}", error.Property, choices));
                    }

                    break;
                case ValidationErrorKind.NoAdditionalPropertiesAllowed:
                    string validProperties = string.Join(", ", error.Schema.ActualProperties.Keys);
                    yield return new ValidationError(error.LineNumber, string.Format(
                            "Property '{0}' is not expected in this object. Valid properties: {1}", error.Property, validProperties));
                    break;
                case ValidationErrorKind.PropertyRequired:
                    yield return new ValidationError(error.LineNumber, string.Format("Missing required property '{0}'.", error.Property));
                    break;
                case ValidationErrorKind.NotAnyOf:
                    string validFormats = " Valid formats:";
                    foreach (var alternativeSchema in error.Schema.AnyOf)
                    {
                        validFormats += "\n  ";
                        validFormats += DescribeSchema(alternativeSchema);
                    }

                    yield return new ValidationError(error.LineNumber, string.Format(
                        "None of the {0} valid formats for {1} match.{2}",
                        error.Schema.AnyOf.Count,
                        string.IsNullOrEmpty(error.Property) ? "the element" : ("'" + error.Property + "'"),
                        validFormats));

                    // Show sub-errors for the closest matching alternative.
                    var multiError = error as ChildSchemaValidationError;
                    if (multiError != null)
                    {
                        int minNumSubErrors = int.MaxValue;
                        var bestSubErrors = new List<NJSValidationError>();
                        foreach (var subErrors in multiError.Errors)
                        {
                            if (subErrors.Value.Count < minNumSubErrors)
                            {
                                bestSubErrors.Clear();
                                bestSubErrors.AddRange(subErrors.Value);
                                minNumSubErrors = subErrors.Value.Count;
                            }
                            else if (subErrors.Value.Count == minNumSubErrors)
                            {
                                bestSubErrors.AddRange(subErrors.Value);
                            }
                        }

                        foreach (var rawSubError in bestSubErrors)
                        {
                            foreach (var subError in GenerateErrorMessages(rawSubError))
                            {
                                yield return subError;
                            }
                        }
                    }

                    yield break;  // Don't process sub-errors. We've handled them already.
                default:
                    var errorString = Regex.Replace(error.Kind.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).ToLower().Trim();
                    if (error.Property != null)
                    {
                        errorString = error.Property + ": " + errorString;
                    }

                    yield return new ValidationError(error.LineNumber, errorString);
                    break;
            }

            // Show all sub-errors.
            if (error is ChildSchemaValidationError)
            {
                foreach (var subErrors in (error as ChildSchemaValidationError).Errors)
                {
                    foreach (var rawSubError in subErrors.Value)
                    {
                        foreach (var subError in GenerateErrorMessages(rawSubError))
                        {
                            yield return subError;
                        }
                    }
                }
            }
        }

        public static string DescribeSchema(JsonSchema4 schema)
        {
            AnnotatedSchema annotatedSchema = new AnnotatedSchema();
            annotatedSchema.Schema = schema;
            annotatedSchema.Title = schema.Title;
            annotatedSchema.Description = schema.Description;
            annotatedSchema.IsRequired = (schema as JsonProperty)?.IsRequired ?? false;
            return DescribeSchema(annotatedSchema);
        }

        public static string DescribeSchema(AnnotatedSchema annotatedSchema)
        {
            var prefix = annotatedSchema.IsRequired ? "[required] " : "";

            if (annotatedSchema.Title != null)
            {
                return prefix + annotatedSchema.Title;
            }

            var schema = annotatedSchema.Schema;
            if (schema.AnyOf.Count > 0)
            {
                return prefix + schema.AnyOf.Count + " possible formats";
            }

            if (schema.Enumeration.Count == 1)
            {
                return prefix + "Must be " + FormatEnumValue(schema.Enumeration.First());
            }

            if (schema.Enumeration.Count > 0)
            {
                return prefix + "One of: " + string.Join(", ", schema.Enumeration.Select(o => FormatEnumValue(o)));
            }

            switch (schema.Type)
            {
                case NJsonSchema.JsonObjectType.Boolean:
                    return prefix + "Boolean";
                case NJsonSchema.JsonObjectType.Integer:
                case NJsonSchema.JsonObjectType.Number:
                    return DescribeNumberType(schema);
                case NJsonSchema.JsonObjectType.Array:
                    return prefix + "Array [...]";
                case NJsonSchema.JsonObjectType.Object:
                    return prefix + "Object {...}";
                case NJsonSchema.JsonObjectType.String:
                    return prefix + "String";
                case NJsonSchema.JsonObjectType.Null:
                    return prefix + "Must be null";
                case NJsonSchema.JsonObjectType.File:
                case NJsonSchema.JsonObjectType.None:
                default:
                    return prefix + "Unspecified format";
            }
        }

        public static string DescribeNumberType(JsonSchema4 schema)
        {
            var result = schema.Type == JsonObjectType.Integer ? "Integer" : "Number";
            if (schema.Minimum != null || schema.Maximum != null)
            {
                result += ' ';
                if (schema.Maximum == null)
                {
                    result += (schema.IsExclusiveMinimum ? "> " : ">= ") + schema.Minimum;
                }
                else if (schema.Minimum == null)
                {
                    result += (schema.IsExclusiveMaximum ? "< " : "<= ") + schema.Maximum;
                }
                else
                {
                    result += (schema.IsExclusiveMinimum ? "(" : "[") + schema.Minimum;
                    result += ", ";
                    result += schema.Maximum.ToString() + (schema.IsExclusiveMaximum ? ")" : "]");
                }
            }

            return result;
        }

        public static string FormatEnumValue(object o)
        {
            return o is string ? '"' + o.ToString() + '"' : JToken.FromObject(o).ToString();
        }

        public static ICollection<AnnotatedSchema> GetSchemasForPath(JsonSchema4 schema, List<string> path, JToken contextObject, string activeProperty)
        {
            var currentSchemas = new List<JsonSchema4> { schema };
            var result = new Dictionary<JsonSchema4, AnnotatedSchema>();

            // Walk the path.
            foreach (var step in path)
            {
                currentSchemas = DescendIntoSchemas(currentSchemas, step);
                if (currentSchemas.Count == 0)
                {
                    return new HashSet<AnnotatedSchema>();
                }
            }

            // Discard less likely schemas.
            currentSchemas = GuessIntendedSchemas(CleanAndDedupeSchemas(currentSchemas).Select(a => a.Schema).ToList(), contextObject);

            // Descend one last time if we are suggesting a property value, after we've finished filtering.
            if (activeProperty != null)
            {
                currentSchemas = DescendIntoSchemas(currentSchemas, activeProperty);
            }

            // Clean/dedupe.
            return CleanAndDedupeSchemas(currentSchemas);
        }

        public static ICollection<AnnotatedSchema> CleanAndDedupeSchemas(List<JsonSchema4> schemas)
        {
            var result = new Dictionary<JsonSchema4, AnnotatedSchema>();
            foreach (var currentSchema in schemas)
            {
                var alternatives = (currentSchema.ActualSchema.AnyOf.Count > 0) ? currentSchema.ActualSchema.AnyOf : new JsonSchema4[] { currentSchema };
                foreach (var alternative in alternatives)
                {
                    AnnotatedSchema entry = new AnnotatedSchema();
                    entry.Schema = alternative.ActualSchema;
                    entry.Title = currentSchema.Title ?? entry.Schema.Title;
                    entry.Description = currentSchema.Description ?? entry.Schema.Description;
                    if (currentSchema is JsonProperty)
                    {
                        entry.Name = (currentSchema as JsonProperty).Name;
                        entry.IsRequired = (currentSchema as JsonProperty).IsRequired;
                    }

                    result[entry.Schema] = entry;
                }
            }

            return result.Values;
        }

        private static List<JsonSchema4> DescendIntoSchemas(List<JsonSchema4> roots, string property)
        {
            var result = new List<JsonSchema4>();

            foreach (JsonSchema4 root in roots)
            {
                result.AddRange(DescendIntoSchema(root, property));
            }

            return result;
        }

        private static List<JsonSchema4> DescendIntoSchema(JsonSchema4 root, string property)
        {
            if (root.SchemaReference != null)
            {
                root = root.SchemaReference;
            }

            if (root.AnyOf.Count > 0)
            {
                var result = new List<JsonSchema4>();
                foreach (var alternative in root.AnyOf)
                {
                    result.AddRange(DescendIntoSchema(alternative, property));
                }

                return result;
            }

            if (root.AllOf.Count == 2 && root.AllOf.First().SchemaReference != null && root.AllOf.Last().Type == JsonObjectType.Object)
            {
                // This is technically wrong, since our suggestions might satisfy one of the schemas but not the other,
                // but in practice the only important usage of AllOf for us is inheritance, so this is good enough.
                // In the conditional above, we verify that this is being used for inheritance by making sure that
                // one of the requirements is a schema reference, and the other an inline object definition.
                var result = new List<JsonSchema4>();
                foreach (var alternative in root.AllOf)
                {
                    result.AddRange(DescendIntoSchema(alternative, property));
                }

                return result;
            }

            switch (root.Type)
            {
                case JsonObjectType.Array:
                    if (property == "0")
                    {
                        if (root.Item != null)
                        {
                            return new List<JsonSchema4> { root.Item };
                        }

                        if (root.AdditionalItemsSchema != null)
                        {
                            return new List<JsonSchema4> { root.AdditionalItemsSchema };
                        }
                    }

                    return new List<JsonSchema4>();
                case JsonObjectType.Object:
                    if (root.ActualProperties != null && root.ActualProperties.ContainsKey(property))
                    {
                        return new List<JsonSchema4> { root.ActualProperties[property] };
                    }

                    var result = new List<JsonSchema4>();
                    if (root.PatternProperties != null)
                    {
                        foreach (var pair in root.PatternProperties)
                        {
                            if (Regex.IsMatch(property, pair.Key))
                            {
                                result.Add(pair.Value);
                            }
                        }
                    }

                    if (root.AdditionalPropertiesSchema != null)
                    {
                        result.Add(root.AdditionalPropertiesSchema);
                    }

                    return result;
                case JsonObjectType.None:
                case JsonObjectType.Null:
                case JsonObjectType.Boolean:
                case JsonObjectType.Integer:
                case JsonObjectType.Number:
                case JsonObjectType.String:
                case JsonObjectType.File:
                default:
                    return new List<JsonSchema4>();
            }
        }

        private static int ScoreSchemaMatch(JsonSchema4 schema, JToken contextObject)
        {
            if (contextObject is JObject && (schema.Type == JsonObjectType.Object || schema.Type == JsonObjectType.None))
            {
                var errors = schema.Validate(contextObject).Where(e => e.Kind != ValidationErrorKind.PropertyRequired);
                return 100 - errors.Count();
            }
            else if (schema.Type == JsonObjectType.Array && (contextObject is JArray))
            {
                return 100 - schema.Validate(contextObject).Count;
            }
            else
            {
                return 0;  // Non-objects can't match.
            }
        }

        private static List<JsonSchema4> GuessIntendedSchemas(List<JsonSchema4> schemas, JToken contextObject)
        {
            if (schemas.Count <= 1 || contextObject == null)
            {
                return schemas;
            }

            var bestScore = 0;
            var bestSchemas = new List<JsonSchema4>();
            foreach (var schema in schemas)
            {
                var score = ScoreSchemaMatch(schema, contextObject);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestSchemas.Clear();
                    bestSchemas.Add(schema);
                }
                else if (score == bestScore)
                {
                    bestSchemas.Add(schema);
                }
            }

            return bestSchemas;
        }
    }
}
