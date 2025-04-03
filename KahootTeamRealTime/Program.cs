using KahootTeamRealTime.HubSginalR;
using Microsoft.EntityFrameworkCore;
using Repositories.Infrastructures;
using Repositories.Models;
using Services.Interfaces;
using Services.Services;

namespace KahootTeamRealTime
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
          //  builder.Services.AddScoped<RealtimeQuizDbContext>();
            builder.Services.AddScoped<UnitOfWork>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<UnitOfWork>();
            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<RealtimeQuizDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add SignalR
            builder.Services.AddSignalR(e =>
            {
                e.EnableDetailedErrors = true;
                e.MaximumReceiveMessageSize = 102400000;
            });

            builder.Services.AddDistributedMemoryCache(); // Bộ nhớ cache cho session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session hết hạn sau 30 phút
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; // Đảm bảo session hoạt động ngay cả khi từ chối cookie không cần thiết
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.MapHub<QuizHub>("/quizHub");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Bật Session Middleware

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
