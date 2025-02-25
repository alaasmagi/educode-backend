using App.DAL.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using App.BLL;

var builder = WebApplication.CreateBuilder(args);



DotNetEnv.Env.Load("../.env");
var host = Environment.GetEnvironmentVariable("HOST");
var port = Environment.GetEnvironmentVariable("PORT");
var user = Environment.GetEnvironmentVariable("DB");
var dbKey = Environment.GetEnvironmentVariable("KEY");

var connectionString = $"Server={host};Port={port};Database={user};User={user};Password={dbKey};";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var jwtKey = Environment.GetEnvironmentVariable("JWTKEY");
var jwtAud = Environment.GetEnvironmentVariable("JWTAUD");
var jwtIss = Environment.GetEnvironmentVariable("JWTISS");

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

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=AdminPanel}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseAuthentication();
app.UseAuthorization();
app.Run();