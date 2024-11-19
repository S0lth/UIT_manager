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
                Name = "Obsolete operating system",
                Priority = 8,
                Severity = SeverityLevel.Critical,
                Norms = new List<Norm> { new Norm { Name = "Windows 10 detected" } }
            },
            new NormGroup {
                Name = "Storage exceeded",
                Priority = 4,
                Severity = SeverityLevel.High,
                Norms = new List<Norm> { new Norm { Name = "Storage over 80%" } }
            },
            new NormGroup {
                Name = "CPU Usage High",
                Priority = 2,
                Severity = SeverityLevel.Medium,
                Norms = new List<Norm> { new Norm { Name = "CPU usage > 90%" } }
            },
            new NormGroup {
                Name = "Memory Usage Warning",
                Priority = 1,
                Severity = SeverityLevel.Low,
                Norms = new List<Norm> { new Norm { Name = "Memory usage > 70%" } }
            }
        };
        context.NormGroups.AddRange(normGroups);

        var brandsAndModels = new[] {
            new { Brand = "Dell", Models = new[] { "Latitude", "OptiPlex", "Precision", "Inspiron" } },
            new { Brand = "HP", Models = new[] { "EliteBook", "ProBook", "ZBook", "Pavilion" } },
            new { Brand = "Lenovo", Models = new[] { "ThinkPad", "IdeaPad", "Legion", "Yoga" } },
            new { Brand = "ASUS", Models = new[] { "ROG", "VivoBook", "ZenBook", "TUF Gaming" } },
            new { Brand = "Acer", Models = new[] { "Aspire", "Predator", "Nitro", "Spin" } },
            new { Brand = "MSI", Models = new[] { "Modern", "Prestige", "Stealth", "Katana" } }
        };

        var random = new Random();
        var machines = new List<Machine>();

        for (int i = 1; i <= 50; i++) {
            var brandAndModel = brandsAndModels[random.Next(brandsAndModels.Length)];
            var brand = brandAndModel.Brand;
            var model = brandAndModel.Models[random.Next(brandAndModel.Models.Length)];

            var machineId = GenerateRandomWindowsMachineName();

            var machineName = $"{brand} {model} ({machineId})";

            machines.Add(new Machine { Name = machineName });
        }

        context.Machines.AddRange(machines);

        string GenerateRandomWindowsMachineName() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomId = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());
            return $"DESKTOP-{randomId}";
        }

        var alarms = new List<Alarm>();

        foreach (var machine in machines) {
            if (random.NextDouble() > 0.5) {
                int alarmCount = random.Next(1, 4);
                for (int i = 0; i < alarmCount; i++) {
                    alarms.Add(new Alarm {
                        Status = (AlarmStatus)random.Next(0, Enum.GetValues<AlarmStatus>().Length),
                        TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(1, 72)),
                        Machine = machine,
                        NormGroup = normGroups[random.Next(normGroups.Count)]
                    });
                }
            }
        }

        context.Alarms.AddRange(alarms);

        var notes = new List<Note>();

        var solutionContents = new[]
        {
            "Resolved issue with outdated drivers.",
            "Patched system vulnerabilities successfully.",
            "Updated operating system to the latest version."
        };

        var nonSolutionContents = new[]
        {
            "Investigating high CPU usage.",
            "Monitoring storage capacity after warning."
        };

        var machinesWithNotes = machines.OrderBy(_ => random.Next()).Take(5).ToList(); // Sélection de 5 machines aléatoires

        for (int i = 0; i < 3; i++)
        {
            notes.Add(new Note
            {
                Content = solutionContents[i],
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48)),
                Machine = machinesWithNotes[i],
                IsSolution = true
            });
        }

        for (int i = 0; i < 2; i++)
        {
            notes.Add(new Note
            {
                Content = nonSolutionContents[i],
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48)),
                Machine = machinesWithNotes[i + 3],
                IsSolution = false
            });
        }

        context.Notes.AddRange(notes);

        context.Notes.AddRange(notes);

        context.SaveChanges();
        Console.WriteLine(
            $"Database populated with {machines.Count} machines, {alarms.Count} alarms, and {notes.Count} notes.");
    }
}