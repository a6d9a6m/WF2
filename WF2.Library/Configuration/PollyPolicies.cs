using Polly;
using Polly.Extensions.Http;

namespace WF2.Library.Configuration;

/// <summary>
/// Polly 弹性策略配置
/// </summary>
public static class PollyPolicies
{
    /// <summary>
    /// 获取重试策略
    /// 指数退避：第1次等待2秒，第2次等待4秒，第3次等待8秒
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // 处理5xx和408错误
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) // 处理404
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"[Polly] 重试第 {retryCount} 次，等待 {timespan.TotalSeconds} 秒。原因: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                });
    }

    /// <summary>
    /// 获取熔断器策略
    /// 连续失败5次后熔断30秒
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, duration) =>
                {
                    Console.WriteLine($"[Polly] 熔断器打开，持续时间: {duration.TotalSeconds} 秒。原因: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                },
                onReset: () =>
                {
                    Console.WriteLine("[Polly] 熔断器重置");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine("[Polly] 熔断器半开状态，尝试恢复");
                });
    }

    /// <summary>
    /// 获取超时策略
    /// 10秒超时
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(
            timeout: TimeSpan.FromSeconds(10),
            onTimeoutAsync: (context, timespan, task) =>
            {
                Console.WriteLine($"[Polly] 请求超时: {timespan.TotalSeconds} 秒");
                return Task.CompletedTask;
            });
    }
}
