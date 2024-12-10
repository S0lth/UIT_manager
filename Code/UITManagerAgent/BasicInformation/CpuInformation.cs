using System.Text.Json;

namespace UITManagerAgent.BasicInformation;

public class CpuInformation : Information {
    private int _logicalCore;
    private string _formatLogical = "NUMBER";
    private int _coreCount;
    private string _formatCore = "NUMBER";
    private int _clockSpeed;
    private string _formatClockSpeed = "%";
    private string _model;
    private string _formatModel = "TEXT";

    public CpuInformation() {
        _logicalCore = 0;
        _coreCount = 0;
        _clockSpeed = 0;
        _model = "";
    }

    /// <summary>
    /// Gets or sets the number of logical CPUs.
    /// </summary>
    /// <value>
    /// The number of logical CPUs. This property allows you to specify or retrieve 
    /// the count of logical processors available for computation. 
    /// The value should be a non-negative integer.
    /// </value>
    public int LogicalCore {
        get => _logicalCore;
        set => _logicalCore = value;
    }
    
    /// <summary>
    /// Gets or sets the format of the CPU information.
    /// </summary>
    /// <value>
    /// A string representing the format of CPU.
    /// </value>
    public string FormatLogical {
        get => _formatLogical;
        set => _formatLogical = value ?? throw new ArgumentNullException(nameof(value));
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
    /// Gets or sets the format of the Core information.
    /// </summary>
    /// <value>
    /// A string representing the format of Core
    /// </value>
    public string FormatCore {
        get => _formatCore;
        set => _formatCore = value ?? throw new ArgumentNullException(nameof(value));
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
    /// Gets or sets the format of the clock speed information.
    /// </summary>
    /// <value>
    /// A string representing the format of clock speed.
    /// </value>
    public string FormatClockSpeed {
        get => _formatClockSpeed;
        set => _formatClockSpeed = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets the model of the CPU.
    /// </summary>
    /// <value>
    /// A string representing the model name or identifier of the CPU. This property 
    /// allows you to specify or retrieve the model of the CPU for identification 
    /// purposes.
    /// </value>
    public string Model {
        get => _model;
        set => _model = value;
    }
    
    /// <summary>
    /// Gets or sets the format of the CPU model information.
    /// </summary>
    /// <value>
    /// A string representing the format of CPU model.
    /// </value>
    public string FormatModel {
        get => _formatModel;
        set => _formatModel = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Returns a string representation of the CPU information, including
    /// the number of logical CPUs, the number of physical cores, and 
    /// the current clock speed in MHz.
    /// </summary>
    /// <returns>A formatted string with the CPU details.</returns>
    public override string ToString() {
        return "CPU Information : " +
               "Logical CPU Count : " + string.Join(" , ", _logicalCore) + ", " +
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