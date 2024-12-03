using Microsoft.AspNetCore.Identity;
using UITManagerWebServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using UITManagerWebServer.Data;

public static class Populate {
    
    public static async Task Initialize(IServiceProvider serviceProvider) {
    
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await SeedUsersAsync(userManager, roleManager, context);

        DeleteDb(context);

        await SeedDatabase(userManager, context);
    }

    private static void DeleteDb(ApplicationDbContext context) {
        if (context.Machines.Any() || context.NormGroups.Any()) {
            context.Alarms.RemoveRange(context.Alarms);
            context.Notes.RemoveRange(context.Notes);
            context.InformationNames.RemoveRange(context.InformationNames);
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
        var directXName = new InformationName { Name = "Direct X" };
        var domainNameName = new InformationName { Name = "Domain Name" };
        var tagServiceName = new InformationName { Name = "Tag Service" };
        var uptimeName = new InformationName { Name = "Uptime" };
        var cpuName = new InformationName {
            Name = "CPU",
            SubInformationNames = new List<InformationName> {
                new InformationName{Name = "Logical Core"},
                new InformationName{Name = "Core Count"},
                new InformationName{Name = "Clock Speed"},
                new InformationName{Name = "Model"},
                new InformationName{Name = "Used"},
            }
        };
        
        //var logicalCoreName = new InformationName { Name = "Logical Core" };
        //var coreCountName = new InformationName { Name = "Core Count" };
        //var clockSpeedName = new InformationName { Name = "Clock Speed" };
        //var modelName = new InformationName { Name = "Model" };
        var ramName = new InformationName {
            Name = "Ram",
            SubInformationNames = new List<InformationName> {
                new InformationName{Name = "Total RAM"},
                new InformationName{Name = "Used RAM"},
                new InformationName{Name = "Free RAM"},
            }
        };
        //var totalRamName = new InformationName { Name = "Total Ram" };
        //var usedRamName = new InformationName { Name = "Used Ram" };
        //var freeRamName = new InformationName { Name = "Free Ram" };
        
        
        var osName = new InformationName {
            Name = "OS",
            SubInformationNames = new List<InformationName> {
                new InformationName{Name = "OS Name"},
                new InformationName{Name = "OS Version"},
                new InformationName{Name = "OS Build"},
            }
        };
        //var osNameName = new InformationName { Name = "OS Name" };
        //var osVersionName = new InformationName { Name = "OS Version" };
        //var osBuildName = new InformationName { Name = "OS Build" };
        
        var ipName = new InformationName { Name = "IP Address" };
        //var IPsName = new InformationName { Name = "IPs" };
        //var freeSizeName = new InformationName { Name = "Disk Free Size" };
        //var totalSizeName = new InformationName { Name = "Disk Total Size" };
        var disksName = new InformationName {
            Name = "Disks",
            SubInformationNames = new List<InformationName> {
                new InformationName { Name = "Disk Free Size" },
                new InformationName { Name = "Disk Total Size" },
                new InformationName { Name = "Disk Used" },
                new InformationName { Name = "List Name" },
                new InformationName { Name="Number" }
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
        //var userScopeName = new InformationName { Name = "User Scope" };
        //var userNameName = new InformationName { Name = "User Name" };
        //var usersListName = new InformationName { Name = "Users List" };
        //var listDisksName = new InformationName { Name = "List Disks" };
        //var numberDisksName = new InformationName { Name = "Number Disks" };
        //var diskUsed = new InformationName { Name = "Disks Used Memory" };

        context.InformationNames.AddRange(
            directXName, domainNameName, tagServiceName, uptimeName,
            cpuName,/* logicalCoreName, coreCountName, clockSpeedName,
            modelName,*/ ramName, /*totalRamName, usedRamName, freeRamName,*/
            osName, /*osNameName, osVersionName, osBuildName,*/ ipName, /*IPsName,
            freeSizeName, totalSizeName,*/ disksName/*, userScopeName, userNameName,
            usersListName, listDisksName, numberDisksName, diskUsed*/
        );
        context.SaveChanges();
        var normGroups = new List<NormGroup> {
            new NormGroup {
                Name = "Obsolete operating system",
                Priority = 8,
                Norms = new List<Norm> { new Norm { Name = "Windows 10 detected" , InformationName = osName.SubInformationNames[0], Condition = "IN", Format = "TEXT", Value = "WINDOWS 10"} },
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = true
            },
            new NormGroup {
                Name = "Storage exceeded",
                Priority = 4,
                Norms = new List<Norm> { new Norm { Name = "Storage over 80%", InformationName = disksName.SubInformationNames[2], Condition = ">", Format = "%", Value = "80" } },
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = true
            },
            new NormGroup {
                Name = "CPU Usage High",
                Priority = 2,
                Norms = new List<Norm> { new Norm { Name = "CPU usage > 90%", InformationName = cpuName.SubInformationNames[4], Condition = ">", Format = "%", Value = "90"} },
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = true
            },
            new NormGroup {
                Name = "Memory Usage Warning",
                Priority = 1,
                Norms = new List<Norm> { new Norm { Name = "Memory usage > 70%", InformationName = disksName.SubInformationNames[2], Condition = ">", Format = "%", Value = "70"} },
                MaxExpectedProcessingTime = TimeSpan.FromDays(5),
                IsEnable = false
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
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-50),
                NormGroup = normGroups[0],
                Severity = severities[2],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow,
                NormGroup = normGroups[0],
                Severity = severities[4],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-19),
                NormGroup = normGroups[1],
                Severity = severities[1],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-5),
                NormGroup = normGroups[1],
                Severity = severities[3],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-46),
                NormGroup = normGroups[2],
                Severity = severities[4],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-146),
                NormGroup = normGroups[2],
                Severity = severities[2],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
            },
            new SeverityHistory {
                UpdateDate = DateTime.UtcNow.AddHours(-200),
                NormGroup = normGroups[3],
                Severity = severities[1],
                UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
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

        var directX = new[] { "Direct X 12", "Direct X 11", "Direct X 10" };
        var Os = new[] { "Microsoft Windows 10 Enterprise", "Microsoft Windows 11 Enterprise" };
        var OsV = new[] { "23h2", "24h2", "22h2", };
        var OsB = new[] { "22631", "26100", "19045" };

        var machines = new List<Machine>();

        for (int i = 1; i <= 50; i++) {
            var brandAndModel = brandsAndModels[random.Next(brandsAndModels.Length)];
            var brand = brandAndModel.Brand;
            var model = brandAndModel.Models[random.Next(brandAndModel.Models.Length)];
            var seen = random.Next(1, 101) <= 30 ? DateTime.UtcNow.AddDays(-3) : DateTime.UtcNow;

            var machineId = GenerateRandomWindowsMachineName();

            var machineName = $"{machineId}";
            var Model = $"{brand} {model}";
            var Machine = new Machine {
                Name = machineName, Model = Model, IsWorking = random.Next(1, 101) <= 90, LastSeen = seen,
            };

            Machine.Informations.Add(
                new Value { Machine = Machine, Name = "Direct X", Values = directX[random.Next(directX.Length)] });

            Machine.Informations.Add(
                new Value { Machine = Machine, Name = "Domain name", Values = "WORKGROUP" });

            Machine.Informations.Add(
                new Value { Machine = Machine, Name = "Tag Service", Values = "AB45CD78" });


            var date = new TimeSpan(random.Next(0, 151), random.Next(0, 24), random.Next(0, 60), random.Next(0, 60));
            Machine.Informations.Add(
                new Value { Machine = Machine, Name = "UpTimes", Values = date.ToString() });

            var modeltype = new[] {
                "AMD Ryzen 7 7700X", "Intel Core i9-13900K", "Intel Core i7-13700K", "Intel Xeon W-3175X"
            };
            var locgical = new[] { "16", "32", "64", "126" };
            var coreCount = new[] { "8", "16", "32", "64" };
            var clock = new[] { "3601", "5204", "2500", "3100", "4600" };
            Machine.Informations.Add(
                new Component {
                    Name = "CPU",
                    Machine = Machine,
                    Values = "Null",
                    Children = new List<Informations> {
                        new Value {
                            Name = "Logical core", Machine = Machine, Values = locgical[random.Next(locgical.Length)],
                        },
                        new Value {
                            Name = "Core count", Machine = Machine, Values = coreCount[random.Next(coreCount.Length)],
                        },
                        new Value {
                            Name = "Clockspeed", Machine = Machine, Values = clock[random.Next(clock.Length)],
                        },
                        new Value {
                            Name = "Model", Machine = Machine, Values = modeltype[random.Next(modeltype.Length)],
                        }
                    }
                });

            var ramT = new[] { 16, 32, 64 };
            var ramU = new[] { 10, 12.8, 25.6, 51.2, 40.4, 60, 20 };
            int ramt;
            double ramu;
            double ramF;
            do {
                ramt = ramT[random.Next(ramT.Length)];
                ramu = ramU[random.Next(ramU.Length)];
                ramF = ramt - ramu;
            } while (ramF < 0);

            Machine.Informations.Add(
                new Component {
                    Name = "Ram",
                    Machine = Machine,
                    Values = "Null",
                    Children = new List<Informations> {
                        new Value { Name = "Total RAM", Machine = Machine, Values = ramt + " Go", },
                        new Value { Name = "Used RAM", Machine = Machine, Values = ramu + " Go", },
                        new Value { Name = "Free RAM", Machine = Machine, Values = ramF + " Go", }
                    }
                });


            Machine.Informations.Add(
                new Component {
                    Name = "OS",
                    Machine = Machine,
                    Values = "Null",
                    Children = new List<Informations> {
                        new Value { Name = "OS Name", Machine = Machine, Values = Os[random.Next(Os.Length)], },
                        new Value { Name = "Os Version", Machine = Machine, Values = OsV[random.Next(OsV.Length)], },
                        new Value { Name = "Os Build", Machine = Machine, Values = OsB[random.Next(OsB.Length)], },
                    }
                });

            var ipAddresses = new[] {
                "192.168.1.15", "10.0.0.87", "172.16.24.65", "203.0.113.42", "8.8.8.8", "172.18.99.22",
                "192.168.10.200", "10.1.1.1", "192.0.2.33", "198.51.100.5", "100.64.0.1", "172.19.45.88",
                "192.168.0.254", "10.255.255.254", "203.0.113.120", "8.8.4.4", "192.168.56.1", "10.100.200.5",
                "172.20.14.1", "192.168.2.25", "172.21.13.50", "10.10.10.10", "192.168.100.100", "198.51.100.1",
                "203.0.113.55", "10.0.5.5", "172.22.99.88", "192.168.3.3", "10.123.45.67", "172.23.7.77", "192.168.4.1",
                "203.0.113.150", "10.10.20.30", "172.24.56.78", "192.168.5.5", "198.51.100.99", "172.25.1.1",
                "192.168.50.2", "10.20.30.40", "172.26.33.44", "192.168.6.6", "10.30.40.50", "172.27.99.100",
                "192.168.7.7", "198.51.100.2", "203.0.113.3", "10.40.50.60", "172.28.88.77", "192.168.8.8",
                "10.50.60.70", "172.29.11.22", "192.168.9.9", "198.51.100.10", "203.0.113.99", "10.60.70.80",
                "172.30.99.123", "192.168.10.10", "10.70.80.90", "172.31.99.45", "192.168.11.11", "198.51.100.20",
                "203.0.113.56", "10.80.90.100", "172.32.14.88", "192.168.12.12", "10.90.100.110", "172.33.33.33",
                "192.168.13.13", "198.51.100.30", "203.0.113.12", "10.100.110.120", "172.34.56.78", "192.168.14.14",
                "10.110.120.130", "172.35.88.99", "192.168.15.15", "198.51.100.40", "203.0.113.22", "10.120.130.140",
                "172.36.10.11",
            };
            List<Informations> ip = new List<Informations>();
            for (int j = 0; j < random.Next(1, 3); j++) {
                var val = new Value {
                    Name = "Ip Address", Machine = Machine, Values = ipAddresses[random.Next(ipAddresses.Length)],
                };
                ip.Add(val);
            }

            Machine.Informations.Add(
                new Component {
                    Name = "IPs", Machine = Machine, Values = "Null", Children = ip,
                });

            var diskNames = new string[] {
                "C:System_Disk", "D:Data_Drive", "E:Backup_Disk", "F:Media_Storage", "G:Games_Drive", "H:VM_Storage",
                "I:Archive_1", "J:Personal_Files", "K:Shared_Drive", "L:Encrypted_Vault"
            };
            List<Informations> Disks = new List<Informations>();
            for (int j = 0; j < random.Next(1, 3); j++) {
                var val = new Component {
                    Name = diskNames[random.Next(diskNames.Length)],
                    Machine = Machine,
                    Values = "Null",
                    Children = new List<Informations> {
                        new Value { Name = "Disk Free Size", Values = random.Next(255, 700) + "Go", Machine = Machine },
                        new Value {
                            Name = "Disk Total Size", Values = random.Next(255, 952) + "Go", Machine = Machine
                        },
                    }
                };
                Disks.Add(val);
            }

            Machine.Informations.Add(
                new Component {
                    Name = "List Disk",
                    Machine = Machine,
                    Values = "Null",
                    Children = new List<Informations> {
                        new Component {
                            Name = "Disks", Machine = Machine, Values = "Null", Children = Disks,
                        },
                        new Value { Name = "Number disks", Machine = Machine, Values = Disks.Count.ToString(), },
                    }
                });

            var scoop = new[] { "local", "domain" };
            var name = new[] { "Secretary", "Commercial", "Employee" };
            Machine.Informations.Add(
                new Component {
                    Name = "Users List",
                    Machine = Machine,
                    Values = "Null",
                    Children = new List<Informations> {
                        new Component {
                            Name = "User",
                            Values = "Null",
                            Machine = Machine,
                            Children =
                                new List<Informations> {
                                    new Value { Name = "User Name", Machine = Machine, Values = "Admin", },
                                    new Value { Name = "User Scope", Machine = Machine, Values = "Local", },
                                }
                        },
                        new Component {
                            Name = "User",
                            Values = "Null",
                            Machine = Machine,
                            Children = new List<Informations> {
                                new Value { Name = "User Name", Machine = Machine, Values = "DefaultAccount", },
                                new Value { Name = "User Scope", Machine = Machine, Values = "Local", },
                            }
                        },
                        
                        new Component {
                            Name = "User",
                            Machine = Machine,
                            Values = "Null",
                            Children = new List<Informations> {
                                new Value {
                                    Name = "User Name", Machine = Machine, Values = name[random.Next(name.Length)],
                                },
                                new Value {
                                    Name = "User Scope", Machine = Machine, Values = scoop[random.Next(scoop.Length)],
                                },
                            }
                        },
                    }
                });

            machines.Add(Machine);
        }

        context.Machines.AddRange(machines);
        context.SaveChanges();

        string GenerateRandomWindowsMachineName() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string charsForSite = "ABC";

            var randomId = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());
            var site = "Site-" +
                       new string(Enumerable.Repeat(charsForSite, 1).Select(s => s[random.Next(s.Length)]).ToArray());

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
                        UserId = usersInRoles[random.Next(0, usersInRoles.Count - 1)].Id
                    };

                    int historyCount = random.Next(1, 5);

                    for (i = 0; i < historyCount; i++) {
                        alarmStatusHistories.Add(
                            new AlarmStatusHistory {
                                Alarm = alarm,
                                StatusType = alarmStatusTypes[i],
                                ModificationDate = DateTime.UtcNow.AddHours(-10 + i),
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

        var solutionTitles = new[] { "Driver Update", "System Vulnerability Patch", "OS Upgrade" };

        var solutionContents = new[] {
            "Resolved issue with outdated drivers. ![Driver Image](image1.jpg)",
            "Patched system vulnerabilities successfully. ![Vulnerability Image](image2.jpg)",
            "Updated operating system to the latest version. ![OS Upgrade Image](image3.jpg)"
        };

        var nonSolutionTitles = new[] { "Investigating CPU Usage", "Storage Monitoring" };
        var nonSolutionContents = new[] {
            "Investigating high CPU usage. ![CPU](image4.jpg)",
            "Monitoring storage capacity after warning. ![Storage](image5.jpg)"
        };

// Assure-toi que la liste des machines et des utilisateurs est bien peuplée
        var machinesWithNotes = machines.OrderBy(_ => random.Next()).Take(5).ToList();
        var usersInRolesNote = context.Users.ToList(); // Utiliser des utilisateurs valides depuis la base de données

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
                Machine = machinesWithNotes[i],
                IsSolution = true,
                AuthorId = usersInRolesNote[random.Next(0, usersInRolesNote.Count)].Id
            };

            var imageFileNames = ExtractImageFileNames(note.Content);
            notes.Add(note); // Ajouter la note à la liste avant d'ajouter les fichiers
        }

        for (int i = 0; i < 2; i++) {
            var note = new Note {
                Title = nonSolutionTitles[i],
                Content = nonSolutionContents[i],
                CreatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48)),
                Machine = machinesWithNotes[i + 3],
                IsSolution = false,
                AuthorId = usersInRolesNote[random.Next(0, usersInRolesNote.Count)].Id
            };

            var imageFileNames = ExtractImageFileNames(note.Content);
            notes.Add(note); // Ajouter la note à la liste avant d'ajouter les fichiers
        }

        try {
            context.Notes.AddRange(notes);
            context.SaveChanges(); // Sauvegarde les notes d'abord
            Console.WriteLine("Notes saved successfully");

            // Ajouter les fichiers après que les notes aient été sauvegardées
            foreach (var note in notes) {
                var imageFileNames = ExtractImageFileNames(note.Content);
                AddFilesToNote(note, imageFileNames); // Ajouter les fichiers à la note
            }

            context.SaveChanges(); // Sauvegarde les fichiers associés après l'enregistrement des notes
            Console.WriteLine("Files saved successfully");
        }
        catch (Exception ex) {
            Console.WriteLine($"Error saving notes: {ex.Message}");
        }

        Console.WriteLine(
            $"Database populated with {machines.Count} machines, {alarms.Count} alarms, and {notes.Count} notes.");
    }
}