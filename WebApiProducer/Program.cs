using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Utils.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<MessageSender>();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("TelemetryMicroservices"))
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
        .AddAspNetCoreInstrumentation()
        .AddSource(nameof(MessageSender))
        .AddZipkinExporter(b => { b.Endpoint = new Uri("http://localhost:9411/api/v2/spans"); }));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.MapControllers();

app.Run();
