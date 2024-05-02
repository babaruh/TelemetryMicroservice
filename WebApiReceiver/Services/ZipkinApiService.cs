using Newtonsoft.Json;
using WebApiReceiver.Models;

namespace WebApiReceiver.Services;

public class ZipkinApiService(HttpClient client)
{
    private static readonly string ZipkinEndpoint = "http://localhost:9411/";

    public async Task<List<Trace>?> GetTracesAsync()
    {
        var response = await client.GetAsync($"{ZipkinEndpoint}/api/v2/traces");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var nestedTraces = JsonConvert.DeserializeObject<List<List<Trace>>>(json);

        var flatTraces = nestedTraces?.SelectMany(x => x).ToList();

        return flatTraces;
    }

    public async Task<Trace?> GetTrace(string traceId)
    {
        var response = await client.GetAsync($"{ZipkinEndpoint}/api/v2/trace/{traceId}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var nestedTraces = JsonConvert.DeserializeObject<List<Trace>>(json);

        var trace = nestedTraces?.FirstOrDefault();

        return trace;
    }

    public async Task<List<Span>?> GetSpans(string serviceName)
    {
        var response = await client.GetAsync($"{ZipkinEndpoint}/api/v2/traces?serviceName={serviceName}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var nestedSpans = JsonConvert.DeserializeObject<List<List<Span>>>(json);

        var flatSpans = nestedSpans?.SelectMany(x => x).ToList();

        return flatSpans;
    }
}
