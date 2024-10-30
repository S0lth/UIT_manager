using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
namespace UITManagerAgent.DataCollectors;


public class CpuCollectors : DataCollector
{
    /// <summary>
    /// Collects CPU information including logical CPU count, core count, and clock speed.
    /// </summary>
    /// <returns>
    /// An instance of Inforamtion containing details about the CPU:
    /// Logical CPU Count: Number of logical processors.
    /// Core Count: Number of physical processor cores.
    /// Clock Speed: Current clock speed in MHz
    /// </returns>
    public Information Collect()
    {
        CpuInformation cpu = new CpuInformation();
        try
        {
            cpu.setLogicalCpu(Environment.ProcessorCount);

            var searcher = new ManagementObjectSearcher("select * from Win32_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                cpu.setCoreCount(Convert.ToInt32(obj["NumberOfCores"]));
                cpu.setClockSpeed(Convert.ToInt32(obj["CurrentClockSpeed"]));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur : {ex.Message}");
        }

        return cpu;
    }
}

