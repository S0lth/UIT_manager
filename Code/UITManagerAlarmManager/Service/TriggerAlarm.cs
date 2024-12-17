using Microsoft.EntityFrameworkCore;
using UITManagerAlarmManager.Data;
using UITManagerAlarmManager.Models;

namespace UITManagerAlarmManager.Service;
public  class TriggerAlarm {
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

            bool alreadyTrigger = false;
            foreach (Norm norm in normGroup.Norms) {
                Console.WriteLine("Norms : "  + norm.InformationName.Name + " ////// Format : " + norm.Format);
                if (!alreadyTrigger) {
                    List<Information> result = machineComponent.Where(m => m.Name == norm.InformationName?.Name && m.Format == norm.Format).ToList();
                    Console.WriteLine("Info : "  + result[0].Name);
                    if (result.Count > 0) {
                        foreach (Information information in result) {
                            switch (norm.Condition) {
                                case "NOT IN":
                                    Console.WriteLine("NOT IN");
                                    if (!norm.Value.Contains(information.Value))
                                        alreadyTrigger = CreateAlarm(machineId, normGroup.Id, listStatus);
                                    break;
                                case "IN":
                                    Console.WriteLine("IN");
                                    if (norm.Value.Contains(information.Value))
                                        alreadyTrigger = CreateAlarm(machineId, normGroup.Id, listStatus);
                                    break;
                                case "=":
                                    Console.WriteLine("=");
                                    if (Double.Parse(norm.Value) == Double.Parse(information.Value))
                                        alreadyTrigger = CreateAlarm(machineId, normGroup.Id, listStatus);
                                    break;
                                case "!=":
                                    Console.WriteLine("!=");
                                    if (Double.Parse(norm.Value) != Double.Parse(information.Value))
                                        alreadyTrigger = CreateAlarm(machineId, normGroup.Id, listStatus);
                                    break;
                                case ">":
                                    Console.WriteLine(">");
                                    if (Double.Parse(norm.Value) < Double.Parse(information.Value))
                                        alreadyTrigger = CreateAlarm(machineId, normGroup.Id, listStatus);
                                    break;
                                case "<":
                                    Console.WriteLine("<");
                                    if (Double.Parse(norm.Value) > Double.Parse(information.Value))
                                        alreadyTrigger = CreateAlarm(machineId, normGroup.Id, listStatus);
                                    break;
                            }
                        }
                    }
                }
            }
        }
        
        _context.SaveChanges();
    }

    private bool CreateAlarm(int machineId, int normGroupId, List<AlarmStatusType> listStatus) {
        Console.WriteLine("Trigger : " + normGroupId);
        Alarm alarm = new Alarm {
            MachineId = machineId, 
            NormGroupId = normGroupId, 
            TriggeredAt = DateTime.UtcNow
        };
        AlarmStatusHistory alarmStatusHistory = new AlarmStatusHistory {
            Alarm = alarm,
            StatusType = listStatus.Find(s => s.Name == "New")!,
            ModificationDate = DateTime.UtcNow
        };
        _context.AlarmHistories.Add(alarmStatusHistory);
        _context.Alarms.Add(alarm);
        
        return true;
    }
}
