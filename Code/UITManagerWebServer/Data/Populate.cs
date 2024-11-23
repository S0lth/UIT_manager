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
            context.AlarmStatuses.RemoveRange(context.AlarmStatuses);
            context.AlarmStatusTypes.RemoveRange(context.AlarmStatusTypes);
            context.Employees.RemoveRange(context.Employees);

            context.SaveChanges();
            Console.WriteLine("Database cleared successfully.");
        }

        var alarmStatusTypes = new List<AlarmStatusType> {
            new AlarmStatusType {
                Name = "New",
                Description = "An alarm that has been recently created and has not yet been acknowledged or acted upon."
            },
            new AlarmStatusType {
                Name = "In Progress",
                Description = "The issue that triggered the alarm is actively being investigated or resolved."
            },
            new AlarmStatusType {
                Name = "Resolved",
                Description =
                    "The issue that caused the alarm has been fully addressed, and no further action is required."
            },
            new AlarmStatusType {
                Name = "Reopened",
                Description =
                    "An issue that previously triggered an alarm has recurred, causing the alarm to be raised again after being marked as resolved."
            },
            new AlarmStatusType {
                Name = "Awaiting Third-Party Assistance",
                Description =
                    "The resolution of the issue causing the alarm depends on action or support from an external party or vendor, and progress is pending their input."
            }
        };
        
        context.AlarmStatusTypes.AddRange(alarmStatusTypes);
        context.SaveChanges();

        var employees = new List<Employee> {
            new Employee { FirstName = "Roger", LastName = "Ô", Role = "D.S.I." },
            new Employee { FirstName = "Pierre", LastName = "BARBE", Role = "D.S.I." },
            new Employee { FirstName = "Camille", LastName = "MILLET", Role = "Responsable Maintenance Site A" },
            new Employee { FirstName = "Bernadette", LastName = "HARDY", Role = "Technicienne Site A" },
            new Employee { FirstName = "Isaac", LastName = "DEVAUX", Role = "Technicien Site A" },
            new Employee { FirstName = "Aimé", LastName = "BOULAY", Role = "Technicien Site A" },
            new Employee { FirstName = "Paul", LastName = "DE BERGERAC", Role = "Technicien Site A" },
            new Employee {
                FirstName = "Alfred-Emmanuel", LastName = "SEGUIN", Role = "Responsable Maintenance Site B"
            },
            new Employee { FirstName = "Martin-Étienne", LastName = "LEFORT", Role = "Technicien Site B" },
            new Employee { FirstName = "Paul", LastName = "GUILBERT", Role = "Technicien Site B" }
        };
        
        context.Employees.AddRange(employees);
        context.SaveChanges();
        
        var alarmStatuses = new List<AlarmStatus> {
            new AlarmStatus {
                ModificationDate = DateTime.UtcNow.AddHours(-2),
                StatusType = alarmStatusTypes[2],
                Modifier = employees[0],
            },
            new AlarmStatus {
                ModificationDate = DateTime.UtcNow.AddHours(-5),
                StatusType = alarmStatusTypes[1],
                Modifier = employees[1],
            },
            new AlarmStatus {
                ModificationDate = DateTime.UtcNow.AddHours(-8),
                StatusType = alarmStatusTypes[4],
                Modifier = employees[2],
            },
            new AlarmStatus { ModificationDate = DateTime.UtcNow, StatusType = alarmStatusTypes[0], },
            new AlarmStatus { ModificationDate = DateTime.UtcNow, StatusType = alarmStatusTypes[0], },
            new AlarmStatus { ModificationDate = DateTime.UtcNow, StatusType = alarmStatusTypes[0], }
        };

        context.AlarmStatuses.AddRange(alarmStatuses);
        context.SaveChanges();

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
        context.SaveChanges();

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
        context.SaveChanges();

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
                        AlarmStatus = alarmStatuses[i],
                        TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(1, 72)),
                        Machine = machine,
                        NormGroup = normGroups[random.Next(normGroups.Count)]
                    });
                }
            }
        }

        context.Alarms.AddRange(alarms);
        context.SaveChanges();

        var notes = new List<Note>();

        var solutionContents = new[] {
            "Resolved issue with outdated drivers.", "Patched system vulnerabilities successfully.",
            "Updated operating system to the latest version."
        };

        var nonSolutionContents =
            new[] { "Investigating high CPU usage.", "Monitoring storage capacity after warning." };

        var machinesWithNotes =
            machines.OrderBy(_ => random.Next()).Take(5).ToList(); // Sélection de 5 machines aléatoires

        for (int i = 0; i < 3; i++) {
            notes.Add(new Note {
                Content = solutionContents[i],
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48)),
                Machine = machinesWithNotes[i],
                IsSolution = true
            });
        }

        for (int i = 0; i < 2; i++) {
            notes.Add(new Note {
                Content = nonSolutionContents[i],
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48)),
                Machine = machinesWithNotes[i + 3],
                IsSolution = false
            });
        }

        context.Notes.AddRange(notes);
        context.SaveChanges();
        
        Console.WriteLine(
            $"Database populated with {machines.Count} machines, {alarms.Count} alarms, and {notes.Count} notes.");
    }
}