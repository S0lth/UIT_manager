using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace UITManagerAgent.DataCollectors;


public class CpuInformation : Information
 {
    private int _LogicalCpu; 
    private int _CoreCount; 
    private int _ClockSpeed;
    private string _Model;

    /// <summary>
    /// Gets the number of logical CPUs.
    /// </summary>
    /// <returns>The logical CPU count.</returns>
    public int getLogicalCpu()
    {
        return _LogicalCpu;
    }

    /// <summary>
    /// Sets the number of logical CPUs.
    /// </summary>
    /// <param name="logicalCpu">The number of logical CPUs to set.</param>
    public void setLogicalCpu(int logicalCpu)
    {
        this._LogicalCpu = logicalCpu;
    }

    /// <summary>
    /// Gets the number of physical cores.
    /// </summary>
    /// <returns>The core count.</returns>
    public int getCoreCount()
    {
        return _CoreCount;
    }

    /// <summary>
    /// Sets the number of physical cores.
    /// </summary>
    /// <param name="coreCount">The number of cores to set.</param>
    public void setCoreCount(int coreCount)
    {
        this._CoreCount = coreCount;
    }

    /// <summary>
    /// Gets the current clock speed in MHz.
    /// </summary>
    /// <returns>The clock speed.</returns>
    public int getClockSpeed()
    {
        return _ClockSpeed;
    }

    /// <summary>
    /// Sets the current clock speed in MHz.
    /// </summary>
    /// <param name="clockSpeed">The clock speed to set.</param>
    public void setClockSpeed(int clockSpeed)
    {
        this._ClockSpeed = clockSpeed;
    }
    /// <summary>
    /// Gets the current cpu model
    /// </summary>
    /// <returns>Cpu model</returns>
    public string getModel()
    {
        return this._Model;
    }
    /// <summary>
    /// Sets the current model name
    /// </summary>
    /// <param name="model">the model to set</param>
    public void setModel(string model)
    {
        this._Model = model;
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
               $"  Logical CPU Count: {_LogicalCpu}\n" +
               $"  Core Count: {_CoreCount}\n" +
               $"  Clock Speed: {_ClockSpeed} MHz \n" + 
               $"  Model: {_Model}";
    }
}

