using App.DAL.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.RateLimiting;
using App.BLL;
using App.Domain;
using Contracts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load("../.env");

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit:14)  
    .CreateLogger();

var envInitializer = new EnvInitializer(loggerFactory.CreateLogger<EnvInitializer>());
envInitializer.InitializeEnv();
builder.Services.AddSingleton(envInitializer);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(); 
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);

builder.Services.AddSingleton<DbInitializer>();
builder.Services.AddScoped<IAdminAccessService, AdminAccessService>();
builder.Services.AddScoped<IAttendanceManagementService, AttendanceManagementService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourseManagementService, CourseManagementService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddSingleton<IHostedService, CleanupService>();

builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseNpgsql(envInitializer.PgDbConnection, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(60);
        npgsqlOptions.EnableRetryOnFailure(3);
    }), poolSize: 500);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(envInitializer.RedisConnection);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policyBuilder =>
    {
        if (!string.IsNullOrWhiteSpace(envInitializer.FrontendUrl))
        {
            policyBuilder
                .WithOrigins(envInitializer.FrontendUrl)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else
        {
            policyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    });

    options.DefaultPolicyName = "Frontend";
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(envInitializer.JwtKey!)),
            ValidateIssuer = true,
            ValidIssuer = envInitializer.JwtIssuer,
            ValidateAudience = true,
            ValidAudience = envInitializer.JwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromSeconds(1);
        limiterOptions.QueueLimit = 2;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddAuthorization(options =>
{
    foreach (EAccessLevel level in Enum.GetValues(typeof(EAccessLevel)))
    {
        options.AddPolicy(level.ToString(), policy =>
            policy.RequireAssertion(context =>
            {
                var userLevel = Helpers.GetAccessLevelFromClaims(context);
                return userLevel >= (int)level;
            }));
    }
});

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EduCodeAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Please enter 'Bearer' followed by your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/AdminPanel/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors("Frontend");
app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Educode API");
});

app.MapStaticAssets();

app.UseStaticFiles();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=AdminPanel}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect($"/AdminPanel/Index")).RequireRateLimiting("fixed");

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    dbInitializer.InitializeDb();
}

app.Run();