using System.Collections.Concurrent;
using System.Threading;

namespace PosTechChallenge.Monitoring;

public interface IExecutionTimeMonitor
{
    void Record(string endpoint, TimeSpan duration);

    ExecutionTimeSummary GetSummary();
}

public sealed class ExecutionTimeMonitor : IExecutionTimeMonitor
{
    private long _totalRequests;
    private long _totalTicks;
    private readonly ConcurrentDictionary<string, EndpointExecutionStats> _endpoints = new();

    public void Record(string endpoint, TimeSpan duration)
    {
        Interlocked.Increment(ref _totalRequests);
        Interlocked.Add(ref _totalTicks, duration.Ticks);

        var stats = _endpoints.GetOrAdd(endpoint, _ => new EndpointExecutionStats());
        stats.Record(duration);
    }

    public ExecutionTimeSummary GetSummary()
    {
        var totalRequests = Interlocked.Read(ref _totalRequests);
        var totalTicks = Interlocked.Read(ref _totalTicks);
        var totalDuration = TimeSpan.FromTicks(totalTicks);
        var averageMilliseconds = totalRequests == 0 ? 0 : totalDuration.TotalMilliseconds / totalRequests;

        var endpoints = _endpoints
            .Select(pair => pair.Value.ToSummary(pair.Key))
            .OrderByDescending(summary => summary.TotalRequests)
            .ThenBy(summary => summary.Endpoint)
            .ToList();

        return new ExecutionTimeSummary(
            TotalRequests: totalRequests,
            TotalDurationMs: totalDuration.TotalMilliseconds,
            AverageDurationMs: averageMilliseconds,
            Endpoints: endpoints);
    }

    private sealed class EndpointExecutionStats
    {
        private long _requests;
        private long _ticks;

        public void Record(TimeSpan duration)
        {
            Interlocked.Increment(ref _requests);
            Interlocked.Add(ref _ticks, duration.Ticks);
        }

        public EndpointExecutionSummary ToSummary(string endpoint)
        {
            var requests = Interlocked.Read(ref _requests);
            var totalTicks = Interlocked.Read(ref _ticks);
            var totalDuration = TimeSpan.FromTicks(totalTicks);
            var averageMilliseconds = requests == 0 ? 0 : totalDuration.TotalMilliseconds / requests;

            return new EndpointExecutionSummary(
                Endpoint: endpoint,
                TotalRequests: requests,
                TotalDurationMs: totalDuration.TotalMilliseconds,
                AverageDurationMs: averageMilliseconds);
        }
    }
}

public sealed record ExecutionTimeSummary(
    long TotalRequests,
    double TotalDurationMs,
    double AverageDurationMs,
    IReadOnlyCollection<EndpointExecutionSummary> Endpoints);

public sealed record EndpointExecutionSummary(
    string Endpoint,
    long TotalRequests,
    double TotalDurationMs,
    double AverageDurationMs);