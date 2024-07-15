using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Backend_ASP_Project.Data;
using Backend_ASP_Project.Endpoint;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backend_ASP_Project.Author;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Backend_ASP_Project
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddDbContext<Backend_ASP_ProjectContext>(options =>
				options.UseSqlite(builder.Configuration.GetConnectionString("Backend_ASP_ProjectContext") ?? throw new InvalidOperationException("Connection string 'Backend_ASP_ProjectContext' not found.")));

			// Add services to the container.
			var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
			builder.Services.AddCors(options =>
			{
				options.AddPolicy(name: MyAllowSpecificOrigins,
					  policy =>
					  {
						  policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
					  });
			});

			builder.Services.AddAntiforgery(options =>
			{
				// Set Cookie properties using CookieBuilder properties†.
				options.FormFieldName = "AntiforgeryFieldname";
				options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
				options.SuppressXFrameOptionsHeader = false;
			});

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
				options.TokenValidationParameters = new TokenValidationParameters
				{
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DAMON_KEY_U_NOOB_1234567891234567")),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = "https://localhost:5236",
					ValidAudience = "localhost",
				}
			);
			builder.Services.AddAuthorization();

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			};

			app.UseHttpsRedirection();

			app.UseCors(MyAllowSpecificOrigins);

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath + "/Asset/Images")),
				RequestPath = "/Asset/Images"
			});

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath + "/Asset/PDF")),
				RequestPath = "/Asset/PDF"
			});

			app.UseAuthentication();

			app.UseAuthorization();
			app.MapControllers();

			app.MapBookEndpoints();

			app.MapGroupEndpoints();

			app.MapRoleEndpoints();

			app.MapUserEndpoints();

			app.Run();
		}
	}
}
