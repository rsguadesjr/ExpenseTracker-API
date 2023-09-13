
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using ExpenseTracker.Security;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IdentityModel.Tokens.Jwt;
using FirebaseAdmin.Auth;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.OutputCaching;
using System.Web;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using ExpenseTracker.CustomExceptionMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Add services to the container.
IdentityModelEventSource.ShowPII = true;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

if (builder.Environment.IsProduction())
{
    var keyVault = builder.Configuration["KeyVault:Uri"]!;
    builder.Configuration.AddAzureKeyVault(new Uri(keyVault), new DefaultAzureCredential(), new KeyVaultSecretManager());
}
else
{
    builder.Configuration.AddUserSecrets<Program>();
}

var firebaseConfig = builder.Configuration["Firebase:Config"];
var jwtKey = builder.Configuration["JwtSettings:Key"];


if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromJson(firebaseConfig),

    });
}

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            NameClaimType = "Name",
            RoleClaimType = "Role",
        };

    });

// Dependency Injection
ExpenseTracker.Model.DependencyInjection.Initialize(builder.Services);
ExpenseTracker.Business.DependencyInjection.Initialize(builder.Services);
ExpenseTracker.Repository.DependencyInjection.Initialize(builder.Services, builder.Configuration);


var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", b =>
    {
        b.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowedOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
