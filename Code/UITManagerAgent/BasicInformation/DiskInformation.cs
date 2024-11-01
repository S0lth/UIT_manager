using System.Management;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.BasicInformation
{
    /// <summary>
    /// Provides information on all disk names, total storage capacity, free storage capacity, and number of disks
    /// </summary>
    public class DiskInformation : Information
    {
        private List<string> _disksName = new();
        private List<long> _diskTotalSize = new();
        private List<long> _disksFreeSize = new();
        private int _numberDisk;

        /// <summary>
        /// accessors of the disksName field
        /// </summary>
        public List<string> DisksName {
            get => _disksName;
            set => _disksName = value;
        }

        /// <summary>
        /// accessors of the diskTotalSize field
        /// </summary>
        public List<long> DiskTotalSize {
            get => _diskTotalSize;
            set => _diskTotalSize = value;
        }


        /// <summary>
        /// accessors of the diskFreeSize field
        /// </summary>
        public List<long> DiskFreeSize {
            get => _disksFreeSize;
            set => _disksFreeSize = value;
        }

        /// <summary>
        /// accessors of the numberDisk field
        /// </summary>
        public int NumberDisk {
            get => _numberDisk;
            set => _numberDisk = value;
        }


        /// <summary>
        /// Returns a formatted string representation of the disk information, 
        /// including disk names, total sizes, free space, and the number of disks.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> representing the disk information in a readable format.
        /// Each property is presented on a new line with appropriate labels.
        /// </returns>
        public override string ToString()
        {
            return string.Join("Nom disque : ", _disksName) + "\n" + string.Join("Total size : ", _diskTotalSize) + "\n" + string.Join("Free size : ", _disksFreeSize) + "\n" +
                   _numberDisk;
        }

    }
}
