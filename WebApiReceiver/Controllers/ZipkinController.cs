using Microsoft.AspNetCore.Mvc;
using WebApiReceiver.Services;

namespace WebApiReceiver.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ZipkinController(ILogger<ZipkinController> logger, ZipkinApiService apiService) : ControllerBase
{
    private static readonly string ServiceName = "telemetrymicroservices";

    [HttpGet("spans")]
    public async Task<IActionResult> GetTracesBySpans()
    {
        var spans = await apiService.GetSpans(ServiceName);
        if (spans is null)
            return NotFound("No spans found.");

        var grouped = spans.GroupBy(s => s.TraceId)
            .Select(g => new { TraceId = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToList();

        var longest = grouped.First();
        var shortest = grouped.Last();

        var longestTrace = await apiService.GetTrace(longest.TraceId);
        var shortestTrace = await apiService.GetTrace(shortest.TraceId);

        return Ok(new { LongestTrace = longestTrace, ShortestTrace = shortestTrace });
    }

    [HttpGet("duration")]
    public async Task<IActionResult> GetTracesByDuration()
    {
        var traces = await apiService.GetTracesAsync();
        if (traces is null)
            return NotFound("No traces found.");

        var longestTrace = traces.MaxBy(t => t.Duration);
        var shortestTrace = traces.MinBy(t => t.Duration);

        return Ok(new { LongestTrace = longestTrace, ShortestTrace = shortestTrace });
    }
}
