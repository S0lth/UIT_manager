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
    
    public int LogicalCpu
    {
        get => _LogicalCpu;
        set => _LogicalCpu = value;
    }

    public int CoreCount
    {
        get => _CoreCount;
        set => _CoreCount = value;
    }

    public int ClockSpeed
    {
        get => _ClockSpeed;
        set => _ClockSpeed = value;
    }

    public string Model
    {
        get => _Model;
        set => _Model = value;
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

