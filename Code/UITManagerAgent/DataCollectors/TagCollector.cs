using System.Management;
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
namespace UITManagerAgent.DataCollectors;


    public class TagCollector : DataCollector{
        [SupportedOSPlatform("windows")]
        private ManagementObjectSearcher _searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
        
        /// <summary>
        /// Retrieves the service tag of the machine
        /// </summary>
        /// <returns>
        /// An <see cref="Information"/> object with the service tag of the machine
        /// </returns>
        [SupportedOSPlatform("windows")]
        public Information Collect() {
            TagInformation tagInformation = new TagInformation();
            ManagementObjectCollection collection = _searcher.Get();
            ManagementObject? managementObject = collection.OfType<ManagementObject>().FirstOrDefault();
            if (managementObject != null){
                tagInformation.TagService.Value = managementObject["SerialNumber"]?.ToString() ?? "Unknown Service Tag";
            }
            
            return tagInformation;
        }
    }