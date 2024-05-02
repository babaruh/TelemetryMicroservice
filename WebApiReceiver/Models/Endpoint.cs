namespace WebApiReceiver.Models;

public class Endpoint
{
    public string ServiceName { get; set; } = default!;

    public string Ipv4 { get; set; } = default!;

    public string Ipv6 { get; set; } = default!;

    public int Port { get; set; } = default!;
}
