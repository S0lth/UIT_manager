using Microsoft.EntityFrameworkCore;
using UITManagerAlarmManager.Data;
using UITManagerAlarmManager.Models;

namespace UITManagerAlarmManager.Service;

public class TriggerAlarm {
    private readonly ApplicationDbContext _context;

    public TriggerAlarm(ApplicationDbContext context) {
        _context = context;
    }


    public async Task Triggered(int machineId) {
        Console.WriteLine("Hello");

        List<Information> machineComponent = await _context.Components
            .Where(c => c.MachinesId == machineId)
            .ToListAsync();

        List<NormGroup> listNormsGroups = await _context.NormGroups
            .Include(m => m.Norms)
            .ThenInclude(n => n.InformationName)
            .ToListAsync();

        List<AlarmStatusType> listStatus = await _context.AlarmStatusTypes.ToListAsync();

        foreach (NormGroup normGroup in listNormsGroups) {
            bool alarmExists = await _context.Alarms
                .Where(a => a.MachineId == machineId && a.NormGroupId == normGroup.Id)
                .AnyAsync(a => a.AlarmHistories
                    .OrderByDescending(h => h.ModificationDate)
                    .FirstOrDefault()!.StatusType.Name != "Resolved");

            Console.WriteLine(alarmExists);

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
                    CreateAlarm(machineId, normGroup.Id, listStatus);
                }
            }
        }


        _context.SaveChanges();
    }

    private bool CreateAlarm(int machineId, int normGroupId, List<AlarmStatusType> listStatus) {
        Console.WriteLine("Trigger : " + normGroupId);
        Alarm alarm = new Alarm { MachineId = machineId, NormGroupId = normGroupId, TriggeredAt = DateTime.UtcNow };
        AlarmStatusHistory alarmStatusHistory = new AlarmStatusHistory {
            Alarm = alarm, StatusType = listStatus.Find(s => s.Name == "New")!, ModificationDate = DateTime.UtcNow
        };
        _context.AlarmHistories.Add(alarmStatusHistory);
        _context.Alarms.Add(alarm);

        return true;
    }
}