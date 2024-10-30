using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
namespace UITManagerAgent.DataCollectors;


public class CpuCollectors : DataCollector
{
    public Information Collect()
    {
        CpuInformation cpu = new CpuInformation();
        try
        {
            cpu.LogicalCpu = Environment.ProcessorCount;

            var searcher = new ManagementObjectSearcher("select * from Win32_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                cpu.CoreCount = Convert.ToInt32(obj["NumberOfCores"]);

                cpu.ClockSpeed = Convert.ToInt32(obj["CurrentClockSpeed"]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur : {ex.Message}");
        }

        return cpu;
    }
}

