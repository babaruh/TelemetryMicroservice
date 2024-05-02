using WebApiReceiver.BackgroundServices;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Utils.Messaging;
using WebApiReceiver.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<MessageReceiver>();
builder.Services.AddTransient<ZipkinApiService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
