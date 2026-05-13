using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusTicketSystem.MVC.ApiResponseWrapper
{
    // You can put this in your DTOs folder
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public object? Errors { get; set; }
        public T Data { get; set; } = default!;
        public int StatusCode { get; set; }

        public List<string> GetErrorList()
        {
            var result = new List<string>();
            if (Errors == null) return result;

            if (Errors is JsonElement element)
            {
                if (element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in element.EnumerateArray())
                    {
                        result.Add(item.GetString() ?? "");
                    }
                }
                else if (element.ValueKind == JsonValueKind.Object)
                {
                    foreach (var property in element.EnumerateObject())
                    {
                        if (property.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in property.Value.EnumerateArray())
                            {
                                result.Add(item.GetString() ?? "");
                            }
                        }
                        else
                        {
                            result.Add(property.Value.GetString() ?? "");
                        }
                    }
                }
            }
            else if (Errors is List<string> list)
            {
                return list;
            }
            
            return result;
        }
    }
}