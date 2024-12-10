using System.Text.Json;

namespace UITManagerAgent.BasicInformation;


public class CpuInformation : Information {
    private int _logicalCpu;
    private string _formatCpu;
    private int _coreCount;
    private string _formatCoreCount;
    private int _clockSpeed;
    private string _formatClockSpeed;
    private string _model;
    private string _formatModel;

    public CpuInformation() {
        _logicalCpu = 0;
        _formatCpu = "Number";
        _coreCount = 0;
        _formatCoreCount = "Number";
        _clockSpeed = 0;
        _formatClockSpeed = "%";
        _model = "";
        _formatModel = "Text";
    }

    /// <summary>
    /// Gets or sets the number of logical CPUs.
    /// </summary>
    /// <value>
    /// The number of logical CPUs. This property allows you to specify or retrieve 
    /// the count of logical processors available for computation. 
    /// The value should be a non-negative integer.
    /// </value>
    public int LogicalCpu {
        get => _logicalCpu;
        set => _logicalCpu = value;
    }

    /// <summary>
    /// Gets or sets the number of CPU cores.
    /// </summary>
    /// <value>
    /// The number of CPU cores. This property allows you to specify or retrieve 
    /// the total count of physical CPU cores available in the system.
    /// The value should be a non-negative integer.
    /// </value>
    public int CoreCount {
        get => _coreCount;
        set => _coreCount = value;
    }

    /// <summary>
    /// Gets or sets the clock speed of the CPU.
    /// </summary>
    /// <value>
    /// The clock speed in megahertz (MHz). This property allows you to specify or 
    /// retrieve the operating frequency of the CPU, which can influence performance.
    /// The value should be a non-negative integer representing the clock speed.
    /// </value>
    public int ClockSpeed {
        get => _clockSpeed;
        set => _clockSpeed = value;
    }

    /// <summary>
    /// Gets or sets the model of the CPU.
    /// </summary>
    /// <value>
    /// A string representing the model name or identifier of the CPU. This property 
    /// allows you to specify or retrieve the model of the CPU for identification 
    /// purposes.
    public string Model {
        get => _model;
        set => _model = value;
    }

    /// <summary>
    /// Returns a string representation of the CPU information, including
    /// the number of logical CPUs, the number of physical cores, and 
    /// the current clock speed in MHz.
    /// </summary>
    /// <returns>A formatted string with the CPU details.</returns>
    public override string ToString() {
        return "CPU Information : " +
               "Logical CPU Count : " + string.Join(" , ", _logicalCpu) + ", " +
               "Core Count : " + string.Join(" , ", _coreCount) + ", " +
               "Clock Speed : " + string.Join(" MHz , ", _clockSpeed) + " MHz, " +
               "Model : " + string.Join(" , ", _model);
    }


    /// <summary>
    /// Returns a Json string representation of the cpuInformation
    /// </summary>
    /// <returns>A Json string that represents the cpuInformation .</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }
}