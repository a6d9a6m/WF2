using Microsoft.Extensions.DependencyInjection;
using Refit;
using WF2.Library.Interfaces;
using WF2.Library.Services;

namespace WF2.Library.Configuration;

/// <summary>
/// 服务注册扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册Refit客户端和Polly策略
    /// </summary>
    public static IServiceCollection AddRefitClients(this IServiceCollection services)
    {
        // 注册WeatherAPI客户端，配置Polly策略
        services.AddRefitClient<IWeatherApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://api.weatherapi.com/v1"))
            .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
            .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy())
            .AddPolicyHandler(PollyPolicies.GetTimeoutPolicy());

        // 注册Pexels API客户端，配置Polly策略
        services.AddRefitClient<IPexelsApiClient>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri("https://api.pexels.com");
                // Pexels需要在请求头中添加Authorization
            })
            .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
            .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy())
            .AddPolicyHandler(PollyPolicies.GetTimeoutPolicy());

        // 注册WeatherService
        services.AddSingleton<IWeatherService, WeatherService>();

        return services;
    }
}
