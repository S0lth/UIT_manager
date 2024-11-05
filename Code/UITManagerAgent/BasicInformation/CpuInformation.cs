namespace UITManagerAgent.BasicInformation;


public class CpuInformation : Information
 {
    private int _logicalCpu; 
    private int _coreCount; 
    private int _clockSpeed;
    private string _model;

    public CpuInformation() {
        _logicalCpu = 0;
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
    public int LogicalCpu
    {
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
    public int CoreCount
    {
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
    public int ClockSpeed
    {
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
    public string Model
    {
        get => _model;
        set => _model = value;
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
               $"  Logical CPU Count: {_logicalCpu}\n" +
               $"  Core Count: {_coreCount}\n" +
               $"  Clock Speed: {_clockSpeed} MHz \n" + 
               $"  Model: {_model}";
    }
}

