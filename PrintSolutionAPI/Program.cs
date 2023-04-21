namespace PrintSolutionAPI
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
                builder.WithOrigins("http://127.0.0.1:5501")
                       .AllowAnyHeader()
                );
            app.Run("http://localhost:9203");
        }
    }
}