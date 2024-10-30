using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace UITManagerAgent.DataCollectors;


public class CpuInformation : Information
 {
    private int LogicalCpu; 
    private int CoreCount; 
    private int ClockSpeed;

    /// <summary>
    /// Gets the number of logical CPUs.
    /// </summary>
    /// <returns>The logical CPU count.</returns>
    public int getLogicalCpu()
    {
        return LogicalCpu;
    }

    /// <summary>
    /// Sets the number of logical CPUs.
    /// </summary>
    /// <param name="logicalCpu">The number of logical CPUs to set.</param>
    public void setLogicalCpu(int logicalCpu)
    {
        this.LogicalCpu = logicalCpu;
    }

    /// <summary>
    /// Gets the number of physical cores.
    /// </summary>
    /// <returns>The core count.</returns>
    public int getCoreCount()
    {
        return CoreCount;
    }

    /// <summary>
    /// Sets the number of physical cores.
    /// </summary>
    /// <param name="coreCount">The number of cores to set.</param>
    public void setCoreCount(int coreCount)
    {
        this.CoreCount = coreCount;
    }

    /// <summary>
    /// Gets the current clock speed in MHz.
    /// </summary>
    /// <returns>The clock speed.</returns>
    public int getClockSpeed()
    {
        return ClockSpeed;
    }

    /// <summary>
    /// Sets the current clock speed in MHz.
    /// </summary>
    /// <param name="clockSpeed">The clock speed to set.</param>
    public void setClockSpeed(int clockSpeed)
    {
        this.ClockSpeed = clockSpeed;
    }

    /// <summary>
    /// Returns a string representation of the CPU information, including
    /// the number of logical CPUs, the number of physical cores, and 
    /// the current clock speed in MHz.
    /// </summary>
    /// <returns>A formatted string with the CPU details.</returns>
    public override string ToString()
    {
        return $"CPU Information:\n" +
               $"  Logical CPU Count: {LogicalCpu}\n" +
               $"  Core Count: {CoreCount}\n" +
               $"  Clock Speed: {ClockSpeed} MHz";
    }
}

