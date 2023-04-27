using PrintSolutionAPI.Scheduler;

namespace PrintSolutionAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Start Scheduler
            LoadStatusSch.Start();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            } 

            app.UseAuthorization();

            app.MapControllers();

            app.UseCors(builder =>
                builder.WithOrigins("*")
                       .AllowAnyHeader()
                );
            app.Run("http://*:9203");
        }
    }
}