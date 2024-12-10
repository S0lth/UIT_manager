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
            private string _formatDiskName = "TEXT";
            private long _diskTotalSize;
            private string _formatTotalSize = "GO";
            private long _diskFreeSize;
            private string _formatFreeSize = "GO";

            /// <summary>
            /// accessors of the disksName field
            /// </summary>
            public string? DiskName {
                get => _diskName;
                set => _diskName = value;
            }

            /// <summary>
            /// Gets or sets the format of the disk name information
            /// </summary>
            /// <value>
            /// A string representing the format of disk name.
            /// </value>
            public string FormatDiskName {
                get => _formatDiskName;
                set => _formatDiskName = value ?? throw new ArgumentNullException(nameof(value));
            }

            /// <summary>
            /// accessors of the diskTotalSize field
            /// </summary>
            public long DiskTotalSize {
                get => _diskTotalSize;
                set => _diskTotalSize = value;
            }

            /// <summary>
            /// Gets or sets the format of the total memory information
            /// </summary>
            /// <value>
            /// A string representing the format of total memory.
            /// </value>
            public string FormatTotalSize {
                get => _formatTotalSize;
                set => _formatTotalSize = value ?? throw new ArgumentNullException(nameof(value));
            }
            
            /// <summary>
            /// accessors of the diskFreeSize field
            /// </summary>
            public long DiskFreeSize {
                get => _diskFreeSize;
                set => _diskFreeSize = value;
            }
            
            /// <summary>
            /// Gets or sets the format of the free memory information
            /// </summary>
            /// <value>
            /// A string representing the format of free memory.
            /// </value>
            public string FormatFreeSize {
                get => _formatFreeSize;
                set => _formatFreeSize = value ?? throw new ArgumentNullException(nameof(value));
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
        private string? _formatNumberDisk = "NUMBER";

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
            return _disks.First().DiskName;
        }

        /// <summary>
        /// accessors of the numberDisk field
        /// </summary>
        public int NumberDisk {
            get => _numberDisk;
            set => _numberDisk = value;
        }

        /// <summary>
        /// Gets or sets the format of the number disk information
        /// </summary>
        /// <value>
        /// A string representing the format of number disk information.
        /// </value>
        public string? FormatNumberDisk {
            get => _formatNumberDisk;
            set => _formatNumberDisk = value;
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