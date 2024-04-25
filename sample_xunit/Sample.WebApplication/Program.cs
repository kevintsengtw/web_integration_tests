using Correlate.AspNetCore;
using Correlate.DependencyInjection;
using FluentValidation.AspNetCore;
using Sample.Domain.Repositories;
using Sample.Repository.Helpers;
using Sample.Repository.Implements;
using Sample.Service.Implements;
using Sample.Service.Interface;
using Sample.WebApplication.Infrastructure.ServiceCollections;
using Sample.WebApplication.Infrastructure.Wrapper.ExceptionHandlers;
using Sample.WebApplication.Infrastructure.Wrapper.Filters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, _, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                       .Enrich.FromLogContext()
                       .WriteTo.Debug()
                       .WriteTo.Console();
});

// Add services to the container.

builder.Services.AddControllers(configure => { configure.Filters.Add<SampleActionResultFilter>(); })
       .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });

builder.Services.AddFluentValidationAutoValidation(configuration => { configuration.DisableDataAnnotationsValidation = true; });

// builder.Services.AddKeyedScoped<IValidator, ShipperIdParameterValidator>(nameof(ShipperIdParameter));
// builder.Services.AddKeyedScoped<IValidator, ShipperParameterValidator>(nameof(ShipperParameter));
// builder.Services.AddKeyedScoped<IValidator, ShipperPageParameterValidator>(nameof(ShipperPageParameter));
// builder.Services.AddKeyedScoped<IValidator, ShipperSearchParameterValidator>(nameof(ShipperSearchParameter));
// builder.Services.AddKeyedScoped<IValidator, ShipperUpdateParameterValidator>(nameof(ShipperUpdateParameter));
builder.Services.AddKeyedParameterValidators();

builder.Services.AddExceptionHandler<FluentValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var basePath = AppContext.BaseDirectory;
    var xmlFiles = Directory.EnumerateFiles(basePath, searchPattern: "*.xml", SearchOption.TopDirectoryOnly);

    foreach (var xmlFile in xmlFiles)
    {
        options.IncludeXmlComments(xmlFile);
    }
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddCorrelate(options => options.RequestHeaders = ["X-Correlation-ID"]);

builder.Services.AddMapSter();
builder.Services.AddRedisConfigurationOptions();
builder.Services.AddDatabaseConnectionOptions();

builder.Services.AddScoped<IDatabaseHelper, DatabaseHelper>();
builder.Services.AddScoped<IShipperRepository, ShipperRepository>();
builder.Services.AddScoped<IShipperService, ShipperService>();

var app = builder.Build();

app.UseCorrelate();
app.UseExceptionHandler(_ => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

/// <summary>
/// Program
/// </summary>
public partial class Program
{
    // 建立 Program 的 partial class, 這是因為 top-level statements 模式下要做整合測試時的權宜方式.
}