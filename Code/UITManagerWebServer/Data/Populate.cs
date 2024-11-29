using Microsoft.AspNetCore.Identity;
using UITManagerWebServer.Models;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;

public static class Populate {
    
    public static async Task Initialize(IServiceProvider serviceProvider) {
    
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
         var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
         var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
         await SeedUsersAsync(userManager,roleManager,context);
         
         DeleteDb(context);
         
         await SeedDatabase(userManager,context);
    }

    private static void DeleteDb(ApplicationDbContext context) {
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
    }
    
    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,ApplicationDbContext context) {
        
        var roles = new List<string> { "MaintenanceManager", "Technician", "ITDirector" };
        foreach (var role in roles) {
            if (!await roleManager.RoleExistsAsync(role)) {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded) {
                    Console.WriteLine($"Error creating role {role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
        
        if (context.Users.Any()) 
        {
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();
        }

        var users = new List<ApplicationUser> {
            new ApplicationUser {
                UserName = "roger",
                Email = "roger@example.com",
                FirstName = "Roger",
                LastName = "Ô",
                StartDate = DateTime.SpecifyKind(new DateTime(2013, 1, 1), DateTimeKind.Utc)
            },
            new ApplicationUser {
                UserName = "pierre",
                Email = "pierre@example.com",
                FirstName = "Pierre",
                LastName = "BARBE",
                StartDate = DateTime.SpecifyKind(new DateTime(2008, 1, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2012, 12, 31), DateTimeKind.Utc) // Ajout de la date de fin
            },
            new ApplicationUser {
                UserName = "camille",
                Email = "camille@example.com",
                FirstName = "Camille",
                LastName = "MILLET",
                StartDate = DateTime.SpecifyKind(new DateTime(1998, 8, 8), DateTimeKind.Utc)
            },
            new ApplicationUser {
                UserName = "bernadette",
                Email = "bernadette@example.com",
                FirstName = "Bernadette",
                LastName = "HARDY",
                StartDate = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2023, 3, 15), DateTimeKind.Utc) // Date de fin ajoutée
            },
            new ApplicationUser {
                UserName = "isaac",
                Email = "isaac@example.com",
                FirstName = "Isaac",
                LastName = "DEVAUX",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc)
            },
            new ApplicationUser {
                UserName = "aime_boulay_1",
                Email = "aime_boulay_1@example.com",
                FirstName = "Aimé",
                LastName = "BOULAY",
                StartDate = DateTime.SpecifyKind(new DateTime(2022, 7, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2022, 8, 30), DateTimeKind.Utc) // Date de fin ajoutée
            },
            new ApplicationUser {
                UserName = "paul_de_bergerac_1",
                Email = "paul_de_bergerac_1@example.com",
                FirstName = "Paul",
                LastName = "DE BERGERAC",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 3, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2023, 7, 1), DateTimeKind.Utc) // Date de fin ajoutée
            },
            new ApplicationUser {
                UserName = "aime_boulay_2",
                Email = "aime_boulay_2@example.com",
                FirstName = "Aimé",
                LastName = "BOULAY",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 7, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2023, 8, 30), DateTimeKind.Utc) // Date de fin ajoutée
            },
            new ApplicationUser {
                UserName = "alfred_emmanuel",
                Email = "alfred_emmanuel@example.com",
                FirstName = "Alfred-Emmanuel",
                LastName = "SEGUIN",
                StartDate = DateTime.SpecifyKind(new DateTime(2000, 3, 15), DateTimeKind.Utc)
            },
            new ApplicationUser {
                UserName = "martin_etienne",
                Email = "martin_etienne@example.com",
                FirstName = "Martin-Étienne",
                LastName = "LEFORT",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc)
            },
            new ApplicationUser {
                UserName = "paul_guilbert",
                Email = "paul_guilbert@example.com",
                FirstName = "Paul",
                LastName = "GUILBERT",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 7, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2023, 8, 30), DateTimeKind.Utc) // Date de fin ajoutée
            }
        };
        
        foreach (var user in users) {
            try {
                var existingUser = await userManager.FindByEmailAsync(user.Email);
                if (existingUser == null) {
                    var result = await userManager.CreateAsync(user, "StrongerPassword!1");
                    if (result.Succeeded) {
                        if (result.Succeeded) {

                            if (user.LastName == "Ô" || user.LastName == "BARBE")
                            {
                                await userManager.AddToRoleAsync(user, "ITDirector");
                            }
                            else if (user.LastName == "MILLET" || user.LastName == "SEGUIN")
                            {
                                await userManager.AddToRoleAsync(user, "MaintenanceManager");
                            }
                            else
                            {
                                await userManager.AddToRoleAsync(user, "Technician");
                            }
                        }
                        Console.WriteLine($"User {user.UserName} created successfully.");
                    }
                    else {
                        Console.WriteLine(
                            $"Error creating user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else {
                    Console.WriteLine($"User {user.UserName} already exists.");
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Exception while processing user {user.UserName}: {ex.Message}");
            }

        }
        Console.WriteLine(
            $"Database populated");
    }

    private static async Task SeedDatabase(UserManager<ApplicationUser> userManager, ApplicationDbContext context) {
        
        var random = new Random();

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
        
        var rolesToFind = new List<string> { "MaintenanceManager", "ITDirector" };
        
        var usersInRoles = new List<ApplicationUser>();
        
        foreach (var role in rolesToFind) {
            var usersInRole = await userManager.GetUsersInRoleAsync(role);
            usersInRoles.AddRange(usersInRole);
        }
        
        usersInRoles = usersInRoles.Distinct().ToList();
        
        var severityHistories = new List<SeverityHistory>() {
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-150),
                NormGroup = normGroups[0],
                Severity = severities[0],
                UserId = usersInRoles[random.Next(0,usersInRoles.Count-1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-50),
                NormGroup = normGroups[0],
                Severity = severities[2],
                UserId = usersInRoles[random.Next(0,usersInRoles.Count-1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow,
                NormGroup = normGroups[0],
                Severity = severities[4],
                UserId = usersInRoles[random.Next(0,usersInRoles.Count-1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-19),
                NormGroup = normGroups[1],
                Severity = severities[1],
                UserId = usersInRoles[random.Next(0,usersInRoles.Count-1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-5),
                NormGroup = normGroups[1],
                Severity = severities[3],
                UserId = usersInRoles[random.Next(0,usersInRoles.Count-1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-46),
                NormGroup = normGroups[2],
                Severity = severities[4],
                UserId = usersInRoles[random.Next(0,usersInRoles.Count-1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-146),
                NormGroup = normGroups[2],
                Severity = severities[2],
                UserId = usersInRoles[random.Next(0,usersInRoles.Count-1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-200),
                NormGroup = normGroups[3],
                Severity = severities[1],
                UserId = usersInRoles[random.Next(0,usersInRoles.Count-1)].Id
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

        var machines = new List<Machine>();

        for (int i = 1; i <= 50; i++) {
            var brandAndModel = brandsAndModels[random.Next(brandsAndModels.Length)];
            var brand = brandAndModel.Brand;
            var model = brandAndModel.Models[random.Next(brandAndModel.Models.Length)];

            var machineId = GenerateRandomWindowsMachineName();

            var machineName = $"{machineId}";
            var Model = $"{brand} {model}"; 
            machines.Add(new Machine { Name = machineName , Model = Model });
        }

        context.Machines.AddRange(machines);
        context.SaveChanges();

        string GenerateRandomWindowsMachineName() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string charsForSite = "ABC";

            var randomId = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());
            var site ="Site-" + new string(Enumerable.Repeat(charsForSite, 1).Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{site}-DESKTOP-{randomId}";
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
                        NormGroup = normGroups[random.Next(normGroups.Count)],
                        UserId = usersInRoles[random.Next(0,usersInRoles.Count-1)].Id
                    };
                    
                    int historyCount = random.Next(1, 5);
                    
                    for (i = 0; i < historyCount; i++) {
                        alarmStatusHistories.Add( 
                            new AlarmStatusHistory {
                            Alarm = alarm, 
                            StatusType = alarmStatusTypes[i],
                            ModificationDate = DateTime.UtcNow.AddHours(-10 + i ),
                            UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                        });
                        
                    }

                    
                    alarms.Add(alarm);
                }
            }
        }
        
        var alarme = new Alarm {
            TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(1, 72)),
            Machine = machines[0],
            NormGroup = normGroups[random.Next(normGroups.Count)],
        };
        alarms.Add(alarme);

        context.AlarmHistories.AddRange(alarmStatusHistories);
        context.Alarms.AddRange(alarms);
        context.SaveChanges();

        var notes = new List<Note>();

        var solutionTitles = new[] {
            "Driver Update", "System Vulnerability Patch", "OS Upgrade"
        };

        var solutionContents = new[] {
            "Resolved issue with outdated drivers. ![Driver Image](image1.jpg)",
            "Patched system vulnerabilities successfully. ![Vulnerability Image](image2.jpg)",
            "Updated operating system to the latest version. ![OS Upgrade Image](image3.jpg)"
        };

        var nonSolutionTitles = new[] {
            "Investigating CPU Usage", "Storage Monitoring"
        };

        var nonSolutionContents = new[] {
            "Investigating high CPU usage. ![CPU](image4.jpg)",
            "Monitoring storage capacity after warning. ![Storage](image5.jpg)"
        };

        var machinesWithNotes = machines.OrderBy(_ => random.Next()).Take(5).ToList(); 

        for (int i = 0; i < 3; i++) {
            notes.Add(new Note {
                Title = solutionTitles[i],  
                Content = solutionContents[i],  
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48)),
                Machine = machinesWithNotes[i],
                IsSolution = true,
                AuthorId = usersInRoles[random.Next(0, usersInRoles.Count)].Id
            });
        }

        for (int i = 0; i < 2; i++) {
            notes.Add(new Note {
                Title = nonSolutionTitles[i],
                Content = nonSolutionContents[i],
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48)),
                Machine = machinesWithNotes[i + 3],
                IsSolution = false,
                AuthorId = usersInRoles[random.Next(0, usersInRoles.Count)].Id
            });
        }

        context.Notes.AddRange(notes);
        context.SaveChanges();

        
        Console.WriteLine(
            $"Database populated with {machines.Count} machines, {alarms.Count} alarms, and {notes.Count} notes.");
    }
}