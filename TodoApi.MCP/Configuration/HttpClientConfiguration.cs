using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace TodoApi.MCP.Configuration;

public static class HttpClientConfiguration
{
    public static IServiceCollection AddTodoApiHttpClient(this IServiceCollection services)
    {
        return services.AddSingleton(_ =>
        {
            var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        });
    }
} 