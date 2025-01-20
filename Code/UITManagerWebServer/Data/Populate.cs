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

        //SeedNotes(context, machines);

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
        var roles = new List<string> { "Maintenance Manager", "Technician", "IT Director" };
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
                                    await userManager.AddToRoleAsync(user, "IT Director");
                                }
                                else if (user.LastName == "MILLET" || user.LastName == "SEGUIN") {
                                    await userManager.AddToRoleAsync(user, "Maintenance Manager");
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

        var rolesToFind = new List<string> { "Maintenance Manager", "IT Director" };

        var usersInRoles = new List<ApplicationUser>();

        foreach (var role in rolesToFind) {
            var usersInRole = await userManager.GetUsersInRoleAsync(role);
            usersInRoles.AddRange(usersInRole);
        }

        usersInRoles = usersInRoles.Where(user => user.IsActivate).Distinct().ToList();
        
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
            "183.172.2.65", "183.172.2.79", "183.172.2.86", "183.172.2.24", "183.172.2.11", "183.172.2.235", "183.172.2.159", "183.172.2.144",
            "183.172.2.185", "183.172.2.252", "183.172.2.136", "183.172.2.99", "183.172.2.213", "183.172.2.249", "183.172.2.102", "183.172.2.100",
            "183.172.2.59", "183.172.2.90", "183.172.2.211", "183.172.2.20", "183.172.2.210", "183.172.2.199", "183.172.2.47", "183.172.2.93",
            "183.172.2.132", "183.172.2.127", "183.172.2.96", "183.172.2.5", "183.172.2.215", "183.172.2.85", "183.172.2.104", "183.172.2.212",
            "183.172.2.52", "183.172.2.93", "183.172.2.76", "183.172.2.102", "183.172.2.81", "183.172.2.154", "183.172.2.59", "183.172.2.153",
            "183.172.2.123", "183.172.2.241", "183.172.2.144", "183.172.2.197", "183.172.2.153", "183.172.2.27", "183.172.2.222", "183.172.2.38",
            "183.172.2.108", "183.172.2.30", "183.172.2.72", "183.172.2.225", "183.172.2.93", "183.172.2.203", "183.172.2.249", "183.172.2.109",
            "183.172.2.112", "183.172.2.26", "183.172.2.198", "183.172.2.71", "183.172.2.127", "183.172.2.235", "183.172.2.92", "183.172.2.205",
            "183.172.2.123", "183.172.2.119", "183.172.2.108", "183.172.2.64", "183.172.2.53", "183.172.2.196", "183.172.2.234", "183.172.2.54",
            "183.172.2.85", "183.172.2.186", "183.172.2.31", "183.172.2.155", "183.172.2.82", "183.172.2.151", "183.172.2.178", "183.172.2.43"
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
            double ramUsedPercent;
            if (i > nbMachines * 0.4 && i < nbMachines * 0.56) {
                ramUsed = GetRandomNumber(ramTotal * 0.8, ramTotal);
                ramUsedPercent = ramUsed / ramTotal * 100;
            }
            else {
                ramUsed = GetRandomNumber(0, ramTotal * 0.81);
                ramUsedPercent = ramUsed / ramTotal * 100;
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
                        new Value { Name = "Used RAM", Machine = machine, Value = ramUsedPercent.ToString(), Format = "%" },
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
                        double diskUsedPercent = diskUsed / diskTotal * 100;

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
                                    Name = "Disk Used", Value = diskUsedPercent.ToString(), Format = "%", Machine = machine
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
                        double diskUsedPercent = diskUsed / diskTotal * 100;

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
                                    Name = "Disk Used", 
                                    Value = diskUsed.ToString(), 
                                    Format = "GB", 
                                    Machine = machine
                                },
                                new Value {
                                    Name = "Disk Used", Value = diskUsedPercent.ToString(), Format = "%", Machine = machine
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
                        double diskUsedPercent = diskUsed / diskTotal * 100;

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
                                    Name = "Disk Used", 
                                    Value = diskUsed.ToString(), 
                                    Format = "GB", 
                                    Machine = machine
                                },
                                new Value {
                                    Name = "Disk Used", Value = diskUsedPercent.ToString(), Format = "%", Machine = machine
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
                    double diskUsedPercent = diskUsed / diskTotal * 100;
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
                                Name = "Disk Used", Value = diskUsedPercent.ToString(), Format = "%", Machine = machine
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

        usersInRoles = usersInRoles.Where(user => user.IsActivate).Distinct().ToList();

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
            },
            new AlarmStatusType {
                Name = "Not Triggered Anymore",
                Description =
                    "An issue that previously triggered an alarm, but the alarm criteria are no longer valid."
            }
        };

        context.AlarmStatusTypes.AddRange(alarmStatusTypes);

        var alarms = new List<Alarm>();
        var notes = new List<Note>();
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
                        TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(3, 24)).AddMinutes(-random.Next(0, 60)),
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
                    String userId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id;
                    var oldAlarm = new Alarm {
                        TriggeredAt = DateTime.UtcNow.AddDays(-random.Next(2, 120)).AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                        Machine = machine,
                        NormGroup = normGroups[2],
                        UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                    };

                    alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, oldAlarm, alarmStatusTypes, usersInRoles,
                        random));
                    alarms.Add(oldAlarm);
                    notes.Add(AddNote(machine,"Investigating CPU Usage",alarmStatusHistories[^1].ModificationDate,userId));
                }
            }

            Console.WriteLine(("-----------------------Ram 2-----------------------------------"));

            //if (ramTot < int.Parse(normGroups[4].Norms.Find(n => n.Name.Equals("Ram < 8GB")).Value)*100) {
            if (ramTot < 8 && !noAlarm) {
                Console.WriteLine(("-----------------------Ram 8GB-----------------------------------"));
                int nbStatus = random.Next(1, 3);

                var alarm = new Alarm {
                    TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(3, 24)).AddMinutes(-random.Next(0, 60)),
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
                String userId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id;
                var alarm = new Alarm {
                    TriggeredAt =
                        DateTime.UtcNow.AddDays(-random.Next(2, 120)).AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                    Machine = machine,
                    NormGroup = normGroups[5],
                    UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                };

                alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                    random));

                alarms.Add(alarm);
                notes.Add(AddNote(machine,"Managed low RAM issue",alarmStatusHistories[^1].ModificationDate,userId));
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
                            TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(3, 24)).AddMinutes(-random.Next(0, 60)),
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
                    int triggerAddNote = random.Next(4,15);
                    for (int j = 0; j < 20; j++) {
                        nbStatus = 3;
                        String userId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id;
                        var oldAlarm = new Alarm {
                            TriggeredAt =
                                DateTime.UtcNow.AddDays(-random.Next(2, 120)).AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                            Machine = machine,
                            NormGroup = normGroups[0],
                            UserId = userId
                        };

                        alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, oldAlarm, alarmStatusTypes,
                            usersInRoles, random));
                        alarms.Add(oldAlarm); 
                        if (j == triggerAddNote)
                            notes.Add(AddNote(machine,"Storage Monitoring",alarmStatusHistories[^1].ModificationDate,userId));
                    }
                }
                //else if (diskUsed / diskTot > Double.Parse(normGroups[3].Norms.Find(n => n.Name.Equals("Storage over 80%")).Value)/100) {
                else if (diskUsed / diskTot > 0.8) {
                    Console.WriteLine(("-----------------------Storage 80-----------------------------------"));
                    int nbStatus;

                    if (!noAlarm) {
                        nbStatus = random.Next(1, 3);
                        var alarm = new Alarm {
                            TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(3, 24)).AddMinutes(-random.Next(0, 60)),
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
                    int triggerAddNote = random.Next(4,15);
                    // Closed Alarm
                    for (int j = 0; j < 20; j++) {
                        nbStatus = 3;
                        String userId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id;
                        var oldAlarm = new Alarm {
                            TriggeredAt =
                                DateTime.UtcNow.AddDays(-random.Next(2, 120)).AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                            Machine = machine,
                            NormGroup = normGroups[3],
                            UserId = userId
                        };

                        alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, oldAlarm, alarmStatusTypes,
                            usersInRoles, random));
                        alarms.Add(oldAlarm);
                        
                        if (j == triggerAddNote)
                            notes.Add(AddNote(machine,"Storage Monitoring 2",alarmStatusHistories[^1].ModificationDate,userId));
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
                    TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(3, 24)).AddMinutes(-random.Next(0, 60)),
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
                string userId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id;
                var alarm = new Alarm {
                    TriggeredAt =
                        DateTime.UtcNow.AddDays(-random.Next(2, 120)).AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                    Machine = machine,
                    NormGroup = normGroups[4],
                    UserId = userId
                };

                alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                    random));

                alarms.Add(alarm);
                notes.Add(AddNote(machine,"Upgraded DirectX Version",alarmStatusHistories[^1].ModificationDate,userId));
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
                    TriggeredAt = DateTime.UtcNow.AddHours(-random.Next(3, 24)).AddMinutes(-random.Next(0, 60)),
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
                string userId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id;
                var alarm = new Alarm {
                    TriggeredAt =
                        DateTime.UtcNow.AddDays(-random.Next(2, 120)).AddHours(-random.Next(0, 24)).AddMinutes(-random.Next(0, 60)),
                    Machine = machine,
                    NormGroup = normGroups[1],
                    UserId = userId
                };

                alarmStatusHistories.AddRange(AddStatusHistory(nbStatus, alarm, alarmStatusTypes, usersInRoles,
                    random));

                alarms.Add(alarm);
                notes.Add(AddNote(machine,"OS Upgrade",alarmStatusHistories[^1].ModificationDate,userId));
            }
        }
     
        notes.Add(AddNote(machines[machines.Count/2],"Driver Update",DateTime.UtcNow.AddDays(-random.Next(30,90)).AddHours(-random.Next(0,25)).AddMinutes(-random.Next(0,61)),usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id));
        notes.Add(AddNote(machines[machines.Count/4],"System Vulnerability Patch",DateTime.UtcNow.AddDays(-random.Next(30,90)).AddHours(-random.Next(0,25)).AddMinutes(-random.Next(0,61)),usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id));

        context.AlarmHistories.AddRange(alarmStatusHistories);
        context.Alarms.AddRange(alarms);
        context.Notes.AddRange(notes);
        context.SaveChanges();
    }
    
    private static Note AddNote(Machine machine, string type, DateTime? date, string authorId) {
        
        var solutionTitles = new[] { "Driver Update", "System Vulnerability Patch", "OS Upgrade" };
        var solutionContents = new[] {
            "### Resolved issue with outdated drivers\n- **Issue**: The outdated drivers were causing frequent system crashes and slow performance, especially with peripheral devices.\n- **Actions Taken**:\n   1. Identified that several drivers were not up to date.\n   2. Updated drivers for network adapter, sound card, and graphics card.\n   3. Restarted the system after the updates were completed to ensure stability.\n- **Outcome**: After the update, the system was much more stable, and there were no crashes during the stress tests.\n![Driver Image](image1.png)\n\n",
            "### Patched system vulnerabilities\n- **Issue**: A security vulnerability was discovered in the system related to outdated security patches and missing updates.\n- **Actions Taken**:\n   1. Applied critical patches to address known vulnerabilities in the system.\n   2. Updated the operating system and security-related software to the latest versions.\n   3. Ran a vulnerability scan to ensure all security patches were correctly installed and no residual threats remained.\n- **Outcome**: The vulnerability has been successfully patched, and a follow-up scan confirmed no remaining issues. The system is now fully up-to-date with current security standards.\n",
            "### Updated operating system\n- **Issue**: The operating system was outdated, with multiple patches missing, exposing the system to potential malware and instability.\n- **Actions Taken**:\n   1. Backed up all critical data and created restore points for the system.\n   2. Installed the latest operating system version along with all pending security updates.\n   3. Checked for application compatibility with the new version.\n- **Outcome**: The upgrade was successful, and performance has improved significantly. All applications are running smoothly, and the system is now more secure.\n"
        };

        var nonSolutionTitles = new[] { "Investigating CPU Usage", "Full storage capacity alarms triggered","Multiple 80% storage capacity alarms triggered","Managed low RAM issue","Upgraded DirectX Version" };
        var nonSolutionContents = new[] {
            "### Investigating high CPU usage\n- **Issue**: Users reported performance lag due to excessive CPU usage when running high-demand applications.\n- **Actions Taken**:\n   1. Analyzed running processes and identified a few applications that were consuming excessive CPU resources.\n   2. Disabled unnecessary background tasks and services that were consuming CPU.\n   3. Monitored CPU performance over a 24-hour period after making adjustments.\n   4. Further investigation revealed a potential memory leak in one of the installed applications.\n- **Current Status**: The issue is still being investigated. The next step is to contact the application vendor for a patch related to the memory leak.\n![CPU](image4.png)\n\n",
            "### Monitoring storage capacity\n- **Issue**: The system's storage reached 99% capacity, triggering warnings of potential performance degradation.\n- **Actions Taken**:\n   1. Checked current disk usage and identified large files and temporary data causing the storage overflow.\n   2. Moved several non-essential files to an external drive to free up space.\n   3. Implemented disk clean-up tasks to remove redundant files and logs that were not required.\n   4. Reconfigured disk quotas to ensure that future storage usage remains manageable.\n- **Current Status**: The system is stable, and storage usage is now below 70%. However, I’m continuing to monitor disk usage to prevent further issues. A long-term solution will involve upgrading the storage device.\n![Storage](image6.png)\n\n",
            "### Monitoring storage capacity\n- **Issue**: The system's storage reached 80% capacity, triggering warnings of potential performance degradation.\n- **Actions Taken**:\n   1. Checked current disk usage and identified large files and temporary data contributing to the high storage usage.\n   2. Moved several non-essential files to an external drive to free up space.\n   3. Implemented disk clean-up tasks to remove redundant files and logs that were not required.\n   4. Reconfigured disk quotas to ensure that future storage usage remains manageable.\n- **Current Status**: The system is stable, and storage usage is now below 70%. However, I’m continuing to monitor disk usage to prevent further issues. A long-term solution will involve upgrading the storage device.\n![Storage](image5.png)\n\n",
            "### Managed low RAM issue\n- **Issue**: The system experienced performance issues due to low available RAM, particularly during multitasking.\n- **Actions Taken**:\n  1. Identified high memory consumption from specific applications and background processes.\n  2. Increased the paging file size to temporarily offset the low RAM issue.\n  3. Ordered additional RAM modules (16 GB), with installation scheduled for next maintenance window.\n- **Outcome**: The system is stable for now, but the long-term resolution will involve upgrading the hardware to support increased workload demands.\n",
            "### Upgraded DirectX Version\n- **Issue**: The system was running an outdated version of DirectX (DirectX 11), which caused performance bottlenecks and limited support for modern graphics features in newer applications and games.\n- **Actions Taken**:\n  1. Verified the current DirectX version and confirmed the system was running DirectX 11.\n  2. Checked hardware compatibility to ensure support for DirectX 12.\n  3. Updated the GPU drivers to the latest version to support DirectX 12.\n  4. Installed the DirectX 12 runtime via the Windows Update service.\n  5. Conducted tests with DirectX 12-enabled applications to confirm stability and improved performance.\n- **Outcome**: The migration to DirectX 12 was successful. The system now supports modern graphics APIs and shows significant performance improvements in graphics-intensive applications. No issues were observed during the post-migration tests.\n"
        };

        string title = "";
        string content = "";
        bool isSolution = false;

        switch (type) {
            case "Driver Update":
                title = solutionTitles[0];
                content = solutionContents[0];
                isSolution = true;
                break;

            case "System Vulnerability Patch":
                title = solutionTitles[1];
                content = solutionContents[1];
                isSolution = true;
                break;

            case "OS Upgrade":
                title = solutionTitles[2];
                content = solutionContents[2];
                isSolution = true;
                break;

            case "Investigating CPU Usage":
                title = nonSolutionTitles[0];
                content = nonSolutionContents[0];
                isSolution = false;
                break;

            case "Storage Monitoring":
                title = nonSolutionTitles[1];
                content = nonSolutionContents[1];
                isSolution = false;
                break;
            
            case "Storage Monitoring 2":
                title = nonSolutionTitles[2];
                content = nonSolutionContents[2];
                isSolution = false;
                break;
            
            case "Managed low RAM issue":
                title = nonSolutionTitles[3];
                content = nonSolutionContents[3];
                isSolution = false;
                break;

            case "Upgraded DirectX Version":
                title = nonSolutionTitles[4];
                content = nonSolutionContents[4];
                isSolution = false;
                break;
            
        }

        var note = new Note {
            Title = title,
            Content = content,
            CreatedAt = (DateTime)date!,
            Machine = machine,
            IsSolution = isSolution,
            AuthorId = authorId
        };

        var imageFileNames = ExtractImageFileNames(content);
        AddFilesToNote(note, imageFileNames);

        return note;
    }

    
    private static List<string> ExtractImageFileNames(string content) {
        var fileNames = new List<string>();
        var regex = new Regex(@"!\[.*?\]\((.*?)\)", RegexOptions.Compiled); // Correspond au format ![alt text](file_path)
        var matches = regex.Matches(content);

        foreach (Match match in matches) {
            var imageFileName = match.Groups[1].Value; // Extrait le chemin ou le nom du fichier entre les parenthèses
            fileNames.Add(imageFileName);
        }

        return fileNames;
    }
    
    private static void AddFilesToNote(Note note, List<string> imageFileNames) {
        var files = new List<UITManagerWebServer.Models.File>();

        foreach (var fileName in imageFileNames) {
            var filePath = Path.Combine("Images", fileName); // Chemin du fichier dans le répertoire "Images"
            if (System.IO.File.Exists(filePath)) {
                var fileContent = System.IO.File.ReadAllBytes(filePath); // Lit le contenu du fichier
                var mimeType = "image/jpg"; // Définit le type MIME (peut être amélioré pour détecter automatiquement le type)

                var file = new UITManagerWebServer.Models.File {
                    FileName = fileName,
                    FileContent = fileContent,
                    MimeType = mimeType,
                    NoteId = note.Id // Lie le fichier à l'identifiant de la note
                };

                files.Add(file);
            }
        }

        note.Files = files; // Associe les fichiers à la note
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
        var alarmStatusHistories = new List<AlarmStatusHistory>();

        if (alarm.UserId == null) {
            nbStatus = 1;
        }

        DateTime startDate = alarm.TriggeredAt.ToUniversalTime();
        
        DateTime endDate;
        if (nbStatus != 3) {
            endDate = DateTime.Now.AddHours(-1).ToUniversalTime();
        }
        else {
            endDate = startDate.AddHours(random.Next(48, 73));
        }

        double totalHours = (endDate - startDate).TotalHours;

        DateTime tempModificationDate = startDate;

        for (int i = 0; i < nbStatus; i++) {

            double maxHoursForStatus = totalHours - (i * (totalHours / nbStatus));
            double hoursToAdd = random.NextDouble() * maxHoursForStatus;

            if (i !=0) {
                tempModificationDate = tempModificationDate.AddHours(hoursToAdd);
            }

            alarmStatusHistories.Add(new AlarmStatusHistory {
                Alarm = alarm,
                StatusType = alarmStatusTypes[i],
                ModificationDate = i == 0 ? alarm.TriggeredAt : tempModificationDate,
                UserId = i == 0 ? null : usersInRoles[random.Next(0, usersInRoles.Count)].Id
            });
        }

        return alarmStatusHistories;
    }

}