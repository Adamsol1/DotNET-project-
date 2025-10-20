using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;


using DOTNET_PROJECT.Application;
using DOTNET_PROJECT.Infrastructure.Data;
using DOTNET_PROJECT.Infrastructure.Repositories;
using DOTNET_PROJECT.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI registrations
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Specific repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IPlayerCharacterRepository, PlayerCharacterRepository>();
builder.Services.AddScoped<IStoryNodeRepository, StoryNodeRepository>();
builder.Services.AddScoped<IDialogueRepository, DialogueRepository>();
builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();

// Application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IGameService, GameService>();



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

app.UseSession();
app.UseExceptionHandler("/Home/Error");
app.UseHsts();
app.UseHttpsRedirection();

/**
    * Enable static files to serve images from wwwroot folder
    * Images are stored in wwwroot/images folder
    * Example: https://localhost:5169/images/character1.png
    */
app.UseStaticFiles();
app.UseRouting();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/Auth/Login"));
app.Run();
