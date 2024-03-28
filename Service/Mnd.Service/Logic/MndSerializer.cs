using System.Text.Json;

namespace Mnd.Service.Logic;

public static class MndSerializer
{
    public static string Serialize<TValue>(TValue value)
    {
        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}