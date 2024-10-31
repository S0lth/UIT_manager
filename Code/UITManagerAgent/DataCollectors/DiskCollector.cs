using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors
{
    public class DiskCollector : DataCollector
    {
        public Information Collect()
        {
            DiskInformation diskInformation = new DiskInformation();
            int diskCount = 0;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    diskCount++;
                    diskInformation.getDiskName().Add(drive.Name);
                    diskInformation.getDiskTotalSize().Add(drive.TotalSize / (1024 * 1024 * 1024));
                    diskInformation.getDisksFreeSizes().Add(drive.TotalFreeSpace / (1024 * 1024 * 1024));
                }
            }

            diskInformation.setNumberDisk(diskCount);
            return diskInformation;
        }
    }
}
