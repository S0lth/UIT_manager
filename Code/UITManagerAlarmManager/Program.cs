

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UITManagerAlarmManager.Data;
using UITManagerAlarmManager.Service;

class Program
{
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
        
        
        
        using (var context = serviceProvider.GetRequiredService<ApplicationDbContext>())
        {
            var connection = new HubConnectionBuilder().WithUrl( "http://localhost:5014/ApiHub").Build();
            
            connection.On<int>("ReceiveMessage", message => {
                Console.WriteLine(message);
                var trigger = new TriggerAlarm(context);
                trigger.Triggered(message);
            });
            
            try {
                await connection.StartAsync();
                Console.WriteLine("Connection established");
                Console.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}