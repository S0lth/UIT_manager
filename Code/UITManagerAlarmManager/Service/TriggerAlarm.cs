using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UITManagerAlarmManager.Data;
using UITManagerAlarmManager.Models;


namespace UITManagerAlarmManager.Service;

public static class TriggerAlarm {
    /// <summary>
    /// This method is responsible for triggering alarms based on the machine's components and norm groups.
    /// It checks if the machine's components satisfy the conditions defined in the associated norm groups.
    /// If all norms are valid, it creates a new alarm for the machine.
    /// </summary>
    /// <param name="context">The database context for accessing data.</param>
    /// <param name="machineId">The ID of the machine to check for alarms.</param>
    /// <param name="normGroupId"></param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task TriggeredAsync(ApplicationDbContext context, int machineId, int normGroupId = -1) {
        Console.WriteLine("Hello");
        
        Machine? machine = context.Machines.FirstOrDefault(m => m.Id == machineId);

        List<Information> machineComponent = await context.Components
            .Where(c => c.MachinesId == machineId)
            .ToListAsync();
        
        List<NormGroup> listNormsGroups = new List<NormGroup>();
        if(normGroupId == -1) {
            
            listNormsGroups.AddRange(await context.NormGroups
                .Include(m => m.Norms)
                .ThenInclude(n => n.InformationName)
                .ToListAsync());            
        }
        else { 
            listNormsGroups.AddRange(await context.NormGroups
                .Where(ng => normGroupId == ng.Id)
                .Include(ng => ng.Norms)
                .ThenInclude(n => n.InformationName)
                .ToListAsync());
        }
        
        foreach (var normGroup in listNormsGroups) {
            await context.Entry(normGroup).ReloadAsync();
            foreach (var norm in normGroup.Norms) {
                await context.Entry(norm).ReloadAsync();
            }
        }
        
        Console.WriteLine(listNormsGroups[0].Norms[0].Value);
        List<AlarmStatusType> listStatus = await context.AlarmStatusTypes.ToListAsync();

        foreach (NormGroup normGroup in listNormsGroups) {
            bool alarmExists = await context.Alarms
                .Where(a => a.MachineId == machineId && a.NormGroupId == normGroup.Id)
                .AnyAsync(a => !context.AlarmHistories
                    .Where(ah2 => ah2.AlarmId == a.Id)
                    .OrderByDescending(ah2 => ah2.ModificationDate)
                    .Take(1)
                    .Any(ah2 => ah2.StatusType.Name == "Resolved" || ah2.StatusType.Name == "Not Triggered Anymore")
                );
            
            Console.WriteLine("Machine : " + machineId + " / " + alarmExists);

            if (!alarmExists) {
                bool allNormsValid = true;

                foreach (Norm norm in normGroup.Norms) {
                    Console.WriteLine("Norms : " + norm.InformationName!.Name + " ////// Format : " + norm.Format);

                    List<Information> result = machineComponent
                        .Where(m => m.Name == norm.InformationName?.Name && m.Format == norm.Format)
                        .ToList();

                    if (result.Count > 0) {
                        bool normValid = false;

                        foreach (Information information in result) {
                            switch (norm.Condition) {
                                case "NOT IN":
                                    Console.WriteLine("NOT IN");
                                    normValid = !norm.Value!.Contains(information.Value);
                                    break;
                                case "IN":
                                    Console.WriteLine("IN");
                                    normValid = norm.Value!.Contains(information.Value);
                                    break;
                                case "=":
                                    Console.WriteLine("=");
                                    normValid = Double.Parse(norm.Value!) == Double.Parse(information.Value);
                                    break;
                                case "!=":
                                    Console.WriteLine("!=");
                                    normValid = Double.Parse(norm.Value!) != Double.Parse(information.Value);
                                    break;
                                case ">":
                                    Console.WriteLine(">");
                                    normValid = Double.Parse(information.Value) > Double.Parse(norm.Value!);
                                    break;
                                case "<":
                                    Console.WriteLine("<");
                                    normValid = Double.Parse(information.Value) < Double.Parse(norm.Value!);
                                    break;
                            }

                            if (normValid) break;
                        }

                        if (!normValid) {
                            allNormsValid = false;
                            break;
                        }
                    }
                    else {
                        allNormsValid = false;
                        break;
                    }
                }
                if (allNormsValid) {
                    CreateAlarm(context, machineId, normGroup.Id, listStatus);
                    Email email = new Email(context);
                    //await email.Send($"An alarm has been triggered on the machine{machine?.Name}.}}");
                }
            }
        }
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new alarm for the specified machine and norm group, and logs the alarm status as "New".
    /// </summary>
    /// <param name="context">The database context for accessing data.</param>
    /// <param name="machineId">The ID of the machine for which the alarm is created.</param>
    /// <param name="normGroupId">The ID of the norm group associated with the alarm.</param>
    /// <param name="listStatus">A list of available alarm status types used to set the initial status of the alarm.</param>
    private static void CreateAlarm(ApplicationDbContext context, int machineId, int normGroupId, List<AlarmStatusType> listStatus) {
        Console.WriteLine("Trigger : " + normGroupId);
        Alarm alarm = new Alarm { MachineId = machineId, NormGroupId = normGroupId, TriggeredAt = DateTime.UtcNow };
        AlarmStatusHistory alarmStatusHistory = new AlarmStatusHistory {
            Alarm = alarm, StatusType = listStatus.Find(s => s.Name == "New")!, ModificationDate = DateTime.UtcNow
        };
        context.AlarmHistories.Add(alarmStatusHistory);
        context.Alarms.Add(alarm);
    }

    /// <summary>
    /// Updates the alarm and triggers processing for all machines in the database.
    /// </summary>
    /// <param name="context"> The database context of type <see cref="ApplicationDbContext"/> used to interact with the database.</param>
    /// <param name="normGroupId"> An integer representing the ID of the norm group to be processed.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    public static async Task UpdateAsync(ApplicationDbContext context, int normGroupId) {
        
        await UpdateAlarmAsync(context, normGroupId);
        
        List<Machine> machines = await context.Machines.ToListAsync();

        foreach (Machine machine in machines) {
            await TriggeredAsync(context, machine.Id,normGroupId);
        }
    }

    /// <summary>
    /// Updates alarms for a specified norm group by validating machines against the group's norms,
    /// resolving alarms that no longer meet the conditions, and triggering alarm checks for all machines.
    /// </summary>
    /// <param name="context">The database context for accessing and modifying data.</param>
    /// <param name="normGroupId">The ID of the norm group to validate and apply to alarms.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    public static async Task UpdateAlarmAsync(ApplicationDbContext context, int normGroupId) {
        
        NormGroup? normGroup = await context.NormGroups
            .Include(m => m.Norms)
            .ThenInclude(n => n.InformationName)
            .Where(m => m.Id == normGroupId)
            .FirstOrDefaultAsync();

        
        await context.Entry(normGroup).ReloadAsync();

        foreach (var norm in normGroup.Norms) {
            await context.Entry(norm).ReloadAsync();
        }
        
        List<Alarm> alarms = await context.Alarms
             .Where(a => a.NormGroupId == normGroupId &&
                 !context.AlarmHistories
                     .Where(ah2 => ah2.AlarmId == a.Id)
                     .OrderByDescending(ah2 => ah2.ModificationDate)
                     .Take(1)
                     .Any(ah2 => ah2.StatusType.Name == "Resolved" || ah2.StatusType.Name == "Not Triggered Anymore"))
             .ToListAsync();

        Console.WriteLine("NB Alarm : " + alarms.Count);

        foreach (Alarm alarm in alarms) {

            List<Information> machineComponent = await context.Components
                .Where(c => c.MachinesId == alarm.MachineId)
                .ToListAsync();

            bool allNormsValid = true;
            Console.WriteLine("Machine : " + alarm.MachineId);
            foreach (Norm norm in normGroup.Norms) {
                Console.WriteLine("Norms : " + norm.InformationName!.Name + " ////// Format : " + norm.Format);

                List<Information> result = machineComponent
                    .Where(m => m.Name == norm.InformationName?.Name && m.Format == norm.Format)
                    .ToList();

                if (result.Count > 0) {
                    Console.WriteLine("Hello");
                    bool normValid = false;

                    foreach (Information information in result) {
                        switch (norm.Condition) {
                            case "NOT IN":
                                Console.WriteLine("NOT IN");
                                normValid = !norm.Value!.Contains(information.Value);
                                break;
                            case "IN":
                                Console.WriteLine("IN");
                                normValid = norm.Value!.Contains(information.Value);
                                break;
                            case "=":
                                Console.WriteLine("=");
                                normValid = Double.Parse(norm.Value!) == Double.Parse(information.Value);
                                break;
                            case "!=":
                                Console.WriteLine("!=");
                                normValid = Double.Parse(norm.Value!) != Double.Parse(information.Value);
                                break;
                            case ">":
                                normValid = Double.Parse(information.Value) > Double.Parse(norm.Value!);
                                break;
                            case "<":
                                Console.WriteLine("<");
                                normValid = Double.Parse(information.Value) < Double.Parse(norm.Value!);
                                break;
                        }

                        if (normValid) break;
                    }

                    if (!normValid) {
                        allNormsValid = false;
                        break;
                    }
                }
                else {
                    allNormsValid = false;
                    break;
                }
            }
            if (!allNormsValid) {
                Console.WriteLine("Modif");
                AlarmStatusHistory alarmStatusHistory = new AlarmStatusHistory() {
                    ModificationDate = DateTime.UtcNow,
                    StatusType = await context.AlarmStatusTypes.Where(s => s.Name == "Not Triggered Anymore").FirstOrDefaultAsync(),
                    Alarm = alarm
                };
                context.Add(alarmStatusHistory);
            }
        }
        
        await context.SaveChangesAsync();
    }
}
