namespace WebApiReceiver.Models;

public class Span
{
    public string Id { get; set; } = default!;

    public string TraceId { get; set; } = default!;

    public string ParentId { get; set; } = default!;

    public string Name { get; set; } = default!;

    public long Timestamp { get; set; } = default!;

    public int Duration { get; set; } = default!;

    public string Kind { get; set; } = default!;

    public Endpoint LocalEndpoint { get; set; } = default!;

    public Endpoint RemoteEndpoint { get; set; } = default!;

    public Dictionary<string, string> Tags { get; set; } = default!;
}
