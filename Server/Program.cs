
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Shared.DTOs;
using Shared.Entities;
using System.Text;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<SchoolDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<SchoolDbContext>()
            .AddDefaultTokenProviders();
            var key = Encoding.ASCII.GetBytes("Nagyon_Hosszu_Es_Titkos_Kulcs_12345"); // Legalább 32 karakter!
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("BlazorPolicy", policy =>
                {
                    policy.WithOrigins("https://localhost:7091") 
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); 
                });
            });
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            TypeAdapterConfig<Student, StudentDto>.NewConfig()
             .Map(dest => dest.SchoolClassName, src => src.SchoolClass.Name);

            TypeAdapterConfig<Course, CourseDto>.NewConfig()
             .Map(dest => dest.SchoolClassName, src => src.SchoolClass.Name)
             .Map(dest => dest.TeacherName, src => src.Teacher.Name);
            
            TypeAdapterConfig<Grade, GradeDto>.NewConfig()
                .Map(dest => dest.CourseName, src => src.Course.Subject.Name);

            var app = builder.Build();
            app.UseRouting();
            app.UseCors("BlazorPolicy");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
