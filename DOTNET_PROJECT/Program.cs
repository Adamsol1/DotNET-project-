using Microsoft.EntityFrameworkCore;
using DOTNET_PROJECT.Infrastructure.Data;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
                            e.Level == LogEventLevel.Information &&
                            e.MessageTemplate.Text.Contains("Executed DbCommand"));

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);


var app = builder.Build();


// Seed the database and apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await DbSeeder.SeedAsync(dbContext);
        logger.Information("Database migration and seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.Error(ex, "An error occurred while migrating or seeding the database.");
        throw; // Re-throw the exception after logging it
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

/**
    * Enable static files to serve images from wwwroot folder
    * Images are stored in wwwroot/images folder
    * Example: https://localhost:5169/images/character1.png
    */
app.UseStaticFiles();

/**
    * In production, the frontend and backend should be hosted on the same domain
    * and cors should be configured accordingly.
    * cors for the frontend to access the api
    * frontend hosted on localhost:3000
    * backend hosted on localhost:5169
    */
app.UseCors(cors => cors.WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    );

app.UseAuthorization();

app.MapControllers();

app.Run();
