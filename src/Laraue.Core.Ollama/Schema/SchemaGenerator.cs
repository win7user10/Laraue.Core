using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laraue.Core.Ollama.Schema;

public static class SchemaGenerator
{
    public static OllamaSchemaProperty GetSchema(Type outputType)
    {
        var propertyType = GetOllamaType(outputType);

        return propertyType switch
        {
            SchemaPropertyType.Object => GetObjectSchema(outputType),
            SchemaPropertyType.Array => GetArraySchema(outputType),
            _ => new OllamaSchemaProperty
            {
                Type = [propertyType]
            }
        };
    }

    private static OllamaSchemaObjectProperty GetObjectSchema(Type outputType)
    {
        var resultProperties = new Dictionary<string, OllamaSchemaProperty>();
        
        var properties = outputType.GetProperties();
        foreach (var property in properties)
        {
            resultProperties.Add(property.Name, GetSchema(property.PropertyType));
        }

        return new OllamaSchemaObjectProperty
        {
            Properties = resultProperties,
            Type = [SchemaPropertyType.Object]
        };
    }
    
    private static OllamaSchemaArrayProperty GetArraySchema(Type outputType)
    {
        var elementClrType = outputType.GetElementType() ?? throw new InvalidOperationException();
        var elementType =  GetOllamaType(elementClrType);

        switch (elementType)
        {
            case SchemaPropertyType.Object:
            {
                var schema = GetObjectSchema(elementClrType);
                return new OllamaSchemaArrayProperty
                {
                    Items = new OllamaSchemaArrayItem
                    {
                        Type = [elementType],
                        Properties = schema.Properties,
                    },
                    Type = [SchemaPropertyType.Array]
                };
            }
            case SchemaPropertyType.Array:
                return new OllamaSchemaArrayProperty
                {
                    Items = new OllamaSchemaArrayItem
                    {
                        Type = [SchemaPropertyType.Array],
                    },
                    Type = [SchemaPropertyType.Array]
                };
            default:
                return new OllamaSchemaArrayProperty
                {
                    Items = new OllamaSchemaArrayItem
                    {
                        Type = [elementType],
                    },
                    Type = [SchemaPropertyType.Array]
                };
        }
    }
    
    private static SchemaPropertyType GetOllamaType(Type type)
    {
        if (type == typeof(string))
        {
            return SchemaPropertyType.String;
        }

        if (type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double) || type == typeof(decimal))
        {
            return SchemaPropertyType.Number;
        }

        if (type == typeof(bool))
        {
            return SchemaPropertyType.Boolean;
        }

        if (type == typeof(DateTime))
        {
            return SchemaPropertyType.String;
        }

        if (type.IsArray || typeof(IList).IsAssignableFrom(type))
        {
            return SchemaPropertyType.Array;
        }

        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            return SchemaPropertyType.Object;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            return SchemaPropertyType.Array;
        }

        return SchemaPropertyType.Object;
    }
    
    public class OllamaSchemaObjectProperty : OllamaSchemaProperty
    {
        [JsonPropertyName("properties")]
        public required Dictionary<string, OllamaSchemaProperty> Properties { get; init; }
    }
    
    public class OllamaSchemaArrayProperty : OllamaSchemaProperty
    {
        public OllamaSchemaArrayProperty()
        {
            Type = [SchemaPropertyType.Array];
        }
        
        [JsonPropertyName("items")]
        public required OllamaSchemaArrayItem Items { get; init; }
    }

    public class OllamaSchemaArrayItem
    {
        [JsonPropertyName("type")]
        public required SchemaPropertyType[] Type { get; init; }
        
        [JsonPropertyName("properties")]
        public Dictionary<string, OllamaSchemaProperty>? Properties { get; init; }
    }
}