using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BranchCrudAPI_JWT.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BranchCrudAPI_JWT.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BranchCrudAPI_JWTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BranchCrudAPI_JWTContext") ?? throw new InvalidOperationException("Connection string 'BranchCrudAPI_JWTContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["AppSettings:Secret"]))
        };
    });

//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        builder => builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()); // Allow credentials if needed
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//cors
app.UseCors("AllowLocalhost3000");

//jwt
app.UseAuthentication();

//to display 'no token' or 'invalid token' message 
app.UseMiddleware<TokenValidationMiddleware>(builder.Configuration["AppSettings:Secret"]);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
