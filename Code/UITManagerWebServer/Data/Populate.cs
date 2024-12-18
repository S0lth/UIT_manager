using Microsoft.AspNetCore.Identity;
using UITManagerWebServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Information = UITManagerWebServer.Models.Information;

namespace UITManagerWebServer.Data;

public static class Populate {
    public static async Task Initialize(IServiceProvider serviceProvider, bool noAlarm) {
        
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        DeleteDb(context);

        await SeedUsersAsync(userManager, roleManager, context);

        var normGroups = await SeedNormGroup(userManager, context);

        var machines = await SeedMachines(context);

        await SeedAlarms(context, machines, normGroups, userManager, noAlarm);

        SeedNotes(context, machines);

        Console.WriteLine($"Database populated");
    }

    private static void DeleteDb(ApplicationDbContext context) {
        if (context.Machines.Any() || context.NormGroups.Any()) {
            context.Alarms.RemoveRange(context.Alarms);
            context.Files.RemoveRange(context.Files);
            context.Notes.RemoveRange(context.Notes);
            context.Norms.RemoveRange(context.Norms);
            context.NormGroups.RemoveRange(context.NormGroups);
            context.Machines.RemoveRange(context.Machines);
            context.Components.RemoveRange(context.Components);
            context.AlarmHistories.RemoveRange(context.AlarmHistories);
            context.AlarmStatusTypes.RemoveRange(context.AlarmStatusTypes);
            context.Severities.RemoveRange(context.Severities);
            context.SeverityHistories.RemoveRange(context.SeverityHistories);

            context.SaveChanges();
            Console.WriteLine("Database cleared successful");
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, ApplicationDbContext context) {
        var roles = new List<string> { "MaintenanceManager", "Technician", "ITDirector" };
        foreach (var role in roles) {
            if (!await roleManager.RoleExistsAsync(role)) {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded) {
                    Console.WriteLine(
                        $"Error creating role {role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        if (context.Users.Any()) {
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();
        }

        var users = new List<ApplicationUser> {
            new ApplicationUser {
                UserName = "oroger",
                Email = "o.roger@uit.be",
                FirstName = "Roger",
                LastName = "Ô",
                StartDate = DateTime.SpecifyKind(new DateTime(2013, 1, 1), DateTimeKind.Utc),
                IsActivate = true
            },
            new ApplicationUser {
                UserName = "barbepierre",
                Email = "barbe.pierre@uit.be",
                FirstName = "Pierre",
                LastName = "BARBE",
                StartDate = DateTime.SpecifyKind(new DateTime(2008, 1, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2012, 12, 31), DateTimeKind.Utc),
                IsActivate = false
            },
            new ApplicationUser {
                UserName = "milletcamille",
                Email = "millet.camille@uit.be",
                FirstName = "Camille",
                LastName = "MILLET",
                StartDate = DateTime.SpecifyKind(new DateTime(1998, 8, 8), DateTimeKind.Utc),
                IsActivate = true
            },
            new ApplicationUser {
                UserName = "hardybernadette",
                Email = "hardy.bernadette@uit.be",
                FirstName = "Bernadette",
                LastName = "HARDY",
                StartDate = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2023, 3, 15), DateTimeKind.Utc),
                IsActivate = false
            },
            new ApplicationUser {
                UserName = "devauxisaac",
                Email = "devaux.isaac@uit.be",
                FirstName = "Isaac",
                LastName = "DEVAUX",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
                IsActivate = true
            },
            new ApplicationUser {
                UserName = "boulayaime",
                Email = "boulay.aime@uit.be",
                FirstName = "Aimé",
                LastName = "BOULAY",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 7, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2023, 8, 30), DateTimeKind.Utc),
                IsActivate = false
            },
            new ApplicationUser {
                UserName = "debergeracpaul",
                Email = "debergerac.paul@uit.com",
                FirstName = "Paul",
                LastName = "DE BERGERAC",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 3, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2023, 7, 1), DateTimeKind.Utc),
                IsActivate = false
            },
            new ApplicationUser {
                UserName = "seguinalfredemmanuel",
                Email = "seguin.alfredemmanuel@uit.be",
                FirstName = "Alfred-Emmanuel",
                LastName = "SEGUIN",
                StartDate = DateTime.SpecifyKind(new DateTime(2000, 3, 15), DateTimeKind.Utc),
                IsActivate = true
            },
            new ApplicationUser {
                UserName = "lefortmartinetienne",
                Email = "lefort.martinetienne_@uit.be",
                FirstName = "Martin-Étienne",
                LastName = "LEFORT",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
                IsActivate = true
            },
            new ApplicationUser {
                UserName = "guilbertpaul",
                Email = "guilbert.paul@uit.be",
                FirstName = "Paul",
                LastName = "GUILBERT",
                StartDate = DateTime.SpecifyKind(new DateTime(2023, 7, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2023, 8, 30), DateTimeKind.Utc),
                IsActivate = false
            }
        };

        foreach (var user in users) {
            try {
                if (user.Email != null) {
                    var existingUser = await userManager.FindByEmailAsync(user.Email);
                    if (existingUser == null) {
                        var result = await userManager.CreateAsync(user, "StrongerPassword!1");
                        if (result.Succeeded) {
                            if (result.Succeeded) {
                                if (user.LastName == "Ô" || user.LastName == "BARBE") {
                                    await userManager.AddToRoleAsync(user, "ITDirector");
                                }
                                else if (user.LastName == "MILLET" || user.LastName == "SEGUIN") {
                                    await userManager.AddToRoleAsync(user, "MaintenanceManager");
                                }
                                else {
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
            }
            catch (Exception ex) {
                Console.WriteLine($"Exception while processing user {user.UserName}: {ex.Message}");
            }
        }

        Console.WriteLine(
            $"Database populated");
    }

    private static async Task<List<NormGroup>> SeedNormGroup(UserManager<ApplicationUser> userManager,
        ApplicationDbContext context) {
        var random = new Random();

        var severities = new List<Severity>() {
            new Severity { Name = "Critical", Description = "Critical Severity" },
            new Severity { Name = "High", Description = "High Severity" },
            new Severity { Name = "Medium", Description = "Medium Severity" },
            new Severity { Name = "Low", Description = "Low Severity" },
            new Severity { Name = "Warning", Description = "Warning Severity" }
        };

        var directXName = new InformationName { Name = "Direct X" };
        var domainNameName = new InformationName { Name = "Domain Name" };
        var tagServiceName = new InformationName { Name = "Tag Service" };
        var uptimeName = new InformationName { Name = "Uptime" };
        var cpuName = new InformationName {
            Name = "CPU",
            SubInformationNames = new List<InformationName> {
                new InformationName { Name = "Logical Core" },
                new InformationName { Name = "Core Count" },
                new InformationName { Name = "Clock Speed" },
                new InformationName { Name = "Model" },
                new InformationName { Name = "Used" },
            }
        };

        var ramName = new InformationName {
            Name = "Ram",
            SubInformationNames = new List<InformationName> {
                new InformationName { Name = "Total RAM" },
                new InformationName { Name = "Used RAM" },
                new InformationName { Name = "Free RAM" },
            }
        };

        var osName = new InformationName {
            Name = "OS",
            SubInformationNames = new List<InformationName> {
                new InformationName { Name = "OS Name" },
                new InformationName { Name = "OS Version" },
                new InformationName { Name = "OS Build" },
            }
        };

        var ipName = new InformationName { Name = "IP Address" };
        var disksName = new InformationName {
            Name = "Disks",
            SubInformationNames = new List<InformationName> {
                new InformationName { Name = "Disk Free Size" },
                new InformationName { Name = "Disk Total Size" },
                new InformationName { Name = "Disk Used" },
                new InformationName { Name = "List Name" },
                new InformationName { Name = "Number" }
            }
        };
        var userName = new InformationName {
            Name = "Users",
            SubInformationNames = new List<InformationName> {
                new InformationName { Name = "Name" },
                new InformationName { Name = "Scope" },
                new InformationName { Name = "List" },
                new InformationName { Name = "Used Memory" },
            }
        };

        context.InformationNames.AddRange(
            directXName, domainNameName, tagServiceName, uptimeName,
            cpuName, ramName, osName, disksName, ipName, userName
        );
        context.SaveChanges();


        var normGroups = new List<NormGroup> {
            new NormGroup {
                Name = "Storage greater than 99%",
                Priority = 9,
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = true,
                Norms =
                    new List<Norm> {
                        new Norm {
                            Name = "Storage full",
                            InformationName = disksName.SubInformationNames[2],
                            Condition = ">",
                            Format = "%",
                            Value = "99"
                        }
                    }
            },
            new NormGroup {
                Name = "Obsolete operating system (to Windows 11)",
                Priority = 8,
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = true,
                Norms =
                    new List<Norm> {
                        new Norm {
                            Name = "Windows 10 detected",
                            InformationName = osName.SubInformationNames[0],
                            Condition = "IN",
                            Format = "TEXT",
                            Value = "10"
                        }
                    }
            },
            new NormGroup {
                Name = "Ram usage greater than 80% of utilisation",
                Priority = 6,
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = true,
                Norms =
                    new List<Norm> {
                        new Norm {
                            Name = "Ram usage > 80%",
                            InformationName = ramName.SubInformationNames[1],
                            Condition = ">",
                            Format = "%",
                            Value = "80"
                        }
                    }
            },
            new NormGroup {
                Name = "Storage usage greater than 80% of utilisation ",
                Priority = 5,
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = true,
                Norms =
                    new List<Norm> {
                        new Norm {
                            Name = "Storage over 80%",
                            InformationName = disksName.SubInformationNames[2],
                            Condition = ">",
                            Format = "%",
                            Value = "80"
                        }
                    }
            },
            new NormGroup {
                Name = "DirectX not up to date",
                Priority = 3,
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = true,
                Norms =
                    new List<Norm> {
                        new Norm {
                            Name = "Old DirectX",
                            InformationName = directXName,
                            Condition = "NOT IN",
                            Format = "TEXT",
                            Value = "DirectX 12"
                        }
                    }
            },
            new NormGroup {
                Name = "Not enough Ram (less than 8 GB)",
                Priority = 1,
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = true,
                Norms = new List<Norm> {
                    new Norm {
                        Name = "Ram < 8GB",
                        InformationName = ramName.SubInformationNames[0],
                        Condition = "<",
                        Format = "GB",
                        Value = "8"
                    }
                }
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
            // Storage exceeded
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-350),
                NormGroup = normGroups[0],
                Severity = severities[2],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-150),
                NormGroup = normGroups[0],
                Severity = severities[1],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-14),
                NormGroup = normGroups[0],
                Severity = severities[0],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            // Obsolete operating system
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-168),
                NormGroup = normGroups[1],
                Severity = severities[3],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-64),
                NormGroup = normGroups[1],
                Severity = severities[1],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-16),
                NormGroup = normGroups[1],
                Severity = severities[0],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            // Ram 80% Used
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-246),
                NormGroup = normGroups[2],
                Severity = severities[4],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-97),
                NormGroup = normGroups[2],
                Severity = severities[3],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-146),
                NormGroup = normGroups[2],
                Severity = severities[2],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            //Storage 80% Used
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-187),
                NormGroup = normGroups[3],
                Severity = severities[1],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-39),
                NormGroup = normGroups[3],
                Severity = severities[2],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            //DirectX Version
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-200),
                NormGroup = normGroups[4],
                Severity = severities[4],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-23),
                NormGroup = normGroups[4],
                Severity = severities[3],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            //Ram < 8GB
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddDays(-59),
                NormGroup = normGroups[5],
                Severity = severities[4],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            }
        };

        context.Severities.AddRange(severities);

        context.NormGroups.AddRange(normGroups);

        context.SeverityHistories.AddRange(severityHistories);
        context.SaveChanges();

        return normGroups;
    }

    private static async Task<List<Machine>> SeedMachines(ApplicationDbContext context) {
        var random = new Random();

        // Model
        var brandsAndModels = new[] {
            new { Brand = "Dell", Models = new[] { "Latitude", "OptiPlex", "Precision", "Inspiron" } },
            new { Brand = "HP", Models = new[] { "EliteBook", "ProBook", "ZBook", "Pavilion" } },
            new { Brand = "Lenovo", Models = new[] { "ThinkPad", "IdeaPad", "Legion", "Yoga" } },
            new { Brand = "ASUS", Models = new[] { "ROG", "VivoBook", "ZenBook", "TUF Gaming" } },
            new { Brand = "Acer", Models = new[] { "Aspire", "Predator", "Nitro", "Spin" } },
            new { Brand = "MSI", Models = new[] { "Modern", "Prestige", "Stealth", "Katana" } }
        };

        // DirectX
        var directX = new[] { "DirectX 12", "DirectX 11", "DirectX 10" };

        // OS
        var Os = new[] { "Microsoft Windows 10 Enterprise", "Microsoft Windows 11 Enterprise" };
        var OsV = new[] { "21H2", "22H2", "23H2", "24H2", };
        var OsB = new[] { "19044", "19045", "22631", "26100" };

        // CPU
        var modeltype = new[] {
            "AMD Ryzen 7 7700X", "Intel Core i9-13900K", "Intel Core i7-13700K", "Intel Xeon W-3175X"
        };
        var locgical = new[] { "16", "32", "64", "126" };
        var coreCount = new[] { "8", "16", "32", "64" };
        var clock = new[] { "3601", "5204", "2500", "3100", "4600" };

        // Ram
        var ramTotals = new[] { 4, 8, 16, 32, 64 };

        // Ip
        var ipAddresses = new[] {
            "192.168.1.15", "10.0.0.87", "172.16.24.65", "203.0.113.42", "8.8.8.8", "172.18.99.22", "192.168.10.200",
            "10.1.1.1", "192.0.2.33", "198.51.100.5", "100.64.0.1", "172.19.45.88", "192.168.0.254", "10.255.255.254",
            "203.0.113.120", "8.8.4.4", "192.168.56.1", "10.100.200.5", "172.20.14.1", "192.168.2.25", "172.21.13.50",
            "10.10.10.10", "192.168.100.100", "198.51.100.1", "203.0.113.55", "10.0.5.5", "172.22.99.88", "192.168.3.3",
            "10.123.45.67", "172.23.7.77", "192.168.4.1", "203.0.113.150", "10.10.20.30", "172.24.56.78", "192.168.5.5",
            "198.51.100.99", "172.25.1.1", "192.168.50.2", "10.20.30.40", "172.26.33.44", "192.168.6.6", "10.30.40.50",
            "172.27.99.100", "192.168.7.7", "198.51.100.2", "203.0.113.3", "10.40.50.60", "172.28.88.77", "192.168.8.8",
            "10.50.60.70", "172.29.11.22", "192.168.9.9", "198.51.100.10", "203.0.113.99", "10.60.70.80",
            "172.30.99.123", "192.168.10.10", "10.70.80.90", "172.31.99.45", "192.168.11.11", "198.51.100.20",
            "203.0.113.56", "10.80.90.100", "172.32.14.88", "192.168.12.12", "10.90.100.110", "172.33.33.33",
            "192.168.13.13", "198.51.100.30", "203.0.113.12", "10.100.110.120", "172.34.56.78", "192.168.14.14",
            "10.110.120.130", "172.35.88.99", "192.168.15.15", "198.51.100.40", "203.0.113.22", "10.120.130.140",
            "172.36.10.11",
        };

        // Disk
        var diskNames = new[] {
            "C:System_Disk", "D:Data_Drive", "E:Backup_Disk", "F:Media_Storage", "G:Games_Drive", "H:VM_Storage",
            "I:Archive_1", "J:Personal_Files", "K:Shared_Drive", "L:Encrypted_Vault"
        };
        var diskTotals = new[] { 128, 256, 512, 1024, 2048 };

        // User
        var scoop = new[] { "local", "domain" };
        var name = new[] {
            "Alice Johnson", "Bob Smith", "Charlie Davis", "Emma Brown", "John Taylor", "Sophia Wilson",
            "Michael Green", "Olivia Martinez", "Ethan Miller", "Isabella Clark", "James Carter", "Ava Harris",
            "Liam Walker", "Mia Thompson", "Noah Lewis", "Charlotte Robinson", "Lucas Young", "Amelia Hall",
            "Elijah Wright", "Harper King"
        };

        var machines = new List<Machine>();
        int nbMachines = 10;
        for (int i = 1; i <= nbMachines; i++) {
            // Model
            var brandAndModel = brandsAndModels[random.Next(brandsAndModels.Length)];
            string brand = brandAndModel.Brand;
            string model = brandAndModel.Models[random.Next(brandAndModel.Models.Length)];
            DateTime seen = random.Next(1, 101) <= 30 ? DateTime.UtcNow.AddDays(-3) : DateTime.UtcNow;

            // Nom de Machine
            string machineId = await GenerateUniqueWindowsMachineName(context);

            string machineName = $"{machineId}";
            string machineModel = $"{brand} {model}";
            Machine machine = new Machine {
                Name = machineName, Model = machineModel, IsWorking = random.Next(1, 101) <= 90, LastSeen = seen,
            };

            // OS et DirectX
            int iOs;
            if (i < nbMachines * 0.76) {
                iOs = random.Next(0, 2);
                machine.Informations.Add(
                    new Component {
                        Name = "OS",
                        Machine = machine,
                        Value = "Null",
                        Format = "Null",
                        Children = new List<Information> {
                            new Value { Name = "OS Name", Machine = machine, Value = Os[0], Format = "TEXT" },
                            new Value { Name = "OS Version", Machine = machine, Value = OsV[iOs], Format = "TEXT" },
                            new Value { Name = "OS Build", Machine = machine, Value = OsB[iOs], Format = "TEXT" },
                        }
                    });

                machine.Informations.Add(
                    new Value {
                        Machine = machine,
                        Name = "Direct X",
                        Value = directX[random.Next(0, directX.Length)],
                        Format = "TEXT"
                    }
                );
            }
            else {
                iOs = random.Next(2, 4);
                machine.Informations.Add(
                    new Component {
                        Name = "OS",
                        Machine = machine,
                        Value = "Null",
                        Format = "Null",
                        Children = new List<Information> {
                            new Value { Name = "OS Name", Machine = machine, Value = Os[1], Format = "TEXT" },
                            new Value { Name = "OS Version", Machine = machine, Value = OsV[iOs], Format = "TEXT" },
                            new Value { Name = "OS Build", Machine = machine, Value = OsB[iOs], Format = "TEXT" },
                        }
                    });
                machine.Informations.Add(
                    new Value { Machine = machine, Name = "Direct X", Value = directX[0], Format = "TEXT" }
                );
            }

            // Domaine
            machine.Informations.Add(
                new Value { Machine = machine, Name = "Domain name", Value = "WORKGROUP", Format = "TEXT" });

            // Tag service
            machine.Informations.Add(
                new Value { Machine = machine, Name = "Tag Service", Value = "AB45CD78", Format = "TEXT" });

            // Uptime
            TimeSpan date;
            if (i > nbMachines * 0.15 && i < nbMachines * 0.21) {
                date = new TimeSpan(random.Next(1, 11), random.Next(0, 24), random.Next(0, 60), random.Next(0, 60));
            }
            else {
                date = new TimeSpan(random.Next(0, 24), random.Next(0, 60), random.Next(0, 60));
            }

            machine.Informations.Add(
                new Value { Machine = machine, Name = "UpTime", Value = date.ToString(), Format = "TEXT" });

            // Cpu
            int nbCore = random.Next(locgical.Length);
            machine.Informations.Add(
                new Component {
                    Name = "CPU",
                    Machine = machine,
                    Value = "Null",
                    Format = "Null",
                    Children = new List<Information> {
                        new Value {
                            Name = "Logical core", Machine = machine, Value = locgical[nbCore], Format = "TEXT"
                        },
                        new Value {
                            Name = "Core count", Machine = machine, Value = coreCount[nbCore], Format = "TEXT"
                        },
                        new Value {
                            Name = "Clockspeed",
                            Machine = machine,
                            Value = clock[random.Next(clock.Length)],
                            Format = "TEXT",
                        },
                        new Value {
                            Name = "Model",
                            Machine = machine,
                            Value = modeltype[random.Next(modeltype.Length)],
                            Format = "TEXT",
                        }
                    }
                });

            // Ram
            int ramTotal;
            if (i > nbMachines * 0.6 && i < nbMachines * 0.76) {
                ramTotal = ramTotals[0];
            }
            else {
                ramTotal = ramTotals[random.Next(1, ramTotals.Length)];
            }

            double ramUsed;
            if (i > nbMachines * 0.4 && i < nbMachines * 0.56) {
                ramUsed = GetRandomNumber(ramTotal * 0.8, ramTotal);
            }
            else {
                ramUsed = GetRandomNumber(0, ramTotal * 0.81);
            }

            double ramFree = ramTotal - ramUsed;

            machine.Informations.Add(
                new Component {
                    Name = "Ram",
                    Machine = machine,
                    Value = "Null",
                    Format = "Null",
                    Children = new List<Information> {
                        new Value { Name = "Total RAM", Machine = machine, Value = ramTotal.ToString(), Format = "GB" },
                        new Value { Name = "Used RAM", Machine = machine, Value = ramUsed.ToString(), Format = "GB" },
                        new Value { Name = "Free RAM", Machine = machine, Value = ramFree.ToString(), Format = "GB" }
                    }
                });

            // IP
            List<Information> ip = new List<Information>();
            for (int j = 0; j < random.Next(1, 3); j++) {
                var val = new Value {
                    Name = "Ip Address",
                    Machine = machine,
                    Value = ipAddresses[random.Next(ipAddresses.Length)],
                    Format = "TEXT"
                };
                ip.Add(val);
            }

            machine.Informations.Add(
                new Component {
                    Name = "IPs",
                    Machine = machine,
                    Value = "Null",
                    Format = "Null",
                    Children = ip,
                });

            // Disk
            List<Information> disks = new List<Information>();

            for (int j = 0; j < random.Next(1, 3); j++) {
                if (j == 0) {
                    if (i < nbMachines * 0.1) {
                        int diskTotal = diskTotals[random.Next(diskTotals.Length)];
                        double diskUsed = GetRandomNumber(diskTotal * 0.99, diskTotal);
                        double diskFree = diskTotal - diskUsed;

                        var val = new Component {
                            Name = diskNames[random.Next(diskNames.Length)],
                            Machine = machine,
                            Value = "Null",
                            Format = "Null",
                            Children = new List<Information> {
                                new Value {
                                    Name = "Disk Total Size",
                                    Value = diskTotal.ToString(),
                                    Format = "GB",
                                    Machine = machine
                                },
                                new Value {
                                    Name = "Disk Used", Value = diskUsed.ToString(), Format = "GB", Machine = machine
                                },
                                new Value {
                                    Name = "Disk Free Size",
                                    Value = diskFree.ToString(),
                                    Format = "GB",
                                    Machine = machine
                                }
                            }
                        };
                        disks.Add(val);
                    }
                    else if (i < nbMachines * 0.5) {
                        int diskTotal = diskTotals[random.Next(diskTotals.Length)];
                        double diskUsed = GetRandomNumber(diskTotal * 0.8, diskTotal * 0.95);
                        double diskFree = diskTotal - diskUsed;

                        var val = new Component {
                            Name = diskNames[random.Next(diskNames.Length)],
                            Machine = machine,
                            Value = "Null",
                            Format = "Null",
                            Children = new List<Information> {
                                new Value {
                                    Name = "Disk Total Size",
                                    Value = diskTotal.ToString(),
                                    Format = "GB",
                                    Machine = machine
                                },
                                new Value {
                                    Name = "Disk Used", Value = diskUsed.ToString(), Format = "GB", Machine = machine
                                },
                                new Value {
                                    Name = "Disk Free Size",
                                    Value = diskFree.ToString(),
                                    Format = "GB",
                                    Machine = machine
                                }
                            }
                        };
                        disks.Add(val);
                    }
                    else {
                        int diskTotal = diskTotals[random.Next(diskTotals.Length)];
                        double diskUsed = GetRandomNumber(0, diskTotal * 0.8);
                        double diskFree = diskTotal - diskUsed;

                        var val = new Component {
                            Name = diskNames[random.Next(diskNames.Length)],
                            Machine = machine,
                            Value = "Null",
                            Format = "Null",
                            Children = new List<Information> {
                                new Value {
                                    Name = "Disk Total Size",
                                    Value = diskTotal.ToString(),
                                    Format = "GB",
                                    Machine = machine
                                },
                                new Value {
                                    Name = "Disk Used", Value = diskUsed.ToString(), Format = "GB", Machine = machine
                                },
                                new Value {
                                    Name = "Disk Free Size",
                                    Value = diskFree.ToString(),
                                    Format = "GB",
                                    Machine = machine
                                }
                            }
                        };
                        disks.Add(val);
                    }
                }
                else {
                    int diskTotal = diskTotals[random.Next(diskTotals.Length)];
                    double diskUsed = GetRandomNumber(0, diskTotal * 0.8);
                    double diskFree = diskTotal - diskUsed;

                    var val = new Component {
                        Name = diskNames[random.Next(diskNames.Length)],
                        Machine = machine,
                        Value = "Null",
                        Format = "Null",
                        Children = new List<Information> {
                            new Value {
                                Name = "Disk Total Size", Value = diskTotal.ToString(), Format = "GB", Machine = machine
                            },
                            new Value {
                                Name = "Disk Used", Value = diskUsed.ToString(), Format = "GB", Machine = machine
                            },
                            new Value {
                                Name = "Disk Free Size", Value = diskFree.ToString(), Format = "GB", Machine = machine
                            }
                        }
                    };
                    disks.Add(val);
                }
            }

            machine.Informations.Add(
                new Component {
                    Name = "List Disk",
                    Machine = machine,
                    Value = "Null",
                    Format = "Null",
                    Children = new List<Information> {
                        new Component {
                            Name = "Disks",
                            Machine = machine,
                            Value = "Null",
                            Format = "Null",
                            Children = disks,
                        },
                        new Value {
                            Name = "Number disks", Machine = machine, Value = disks.Count.ToString(), Format = "TEXT"
                        },
                    }
                });

            // User
            machine.Informations.Add(
                new Component {
                    Name = "Users List",
                    Machine = machine,
                    Value = "Null",
                    Format = "Null",
                    Children = new List<Information> {
                        new Component {
                            Name = "User",
                            Value = "Null",
                            Format = "Null",
                            Machine = machine,
                            Children =
                                new List<Information> {
                                    new Value {
                                        Name = "User Name", Machine = machine, Value = "Admin", Format = "TEXT"
                                    },
                                    new Value {
                                        Name = "User Scope", Machine = machine, Value = "Local", Format = "TEXT"
                                    }
                                }
                        },
                        new Component {
                            Name = "User",
                            Value = "Null",
                            Format = "Null",
                            Machine = machine,
                            Children = new List<Information> {
                                new Value {
                                    Name = "User Name", Machine = machine, Value = "DefaultAccount", Format = "TEXT"
                                },
                                new Value { Name = "User Scope", Machine = machine, Value = "Local", Format = "TEXT" }
                            }
                        },
                        new Component {
                            Name = "User",
                            Machine = machine,
                            Value = "Null",
                            Format = "Null",
                            Children = new List<Information> {
                                new Value {
                                    Name = "User Name",
                                    Machine = machine,
                                    Value = name[random.Next(name.Length)],
                                    Format = "TEXT"
                                },
                                new Value {
                                    Name = "User Scope",
                                    Machine = machine,
                                    Value = scoop[random.Next(scoop.Length)],
                                    Format = "TEXT"
                                }
                            }
                        },
                    }
                });

            machines.Add(machine);
        }

        context.Machines.AddRange(machines);
        context.SaveChanges();

        return machines;
    }

    private static async Task SeedAlarms(ApplicationDbContext context, List<Machine> machines,
        List<NormGroup> normGroups, UserManager<ApplicationUser> userManager, bool noAlarm) {
        var random = new Random();

        var rolesToFind = new List<string> { "Technician" };

        var usersInRoles = new List<ApplicationUser>();

        foreach (var role in rolesToFind) {
            var usersInRole = await userManager.GetUsersInRoleAsync(role);
            usersInRoles.AddRange(usersInRole);
        }

        usersInRoles = usersInRoles.Distinct().ToList();

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

        var alarms = new List<Alarm>();
        var alarmStatusHistories = new List<AlarmStatusHistory>();

        foreach (var machine in machines) {
            // Get Ram
            Information infoRam = machine.Informations.Find(i => i.Name == "Ram");

            int ramTot = int.Parse(infoRam.Children.Find(i => i.Name == "Total RAM").Value);
            double ramUsed = Double.Parse(infoRam.Children.Find(i => i.Name == "Used RAM").Value);

            Console.WriteLine(("-----------------------Ram-----------------------------------"));

            //if(ramUsed / ramTot > Double.Parse(normGroups[2].Norms.Find(n => n.Name.Equals("Ram usage > 80%")).Value)/100){
            if (ramUsed / ramTot > 0.8) {
                Console.WriteLine(("-----------------------Ram 80-----------------------------------"));
                int nbStatus;
                if (!noAlarm) {
                    nbStatus = random.Next(1, 3);

                    var alarm = new Alarm {
                        TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                        Machine = machine,
                        NormGroup = normGroups[2],
                        UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                    };

                    alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                        random));
                }

                // Closed Alarm
                for (int j = 0; j < 3; j++) {
                    nbStatus = 3;
                    var oldAlarm = new Alarm {
                        TriggeredAt =
                            DateTime.UtcNow.AddDays(-random.Next(1, 365)).AddHours(-random.Next(0, 24))
                                .AddMinutes(-random.Next(0, 60)),
                        Machine = machine,
                        NormGroup = normGroups[2],
                        UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                    };

                    alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, oldAlarm, alarmStatusTypes, usersInRoles,
                        random));
                    alarms.Add(oldAlarm);
                }
            }

            Console.WriteLine(("-----------------------Ram 2-----------------------------------"));

            //if (ramTot < int.Parse(normGroups[4].Norms.Find(n => n.Name.Equals("Ram < 8GB")).Value)*100) {
            if (ramTot < 8 && !noAlarm) {
                Console.WriteLine(("-----------------------Ram 8GB-----------------------------------"));
                int nbStatus = random.Next(1, 3);

                var alarm = new Alarm {
                    TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                    Machine = machine,
                    NormGroup = normGroups[5],
                    UserId = random.Next(0, 101) < 80 ? usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id : null
                };

                alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                    random));

                alarms.Add(alarm);
            }
            else {
                // Closed alarm
                int nbStatus = 3;
                var alarm = new Alarm {
                    TriggeredAt =
                        DateTime.UtcNow.AddDays(-random.Next(1, 365)).AddHours(-random.Next(0, 24))
                            .AddMinutes(-random.Next(0, 60)),
                    Machine = machine,
                    NormGroup = normGroups[5],
                    UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                };

                alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                    random));

                alarms.Add(alarm);
            }

            // Get Storage
            Information infoDisk = machine.Informations.Find(i => i.Name == "List Disk");
            Console.WriteLine(("-----------------------Storage-----------------------------------"));
            foreach (Information info in infoDisk.Children.Find(i => i.Name == "Disks").Children) {
                int diskTot = int.Parse(info.Children.Find(i => i.Name == "Disk Total Size").Value);

                double diskUsed = Double.Parse(info.Children.Find(i => i.Name == "Disk Used").Value);
                Console.WriteLine(("-----------------------Storage Boucle-----------------------------------"));
                //if (diskUsed / diskTot > Double.Parse(normGroups[0].Norms.Find(n => n.Name.Equals("Storage full")).Value)/100) {
                if (diskUsed / diskTot > 0.99) {
                    Console.WriteLine(("-----------------------Storage Full-----------------------------------"));
                    int nbStatus;

                    if (!noAlarm) {
                        nbStatus = random.Next(1, 3);
                        var alarm = new Alarm {
                            TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                            Machine = machine,
                            NormGroup = normGroups[0],
                            UserId = random.Next(0, 101) < 80
                                ? usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                                : null
                        };

                        alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                            random));

                        alarms.Add(alarm);
                    }

                    // Closed alarm
                    for (int j = 0; j < 20; j++) {
                        nbStatus = 3;
                        var oldAlarm = new Alarm {
                            TriggeredAt =
                                DateTime.UtcNow.AddDays(-random.Next(1, 365)).AddHours(-random.Next(0, 24))
                                    .AddMinutes(-random.Next(0, 60)),
                            Machine = machine,
                            NormGroup = normGroups[0],
                            UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                        };

                        alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, oldAlarm, alarmStatusTypes,
                            usersInRoles, random));
                        alarms.Add(oldAlarm);
                    }
                }
                //else if (diskUsed / diskTot > Double.Parse(normGroups[3].Norms.Find(n => n.Name.Equals("Storage over 80%")).Value)/100) {
                else if (diskUsed / diskTot > 0.8) {
                    Console.WriteLine(("-----------------------Storage 80-----------------------------------"));
                    int nbStatus;

                    if (!noAlarm) {
                        nbStatus = random.Next(1, 3);
                        var alarm = new Alarm {
                            TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                            Machine = machine,
                            NormGroup = normGroups[3],
                            UserId = random.Next(0, 101) < 80
                                ? usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                                : null
                        };

                        alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                            random));

                        alarms.Add(alarm);
                    }

                    // Closed Alarm
                    for (int j = 0; j < 20; j++) {
                        nbStatus = 3;
                        var oldAlarm = new Alarm {
                            TriggeredAt =
                                DateTime.UtcNow.AddDays(-random.Next(1, 365)).AddHours(-random.Next(0, 24))
                                    .AddMinutes(-random.Next(0, 60)),
                            Machine = machine,
                            NormGroup = normGroups[3],
                            UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                        };

                        alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, oldAlarm, alarmStatusTypes,
                            usersInRoles, random));
                        alarms.Add(oldAlarm);
                    }
                }
            }

            // Get DirectX
            Information infoDirectX = machine.Informations.Find(i => i.Name == "Direct X");

            Console.WriteLine(("-----------------------DirectX-----------------------------------"));
            //if (!infoDirectX.Value.Equals(normGroups[4].Norms.Find(n => n.Name.Equals("Old Direct")).Value)) {
            if (!infoDirectX.Value.Equals("DirectX 12") && !noAlarm) {
                Console.WriteLine(("-----------------------Wrong DirectX-----------------------------------"));
                int nbStatus = random.Next(1, 3);

                var alarm = new Alarm {
                    TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                    Machine = machine,
                    NormGroup = normGroups[4],
                    UserId = random.Next(0, 101) < 80 ? usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id : null
                };

                alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                    random));

                alarms.Add(alarm);
            }
            else {
                // Closed alarm
                int nbStatus = 3;
                var alarm = new Alarm {
                    TriggeredAt =
                        DateTime.UtcNow.AddDays(-random.Next(1, 365)).AddHours(-random.Next(0, 24))
                            .AddMinutes(-random.Next(0, 60)),
                    Machine = machine,
                    NormGroup = normGroups[4],
                    UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                };

                alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                    random));

                alarms.Add(alarm);
            }

            // Get Windows
            Information infoOs = machine.Informations.Find(i => i.Name == "OS");
            Console.WriteLine(("-----------------------OS-----------------------------------"));
            Console.WriteLine(infoOs.Children.Find(i => i.Name == "OS Name").Value);
            //if (infoOs.Children.Find(i => i.Name == "OS Name").Value.Contains(normGroups[1].Norms.Find(n => n.Name.Equals("Windows 10 detected")).Value)) {
            if (infoOs.Children.Find(i => i.Name == "OS Name").Value.Contains("10") && !noAlarm) {
                Console.WriteLine(("-----------------------Wrong OS-----------------------------------"));
                int nbStatus = random.Next(1, 3);

                var alarm = new Alarm {
                    TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                    Machine = machine,
                    NormGroup = normGroups[1],
                    UserId = random.Next(0, 101) < 80 ? usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id : null
                };

                alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                    random));

                alarms.Add(alarm);
            }
            else {
                //Closed alarm
                int nbStatus = 3;
                var alarm = new Alarm {
                    TriggeredAt =
                        DateTime.UtcNow.AddDays(-random.Next(1, 365)).AddHours(-random.Next(0, 24))
                            .AddMinutes(-random.Next(0, 60)),
                    Machine = machine,
                    NormGroup = normGroups[1],
                    UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                };

                alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                    random));

                alarms.Add(alarm);
            }
        }

        context.AlarmHistories.AddRange(alarmStatusHistories);
        context.Alarms.AddRange(alarms);
        context.SaveChanges();
    }

    private static void SeedNotes(ApplicationDbContext context, List<Machine> machines) {
        var random = new Random();
        var notes = new List<Note>();

        var solutionTitles = new[] { "Driver Update", "System Vulnerability Patch", "OS Upgrade" };

        var solutionContents = new[] {
            "### Resolved issue with outdated drivers\n- **Issue**: The outdated drivers were causing frequent system crashes and slow performance, especially with peripheral devices.\n- **Actions Taken**:\n   1. Identified that several drivers were not up to date.\n   2. Updated drivers for network adapter, sound card, and graphics card.\n   3. Restarted the system after the updates were completed to ensure stability.\n- **Outcome**: After the update, the system was much more stable, and there were no crashes during the stress tests.\n![Driver Image](image1.png)\n\n",
            "### Patched system vulnerabilities\n- **Issue**: A security vulnerability was discovered in the system related to outdated security patches and missing updates.\n- **Actions Taken**:\n   1. Applied critical patches to address known vulnerabilities in the system.\n   2. Updated the operating system and security-related software to the latest versions.\n   3. Ran a vulnerability scan to ensure all security patches were correctly installed and no residual threats remained.\n- **Outcome**: The vulnerability has been successfully patched, and a follow-up scan confirmed no remaining issues. The system is now fully up-to-date with current security standards.\n",
            "### Updated operating system\n- **Issue**: The operating system was outdated, with multiple patches missing, exposing the system to potential malware and instability.\n- **Actions Taken**:\n   1. Backed up all critical data and created restore points for the system.\n   2. Installed the latest operating system version along with all pending security updates.\n   3. Checked for application compatibility with the new version.\n- **Outcome**: The upgrade was successful, and performance has improved significantly. All applications are running smoothly, and the system is now more secure.\n"
        };

        var nonSolutionTitles = new[] { "Investigating CPU Usage", "Storage Monitoring" };
        var nonSolutionContents = new[] {
            "### Investigating high CPU usage\n- **Issue**: Users reported performance lag due to excessive CPU usage when running high-demand applications.\n- **Actions Taken**:\n   1. Analyzed running processes and identified a few applications that were consuming excessive CPU resources.\n   2. Disabled unnecessary background tasks and services that were consuming CPU.\n   3. Monitored CPU performance over a 24-hour period after making adjustments.\n   4. Further investigation revealed a potential memory leak in one of the installed applications.\n- **Current Status**: The issue is still being investigated. The next step is to contact the application vendor for a patch related to the memory leak.\n- ![CPU](image4.png)\n\n",
            "### Monitoring storage capacity\n- **Issue**: The system's storage reached 95% capacity, triggering warnings of potential performance degradation.\n- **Actions Taken**:\n   1. Checked current disk usage and identified large files and temporary data causing the storage overflow.\n   2. Moved several non-essential files to an external drive to free up space.\n   3. Implemented disk clean-up tasks to remove redundant files and logs that were not required.\n   4. Reconfigured disk quotas to ensure that future storage usage remains manageable.\n- **Current Status**: The system is stable, and storage usage is now below 70%. However, I’m continuing to monitor disk usage to prevent further issues. A long-term solution will involve upgrading the storage device.\n- ![Storage](image5.png)\n\n"
        };

        var usersInRolesNote = context.Users.ToList();

        List<string> ExtractImageFileNames(string content) {
            var fileNames = new List<string>();
            var regex = new Regex(@"!\[.*?\]\((.*?)\)", RegexOptions.Compiled);
            var matches = regex.Matches(content);

            foreach (Match match in matches) {
                var imageFileName = match.Groups[1].Value;
                fileNames.Add(imageFileName);
            }

            return fileNames;
        }

        void AddFilesToNote(Note note, List<string> imageFileNames) {
            var files = new List<UITManagerWebServer.Models.File>();

            foreach (var fileName in imageFileNames) {
                var filePath = Path.Combine("Images", fileName);
                if (System.IO.File.Exists(filePath)) {
                    var fileContent = System.IO.File.ReadAllBytes(filePath);
                    var mimeType = "image/jpg";

                    var file = new UITManagerWebServer.Models.File {
                        FileName = fileName, FileContent = fileContent, MimeType = mimeType, NoteId = note.Id
                    };

                    files.Add(file);
                }
            }

            note.Files = files;
        }


        for (int i = 0; i < 3; i++) {
            var note = new Note {
                Title = solutionTitles[i],
                Content = solutionContents[i],
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48)),
                Machine = machines[i],
                IsSolution = true,
                AuthorId = usersInRolesNote[random.Next(0, usersInRolesNote.Count)].Id
            };

            var imageFileNames = ExtractImageFileNames(note.Content);
            notes.Add(note);
        }

        for (int i = 0; i < 2; i++) {
            var note = new Note {
                Title = nonSolutionTitles[i],
                Content = nonSolutionContents[i],
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48)),
                Machine = machines[i + 3],
                IsSolution = false,
                AuthorId = usersInRolesNote[random.Next(0, usersInRolesNote.Count)].Id
            };

            var imageFileNames = ExtractImageFileNames(note.Content);
            notes.Add(note);
        }

        try {
            context.Notes.AddRange(notes);
            context.SaveChanges();

            foreach (var note in notes) {
                var imageFileNames = ExtractImageFileNames(note.Content);
                AddFilesToNote(note, imageFileNames);
            }

            context.SaveChanges();
        }
        catch (Exception ex) {
            Console.WriteLine($"Error saving notes: {ex.Message}");
        }
    }

     private static async Task<string> GenerateUniqueWindowsMachineName(ApplicationDbContext context) {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const string charsForSite = "ABC";

        while (true) {
            var randomId = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());
            var site = new string(Enumerable.Repeat(charsForSite, 1).Select(s => s[random.Next(s.Length)]).ToArray());
            var generatedName = $"{site}-DESKTOP-{randomId}";

            var nameExists = await  context.Machines.AnyAsync(m => m.Name == generatedName);
            if (!nameExists) {
                return generatedName; 
            }
        }
    }


    private static double GetRandomNumber(double minimum, double maximum) {
        Random random = new Random();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }

    private static List<AlarmStatusHistory> AddStatusHistory(int nbStatus, Alarm alarm,
        List<AlarmStatusType> alarmStatusTypes, List<ApplicationUser> usersInRoles, Random random) {
        List<AlarmStatusHistory> alarmStatusHistories = new List<AlarmStatusHistory>();

        if (alarm.UserId == null) {
            nbStatus = 1;
        }

        DateTime tempModificationDate = default;
        for (int i = 0; i < nbStatus; i++) {
            DateTime earliestDate = (DateTime.Now < alarm.TriggeredAt) ? DateTime.Now : alarm.TriggeredAt;
            if (i == 0) tempModificationDate = earliestDate.AddHours(-random.Next(120, 240));
            else tempModificationDate = earliestDate.AddHours(-random.Next(1, 120));
            alarmStatusHistories.Add(
                new AlarmStatusHistory {
                    Alarm = alarm,
                    StatusType = alarmStatusTypes[i],
                    ModificationDate = tempModificationDate,
                    UserId = (i == 0) ? null : usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                });
        }

        return alarmStatusHistories;
    }

}