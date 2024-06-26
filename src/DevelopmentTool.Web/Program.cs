using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Cors", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        var types = new[]
        {
            typeof(Program)
        };

        foreach (var type in types)
        {
            var docPath = type.Assembly.Location.Replace(".dll", ".xml");
            if (File.Exists(docPath))
                options.IncludeXmlComments(docPath, true);
        }
    });
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        //options.InstanceName = "SampleInstance";
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AnyOrigin", policy =>
        {
            policy.AllowAnyOrigin() //允许任何来源的主机访问
                .AllowAnyMethod()
                .AllowAnyHeader();
                //.AllowCredentials(); //指定处理cookie
        });
    });

    // 使用日志
    builder.Host.UseSerilog((context, logger) =>
    {
        logger.ReadFrom.Configuration(context.Configuration);
        logger.Enrich.FromLogContext();
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseCors("AnyOrigin");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}");

    app.MapGet("tool/current-system-time", () => DateTime.UtcNow);

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}