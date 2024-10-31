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
            cpu.setLogicalCpu(getProcessorCount());
            cpu.setCoreCount(getNumberOfCores());
            cpu.setClockSpeed(getCurrentClockSpeed());
      
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur : {ex.Message}");
        }

        return cpu;
    }
    /// <summary>
    /// Get CPU count
    /// </summary>
    /// <returns>Core Count: Number of physical processor cores.</returns>
    public int getProcessorCount()
    {
        return Environment.ProcessorCount;
    }

    public int getNumberOfCores()
    {
        int res = 0;
        var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
        foreach (ManagementObject obj in searcher.Get())
        {
            res = (Convert.ToInt32(obj["NumberOfCores"]));
        }
        return res;
    }

    public int getCurrentClockSpeed()
    {
        int res = 0;
        var searcher = new ManagementObjectSearcher("select * from Win32_Processor");

        foreach (ManagementObject obj in searcher.Get())
        {
           res = Convert.ToInt32(obj["CurrentClockSpeed"]);
        }
        return res;
    }
}

