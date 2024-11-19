using UITManagerWebServer.Models;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;


public static class Populate {
    public static void Initialize(IServiceProvider serviceProvider) {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        if (context.Machines.Any() || context.NormGroups.Any()) {
            context.Alarms.RemoveRange(context.Alarms);
            context.Notes.RemoveRange(context.Notes);
            context.Norms.RemoveRange(context.Norms);
            context.NormGroups.RemoveRange(context.NormGroups);
            context.Machines.RemoveRange(context.Machines);

            context.SaveChanges();
            Console.WriteLine("Database cleared successfully.");
        }

        var normGroups = new List<NormGroup> {
            new NormGroup {
                Name = "Obsolete operating ................. ................ ................" ,
                Priority = 8,
                Severity = SeverityLevel.Critical,
                Norms = new List<Norm> { new Norm { Name = "Windows 10 detected" } }
            },
            new NormGroup {
                Name = "Storage exceded",
                Priority = 4,
                Severity = SeverityLevel.High,
                Norms = new List<Norm> { new Norm { Name = "Storage over than 80%" } }
            },
            new NormGroup {
                Name = "Storage exceded",
                Priority = 1,
                Severity = SeverityLevel.Low,
                Norms = new List<Norm> { new Norm { Name = "Storage over than 30%" } }
            }
        };
        context.NormGroups.AddRange(normGroups);

// Créer des Machines
        var machines = new List<Machine> {
            new Machine { Name = "ThinkPad de Germain" },
            new Machine { Name = "ThinkPad de Alex" },
            new Machine { Name = "Legion de Mathis" },
            new Machine { Name = "Acer de Pauline" },
            new Machine { Name = "VivoBook de Romain" },
            new Machine { Name = "ThinkBook de Dorain" },
            new Machine { Name = "Laptop de Florentin" },
            new Machine { Name = "Desktop-AAAAAAA" },
            new Machine { Name = "Desktop-BBBBBBB" },
            new Machine { Name = "Desktop-CCCCCCC" },

        };
        context.Machines.AddRange(machines);

        var alarms = new List<Alarm> {
            new Alarm {
                Status = AlarmStatus.New,
                TriggeredAt = DateTime.UtcNow,
                Machine = machines[0],
                NormGroup = normGroups[0]
            },
            new Alarm {
                Status = AlarmStatus.Acknowledged,
                TriggeredAt = DateTime.UtcNow.AddHours(-2),
                Machine = machines[1],
                NormGroup = normGroups[1]
            },
            new Alarm {
                Status = AlarmStatus.Acknowledged,
                TriggeredAt = DateTime.UtcNow.AddHours(-2),
                Machine = machines[5],
                NormGroup = normGroups[1]
            },
            new Alarm {
                Status = AlarmStatus.New,
                TriggeredAt = DateTime.UtcNow,
                Machine = machines[5],
                NormGroup = normGroups[0]
            },
            new Alarm {
                Status = AlarmStatus.New,
                TriggeredAt = DateTime.UtcNow,
                Machine = machines[6],
                NormGroup = normGroups[0]
            },

        };
        context.Alarms.AddRange(alarms);

        var notes = new List<Note> {

            new Note {
                Content = "check OS everything is ok because is on linux",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Machine = machines[0],
                IsSolution = true
            },
            new Note {
                Content = "routine maintenance",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Machine = machines[4],
                IsSolution = false
            },
            new Note {
                Content = "routine maintenance",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Machine = machines[5],
                IsSolution = false
            },
            new Note {
                Content = "routine maintenance",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Machine = machines[6],
                IsSolution = false
            }
        };
        context.Notes.AddRange(notes);

        context.SaveChanges();
    }
}