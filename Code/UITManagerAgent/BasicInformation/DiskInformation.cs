using System.Text.Json;

namespace UITManagerAgent.BasicInformation {
    /// <summary>
    /// Provides information on all disk names, total storage capacity, free storage capacity, and number of disks
    /// </summary>
    public class DiskInformation : Information {

        /// <summary>
        /// Represents a disk with properties for its name, total size, and available free space.
        /// </summary>
        public class Disk {
            private string? _diskName = String.Empty;
            private long _diskTotalSize;
            private long _diskFreeSize;
            private string _formatValue = "GB";

            /// <summary>
            /// accessors of the disksName field
            /// </summary>
            public string? DisksName {
                get => _diskName;
                set => _diskName = value;
            }

            /// <summary>
            /// accessors of the diskTotalSize field
            /// </summary>
            public long DiskTotalSize {
                get => _diskTotalSize;
                set => _diskTotalSize = value;
            }


            /// <summary>
            /// accessors of the diskFreeSize field
            /// </summary>
            public long DiskFreeSize {
                get => _diskFreeSize;
                set => _diskFreeSize = value;
            }

            /// <summary>
            /// Returns a formatted string representation of the disk information, 
            /// including disk names, total sizes, free space, and the number of disks.
            /// </summary>
            /// <returns>
            /// A <see cref="string"/> representing the disk information in a readable format.
            /// Each property is presented on a new line with appropriate labels.
            /// </returns>
            public override string ToString() {
                return "Disk name : " + _diskName + ", disk total size : " + _diskTotalSize +
                       "Go, disk total free size : " + _diskFreeSize + "Go" + Environment.NewLine;
            }

            /// <summary>
            /// Returns a Json string representation of the disk
            /// </summary>
            /// <returns>A Json string that represents the disk.</returns>
            public string ToJson() {
                return JsonSerializer.Serialize(this);
            }
        }
        private List<Disk> _disks = new List<Disk>();
        private int _numberDisk;
        private string? _format = "Number";

        /// <summary>
        /// accessors of the list disks field
        /// </summary>
        public List<Disk> Disks {
            get => _disks;
            set => _disks = value;
        }

        /// <summary>
        /// Retrieves the name of the first disk in the list of disks.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> representing the name of the first disk, 
        /// </returns>
        public String? GetFirstDiskName() {
            return _disks.First().DisksName;
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
        public override string ToString() {
            return string.Join(", ", _disks) + "Number disk : " + _numberDisk;
        }

        /// <summary>
        /// Returns a Json string representation of the diskInformation
        /// </summary>
        /// <returns>A Json string that represents the diskInformation .</returns>
        public override string ToJson() {
            return JsonSerializer.Serialize(this);
        }
    }
}