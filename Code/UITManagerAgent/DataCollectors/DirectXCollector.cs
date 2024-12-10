using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// Collects information about the DirectX version installed on the system.
/// </summary>
public class DirectXCollector : DataCollector {
    /// <summary>
    /// Collects the DirectX version information from the system using the DirectX Diagnostic Tool (dxdiag).
    /// </summary>
    /// <returns>
    /// A <see cref="DirectXInformation"/> object containing the DirectX version.
    /// If the DirectX version cannot be retrieved, the object will contain: DirectX not found.
    /// </returns>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        DirectXInformation directXInformation = new DirectXInformation();

        try {
            ProcessStartInfo startInfo = new ProcessStartInfo("dxdiag.exe") {
                Arguments = "/t dxdiag.txt",
                UseShellExecute = false,
                RedirectStandardOutput = false,
                CreateNoWindow = true
            };

            Process? dxDiagProcess = Process.Start(startInfo);
            dxDiagProcess?.WaitForExit();

            string dxDiagOutput = System.IO.File.ReadAllText("dxdiag.txt");

            System.IO.File.Delete("dxdiag.txt");

            Match match = Regex.Match(dxDiagOutput, @"DirectX Version:\s*DirectX\s*(\d+)");
            if (match.Success) {
                directXInformation.DirectXVersion = "DirectX " + match.Groups[1].Value;
            }
        }
        catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }

        return directXInformation;
    }
}