using App.DAL.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.RateLimiting;
using App.BLL;
using Microsoft.AspNetCore.RateLimiting;


var builder = WebApplication.CreateBuilder(args);



DotNetEnv.Env.Load("../.env");
var host = Environment.GetEnvironmentVariable("HOST");
var port = Environment.GetEnvironmentVariable("PORT");
var user = Environment.GetEnvironmentVariable("USER");
var db = Environment.GetEnvironmentVariable("DB");
var dbKey = Environment.GetEnvironmentVariable("DBKEY");

var connectionString = $"Server={host};Port={port};Database={db};User={user};Password={dbKey};";

builder.Services.AddTransient<EmailSender>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var jwtKey = Environment.GetEnvironmentVariable("JWTKEY");
var jwtAud = Environment.GetEnvironmentVariable("JWTAUD");
var jwtIss = Environment.GetEnvironmentVariable("JWTISS");

var frontendUrl = Environment.GetEnvironmentVariable("FRONTENDURL");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins(frontendUrl ?? string.Empty)
            .AllowAnyMethod()
            .AllowAnyHeader());
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
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.QueueLimit = 2;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddAuthorization(); 
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
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

app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();


app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Educode API");
});

app.MapStaticAssets();

app.UseStaticFiles();


app.UseCors("AllowFrontend");

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=AdminPanel}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.MapGet("/", () => Results.Redirect($"/AdminPanel/Index")).RequireRateLimiting("fixed");
app.Run();