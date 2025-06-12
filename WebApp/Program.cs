using App.DAL.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading.RateLimiting;
using App.BLL;
using Contracts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load("../.env");
/*var host = Environment.GetEnvironmentVariable("HOST");
var port = Environment.GetEnvironmentVariable("PORT");
var user = Environment.GetEnvironmentVariable("USER");
var db = Environment.GetEnvironmentVariable("DB");
var dbKey = Environment.GetEnvironmentVariable("DBKEY");*/

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

var jwtKey = Environment.GetEnvironmentVariable("JWTKEY");
var jwtAud = Environment.GetEnvironmentVariable("JWTAUD");
var jwtIss = Environment.GetEnvironmentVariable("JWTISS");

var frontendUrl = Environment.GetEnvironmentVariable("FRONTENDURL");

builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(60);
        npgsqlOptions.EnableRetryOnFailure(3);
    }), poolSize: 500);



Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit:14)  
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(); 
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);

builder.Services.AddScoped<IAdminAccessService, AdminAccessService>();
builder.Services.AddScoped<IAttendanceManagementService, AttendanceManagementService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourseManagementService, CourseManagementService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

builder.Services.AddSingleton<IHostedService, CleanupService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(frontendUrl ?? "")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        
    });
    
    options.DefaultPolicyName = "Frontend";
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey!)),
            ValidateIssuer = true,
            ValidIssuer = jwtIss,
            ValidateAudience = true,
            ValidAudience = jwtAud,
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

builder.Services.AddAuthorization(); 
builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EduCodeAPI", Version = "v1" });

    // Adding JWT Bearer authentication to Swagger
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

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.MapGet("/", () => Results.Redirect($"/AdminPanel/Index")).RequireRateLimiting("fixed");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.SeedUserTypes(context);
    DbInitializer.SeedCourseStatuses(context);
    DbInitializer.SeedAttendanceTypes(context);

}

app.Run();

app.Run();