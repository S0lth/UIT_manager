using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using UITManagerAlarmManager.Data;
using UITManagerAlarmManager.Service;

namespace UITManagerAlarmManager;

class Program {
    static async Task Main(string[] args) {
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var connectionApi = new HubConnectionBuilder().WithUrl( "http://localhost:5014/ApiHub").Build();
        var connectionWebApp = new HubConnectionBuilder().WithUrl( "http://localhost:5287/WebAppHub").Build();
        
        try {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //Email email = new Email(context);
            //email.Send("pomme");

            // API Hub: Handle messages
            /*connectionApi.On<int>("ReceiveMessage", async message =>
            {
                Console.WriteLine($"API Hub message received: {message}");
                await TriggerAlarm.TriggeredAsync(context, message);
            });

            await connectionApi.StartAsync();
            Console.WriteLine("Connection to API Hub established");*/

            // WebApp Hub: Handle messages
            connectionWebApp.On<int>("ReceiveMessage", async message =>
            {
                Console.WriteLine($"WebApp Hub message received: {message}");
                
                await Task.Delay(30000);
                await TriggerAlarm.UpdateAlarmAsync(context, message);
            });

            await connectionWebApp.StartAsync();
            Console.WriteLine("Connection to WebApp Hub established");

            Console.WriteLine("Press Ctrl+C to exit...");
            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception e) {
            Console.WriteLine(e);
        }
        finally {
            await connectionApi.DisposeAsync();
            await connectionWebApp.DisposeAsync();
        }
        
    }
}