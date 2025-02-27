
using birthday_notification_api.Servicios;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace birthday_notification_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //builder.Services.AddHealthChecks().AddMySql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "database");

            builder.Services.AddScoped<BirthdayService>();

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("https://birthdayapi.sykos.dev"). 
                           //AllowAnyOrigin().
                           AllowAnyHeader().
                           AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            //app.MapHealthChecks("/health", new HealthCheckOptions
            //{
            //    ResponseWriter = async (context, report) =>
            //    {
            //        context.Response.ContentType = "application/json";

            //        var response = new
            //        {
            //            status = report.Status.ToString(),
            //            checks = report.Entries.Select(entry => new
            //            {
            //                check = entry.Key,
            //                status = entry.Value.Status.ToString(),
            //            }),
            //            duration = report.TotalDuration
            //        };

            //        await context.Response.WriteAsJsonAsync(response);
            //    }
            //});


            app.MapControllers();

            app.Run();
        }
    }
}
