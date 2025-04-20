using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Only apply to operations with multipart/form-data content type
        var consumes = context.MethodInfo.GetCustomAttribute<Microsoft.AspNetCore.Mvc.ConsumesAttribute>();
        if (consumes == null || !consumes.ContentTypes.Contains("multipart/form-data"))
            return;

        // Find parameters with IFormFile or FromForm attribute
        var fileParameters = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) ||
                   p.GetCustomAttribute<Microsoft.AspNetCore.Mvc.FromFormAttribute>() != null)
            .ToList();

        if (!fileParameters.Any())
            return;

        // Remove existing parameters that will be handled in form
        foreach (var parameter in fileParameters)
        {
            var existingParam = operation.Parameters
                .FirstOrDefault(p => p.Name == parameter.Name);

            if (existingParam != null)
                operation.Parameters.Remove(existingParam);
        }

        // Create schema
        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>(),
            Required = new HashSet<string>()
        };

        // Process parameters
        foreach (var parameter in fileParameters)
        {
            if (parameter.ParameterType == typeof(IFormFile))
            {
                // File parameter
                schema.Properties[parameter.Name] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                };

                if (!parameter.IsOptional)
                    schema.Required.Add(parameter.Name);
            }
            else if (parameter.ParameterType.IsClass && parameter.ParameterType != typeof(string))
            {
                // Process complex type properties
                foreach (var prop in parameter.ParameterType.GetProperties())
                {
                    if (prop.PropertyType == typeof(IFormFile))
                    {
                        // File property
                        schema.Properties[prop.Name] = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        };
                    }
                    else
                    {
                        // Regular property
                        schema.Properties[prop.Name] = new OpenApiSchema
                        {
                            Type = "string"
                        };
                    }
                }
            }
        }

        // Set request body
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = schema
                }
            },
            Required = true
        };
    }
}