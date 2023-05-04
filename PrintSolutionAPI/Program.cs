using PrintSolutionAPI.Scheduler;

namespace PrintSolutionAPI
{
    public class Program
    {
        /// <summary>
        /// ���α׷� ���� �� ���� Ȯ��
        /// </summary>
        public static bool Running { get; private set; }

        /// <summary>
        /// ���α׷� ���� ����
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            #region ���α׷� ���� ����
            Running = true;

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

            // Web App ���� ����
            app.Run("http://*:9203");
            #endregion

            #region ���α׷� ���� ����
            // After: Ctrl + C
            Console.WriteLine("\r\nStoping Schedules...");
            Running = false;
            #endregion

        }
    }
}