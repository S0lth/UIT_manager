using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
namespace UITManagerAgent.DataCollectors;


public class CpuCollectors : DataCollector
{

    ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");

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
            cpu.LogicalCpu = GetProcessorCount();
            cpu.CoreCount =GetNumberOfCores();
            cpu.ClockSpeed = GetCurrentClockSpeed();
            cpu.Model = GetModelCPU();
      
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
    /// <returns>Logical CPU Count: Number of logical processors.</returns>
    public int GetProcessorCount()
    {
        return Environment.ProcessorCount;
    }

    /// <summary>
    /// Get Number of logical CPU
    /// </summary>
    /// <returns>Core Count: Number of physical processor cores.</returns>
    public int GetNumberOfCores()
    {
        int res = 0;
        foreach (ManagementObject obj in searcher.Get())
        {
            res = (Convert.ToInt32(obj["NumberOfCores"]));
        }
        return res;
    }
    /// <summary>
    /// get the curente clockSpeed 
    /// </summary>
    /// <returns>Clock Speed: Current clock speed in MHz</returns>
    public int GetCurrentClockSpeed()
    {
        int res = 0;

        foreach (ManagementObject obj in searcher.Get())
        {
           res = Convert.ToInt32(obj["CurrentClockSpeed"]);
        }
        return res;
    }
    /// <summary>
    /// get the name of the processor
    /// </summary>
    /// <returns>A string for the name of the cpu</returns>

    public string GetModelCPU()
    {
        string cpuModel = "";
        foreach (ManagementObject obj in searcher.Get())
        {
            cpuModel = obj["Name"].ToString();
            
        }
        return cpuModel;
    }
}

