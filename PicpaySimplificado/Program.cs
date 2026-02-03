using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PicpaySimplificado.Infra;
using PicpaySimplificado.Infra.Repository.Carteiras;
using PicpaySimplificado.Infra.Repository.Transferencias;
using PicpaySimplificado.Services.Autorizador;
using PicpaySimplificado.Services.Carteiras;
using PicpaySimplificado.Services.Notificacoes;
using PicpaySimplificado.Services.Transferencias;
using PicPaySimplificado.Infra;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "PicpaySimplificado";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation() 
        .AddEntityFrameworkCoreInstrumentation() 
        .AddConsoleExporter()) 
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddMeter("Microsoft.AspNetCore.Hosting") 
        .AddConsoleExporter());


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serverVersion = new MySqlServerVersion(new Version(8, 0, 45));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("defaultConnection"), serverVersion));

builder.Services.AddScoped<ICarteiraRepository, CarteiraRepository>();
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
builder.Services.AddScoped<ICarteiraService, CarteiraService>();

builder.Services.AddHttpClient<IAutorizadorService, AutorizadorService>();
builder.Services.AddScoped<INotificacaoService, NotificacaoService>();

builder.Services.AddScoped<ITransferenciaService, TransferenciaService>();

var app = builder.Build();

app.UseHttpMetrics();
app.MapMetrics();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
