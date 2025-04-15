
using DeliveryRentals.API.Middleware;
using DeliveryRentals.Application.Services;
using DeliveryRentals.Application.UseCases.Auth;
using DeliveryRentals.Domain.Entities;
using DeliveryRentals.Infrastructure.Messaging;
using DeliveryRentals.Infrastructure.Repositories;
using DeliveryRentals.Infrastructure.Seed;
using DeliveryRentals.Infrastructure.Security;
using DeliveryRentals.Infrastructure.Storage;
using DeliveryRentals.Persistence.Context;
using DeliveryRentals.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

AddRepositories(services);

AddServices(services);

services.AddScoped<IAuthorizationHandler, RoleHandler>();
services.AddHttpContextAccessor();
services.AddScoped<LoginHandler>();
services.AddControllers();

services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(configuration.GetConnectionString("Default")));

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(ConfigureSwagger);

var jwtKey = configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("Error: JWT Key not configured. Application will be terminated.");
	Console.ResetColor();
	Environment.Exit(1);
}

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options => ConfigureJwt(options, jwtKey));

services.AddAuthorization(options =>
{
	options.AddPolicy("CourierOnly", policy =>
		policy.Requirements.Add(new RoleRequirement("courier", "Only couriers can access this feature.")));

	options.AddPolicy("AdminOnly", policy =>
		policy.Requirements.Add(new RoleRequirement("admin", "Only administrators can access this feature.")));
});

services.AddHostedService<KafkaEventConsumerService>();

var app = builder.Build();

//app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();

app.Use(async (context, next) =>
{
	await next();

	if (context.Response.StatusCode == 403 &&
		context.Items.TryGetValue("AccessDeniedMessage", out var mensagem))
	{
		context.Response.ContentType = "application/json";
		var response = JsonSerializer.Serialize(new { error = mensagem?.ToString() });
		await context.Response.WriteAsync(response);
	}
});

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
	app.Use(async (context, next) =>
	{
		if (context.Request.Path == "/")
		{
			context.Response.Redirect("/swagger");
			return;
		}

		await next();
	});
}

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	//await db.Database.MigrateAsync();

	var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
	var user = new User("admin", "", "admin");
	await UserSeeder.SeedAsync(userRepository, user);
}

app.MapControllers();
app.Run();


// --- Auxiliary functions ---

void AddRepositories(IServiceCollection services)
{
	services.AddScoped<IEventRepository, EfEventRepository>();
	services.AddScoped<IMotoRepository, EfMotoRepository>();
	services.AddScoped<ICourierRepository, EfCourierRepository>();
	services.AddScoped<IRentalRepository, EfRentalRepository>();
	services.AddScoped<IUserRepository, EfUserRepository>();
	services.AddScoped<ILogRepository, EfLogRepository>();
}

void AddServices(IServiceCollection services)
{
	services.AddScoped<IEventConsumer, RegisteredMotoConsumer>();
	services.AddScoped<ICnhStorageService>(_ => new DiskCnhStorageService("CnhImages"));
	services.AddScoped<LoggingService>();
	services.AddScoped<IUserContextService, UserContextService>();
	services.AddSingleton<IEventPublisher>(_ => new KafkaEventPublisher("localhost:9092"));	
}

void ConfigureSwagger(SwaggerGenOptions options)
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Delivery Rentals API",
		Version = "v1",
		Description = "API for managing motorcycle rentals, couriers, and logistics."
	});

	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "Enter the JWT token. Ex: Bearer {your_token}",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
			new string[] {}
		}
	});

	options.EnableAnnotations();

	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	options.IncludeXmlComments(xmlPath);
}

void ConfigureJwt(JwtBearerOptions options, string jwtKey)
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
	};

	options.Events = new JwtBearerEvents
	{
		OnChallenge = context =>
		{
			context.HandleResponse();

			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			context.Response.ContentType = "application/json";

			var json = JsonSerializer.Serialize(new
			{
				error = "You must be authenticated to access this resource."
			});

			return context.Response.WriteAsync(json);
		}
	};
}