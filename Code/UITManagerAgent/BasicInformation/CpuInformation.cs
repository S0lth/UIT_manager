using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace UITManagerAgent.DataCollectors;


public class CpuInformation : Information
 {
    public int LogicalCpu { get; set; }
    public int CoreCount { get; set; }
    public int ClockSpeed { get; set; }



    public override string ToString()
    {
        return $"CPU Information:\n" +
               $"  Logical CPU Count: {LogicalCpu}\n" +
               $"  Core Count: {CoreCount}\n" +
               $"  Clock Speed: {ClockSpeed} MHz";
    }
}

