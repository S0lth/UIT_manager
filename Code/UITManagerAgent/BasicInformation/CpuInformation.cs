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
    /// Gets or sets the number of logical CPUs.
    /// </summary>
    /// <value>
    /// The number of logical CPUs. This property allows you to specify or retrieve 
    /// the count of logical processors available for computation. 
    /// The value should be a non-negative integer.
    /// </value>
    public int LogicalCpu
    {
        get => _LogicalCpu;
        set => _LogicalCpu = value;
    }

    /// <summary>
    /// Gets or sets the number of CPU cores.
    /// </summary>
    /// <value>
    /// The number of CPU cores. This property allows you to specify or retrieve 
    /// the total count of physical CPU cores available in the system.
    /// The value should be a non-negative integer.
    /// </value>
    public int CoreCount
    {
        get => _CoreCount;
        set => _CoreCount = value;
    }

    /// <summary>
    /// Gets or sets the clock speed of the CPU.
    /// </summary>
    /// <value>
    /// The clock speed in megahertz (MHz). This property allows you to specify or 
    /// retrieve the operating frequency of the CPU, which can influence performance.
    /// The value should be a non-negative integer representing the clock speed.
    /// </value>
    public int ClockSpeed
    {
        get => _ClockSpeed;
        set => _ClockSpeed = value;
    }

    /// <summary>
    /// Gets or sets the model of the CPU.
    /// </summary>
    /// <value>
    /// A string representing the model name or identifier of the CPU. This property 
    /// allows you to specify or retrieve the model of the CPU for identification 
    /// purposes.
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

