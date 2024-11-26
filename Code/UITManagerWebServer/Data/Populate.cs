using Microsoft.AspNetCore.Identity;
using UITManagerWebServer.Models;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;

public static class Populate {
    public static async void Initialize(IServiceProvider serviceProvider) {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        if (context.Machines.Any() || context.NormGroups.Any()) {
            context.Alarms.RemoveRange(context.Alarms);
            context.Notes.RemoveRange(context.Notes);
            context.Norms.RemoveRange(context.Norms);
            context.NormGroups.RemoveRange(context.NormGroups);
            context.Machines.RemoveRange(context.Machines);
            context.AlarmHistories.RemoveRange(context.AlarmHistories);
            context.AlarmStatusTypes.RemoveRange(context.AlarmStatusTypes);
            context.Severities.RemoveRange(context.Severities);
            context.SeverityHistories.RemoveRange(context.SeverityHistories);

            context.SaveChanges();
            Console.WriteLine("Database cleared successful");
        }
        
        var severities = new List<Severity>() {
            new Severity { Name = "Warning", Description = "Warning Severity" },
            new Severity { Name = "Low", Description = "Low Severity" },
            new Severity { Name = "Medium", Description = "Medium Severity" },
            new Severity { Name = "High", Description = "High Severity" },
            new Severity { Name = "Critical", Description = "Critical Severity" }
        };

        var normGroups = new List<NormGroup> {
            new NormGroup {
                Name = "Obsolete operating system",
                Priority = 8,
                Norms = new List<Norm> { new Norm { Name = "Windows 10 detected" } }
            },
            new NormGroup {
                Name = "Storage exceeded",
                Priority = 4,
                Norms = new List<Norm> { new Norm { Name = "Storage over 80%" } }
            },
            new NormGroup {
                Name = "CPU Usage High",
                Priority = 2,
                Norms = new List<Norm> { new Norm { Name = "CPU usage > 90%" } }
            },
            new NormGroup {
                Name = "Memory Usage Warning",
                Priority = 1,
                Norms = new List<Norm> { new Norm { Name = "Memory usage > 70%" } }
            }
        };
        
        var severityHistories = new List<SeverityHistory>() {
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow,
                NormGroup = normGroups[0],
                Severity = severities[4],
                Username = "O Roger"
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-1),
                NormGroup = normGroups[1],
                Severity = severities[3],
                Username = "O Roger"
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-8),
                NormGroup = normGroups[2],
                Severity = severities[2],
                Username = "O Roger"
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-10),
                NormGroup = normGroups[3],
                Severity = severities[1],
                Username = "O Roger"
            }
        };
        
        context.Severities.AddRange(severities);

        context.NormGroups.AddRange(normGroups);
        
        context.SeverityHistories.AddRange(severityHistories);
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
        
        var alarms = new List<Alarm>();                    
        var alarmStatusHistories = new List<AlarmStatusHistory>();

        foreach (var machine in machines) {
            if (random.NextDouble() > 0.5) {
                int alarmCount = random.Next(1, 4);
                for (int i = 0; i < alarmCount; i++) {
                    var alarm = new Alarm {
                        TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(1, 72)),
                        Machine = machine,
                        NormGroup = normGroups[random.Next(normGroups.Count)]
                    };
                    
                    int historyCount = random.Next(1, 5);
                    
                    for (i = 0; i < historyCount; i++) {
                        alarmStatusHistories.Add( 
                            new AlarmStatusHistory {
                            Alarm = alarm, 
                            StatusType = alarmStatusTypes[i],
                            ModificationDate = DateTime.UtcNow.AddHours(-10 + i ),
                            Username =  "O Roger"
                        });
                    }
                    
                    alarms.Add(alarm);
                }
            }
        }
        
        context.AlarmHistories.AddRange(alarmStatusHistories);
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