
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using ExpenseTracker.Security;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

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

if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(builder.Configuration["Firebase:ConfigPath"])
    });
}


//var firebaseid = "rsg-expense-tracker";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, opt =>
    {
    } );


var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

ExpenseTracker.Model.DependencyInjection.Initialize(builder.Services);
ExpenseTracker.Business.DependencyInjection.Initialize(builder.Services);
ExpenseTracker.Repository.DependencyInjection.Initialize(builder.Services, builder.Configuration);


var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? new string[] { };
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

app.MapControllers();

app.Run();
